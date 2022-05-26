using System;
using System.Collections.Generic;
using System.Linq;

namespace ExamPaperGenerator
{
    public class Paper
    {
        public Exam Exam { get; set; }
        public IList<Question> Questions { get; set; }

        public Paper()
        {
            Questions = new List<Question>();
        }

        public int NumberOfQuestions
        {
            get
            {
                return Questions.Count;
            }
        }

        public int NumberOfUniqueQuestions
        {
            get
            {
                return Questions.Select(q => q.Id).Distinct().Count();
            }
        }

        public bool ContainsQuestion(string questionId)
        {
            return Questions.Any(q => q.Id == questionId);
        }

        public int NumberOfQuestionsInLesson(string lessonId)
        {
            return Questions.Where(q => q.LessonId == lessonId).Count();
        }

        public int NumberOfQuestionsInLessons(IEnumerable<string> lessonIds)
        {
            return Questions.Where(q => q.LessonId.IsIn(lessonIds)).Count();
        }

        public int NumberOfQuestionsWithTag(string tag)
        {
            return Questions.Where(q => tag.IsIn(q.Tags)).Count();
        }

        public List<string> AllTags
        {
            get
            {
                var tags = new List<string>();

                foreach (var question in Questions)
                {
                    tags = tags.Concat(question.Tags).ToList();
                }

                return tags.Distinct().ToList();
            }
        }

        public List<string> ExclusionGroupTags
        {
            get
            {
                var tags = Questions.Select(q => q.ExclusionGroupTag).Where(t => t != "").Distinct().ToList();

                return tags;
            }
        }
    }
}
