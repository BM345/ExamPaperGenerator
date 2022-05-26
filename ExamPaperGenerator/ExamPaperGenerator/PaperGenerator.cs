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

        public PaperGenerator(Exam exam, QuestionsDatabase questionsDatabase)
        {
            Exam = exam;
            QuestionsDatabase = questionsDatabase;
            Papers = new List<Paper>();
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

                        direction += -(d2 - d1);
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

        public void GenerateNPapers(int n)
        {
            Papers.Clear();

            for (var m = 0; m < n; m++)
            {
                var paper = new Paper();

                paper.Exam = Exam;

                Papers.Add(paper);

                var tries = 0;
                var maximumTries = 1000;

                while (tries < maximumTries)
                {
                    var question1 = GetBestQuestionToAdd(paper);

                    if (CanAddQuestionToPaper(question1, paper))
                    {
                            paper.Questions.Add(question1);
                    }

                    tries++;
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

        public bool CheckPapersAgainstExamRules()
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
                }
            }

            Console.WriteLine($"Papers fail to pass {m} rules.");

            return m == 0;
        }
    }
}
