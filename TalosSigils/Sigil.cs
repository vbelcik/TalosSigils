#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;

namespace TalosSigils
{
    sealed class Sigil
    {
        public readonly char BoardChar;
        public readonly (int, int)[][] OrientPoints;

        public Sigil(int index, (int, int)[] points)
        {
            this.BoardChar = (char)('a' + index);

            // 4 orientations
            (int, int)[][] rotPoints = new (int, int)[4][];

            rotPoints[0] = points;

            for (int i = 1; i < rotPoints.Length; i++)
            {
                rotPoints[i] = Array.ConvertAll(rotPoints[i - 1], Rotate);
            }

            foreach ((int, int)[] ps in rotPoints)
            {
                NormalizePoints(ps);
                SortPoints(ps);
            }

            // e.g. all orientations of a square are identical => 1 remain
            rotPoints = rotPoints.Distinct(new PointArrayEqComparer()).ToArray();

            this.OrientPoints = rotPoints;
        }

        private static (int, int) Rotate((int, int) p)
        {
            (int x, int y) = p;
            return (-y, x);
        }

        private static void NormalizePoints((int x, int y)[] ps)
        {
            int min_x = ps.Select(p => p.x).Min();
            int min_y = ps.Select(p => p.y).Min();

            for (int i = 0; i < ps.Length; i++)
            {
                ps[i] = (
                    ps[i].x - min_x,
                    ps[i].y - min_y);
            }
        }

        private static void SortPoints((int, int)[] ps)
        {
            Array.Sort(ps, Comparison);

            static int Comparison((int x, int y) a, (int x, int y) b)
            {
                int cmp = Comparer<int>.Default.Compare(a.y, b.y);

                if (cmp != 0)
                {
                    return cmp;
                }

                return Comparer<int>.Default.Compare(a.x, b.x);
            }
        }

        private sealed class PointArrayEqComparer : IEqualityComparer<(int, int)[]>
        {
            public bool Equals((int, int)[]? a, (int, int)[]? b)
            {
                if (a!.Length != b!.Length)
                {
                    return false;
                }

                for (int i = 0; i < a.Length; i++)
                {
                    if (a[i] != b[i])
                    {
                        return false;
                    }
                }

                return true;
            }

            public int GetHashCode([DisallowNull] (int, int)[] obj)
            {
                var hc = new HashCode();

                foreach ((int x, int y) in obj)
                {
                    hc.Add(x);
                    hc.Add(y);
                }

                return hc.ToHashCode();
            }
        }
    }
}
