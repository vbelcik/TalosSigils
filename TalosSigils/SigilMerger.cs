#nullable enable

using System.Collections.Generic;

namespace TalosSigils
{
    static class SigilMerger
    {
        /// <summary>
        /// Merges same sigils into one with summed "count"
        /// </summary>
        public static Sigil[] Merge(Sigil[] sigils)
        {
            var lst = new List<Sigil>();

            bool[] used = new bool[sigils.Length];

            for (int i = 0; i < sigils.Length; i++)
            {
                if (used[i])
                {
                    continue;
                }

                Sigil sgl_a = sigils[i];
                int count = 0;

                for (int j = i; j < sigils.Length; j++)
                {
                    Sigil sgl_b = sigils[j];

                    if (!used[j] && sgl_a.IsSameAs(sgl_b))
                    {
                        count += sgl_b.BoardChars.Length;
                        used[j] = true;
                    }
                }

                lst.Add(sgl_a.WithCount(count));
            }

            return lst.ToArray();
        }
    }
}
