using System;

namespace GlumOrigins.Common.Logging
{
    [Flags]
    public enum LoggerDestination
    {
        Output = 1,
        File = 2,
        All = File | Output
    }
}
