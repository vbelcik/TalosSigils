#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            bool noMerge;

            try
            {
                noPause = RemoveFlagArg(args: ref args, flag: "--no-pause");
                noMerge = RemoveFlagArg(args: ref args, flag: "--no-merge");

                if (args.Length != 1)
                {
                    throw new Exception("One argument expected: the input text file path");
                }

                Script(path: args[0], mergeSigils: !noMerge);
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

        private static void Script(string path, bool mergeSigils)
        {
            Console.WriteLine();
            Console.WriteLine("The Talos Sigils Puzzle Solver");

            string[] text = File.ReadAllLines(path: path);

            Board board = InputParser.Parse(text: text, mergeSigils: mergeSigils);

            var comp = new Computation(board, progressAction: DisplayProgress)
            {
                ProgressInterval = TimeSpan.FromSeconds(1)
            };

            Stopwatch stopwatch = Stopwatch.StartNew();

            bool success = comp.Solve();

            stopwatch.Stop();

            PrintBoardDraft(board);

            text = success ? BoardDrawing.TextFromBoard(board) : Array.Empty<string>();

            text = text
                .Append("")
                .Append("Status: " + (success ? "Solution found" : "No solution"))
                .Append("Duration: " + FormatDuration(stopwatch.Elapsed))
                .ToArray();

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

            Console.WriteLine();
        }

        private static void PrintText(string[] text)
        {
            foreach (string line in text)
            {
                Console.WriteLine(line);
            }
        }

        private static string FormatDuration(TimeSpan duration)
        {
            return (duration >= TimeSpan.FromSeconds(20))
                ? $"{(long)duration.TotalSeconds} seconds"
                : $"{(long)duration.TotalMilliseconds} ms";
        }

        private static bool RemoveFlagArg(ref string[] args, string flag)
        {
            var lstArgs = new List<string>(args);

            bool f = lstArgs.Remove(flag);

            args = lstArgs.ToArray();

            return f;
        }

        const string HelpText = @"
usage: TalosSigils.exe [--no-pause] [--no-merge] <input.txt>

    --no-pause ..... does not wait for a key press at the end (optional)
    --no-merge ..... turn off optimization of sigils merging (optional)
    <input.txt> .... path to a text file with description of a puzzle

    Note: The solution will be printed on the screen and also
          written to a text file <input.txt>.solution.txt
";
    }
}
