#nullable enable

namespace TalosSigils
{
    sealed class Putting
    {
        readonly Board m_board;

        int m_x;
        int m_y;
        int m_sigilIdx;
        int m_sigilOrient;
        int m_sigilCenterPointIdx;

        public Putting(Board board)
        {
            m_board = board;
        }

        public void ResetToRoot()
        {
            m_x = 0;
            m_y = 0;
            m_sigilIdx = 0;
            m_sigilOrient = 0;
            m_sigilCenterPointIdx = 0;
        }

        private bool AdvancePlaceXY()
        {
            if (m_x + 1 < m_board.Dim_x)
            {
                m_x++;
                return true;
            }

            if (m_y + 1 < m_board.Dim_y)
            {
                m_x = 0;
                m_y++;
                return true;
            }

            return false;
        }

        public bool FindNextFreePlaceXY(Putting prevPutting)
        {
            m_x = prevPutting.m_x;
            m_y = prevPutting.m_y;

            while (m_board.Get(m_x, m_y) != ' ')
            {
                if (!AdvancePlaceXY())
                {
                    return false;
                }
            }

            return true;
        }

        private bool FindNextFreeSigil()
        {
            while (m_sigilIdx < m_board.SigilUseCount.Length)
            {
                if (m_board.SigilUseCount[m_sigilIdx] < m_board.Sigils[m_sigilIdx].BoardChars.Length)
                {
                    return true;
                }

                m_sigilIdx++;
            }

            return false;
        }

        public bool FirstTrial()
        {
            m_sigilIdx = 0;
            m_sigilOrient = 0;
            m_sigilCenterPointIdx = 0;

            return FindNextFreeSigil();
        }

        public bool NextTrial()
        {
            Sigil sigil = m_board.Sigils[m_sigilIdx];

            if (m_sigilCenterPointIdx + 1 < sigil.OrientPoints[m_sigilOrient].Length)
            {
                m_sigilCenterPointIdx++;
                return true;
            }

            if (m_sigilOrient + 1 < sigil.OrientPoints.Length)
            {
                m_sigilCenterPointIdx = 0;
                m_sigilOrient++;
                return true;
            }

            int tmp = m_sigilIdx++;

            if (FindNextFreeSigil())
            {
                m_sigilCenterPointIdx = 0;
                m_sigilOrient = 0;
                return true;
            }
            else
            {
                m_sigilIdx = tmp;
                return false;
            }
        }

        public bool CanPutSigil()
        {
            return WholeSigil(clear: false, test: true, sigilBoardCharIdx: 0);
        }

        public void PutSigil()
        {
            WholeSigil(clear: false, test: false, 
                sigilBoardCharIdx: m_board.SigilUseCount[m_sigilIdx]++);
        }

        public void TakeSigil()
        {
            WholeSigil(clear: true, test: false,
                sigilBoardCharIdx: --m_board.SigilUseCount[m_sigilIdx]);
        }

        private bool WholeSigil(bool clear, bool test, int sigilBoardCharIdx)
        {
            Board board = m_board;

            Sigil sigil = board.Sigils[m_sigilIdx];

            (int, int)[] points = sigil.OrientPoints[m_sigilOrient];

            (int cx, int cy) = points[m_sigilCenterPointIdx];

            // = x - cx + m_x
            int ofs_x = -cx + m_x;
            int ofs_y = -cy + m_y;

            char sigilChar = sigil.BoardChars[sigilBoardCharIdx];

            foreach ((int x, int y) in points)
            {
                if (!Dot(x + ofs_x, y + ofs_y))
                {
                    return false;
                }
            }

            return true;

            bool Dot(int x, int y)
            {
                if (clear)
                {
                    board.Put(x, y, ' ');
                    return true;
                }

                if (board.Get(x, y) != ' ')
                {
                    return false;
                }

                if (!test)
                {
                    board.Put(x, y, sigilChar);
                }

                return true;
            }
        }
    }
}
