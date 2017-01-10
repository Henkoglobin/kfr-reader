using System;
using System.Collections.Generic;
using System.Text;

namespace KfrBinaryReader.Extensions {
    public static class ByteArrayExtensions {
        private static readonly Dictionary<string, int[]> BadCharacterJumps = new Dictionary<string, int[]>();
        private static readonly Dictionary<string, int[]> GoodSuffixJumps = new Dictionary<string, int[]>();

        public static int IndexOfSequence(this byte[] haystack, string needle, int startAt) {
            int[] badCharacterJumps;
            int[] goodSuffixJumps;
            if (!ByteArrayExtensions.BadCharacterJumps.TryGetValue(needle, out badCharacterJumps) 
                || !ByteArrayExtensions.GoodSuffixJumps.TryGetValue(needle, out goodSuffixJumps)) {

                badCharacterJumps = ByteArrayExtensions.CalculateBadCharacterJumps(Encoding.ASCII.GetBytes(needle));
                goodSuffixJumps = ByteArrayExtensions.CalculateGoodSuffixJumps(Encoding.ASCII.GetBytes(needle));

                ByteArrayExtensions.BadCharacterJumps.Add(needle, badCharacterJumps);
                ByteArrayExtensions.GoodSuffixJumps.Add(needle, goodSuffixJumps);
            }

            return ByteArrayExtensions.IndexOfSequenceImpl(haystack, Encoding.ASCII.GetBytes(needle), startAt, badCharacterJumps, goodSuffixJumps);
        }

        public static int IndexOfSequence(this byte[] haystack, byte[] needle, int startAt) {
            var badCharacterJumps = ByteArrayExtensions.CalculateBadCharacterJumps(needle);
            var goodSuffixJumps = ByteArrayExtensions.CalculateGoodSuffixJumps(needle);
            return ByteArrayExtensions.IndexOfSequenceImpl(haystack, needle, startAt, badCharacterJumps, goodSuffixJumps);
        }

        private static int IndexOfSequenceImpl(byte[] haystack, byte[] needle, int startAt, int[] occurences, int[] goodSuffixJumps) {
            for (int j, i = startAt + needle.Length - 1; i < haystack.Length;) {
                for (j = needle.Length -1; needle[j] == haystack[i]; --i, --j) {
                    if (j == 0) {
                        return i;
                    }
                }

                i += Math.Max(occurences[haystack[i]], goodSuffixJumps[needle.Length - 1 - j]);
            }

            return -1;
        }

        private static int[] CalculateBadCharacterJumps(byte[] needle) {
            var table = new int[byte.MaxValue + 1];
            for (var i = 0; i < table.Length; i++) {
                table[i] = needle.Length;
            }
            for (var i = 0; i < needle.Length - 1; i++) {
                table[needle[i]] = needle.Length - 1 - i;
            }
            return table;
        }

        private static int[] CalculateGoodSuffixJumps(byte[] needle) {
            var table = new int[needle.Length];
            var lastPrefixPosition = needle.Length;
            for(int i = needle.Length - 1; i >= 0; i--) {
                if(IsPrefix(needle, i + 1)) {
                    lastPrefixPosition = i + 1;
                }
                table[needle.Length - 1 - i] = lastPrefixPosition - i + needle.Length - 1;
            }

            for(var i = 0; i < needle.Length - 1; i++) {
                var suffixLength = GetSuffixLength(needle, i);
                table[suffixLength] = needle.Length - 1 - i + suffixLength;
            }

            return table;
        }

        private static bool IsPrefix(byte[] needle, int p) {
            for(int i = p, j = 0; i < needle.Length; i++, j++) {
                if(needle[i] != needle[j]) {
                    return false;
                }
            }

            return true;
        }

        private static int GetSuffixLength(byte[] needle, int p) {
            var length = 0;
            for(int i = p, j = needle.Length - 1; i >= 0 && needle[i] == needle[j]; i--, j--) {
                length++;
            }

            return length;
        }
    }
}
