using System;
using System.Collections.Generic;
using System.Linq;

namespace ExamPaperGenerator
{
    public class LessonGroupRule : CountRule
    {
        public IList<string> LessonIds { get; set; }

        public LessonGroupRule(IEnumerable<string> lessonIds = null, int minimumAllowedNumberOfQuestions = 0, int maximumAllowedNumberOfQuestions = 0) : base(minimumAllowedNumberOfQuestions, maximumAllowedNumberOfQuestions)
        {
            if (lessonIds == null)
            {
                LessonIds = new List<string>();
            }
            else
            {
                LessonIds = lessonIds.ToList();
            }
        }

        public override string ToString()
        {
            return $"Lesson Group Rule, {string.Join(", ", LessonIds)}, {MinimumAllowedNumberOfQuestions} to {MaximumAllowedNumberOfQuestions}.";
        }
    }
}
