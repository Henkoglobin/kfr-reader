using KfrBinaryReader.Core;
using System;
using System.IO;
using System.Threading.Tasks;

namespace KfrBinaryReaderConsole {
	public class WavefrontObjExporter : IMeshWriter {
		public async Task WriteMeshToFileAsync(string fileName, string name, Mesh mesh) {
			if(mesh == null) {
				throw new ArgumentNullException(nameof(mesh));
			}
			
			using (var stream = new FileStream(fileName, FileMode.Create)) {
				using (var writer = new StreamWriter(stream)) {
					foreach (var vertex in mesh.Vertices) {
						await writer.WriteLineAsync($"v {vertex.X} {vertex.Y} {vertex.Z} {vertex.W}");
					}

					foreach (var face in mesh.Faces) {
						await writer.WriteLineAsync($"f {face[0] + 1} {face[1] + 1} {face[2] + 1}");
					}
				}
			}
		}
	}
}
