namespace KfrBinaryReader.Core {
	public struct Vector4f {
		public float X { get; set; }
		public float Y { get; set; }
		public float Z { get; set; }
		public float W { get; set; }

		public override string ToString() {
			return $"({this.X}, {this.Y}, {this.Z}, {this.W})";
		}
	}
}
