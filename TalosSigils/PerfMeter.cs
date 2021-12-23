#nullable enable

using System;

namespace TalosSigils
{
    sealed class PerfMeter
    {
        DateTime m_startTime;
        long m_counter;

        public PerfMeter()
        {
            Reset();
        }

        public void Reset()
        {
            m_startTime = GetTime();
            m_counter = 0;
        }

        public void Tick()
        {
            m_counter++;
        }

        public double GetAvgFreq()
        {
            TimeSpan duration = GetTime() - m_startTime;

            double durationSecs = duration.TotalSeconds;

            return m_counter / durationSecs;
        }

        private static DateTime GetTime()
        {
            return DateTime.Now;
        }
    }
}
