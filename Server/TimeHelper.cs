using System;
using System.Diagnostics;

namespace GlumOrigins.Server
{
    public static class TimeHelper
    {
        public static long NanoTime
        {
            get
            {
                long nano = 10000L * Stopwatch.GetTimestamp();
                nano /= TimeSpan.TicksPerMillisecond;
                nano *= 100L;
                return nano;
            }
        }
    }
}
