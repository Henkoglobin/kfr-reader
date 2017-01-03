using System.Collections.Generic;

namespace KfrBinaryReader.Core {
	public class Mesh {
		public IReadOnlyList<Vector4f> Vertices { get; }
		public IReadOnlyList<IReadOnlyList<ushort>> Faces { get; }
		public IReadOnlyList<Vector3f> Normals { get; }

		public Mesh(
			IReadOnlyList<Vector4f> vertices,
			IReadOnlyList<IReadOnlyList<ushort>> faces,
			IReadOnlyList<Vector3f> normals
		) {
			this.Vertices = vertices;
			this.Faces = faces;
			this.Normals = normals;
		}
	}
}
