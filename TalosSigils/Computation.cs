#nullable enable

using System;
using System.Collections.Generic;

namespace TalosSigils
{
    delegate void CompProgressDelegate(Board board, double freq);

    sealed class Computation
    {
        readonly Board m_board;

        readonly PerfMeter m_sigilPut_PerfMeter = new();
        readonly PassiveTimer m_passiveTimer;

        public Computation(Board board, CompProgressDelegate progressAction)
        {
            m_board = board;

            m_passiveTimer = new PassiveTimer(
                action: Progress, 
                interval: TimeSpan.FromSeconds(1));

            void Progress()
            {
                // statistics
                double freq = m_sigilPut_PerfMeter.GetAvgFreq();

                progressAction.Invoke(m_board, freq: freq);
            }
        }

        public TimeSpan ProgressInterval
        {
            get => m_passiveTimer.Interval;
            set => m_passiveTimer.Interval = value;
        }

        public bool Solve()
        {
            var poolStack = new Stack<Putting>();
            var slnStack = new Stack<Putting>();

            var p = new Putting(m_board);

            p.ResetToRoot();

            m_sigilPut_PerfMeter.Reset();

            while (true)
            {
                m_passiveTimer.Hit();

                if (p.CanPutSigil())
                {
                    m_sigilPut_PerfMeter.Tick();
                    p.PutSigil();
                    slnStack.Push(p);

                    Putting p2 = (poolStack.Count > 0) 
                        ? poolStack.Pop() 
                        : new Putting(m_board);

                    if (!p2.FindNextFreePlaceXY(prevPutting: p))
                    {
                        // whole board is filled
                        return true;    // hooray, solution
                    }

                    if (!p2.FirstTrial())
                    {
                        // we have nothing to fill the free place
                        return false;   // no solution
                    }

                    p = p2;
                    continue;
                }

                A_0:

                if (!p.NextTrial())
                {
                    poolStack.Push(p);

                    if (slnStack.Count > 0)
                    {
                        p = slnStack.Pop();
                        p.TakeSigil();
                        goto A_0;
                    }
                    else
                    {
                        // whole state space has been exhausted
                        return false;   // no solution
                    }
                }
            }
        }
    }
}
