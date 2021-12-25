#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

namespace TalosSigils
{
    static class InputParser
    {
        public static Board Parse(string[] text)
        {
            string[][] sections = SplitText(text);

            if (sections.Length == 0)
            {
                throw new Exception("File sections expected. Every section ends with \"----\" line.");
            }

            (int dim_x, int dim_y) = ParseDim(dimSection: sections[0]);

            Sigil[] sigils = sections
                .Skip(1)
                .Select((p, i) => ParseSigil(index: i, pattern: p))
                .Where(s => s != null)
                .Cast<Sigil>()
                .ToArray();

            return new Board(dx: dim_x, dy: dim_y, sigils);
        }

        private static (int dim_x, int dim_y) ParseDim(string[] dimSection)
        {
            try
            {
                dimSection = dimSection.Where(ln => !string.IsNullOrWhiteSpace(ln)).ToArray();

                string str_dim = dimSection[0];
                string[] strArr_dim = str_dim.Split('*');

                if (strArr_dim.Length != 2)
                {
                    throw new Exception();
                }

                return (
                    dim_x: int.Parse(strArr_dim[0].Trim()),
                    dim_y: int.Parse(strArr_dim[1].Trim()));
            }
            catch
            {
                throw new Exception(
                    "First line of first section must contain board's dimensions \"X*Y\"");
            }
        }

        private static Sigil? ParseSigil(int index, string[] pattern)
        {
            (int, int)[] points = ParseSigilPoints(pattern: pattern);

            if (points.Length == 0)
            {
                // empty sigil encountered
                return null;
            }

            return new Sigil(index: index, points: points);
        }

        private static (int, int)[] ParseSigilPoints(string[] pattern)
        {
            var lst = new List<(int, int)>();

            for (int y = 0; y < pattern.Length; y++)
            {
                string patternLine = pattern[y];

                for (int x = 0; x < patternLine.Length; x++)
                {
                    if (!char.IsWhiteSpace(patternLine[x]))
                    {
                        lst.Add((x, y));
                    }
                }
            }

            return lst.ToArray();
        }

        private static string[][] SplitText(string[] text)
        {
            var lstArrs = new List<string[]>();
            var lst = new List<string>();

            foreach (string line in text)
            {
                if (IsSeparator(line))
                {
                    lstArrs.Add(lst.ToArray());
                    lst.Clear();
                }
                else
                {
                    lst.Add(line);
                }
            }

            return lstArrs.ToArray();

            static bool IsSeparator(string s) => s.Trim().StartsWith("--");
        }
    }
}
