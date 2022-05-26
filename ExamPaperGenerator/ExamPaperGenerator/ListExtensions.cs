using System;
using System.Collections.Generic;
using System.Linq;

namespace ExamPaperGenerator
{
    public static class ListExtensions
    {
        public static IList<T> Shuffle<T>(this IList<T> list, Random random = null)
        {
            if (random == null)
            {
                random = new Random();
            }

            int n = list.Count;

            while (n > 1)
            {
                n--;
                int m = random.Next(n + 1);
                T v = list[m];
                list[m] = list[n];
                list[n] = v;
            }

            return list;
        }
    }
}
