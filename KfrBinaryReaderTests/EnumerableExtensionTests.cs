using KfrBinaryReader.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace KfrBinaryReaderTests {
	[TestClass]
	public class EnumerableExtensionTests {
		[TestMethod]
		public void TestSmallStepWidth() {
			var a = new[] { 1, 2, 3, 4, 5 };
			var expectedSums = new[] { 6, 9, 12 };
			var actualSums = a
				.Windowed(3, 1)
				.Select(x => x.Sum())
				.ToList();

			CollectionAssert.AreEqual(expectedSums, actualSums);
		}
	}
}
