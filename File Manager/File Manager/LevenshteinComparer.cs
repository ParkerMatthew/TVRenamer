using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace File_Manager
{
    class LevenshteinComparer : IEqualityComparer<string>
    {
        public int MaxDistance { get; set; }
        //private Levenshtein _Levenshtein = new Levenshtein();

        public LevenshteinComparer() : this(0) { }

        public LevenshteinComparer(int maxDistance)
        {
            this.MaxDistance = maxDistance;
        }



        public bool Equals(string x, string y)
        {
            //int distance = _Levenshtein.EditDistance(x, y);
            int distance = Distance(x, y);
            return distance <= MaxDistance;
        }

        public int GetHashCode(string obj)
        {
            return 0;
        }

        public Int32 Distance(String a, String b) {

            if (string.IsNullOrEmpty(a)) {
                if (!string.IsNullOrEmpty(b)) {
                    return b.Length;
                }
                return 0;
            }

            if (string.IsNullOrEmpty(b)) {
                if (!string.IsNullOrEmpty(a)) {
                    return a.Length;
                }
                return 0;
            }

            Int32 cost;
            Int32[,] d = new int[a.Length + 1, b.Length + 1];
            Int32 min1;
            Int32 min2;
            Int32 min3;

            for (Int32 i = 0; i <= d.GetUpperBound(0); i += 1) {
                d[i, 0] = i;
            }

            for (Int32 i = 0; i <= d.GetUpperBound(1); i += 1) {
                d[0, i] = i;
            }

            for (Int32 i = 1; i <= d.GetUpperBound(0); i += 1) {
                for (Int32 j = 1; j <= d.GetUpperBound(1); j += 1) {
                    cost = Convert.ToInt32(!(a[i - 1] == b[j - 1]));

                    min1 = d[i - 1, j] + 1;
                    min2 = d[i, j - 1] + 1;
                    min3 = d[i - 1, j - 1] + cost;
                    d[i, j] = Math.Min(Math.Min(min1, min2), min3);
                }
            }

            return d[d.GetUpperBound(0), d.GetUpperBound(1)];

        }
    }
}
