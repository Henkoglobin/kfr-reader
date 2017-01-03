using System.Threading.Tasks;

namespace KfrBinaryReader.Core {
	public interface IMeshWriter {
		Task WriteMeshToFileAsync(string fileName, string name, Mesh mesh);
	}
}
