using KfrBinaryReader.Core;
using KfrBinaryReader.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace KfrBinaryReader.Parsers {
	public class KfrStdParser : IMeshParser {
		public bool TryParse(string fileName, out IList<ParseResult> meshes) {
			return new KfrStdParserImpl().TryParse(fileName, out meshes);
		}

		private class KfrStdParserImpl : IMeshParser {
			private byte[] binaryContent;
			private string asciiContent;

			public bool TryParse(string fileName, out IList<ParseResult> meshes) {
				using (FileStream f = new FileStream(fileName, FileMode.Open)) {
					using (MemoryStream stream = new MemoryStream()) {
						f.CopyTo(stream);
						binaryContent = stream.ToArray();
					}
				}

				asciiContent = Encoding.ASCII.GetString(binaryContent);

				List<ParseResult> results = new List<ParseResult>();
				int offset = 0;
				bool success = false;
				while (offset >= 0) {
					offset = asciiContent.IndexOf("\0Name", offset + 1);
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
				int index = GetPropertyOffset(propertyName, offset);
				return BitConverter.ToUInt32(binaryContent, index);
			}

			private IEnumerable<float> ReadFloat32Table(string propertyName, int offset, uint numEntries) {
				int index = GetPropertyOffset(propertyName, offset);
				return Enumerable.Range(0, (int)numEntries)
					.Select(x => index + 4 * x)
					.Select(addr => BitConverter.ToSingle(binaryContent, addr))
					.ToList();
			}

			private IEnumerable<ushort> ReadInt16Table(string propertyName, int offset, uint numEntries) {
				int index = GetPropertyOffset(propertyName, offset);
				return Enumerable.Range(0, (int)numEntries)
					.Select(x => index + 2 * x)
					.Select(addr => BitConverter.ToUInt16(binaryContent, addr))
					.ToList();
			}

			private int GetPropertyOffset(string propertyName, int offset) {
				return asciiContent.IndexOf($"\0{propertyName}", offset) + propertyName.Length + 1;
			}

			private string ReadString(string propertyName, int offset) {
				int index = GetPropertyOffset(propertyName, offset);
				return new string(asciiContent.Skip(index).TakeWhile(x => x != 0x01).ToArray());
			}
		}
	}
}
