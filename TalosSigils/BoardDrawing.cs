#nullable enable

using System;
using System.Linq;

namespace TalosSigils
{
    sealed class BoardDrawing
    {
        readonly Board m_board;
        readonly char[][] m_array;

        private BoardDrawing(Board board)
        {
            m_board = board;
            m_array = CreateSurface(board);

            PrintEmptyBoard();
            GlueSigils();
            FlattenCorners();
        }

        public static string[] TextFromBoard(Board board)
        {
            return new BoardDrawing(board).GetText();
        }

        private string[] GetText()
        {
            return m_array.Select(chrs => new string(chrs)).ToArray();
        }

        private static char[][] CreateSurface(Board board)
        {
            (int dx, int dy) = GetSquareBase(board.Dim_x, board.Dim_y);
            dx++;
            dy++;

            char[][] array = new char[dy][];

            for (int y = 0; y < dy; y++)
            {
                array[y] = new char[dx];

                Array.Fill(array[y], ' ');
            }

            return array;
        }

        private void PrintEmptyBoard()
        {
            for (int y = 0; y < m_board.Dim_y; y++)
            {
                for (int x = 0; x < m_board.Dim_x; x++)
                {
                    PrintSquare(x, y);
                }
            }
        }

        private void GlueSigils()
        {
            for (int y = 0; y < m_board.Dim_y; y++)
            {
                for (int x = 0; x < m_board.Dim_x; x++)
                {
                    char c = m_board.Get(x, y);

                    if (c == m_board.Get(x + 1, y))
                    {
                        PrintSide_Right(x, y, ' ');
                    }

                    if (c == m_board.Get(x, y + 1))
                    {
                        PrintSide_Bottom(x, y, ' ');
                    }
                }
            }
        }

        private void FlattenCorners()
        {
            for (int y = 0; y <= m_board.Dim_y; y++)
            {
                for (int x = 0; x <= m_board.Dim_x; x++)
                {
                    // coords of '+' corner
                    (int bx, int by) = GetSquareBase(x, y);

                    bool P(int rx, int ry) => (GetSurfaceChar(bx + rx, by + ry) != ' ');

                    bool l = P(-1, 0);  // left
                    bool r = P(+1, 0);  // right
                    bool t = P(0, -1);  // top
                    bool b = P(0, +1);  // bottom

                    int horz = (l | r) ? 1 : 0;
                    int vert = (t | b) ? 1 : 0;

                    char ch = (horz, vert) switch
                    {
                        (0, 0) => ' ',
                        (0, 1) => '|',
                        (1, 0) => '-',
                        (1, 1) => '+',
                        _ => throw new Exception()
                    };

                    SetSurfaceChar(bx, by, ch);
                }
            }
        }

        private void PrintSquare(int x, int y)
        {
            PrintCorners(x, y);
            PrintSide_Left(x, y);
            PrintSide_Top(x, y);
            PrintSide_Right(x, y);
            PrintSide_Bottom(x, y);
        }

        private void PrintCorners(int x, int y, char ch = '+')
        {
            P(x + 0, y + 0);    // left-top '+'
            P(x + 0, y + 1);
            P(x + 1, y + 0);
            P(x + 1, y + 1);

            void P(int x, int y)
            {
                (x, y) = GetSquareBase(x, y);
                SetSurfaceChar(x, y, ch);
            }
        }

        private void PrintSide_Left(int x, int y, char ch = '|')
        {
            (x, y) = GetSquareBase(x, y);   // left-top '+'
            SetSurfaceChar(x, y + 1, ch);
        }

        private void PrintSide_Top(int x, int y, char ch = '-')
        {
            (x, y) = GetSquareBase(x, y);   // left-top '+'
            SetSurfaceChar(x + 1, y, ch);
            SetSurfaceChar(x + 2, y, ch);
        }

        private void PrintSide_Right(int x, int y, char ch = '|')
        {
            PrintSide_Left(x + 1, y, ch);
        }

        private void PrintSide_Bottom(int x, int y, char ch = '-')
        {
            PrintSide_Top(x, y + 1, ch);
        }

        private static (int, int) GetSquareBase(int x, int y)
        {
            return (x * 3, y * 2);
        }

        private void SetSurfaceChar(int x, int y, char ch)
        {
            m_array[y][x] = ch;
        }

        private char GetSurfaceChar(int x, int y)
        {
            if ((y >= 0) && (y < m_array.Length))
            {
                char[] a0 = m_array[y];

                if ((x >= 0) && (x < a0.Length))
                {
                    return a0[x];
                }
            }

            return ' ';
        }
    }
}
