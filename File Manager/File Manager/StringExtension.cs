using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace File_Manager {
    static class StringExtension {

        public static string Between(this string value, string a, string b) {
            int posA = value.IndexOf(a);
            int posB = value.LastIndexOf(b);
            if (posA == -1) {
                return "";
            }
            if (posB == -1) {
                return "";
            }
            int adjustedPosA = posA + a.Length;
            if (adjustedPosA >= posB) {
                return "";
            }
            return value.Substring(adjustedPosA, posB - adjustedPosA);
        }

        public static string Before(this string value, string a) {
            int posA = value.IndexOf(a);
            if (posA == -1) {
                return "";
            }
            return value.Substring(0, posA);
        }

        public static string After(this string value, string a) {
            int posA = value.LastIndexOf(a);
            if (posA == -1) {
                return "";
            }
            int adjustedPosA = posA + a.Length;
            if (adjustedPosA >= value.Length) {
                return "";
            }
            return value.Substring(adjustedPosA);
        }

        public static string[] LevenIntersect(this string[] words1, string[] words2, LevenshteinComparer leven) {
            //Calling string.intersect with leven comparator doesn't work as expected in a few cases
            //but this one isn't great either
            int d = leven.MaxDistance;
            List<string> result = new List<string>();
            foreach (string s1 in words1) {
                foreach (string s2 in words2) {
                    if (leven.Distance(s1, s2) <= d && !result.Contains(s2))
                        result.Add(s2);
                }
            }
            return result.ToArray();
        }
    }
}
