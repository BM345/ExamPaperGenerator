using System;
using System.Collections.Generic;
using System.Linq;

namespace ExamPaperGenerator
{
    public static class StringExtensions
    {
        public static bool IsIn(this string s, IEnumerable<string> e)
        {
            return e.Any(s1 => s1 == s);
        }
    }
}
