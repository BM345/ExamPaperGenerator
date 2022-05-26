using System;
using System.Collections.Generic;
using System.Linq;

namespace ExamPaperGenerator
{
    public class QuestionsDatabase
    {
        public IList<Question> Questions { get; set; }
        protected Random Random { get; set; }

        public Dictionary<string, int> TagFrequencies { get; set; }
        public Dictionary<string, double> NormalisedTagFrequencies { get; set; }
        public Dictionary<string, int> LessonFrequencies { get; set; }
        public Dictionary<string, double> NormalisedLessonFrequencies { get; set; }
        public Dictionary<string, double> QuestionRarities { get; set; }

        public QuestionsDatabase(IList<Question> questions, Random random)
        {
            Questions = questions;
            Random = random;

            TagFrequencies = new Dictionary<string, int>();
            NormalisedTagFrequencies = new Dictionary<string, double>();
            LessonFrequencies = new Dictionary<string, int>();
            NormalisedLessonFrequencies = new Dictionary<string, double>();
            QuestionRarities = new Dictionary<string, double>();

            SetTagFrequencies();
            SetLessonFrequencies();
            SetQuestionRarities();
        }

        protected void SetTagFrequencies()
        {
            var totalNumberOfTags = 0.0;

            foreach (var question in Questions)
            {
                foreach (var tag in question.Tags)
                {
                    if (TagFrequencies.ContainsKey(tag))
                    {
                        TagFrequencies[tag]++;
                    }
                    else
                    {
                        TagFrequencies[tag] = 1;
                    }

                    totalNumberOfTags++;
                }
            }

            foreach (var tag in TagFrequencies)
            {
                NormalisedTagFrequencies[tag.Key] = ((double)tag.Value) / totalNumberOfTags;
            }
        }

        protected void SetLessonFrequencies()
        {
            foreach (var question in Questions)
            {
                if (LessonFrequencies.ContainsKey(question.LessonId))
                {
                    LessonFrequencies[question.LessonId]++;
                }
                else
                {
                    LessonFrequencies[question.LessonId] = 1;
                }
            }

            foreach (var lesson in LessonFrequencies)
            {
                NormalisedLessonFrequencies[lesson.Key] = ((double)lesson.Value) / Questions.Count();
            }
        }

        protected void SetQuestionRarities()
        {
            foreach (var question in Questions)
            {
                var rarity = 1.0;

                foreach (var tag in question.Tags)
                {
                    if (!tag.StartsWith("group:"))
                    {
                        var f1 = NormalisedTagFrequencies[tag];

                        rarity = rarity * f1;
                    }
                }

                var f2 = NormalisedLessonFrequencies[question.LessonId];

                rarity = rarity * f2;

                QuestionRarities[question.Id] = rarity;
                question.Rarity = rarity;
            }
        }

        public double GetQuestionRarity(Question question)
        {
            return QuestionRarities[question.Id];
        }

        public IEnumerable<Question> GetQuestionsByRarity()
        {
            return Questions.OrderBy(q => q.Rarity);
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
            var excludingIds = excluding.Select(q => q.Id).ToList();

            return Questions.Where(q => q.Id.IsNotIn(excludingIds));
        }



        public Question GetRandomQuestion()
        {
            return RandomChoice(Questions);
        }

        public Question GetRandomQuestion(IEnumerable<Question> excluding)
        {
            var excludingIds = excluding.Select(q => q.Id).ToList();

            return RandomChoice(Questions.Where(q => q.Id.IsNotIn(excludingIds)).ToList());
        }

        public IEnumerable<Question> GetNRandomQuestions(int n, IEnumerable<Question> excluding)
        {
            var excludingIds = excluding.Select(q => q.Id).ToList();

            return Questions.Where(q => q.Id.IsNotIn(excludingIds)).ToList().Shuffle(Random).Take(n);
        }



        public IEnumerable<Question> GetQuestionsInLesson(string lessonId)
        {
            return Questions.Where(q => q.LessonId == lessonId);
        }

        public IEnumerable<Question> GetQuestionsInLesson(string lessonId, IEnumerable<Question> excluding)
        {
            var excludingIds = excluding.Select(q => q.Id).ToList();

            return Questions.Where(q => q.LessonId == lessonId && q.Id.IsNotIn(excludingIds));
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
            return Questions.Where(q => q.LessonId.IsIn(lessonIds));
        }

        public IEnumerable<Question> GetQuestionsInLessons(IEnumerable<string> lessonIds, IEnumerable<Question> excluding)
        {
            var excludingIds = excluding.Select(q => q.Id).ToList();

            return Questions.Where(q => q.LessonId.IsIn(lessonIds) && q.Id.IsNotIn(excludingIds));
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
            return Questions.Where(q => tag.IsIn(q.Tags));
        }

        public IEnumerable<Question> GetQuestionsWithTag(string tag, IEnumerable<Question> excluding)
        {
            var excludingIds = excluding.Select(q => q.Id).ToList();

            return Questions.Where(q => tag.IsIn(q.Tags) && q.Id.IsNotIn(excludingIds));
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
            var excludingIds = excluding.Select(q => q.Id).ToList();

            return Questions.Where(q => !tags.Where(t1 => !q.Tags.Any(t2 => t2 == t1)).Any() && q.Id.IsNotIn(excludingIds));
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
