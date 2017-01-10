using System.Text;
using KfrBinaryReader.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KfrBinaryReaderTests {
	[TestClass]
	public class ArrayExtensionsTests {
		[TestMethod]
		public void TestMultipleMatches() {
			var haystack = new byte[] { 3, 1, 3, 1, 3, 1 };
			var needle = new byte[] { 3, 1 };

			Assert.AreEqual(0, haystack.IndexOfSequence(needle, 0));
			Assert.AreEqual(2, haystack.IndexOfSequence(needle, 1));
			Assert.AreEqual(2, haystack.IndexOfSequence(needle, 2));
			Assert.AreEqual(4, haystack.IndexOfSequence(needle, 3));
			Assert.AreEqual(4, haystack.IndexOfSequence(needle, 4));
			Assert.AreEqual(-1, haystack.IndexOfSequence(needle, 5));
		}

        [TestMethod]
        public void TestHalfMatch() {
            var haystack = new byte[] { 0, 1, 2, 4 };
            var needle = new byte[] { 0, 1, 2, 3 };

            Assert.AreEqual(-1, haystack.IndexOfSequence(needle, 0));
        }

        [TestMethod]
        public void TestStringMatch() {
            var haystack = "Dies ist ein grandioser Test.";

            Assert.AreEqual(13, Encoding.ASCII.GetBytes(haystack).IndexOfSequence("grandios", 0));
            Assert.AreEqual(2, Encoding.ASCII.GetBytes(haystack).IndexOfSequence("es", 0));
            Assert.AreEqual(25, Encoding.ASCII.GetBytes(haystack).IndexOfSequence("es", 3));
        }
        
	}
}
