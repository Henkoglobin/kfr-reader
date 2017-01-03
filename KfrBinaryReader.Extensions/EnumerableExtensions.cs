using System.Collections.Generic;
using System.Linq;

namespace KfrBinaryReader.Extensions {
	public static class EnumerableExtensions {
		public static IEnumerable<IReadOnlyList<T>> Windowed<T>(this IEnumerable<T> self, int windowWidth, int step) {
			var list = self.ToList();
			for (int i = 0; i < list.Count - windowWidth + 1; i += step) {
				var retList = new List<T>(windowWidth);
				for (int j = 0; j < windowWidth; j++) {
					retList.Add(list[i + j]);
				}
				yield return retList;
			}
		}
	}
}
