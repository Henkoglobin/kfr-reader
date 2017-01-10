using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using KfrBinaryReader.Core;
using KfrBinaryReader.Parsers;
using KfrBinaryReader.Utils;

namespace KfrBinaryReaderConsole {
    public class Program {
        private static readonly Regex escapeStringRegex = new Regex(@"[^A-Za-z0-9_\-]");

        static void Main(string[] args) {
            MainAsync(args).Wait();
        }

        private static async Task MainAsync(string[] args) {
            var searchDir = new DirectoryInfo(args[0]);
            Console.WriteLine("Scanning directory...");
            var files = new FileSystemUtil().SearchFiles(searchDir, new Regex("\\.(kfr|std)")).ToList();

            Console.WriteLine($"Found {files.Count} files. Now parsing meshes...");

            var parser = new KfrStdParser();
            var parsedFiles = files
                .Select((file, i) => {
                    Debug.WriteLine($"File #{i}: {file.Name}");
                    Console.Write(MakeConsoleProgressBar(i, files.Count));
                    IList<ParseResult> result;
                    var success = parser.TryParse(file.FullName, out result);
                    return new {
                        SourceDirectoryName = file.Directory.Name,
                        SourceFileName = file.Name,
                        IsSuccess = success,
                        Results = success
                            ? result
                            : new ParseResult[] { }
                    };
                })
                .ToList();

            Console.WriteLine();

            var writer = new WavefrontObjExporter();
            foreach (var parsedFile in parsedFiles) {
                if (parsedFile.IsSuccess) {
                    var directoryPath = $"{parsedFile.SourceDirectoryName}\\{parsedFile.SourceFileName}";
                    Directory.CreateDirectory(directoryPath);
                    foreach (var result in parsedFile.Results) {
                        if (result.IsSuccess) {
                            var outputFile = new FileInfo($"{directoryPath}\\{EscapeString(result.Name)}.obj");
                            await writer.WriteMeshToFileAsync(outputFile.FullName, result.Name, result.Mesh);
                        }
                    }

                } else {
                    Console.WriteLine($"Error parsing file {parsedFile.SourceDirectoryName}\\{parsedFile.SourceFileName}");
                }
            }

            Console.WriteLine("Converted all Files!");
        }

        private static string EscapeString(string name) {
            return escapeStringRegex.Replace(name, "_");
        }

        private static string MakeConsoleProgressBar(int i, int count) {
            int totalWidth = Console.WindowWidth - 8;
            float progress = (float)i / count;
            int done = (int)Math.Ceiling(totalWidth * progress);
            int todo = totalWidth - done;

            return $"\r[{new string('=', done)}{new string(' ', todo)}] {progress * 100,3:###}%";
        }
    }
}
