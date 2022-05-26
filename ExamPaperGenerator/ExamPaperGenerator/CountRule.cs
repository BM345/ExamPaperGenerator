using System;
namespace ExamPaperGenerator
{
    public abstract class CountRule : Rule
    {
        public int MinimumAllowedNumberOfQuestions { get; set; }
        public int MaximumAllowedNumberOfQuestions { get; set; }

        public int Midpoint
        {
            get
            {
                return (int)Math.Round((MinimumAllowedNumberOfQuestions + MaximumAllowedNumberOfQuestions) / 2.0);
            }
        }

        public bool IsInRange(int n)
        {
            return n >= MinimumAllowedNumberOfQuestions && n <= MaximumAllowedNumberOfQuestions;
        }

        public CountRule(int minimumAllowedNumberOfQuestions = 0, int maximumAllowedNumberOfQuestions = 0)
        {
            MinimumAllowedNumberOfQuestions = minimumAllowedNumberOfQuestions;
            MaximumAllowedNumberOfQuestions = maximumAllowedNumberOfQuestions;
        }
    }
}
