using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace ExamPaperGenerator
{
    public class PaperGenerator
    {
        public Exam Exam { get; set; }
        public QuestionsDatabase QuestionsDatabase { get; set; }
        public List<Paper> Papers { get; set; }
        public Random Random { get; set; }

        public PaperGenerator(Exam exam, QuestionsDatabase questionsDatabase, Random random)
        {
            Exam = exam;
            QuestionsDatabase = questionsDatabase;
            Papers = new List<Paper>();
            Random = random;
        }

        public bool QuestionHasBeenUsed(string questionId)
        {
            return Papers.Any(p => p.ContainsQuestion(questionId));
        }

        public IEnumerable<Question> AllQuestionsUsed()
        {
            var allQuestions = new List<Question>();

            foreach (var paper in Papers)
            {
                allQuestions = allQuestions.Concat(paper.Questions).ToList();
            }

            return allQuestions;
        }

        protected bool CanAddQuestionToPaper(Question question, Paper paper)
        {
            var maximumAllowedNumberOfQuestions = Exam.Rules.Where(r => r is TotalCountRule).Select(r => r as TotalCountRule).First().MaximumAllowedNumberOfQuestions;

            if (paper.NumberOfQuestions >= maximumAllowedNumberOfQuestions)
            {
                return false;
            }

            if (paper.ContainsQuestion(question.Id))
            {
                return false;
            }

            if (question.ExclusionGroupTag.IsIn(paper.ExclusionGroupTags))
            {
                return false;
            }

            return true;
        }

        protected int CalculateDirection(Question question, Paper paper)
        {
            var direction = 0;

            foreach (var rule in Exam.Rules)
            {
                if (rule is TagRule)
                {
                    var r = rule as TagRule;

                    var n1 = paper.NumberOfQuestionsWithTag(r.Tag);
                    var n2 = n1 + (question.HasTag(r.Tag) ? 1 : 0);

                    if (r.IsInRange(n1) && r.IsInRange(n2))
                    {
                        direction += 0;
                    }
                    else
                    {
                        var d1 = Math.Abs(n1 - r.Midpoint);
                        var d2 = Math.Abs(n2 - r.Midpoint);

                        direction += -(d2 - d1);
                    }
                }
                if (rule is LessonGroupRule)
                {
                    var r = rule as LessonGroupRule;

                    var n1 = paper.NumberOfQuestionsInLessons(r.LessonIds);
                    var n2 = n1 + (question.LessonId.IsIn(r.LessonIds) ? 1 : 0);

                    if (r.IsInRange(n1) && r.IsInRange(n2))
                    {
                        direction += 0;
                    }
                    else
                    {
                        var d1 = Math.Abs(n1 - r.Midpoint);
                        var d2 = Math.Abs(n2 - r.Midpoint);

                        direction += -(d2 - d1) * 1;
                    }
                }
            }

            return direction;
        }

        public Question GetBestQuestionToAdd(Paper paper)
        {
            var questionsAndDirections = new List<Tuple<Question, int>>();

            foreach (var question in QuestionsDatabase.GetNRandomQuestions(10, AllQuestionsUsed()))
            {
                var direction = CalculateDirection(question, paper);

                questionsAndDirections.Add(new Tuple<Question, int>(question, direction));
            }

            if (questionsAndDirections.Count() > 0)
            {
                return questionsAndDirections.OrderBy(qad => qad.Item2).Last().Item1;
            }

            return null;
        }

        public Paper GetBestPaperToAddQuestionTo(Question question)
        {
            var papersAndDirections = new List<Tuple<Paper, int>>();

            var highestDirection = -100;

            foreach (var paper in Papers)
            {
                if (CanAddQuestionToPaper(question, paper))
                {
                    var direction = CalculateDirection(question, paper);

                    if (direction > highestDirection)
                    {
                        highestDirection = direction;
                    }

                    papersAndDirections.Add(new Tuple<Paper, int>(paper, direction));
                }
            }

            if (papersAndDirections.Count() > 0)
            {
                return papersAndDirections.Where(pad => pad.Item2 == highestDirection).ToList().Shuffle(Random).First().Item1;
            }

            return null;
        }

        public List<List<string>> GetStrictestLessonGroups()
        {
            return Exam.Rules.Where(r => r is LessonGroupRule).Select(r => r as LessonGroupRule).OrderBy(r => r.Range).Select(r => r.LessonIds.ToList()).ToList();
        }

        public Dictionary<string, int> GetLessonGroupStrictnesses()
        {
            var strictnesses = new Dictionary<string, int>();

            var n = 1;

            foreach(var lessonGroup in GetStrictestLessonGroups())
            {
                foreach (var lesson in lessonGroup)
                {
                    strictnesses[lesson] = n;
                }

                n++;
            }

            return strictnesses;
        }

        public void GenerateNPapers(int n, double p1 = 0.3, double p2 = 0.3)
        {
            Papers.Clear();

            for (var m = 0; m < n; m++)
            {
                var paper = new Paper();

                paper.Exam = Exam;

                Papers.Add(paper);
            }

            var questionsByRarity = QuestionsDatabase.GetQuestionsByRarity();
            var strictnesses = GetLessonGroupStrictnesses();

            var p3 = Random.NextDouble() * (p2 - p1) + p1;

            questionsByRarity = questionsByRarity.OrderBy(q => strictnesses[q.LessonId]).ThenBy(q => q.Rarity * (Random.NextDouble() * p3  + 1));

            foreach (var question in questionsByRarity)
            {
                var paper = GetBestPaperToAddQuestionTo(question);

                if (paper != null)
                {
                    var direction = CalculateDirection(question, paper);

                    paper.Questions.Add(question);
                }
            }
        }

        public void ExportPapers(string folderPath)
        {
            var n = 1;

            foreach (var paper in Papers)
            {
                PaperExporter.ExportPaper(paper, Path.Combine(folderPath, "paper" + n + ".xml"));

                n++;
            }
        }

        public int CheckPapersAgainstExamRules()
        {
            var m = 0;

            foreach (var rule in Exam.Rules)
            {
                var n = 1;

                foreach (var paper in Papers)
                {
                    if (rule is TagRule)
                    {
                        var r = rule as TagRule;

                        var p = paper.NumberOfQuestionsWithTag(r.Tag);
                        var ll = r.MinimumAllowedNumberOfQuestions;
                        var ul = r.MaximumAllowedNumberOfQuestions;

                        if (p >= ll && p <= ul)
                        {
                            Console.WriteLine($"Paper {n} has the right number of questions with the tag {r.Tag} (has {p}, should have {ll}-{ul}).");
                        }
                        else
                        {
                            Console.WriteLine($"Paper {n} does NOT have the right number of questions with the tag {r.Tag} (has {p}, should have {ll}-{ul}).");
                            m++;
                        }
                    }
                    if (rule is LessonGroupRule)
                    {
                        var r = rule as LessonGroupRule;

                        var p = paper.NumberOfQuestionsInLessons(r.LessonIds);
                        var ll = r.MinimumAllowedNumberOfQuestions;
                        var ul = r.MaximumAllowedNumberOfQuestions;

                        if (p >= ll && p <= ul)
                        {
                            Console.WriteLine($"Paper {n} has the right number of questions from lessons {string.Join(", ", r.LessonIds)} (has {p}, should have {ll}-{ul}).");
                        }
                        else
                        {
                            Console.WriteLine($"Paper {n} does NOT have the right number of questions from lessons {string.Join(", ", r.LessonIds)} (has {p}, should have {ll}-{ul}).");
                            m++;
                        }
                    }
                    if (rule is TotalCountRule)
                    {
                        var r = rule as TotalCountRule;

                        var p = paper.NumberOfQuestions;
                        var ll = r.MinimumAllowedNumberOfQuestions;
                        var ul = r.MaximumAllowedNumberOfQuestions;

                        if (p >= ll && p <= ul)
                        {
                            Console.WriteLine($"Paper {n} has the right total number of questions (has {p}, should have {ll}-{ul}).");
                        }
                        else
                        {
                            Console.WriteLine($"Paper {n} does NOT have the right total number of questions (has {p}, should have {ll}-{ul}).");
                            m++;
                        }
                    }

                    n++;
                }
            }

            Console.WriteLine($"Papers fail to pass {m} rules.");

            return m;
        }
    }
}
