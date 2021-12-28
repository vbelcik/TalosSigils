#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

namespace TalosSigils
{
    static class InputParser
    {
        public static Board Parse(string[] text, bool mergeSigils)
        {
            Section[] sections = SplitTextIntoSections(text);

            if (sections.Length == 0)
            {
                throw new Exception("File sections expected. Every section ends with \"----\" line.");
            }

            (int dim_x, int dim_y) = ParseDim(dimSectionText: sections[0].text);

            Sigil[] sigils = sections
                .Skip(1)
                .Select(ParseSigil)
                .Where(s => s != null)
                .Cast<Sigil>()
                .ToArray();

            if (mergeSigils)
            {
                sigils = SigilMerger.Merge(sigils);
            }

            SetSigilBoardChars(sigils);

            return new Board(dx: dim_x, dy: dim_y, sigils);
        }

        private static (int dim_x, int dim_y) ParseDim(string[] dimSectionText)
        {
            try
            {
                dimSectionText = dimSectionText
                    .Where(ln => !string.IsNullOrWhiteSpace(ln))
                    .ToArray();

                string str_dim = dimSectionText[0];
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

        private static Sigil? ParseSigil(Section section)
        {
            (int, int)[] points = ParseSigilPoints(pattern: section.text);

            if (points.Length == 0)
            {
                // empty sigil encountered
                return null;
            }

            return new Sigil(points: points, count: section.count);
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

        private static void SetSigilBoardChars(Sigil[] sigils)
        {
            int index = 0;

            foreach (Sigil sigil in sigils)
            {
                sigil.SetBoardChars(firstSigilIndex: index);
                index += sigil.BoardChars.Length;
            }
        }

        private static Section[] SplitTextIntoSections(string[] text)
        {
            var lstSections = new List<Section>();
            var lst = new List<string>();

            int currentCount = 1;
            int lineNum = 0;

            foreach (string line in text)
            {
                lineNum++;

                if (IsSeparator(line))
                {
                    Section section;
                    section.count = currentCount;
                    section.text = lst.ToArray();

                    lstSections.Add(section);
                    lst.Clear();

                    currentCount = ParseSeparatorCount(line, lineNum: lineNum);
                }
                else
                {
                    lst.Add(line);
                }
            }

            return lstSections.ToArray();
        }

        private static bool IsSeparator(string s)
        {
            return s.Trim().StartsWith("--");
        }

        private static int ParseSeparatorCount(string s, int lineNum)
        {
            int i = s.IndexOf('(');

            if (i < 0)      // no '(N)'
            {
                return 1;   // default count
            }

            s = s[(i + 1)..];     // skip '...('

            i = s.IndexOf(')');

            if (i < 0)
            {
                throw new Exception("Closing ')' missing on line " + lineNum);
            }

            s = s[..i];     // exclude ')...'

            if (!int.TryParse(s.Trim(), out int count))
            {
                throw new Exception("'( count )' expected on line " + lineNum);
            }

            if (count < 0)
            {
                throw new Exception("Negative '( count )' on line " + lineNum);
            }

            return count;
        }

        private struct Section
        {
            public int count;
            public string[] text;
        }
    }
}
