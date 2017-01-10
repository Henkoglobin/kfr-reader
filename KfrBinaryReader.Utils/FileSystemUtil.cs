using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace KfrBinaryReader.Utils {
	public class FileSystemUtil {
		public IEnumerable<FileInfo> SearchFiles(DirectoryInfo searchDir, Regex regex) {
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
