using System;
namespace ExamPaperGenerator
{
    public class TagRule : CountRule
    {
        public string Tag { get; set; }

        public TagRule(string tag = "", int minimumAllowedNumberOfQuestions = 0, int maximumAllowedNumberOfQuestions = 0) : base(minimumAllowedNumberOfQuestions, maximumAllowedNumberOfQuestions)
        {
            Tag = tag;
        }

        public override string ToString()
        {
            return $"Tag Rule, '{Tag}', {MinimumAllowedNumberOfQuestions} to {MaximumAllowedNumberOfQuestions}.";
        }
    }
}
