#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;

namespace TalosSigils
{
    sealed class Sigil
    {
        static readonly PointArrayEqComparer s_pointArrayEqComparer = new();

        public readonly (int, int)[][] OrientPoints;
        public readonly char[] BoardChars;

        private Sigil((int, int)[][] orientPoints, int count)
        {
            this.OrientPoints = orientPoints;
            this.BoardChars = new char[count];

            Array.Fill(this.BoardChars, '*');   // '*' some dummy char for now
        }

        public Sigil((int, int)[] points, int count)
            : this(orientPoints: MakeOrientPoints(points: points), count: count)
        {
        }

        public Sigil WithCount(int count)
        {
            return new Sigil(orientPoints: this.OrientPoints, count: count);
        }

        private static (int, int)[][] MakeOrientPoints((int, int)[] points)
        {
            // 4 orientations
            (int, int)[][] rotPoints = new (int, int)[4][];

            rotPoints[0] = points;

            for (int i = 1; i < rotPoints.Length; i++)
            {
                rotPoints[i] = Array.ConvertAll(rotPoints[i - 1], Rotate90);
            }

            foreach ((int, int)[] ps in rotPoints)
            {
                NormalizePoints(ps);
                SortPoints(ps);
            }

            // e.g. all orientations of a square are identical => 1 remain
            rotPoints = rotPoints.Distinct(s_pointArrayEqComparer).ToArray();

            return rotPoints;
        }

        /// <summary>
        /// coords rotation by 90 deg
        /// </summary>
        private static (int, int) Rotate90((int, int) p)
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

        public void SetBoardChars(int firstSigilIndex)
        {
            for (int i = 0; i < this.BoardChars.Length; i++)
            {
                this.BoardChars[i] = BoardCharFromSigilIndex(index: firstSigilIndex + i);
            }
        }

        // we are limited to 62 sigils (a..z, A..Z, 0..9)
        private static char BoardCharFromSigilIndex(int index)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            const int letterCnt = 'z' - 'a' + 1;

            if (index < letterCnt)
            {
                return (char)('a' + index);
            }

            index -= letterCnt;

            if (index < letterCnt)
            {
                return (char)('A' + index);
            }

            index -= letterCnt;

            if (index < 10)
            {
                return (char)('0' + index);
            }

            throw new Exception("Too many sigils");
        }

        public bool IsSameAs(Sigil other)
        {
            // pick one orientation
            (int, int)[] thisPoints = this.OrientPoints[0];

            // iterate through all orientations
            foreach ((int, int)[] otherPoints in other.OrientPoints)
            {
                if (s_pointArrayEqComparer.Equals(thisPoints, otherPoints))
                {
                    // one match is enough
                    // the sigils represent the same sigil pattern
                    return true;
                }
            }

            return false;
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
                var hash = new HashCode();

                foreach ((int x, int y) in obj)
                {
                    hash.Add(x);
                    hash.Add(y);
                }

                return hash.ToHashCode();
            }
        }
    }
}
