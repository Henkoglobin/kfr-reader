namespace KfrBinaryReader.Core {
	public class ParseResult {
		public string Name { get; }
		public Mesh Mesh { get; }

		public bool IsSuccess
			=> this.Mesh != null;

		public ParseResult(string name, Mesh mesh) {
			this.Name = name;
			this.Mesh = mesh;
		}

		public static ParseResult ForFailure(string name) {
			return new ParseResult(name, null);
		}
	}
}
