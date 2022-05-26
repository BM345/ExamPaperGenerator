using System;
namespace ExamPaperGenerator
{
    public class TotalCountRule : CountRule
    {
        public TotalCountRule(int minimumAllowedNumberOfQuestions = 0, int maximumAllowedNumberOfQuestions = 0) : base(minimumAllowedNumberOfQuestions, maximumAllowedNumberOfQuestions)
        {
        }

        public override string ToString()
        {
            return $"Total Count Rule, {MinimumAllowedNumberOfQuestions} to {MaximumAllowedNumberOfQuestions}.";
        }
    }
}
