using System.Linq;

namespace GlumOrigins.Common.Helpers
{
    public static class StringHelper
    {
        public const string Space = " ";
        public const string Overline = "‾";

        public static string Multiply(this string a, int length)
        {
            return string.Concat(Enumerable.Repeat(a, length));
        }
    }
}
