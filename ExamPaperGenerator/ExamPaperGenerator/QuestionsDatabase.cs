using System;
using System.Collections.Generic;
using System.Linq;

namespace ExamPaperGenerator
{
    public class QuestionsDatabase
    {
        public IList<Question> Questions { get; set; }
        protected Random Random { get; set; }

        public QuestionsDatabase(IList<Question> questions, Random random)
        {
            Questions = questions;
            Random = random;
        }

        protected int RandomNumberBetween(int lowerLimit, int upperLimit)
        {
            return (int)Math.Round(Random.NextDouble() * (upperLimit - lowerLimit)) + lowerLimit;
        }

        protected T RandomChoice<T>(IList<T> objects)
        {
            var n = RandomNumberBetween(0, objects.Count() - 1);

            return objects[n];
        }

        public IEnumerable<Question> GetQuestionsExcluding(IEnumerable<Question> excluding)
        {
            return Questions.Where(q => !excluding.Any(q2 => q2.Id == q.Id));
        }

        public Question GetRandomQuestion()
        {
            return RandomChoice(Questions);
        }

        public Question GetRandomQuestion(IEnumerable<Question> excluding)
        {
            var excludingIds = excluding.Select(q => q.Id).ToList();

            return RandomChoice(Questions.Where(q => !q.Id.IsIn(excludingIds)).ToList());
        }

        public IEnumerable<Question> GetNRandomQuestions(int n, IEnumerable<Question> excluding)
        {
            var excludingIds = excluding.Select(q => q.Id).ToList();

            return Questions.Where(q => !q.Id.IsIn(excludingIds)).ToList().Shuffle(Random).Take(n);
        }

        public IEnumerable<Question> GetQuestionsInLesson(string lessonId)
        {
            return Questions.Where(q => q.LessonId == lessonId);
        }

        public IEnumerable<Question> GetQuestionsInLesson(string lessonId, IEnumerable<Question> excluding)
        {
            return Questions.Where(q => q.LessonId == lessonId && !excluding.Any(q2 => q2.Id == q.Id));
        }

        public Question GetRandomQuestionInLesson(string lessonId)
        {
            return RandomChoice(GetQuestionsInLesson(lessonId).ToList());
        }

        public Question GetRandomQuestionInLesson(string lessonId, IEnumerable<Question> excluding)
        {
            return RandomChoice(GetQuestionsInLesson(lessonId, excluding).ToList());
        }

        public IEnumerable<Question> GetQuestionsInLessons(IEnumerable<string> lessonIds)
        {
            return Questions.Where(q => lessonIds.Any(lid => q.LessonId == lid));
        }

        public IEnumerable<Question> GetQuestionsInLessons(IEnumerable<string> lessonIds, IEnumerable<Question> excluding)
        {
            return Questions.Where(q => lessonIds.Any(lid => q.LessonId == lid) && !excluding.Any(q2 => q2.Id == q.Id));
        }

        public Question GetRandomQuestionInLessons(IEnumerable<string> lessonIds)
        {
            return RandomChoice(GetQuestionsInLessons(lessonIds).ToList());
        }

        public Question GetRandomQuestionInLessons(IEnumerable<string> lessonIds, IEnumerable<Question> excluding)
        {
            return RandomChoice(GetQuestionsInLessons(lessonIds, excluding).ToList());
        }

        public IEnumerable<Question> GetQuestionsWithTag(string tag)
        {
            return Questions.Where(q => q.Tags.Any(t => t == tag));
        }

        public IEnumerable<Question> GetQuestionsWithTag(string tag, IEnumerable<Question> excluding)
        {
            return Questions.Where(q => q.Tags.Any(t => t == tag) && !excluding.Any(q2 => q2.Id == q.Id));
        }

        public Question GetRandomQuestionWithTag(string tag)
        {
            return RandomChoice(GetQuestionsWithTag(tag).ToList());
        }

        public Question GetRandomQuestionWithTag(string tag, IEnumerable<Question> excluding)
        {
            return RandomChoice(GetQuestionsWithTag(tag, excluding).ToList());
        }

        public IEnumerable<Question> GetQuestionsWithTags(IEnumerable<string> tags)
        {
            return Questions.Where(q => !tags.Where(t1 => !q.Tags.Any(t2 => t2 == t1)).Any());
        }

        public IEnumerable<Question> GetQuestionsWithTags(IEnumerable<string> tags, IEnumerable<Question> excluding)
        {
            return Questions.Where(q => !tags.Where(t1 => !q.Tags.Any(t2 => t2 == t1)).Any() && !excluding.Any(q2 => q2.Id == q.Id));
        }

        public Question GetRandomQuestionWithTags(IEnumerable<string> tags)
        {
            return RandomChoice(GetQuestionsWithTags(tags).ToList());
        }

        public Question GetRandomQuestionWithTags(IEnumerable<string> tags, IEnumerable<Question> excluding)
        {
            return RandomChoice(GetQuestionsWithTags(tags, excluding).ToList());
        }
    }
}
