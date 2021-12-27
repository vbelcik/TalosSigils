#nullable enable

using System;

namespace TalosSigils
{
    sealed class Board
    {
        public readonly int Dim_x;
        public readonly int Dim_y;

        readonly char[] m_array;

        public readonly Sigil[] Sigils;
        public readonly int[] SigilUseCount;

        public Board(int dx, int dy, Sigil[] sigils)
        {
            this.Dim_x = dx;
            this.Dim_y = dy;

            this.Sigils = sigils;
            this.SigilUseCount = new int[sigils.Length];

            m_array = new char[dx * dy];

            Array.Fill(m_array, ' ');
        }

        public void Put(int x, int y, char ch)
        {
            if ((x >= 0) && (x < Dim_x) &&
                (y >= 0) && (y < Dim_y))
            {
                m_array[(y * Dim_x) + x] = ch;
            }
        }

        public char Get(int x, int y)
        {
            if ((x >= 0) && (x < Dim_x) &&
                (y >= 0) && (y < Dim_y))
            {
                return m_array[(y * Dim_x) + x];
            }
            else
            {
                return '#';
            }
        }
    }
}
