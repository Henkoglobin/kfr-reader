using System.Collections.Generic;

namespace KfrBinaryReader.Core {
	public interface IMeshParser {
		/// <summary>
		/// Parses a supported file format into a structure of Meshes.
		/// A return value indicates if at least one conversion was successful.
		/// </summary>
		/// <param name="fileName">The name or path of the file to parse.</param>
		/// <param name="meshes">
		/// When this method returns, contains an <see cref="IList{ParseResult}"/>, each
		/// describing a mesh found in the specified file, if the file was parsed successfully, 
		/// or an empty list if the file could not be parsed. 
		/// This parameter is passed uninitialized; any value originally supplied in meshes will 
		/// be overwritten.
		/// </param>
		/// <returns>
		/// <code>true</code>, if at least a single mesh was successfully parsed.
		/// <code>false</code> otherwise.
		/// </returns>
		bool TryParse(string fileName, out IList<ParseResult> meshes);

		/// <summary>
		/// Quickly parses a supported file format to obtain details about the contained meshes.
		/// A return value indicates if details about at least one mesh were successfully retrieved.
		/// </summary>
		/// <param name="fileName">The name or path of the file to parse.</param>
		/// <param name="meshStatistics">
		/// When this method returns, contains an <see cref="IList{MeshStatistics}"/>, each
		/// containing meta-information about one of the meshes in a file.
		/// This parameter is passed uninitialized; any value originally supplied in meshStatistics
		/// will be overwritten.
		/// </param>
		/// <returns>
		/// <code>true</code>, if details about at least one mesh were successfully retrieved.
		/// <code>false</code> otherwise.
		/// </returns>
		bool TryParseStatistics(string fileName, out IList<MeshStatistics> meshStatistics);
	}
}
