#nullable enable

using System;

namespace TalosSigils
{
    sealed class PassiveTimer
    {
        readonly Action m_action;
        TimeSpan m_interval;

        int m_counter = 0;
        DateTime m_prevActionTime;
        bool m_firstHit_1 = true;

        public PassiveTimer(Action action, TimeSpan interval)
        {
            m_action = action;
            m_interval = interval;
        }

        public TimeSpan Interval
        {
            get => m_interval;
            set => m_interval = value;
        }

        public void Hit()
        {
            if (unchecked(++m_counter & 0xFF) == 0)
            {
                Hit_1();
            }
        }

        private void Hit_1()
        {
            if (m_firstHit_1)
            {
                m_firstHit_1 = false;
                m_prevActionTime = GetTime();
            }

            DateTime t = GetTime();
            TimeSpan diff = t - m_prevActionTime;

            if (diff >= m_interval)
            {
                m_prevActionTime = t;

                m_action.Invoke();
            }
        }

        private static DateTime GetTime()
        {
            return DateTime.Now;
        }
    }
}
