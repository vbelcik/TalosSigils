#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TalosSigils
{
    static class Program
    {
        static void Main(string[] args)
        {
            bool mainCatch = true;
            //bool mainCatch = false;

            //string path = @"C:\Users\Vlado\Desktop\zadanie.txt";

            bool? noPause = null;

            try
            {
                noPause = RemoveFlagArg(args: ref args, flag: "--no-pause");

                if (args.Length != 1)
                {
                    throw new Exception("One argument expected: the input text file path");
                }

                Script(path: args[0]);
            }
            catch (Exception ex) when (mainCatch)
            {
                Console.WriteLine();
                Console.WriteLine("#### Error: " + ex.Message);
                Console.WriteLine();
                Console.WriteLine(HelpText);
            }

            if (!(noPause ?? false))
            {
                Console.WriteLine();
                Console.WriteLine("Press any key...");
                Console.ReadKey(intercept: true);
            }
        }

        private static void Script(string path)
        {
            Console.WriteLine();
            Console.WriteLine("The Talos Sigils Solver");

            string[] text = File.ReadAllLines(path: path);

            Board board = InputParser.Parse(text: text);

            var comp = new Computation(board, progressAction: DisplayProgress)
            {
                ProgressInterval = TimeSpan.FromSeconds(1)
            };

            bool ok = comp.Solve();

            PrintBoardDraft(board);

            if (ok)
            {
                text = BoardDrawing.TextFromBoard(board)
                    .Append("")
                    .Append("Status: OK")
                    .ToArray();
            }
            else
            {
                text = new[] { "Status: No solution" };
            }

            Console.WriteLine();
            PrintText(text);

            string slnPath = path + ".solution.txt";

            File.WriteAllLines(path: slnPath, text);

            Console.WriteLine();
            Console.WriteLine($"Solution file created: \"{slnPath}\"");
        }

        private static void DisplayProgress(Board board, double freq)
        {
            PrintBoardDraft(board);

            // statistics
            Console.WriteLine();
            Console.WriteLine($"freq = {freq:E2} Hz");
        }

        private static void PrintBoardDraft(Board board)
        {
            Console.WriteLine();
            Console.WriteLine(new string('=', count: 70));
            Console.WriteLine();

            int dim_x = board.Dim_x;
            int dim_y = board.Dim_y;

            char[] ar = new char[dim_x];

            for (int y = 0; y < dim_y; y++)
            {
                for (int x = 0; x < dim_x; x++)
                {
                    ar[x] = board.Get(x, y);
                }

                Console.WriteLine(new string(ar));
            }
        }

        private static void PrintText(string[] text)
        {
            foreach (string line in text)
            {
                Console.WriteLine(line);
            }
        }

        private static bool RemoveFlagArg(ref string[] args, string flag)
        {
            var lstArgs = new List<string>(args);

            bool f = lstArgs.Remove(flag);

            args = lstArgs.ToArray();

            return f;
        }

        const string HelpText = @"
usage: TalosSigils.exe [--no-pause] <input.txt>

    --no-pause ..... does not wait for a key press at the end (optional)
    <input.txt> .... path to a text file with description of a puzzle

    Note: The solution will be printed on the screen and also
          written to a text file <input.txt>.solution.txt
";
    }
}
