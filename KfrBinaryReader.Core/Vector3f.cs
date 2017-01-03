namespace KfrBinaryReader.Core {
	public struct Vector3f {
		public float X { get; set; }
		public float Y { get; set; }
		public float Z { get; set; }

		public override string ToString() {
			return $"({this.X}, {this.Y}, {this.Z})";
		}
	}
}
