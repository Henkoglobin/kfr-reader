using KfrBinaryReader.Core;
using KfrBinaryReader.Parsers;
using KfrBinaryReaderConsole;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KfrBinaryReaderConsole {


	public class Program {
		private static readonly Regex escapeStringRegex = new Regex(@"[^A-Za-z0-9_\-]");

		static void Main(string[] args) {
			MainAsync(args).Wait();
		}

		private static async Task MainAsync(string[] args) {
			var searchDir = new DirectoryInfo(args[0]);
			Console.WriteLine("Scanning directory...");
			var files = SearchFiles(searchDir, new Regex("\\.(kfr|std)")).ToList();

			Console.WriteLine($"Found {files.Count} files. Now parsing meshes...");

			var parser = new KfrStdParser();
			var parsedFiles = files
				.Select((file, i) => {
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

			var writer = new WavefrontObjExporter();
			foreach (var parsedFile in parsedFiles) {
				if (parsedFile.IsSuccess) {
					var directoryPath = $"{parsedFile.SourceDirectoryName}\\{parsedFile.SourceFileName}";
					Directory.CreateDirectory(directoryPath);
					foreach (var result in parsedFile.Results) {
						if (result.IsSuccess) {
							var outputFile = new FileInfo($"{directoryPath}\\{EscapeString(result.Name)}.obj");
							await writer.WriteMeshToFileAsync(outputFile.FullName, result.Name, result.Mesh);
						} else {
							//Console.WriteLine()
						}
					}

				} else {
					Console.WriteLine($"Error parsing file {parsedFile.SourceDirectoryName}\\{parsedFile.SourceFileName}");
				}
			}
		}

		private static string EscapeString(string name) {
			return escapeStringRegex.Replace(name, "_");
		}

		private static string MakeConsoleProgressBar(int i, int count) {
			int totalWidth = Console.WindowWidth - 8;
			float progress = (float)i / count;
			int done = (int)(totalWidth * progress);
			int todo = totalWidth - done;

			return $"\r[{new string('=', done)}{new string(' ', todo)}] {progress * 100,3:###}%";
		}

		private static IEnumerable<FileInfo> SearchFiles(DirectoryInfo searchDir, Regex regex) {
			foreach (var file in searchDir.EnumerateFiles()) {
				if (regex.IsMatch(file.Name)) {
					yield return file;
				}
			}

			foreach (var directory in searchDir.EnumerateDirectories()) {
				var result = SearchFiles(directory, regex);
				foreach (var file in result) {
					yield return file;
				}
			}
		}
	}
}
