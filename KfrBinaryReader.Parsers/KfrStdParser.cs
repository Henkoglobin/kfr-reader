using KfrBinaryReader.Core;
using KfrBinaryReader.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace KfrBinaryReader.Parsers {
    public class KfrStdParser : IMeshParser {
        public bool TryParse(string fileName, out IList<ParseResult> meshes) {
            return new KfrStdParserImpl().TryParse(fileName, out meshes);
        }

        public bool TryParseStatistics(string fileName, out IList<MeshStatistics> meshStatistics) {
            throw new NotImplementedException();
        }

        private class KfrStdParserImpl {
            private byte[] binaryContent;

            public bool TryParse(string fileName, out IList<ParseResult> meshes) {
                using (FileStream f = new FileStream(fileName, FileMode.Open)) {
                    using (MemoryStream stream = new MemoryStream()) {
                        f.CopyTo(stream);
                        binaryContent = stream.ToArray();
                    }
                }


                List<ParseResult> results = new List<ParseResult>();
                int offset = 0;
                bool success = false;
                while (offset >= 0) {
                    offset = this.GetPropertyOffset("Name", offset + 1) - 1 - "Name".Length;
                    if (offset > 0) {
                        string name = ReadString("Name", offset);
                        try {
                            Mesh mesh = ParseMesh(offset);
                            results.Add(new ParseResult(name, mesh));
                            success = true;
                        } catch {
                            results.Add(ParseResult.ForFailure(name));
                        }
                    }
                };

                meshes = results;
                return success;
            }

            private Mesh ParseMesh(int offset) {
                var vertexNum = ReadInt32("VertexesNum", offset);
                var facesNum = ReadInt32("FacesNum", offset);

                var vertices = ReadFloat32Table("VertexTable", offset, vertexNum * 4)
                    .Windowed(4, 4)
                    .Select(x => new Vector4f() { X = x[0], Y = x[1], Z = x[2], W = x[3] })
                    .ToList();

                var faces = ReadInt16Table("FacesTable", offset, facesNum)
                    .Windowed(3, 3)
                    .ToList();

                return new Mesh(vertices, faces, null);
            }

            //private void DoDebugShit(string fileName) {
            //	var a = fileName;
            //	foreach (Match match in new Regex("[A-Za-z0-9_]{5,}").Matches(asciiContent)) {
            //		var keyWord = match.Captures[0].Value;
            //		var index = match.Index;

            //		a += $"\n{index:X8}, {index + keyWord.Length:X8} - {keyWord}";
            //	}

            //	File.WriteAllText("C:\\Users\\Henrik\\Desktop\\test..txt", a);
            //}

            private uint ReadInt32(string propertyName, int offset) {
                int index = this.GetPropertyOffset(propertyName, offset);
                return BitConverter.ToUInt32(binaryContent, index);
            }

            private IEnumerable<float> ReadFloat32Table(string propertyName, int offset, uint numEntries) {
                int index = this.GetPropertyOffset(propertyName, offset);

                if (numEntries > binaryContent.Length * 4) {
                    throw new ArgumentException(nameof(numEntries));
                }

                return Enumerable.Range(0, (int)numEntries)
                    .Select(x => index + 4 * x)
                    .Select(addr => BitConverter.ToSingle(binaryContent, addr))
                    .ToList();
            }

            private IEnumerable<ushort> ReadInt16Table(string propertyName, int offset, uint numEntries) {
                int index = this.GetPropertyOffset(propertyName, offset);
                return Enumerable.Range(0, (int)numEntries)
                    .Select(x => index + 2 * x)
                    .Select(addr => BitConverter.ToUInt16(binaryContent, addr))
                    .ToList();
            }

            private int GetPropertyOffset(string propertyName, int offset) {
                var index = binaryContent.IndexOfSequence($"\0{propertyName}", offset);

                return index + propertyName.Length + 1;
            }

            private string ReadString(string propertyName, int offset) {
                int index = this.GetPropertyOffset(propertyName, offset);
                int endIndex = Array.IndexOf(binaryContent, (byte)0x01, index);
                if (endIndex - index <= 0) {
                    Debugger.Break();
                }
                return Encoding.ASCII.GetString(binaryContent, index, endIndex - index);
            }
        }
    }
}
