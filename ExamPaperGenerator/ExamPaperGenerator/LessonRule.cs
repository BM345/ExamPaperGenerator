using System;
namespace ExamPaperGenerator
{
    public class LessonRule : CountRule
    {
        public string LessonId { get; set; }

        public LessonRule(string lessonId = "", int minimumAllowedNumberOfQuestions = 0, int maximumAllowedNumberOfQuestions = 0) : base(minimumAllowedNumberOfQuestions, maximumAllowedNumberOfQuestions)
        {
            LessonId = lessonId;
        }
    }
}
