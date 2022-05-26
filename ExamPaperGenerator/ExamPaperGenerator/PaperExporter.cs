﻿using System;
using System.Xml;

namespace ExamPaperGenerator
{
    public class PaperExporter
    {
        public PaperExporter()
        {
        }

        public static void ExportPaper(Paper paper, string filePath)
        {
            var xmlDocument = new XmlDocument();

            var xmlDeclaration = xmlDocument.CreateXmlDeclaration("1.0", "utf-8", null);
            var root = xmlDocument.DocumentElement;

            xmlDocument.InsertBefore(xmlDeclaration, root);

            var e1 = xmlDocument.CreateElement("paper");

            var e5 = xmlDocument.CreateElement("statistics");

            e5.InnerText += "\n";
            e5.InnerText += $"- Total number of questions in the paper: {paper.NumberOfQuestions}.\n";
            e5.InnerText += $"- Total number of unique questions in the paper: {paper.NumberOfUniqueQuestions}.\n";

            foreach(var rule in paper.Exam.Rules)
            {
                if (rule is TagRule)
                {
                    var r = rule as TagRule;

                    e5.InnerText += $"- Number of questions in the paper with the tag {r.Tag}: {paper.NumberOfQuestionsWithTag(r.Tag)} (should be {r.MinimumAllowedNumberOfQuestions}-{r.MaximumAllowedNumberOfQuestions}).\n";
                }
                if (rule is LessonGroupRule)
                {
                    var r = rule as LessonGroupRule;

                    e5.InnerText += $"- Number of questions in the paper from the lessons {string.Join(", ", r.LessonIds)}: {paper.NumberOfQuestionsInLessons(r.LessonIds)} (should be {r.MinimumAllowedNumberOfQuestions}-{r.MaximumAllowedNumberOfQuestions}).\n";
                }
            }

            e1.AppendChild(e5);

            foreach (var question in paper.Questions)
            {
                var e2 = xmlDocument.CreateElement("question");

                e2.SetAttribute("id", question.Id);
                e2.SetAttribute("lesson_id", question.LessonId);

                var e3 = xmlDocument.CreateElement("tags");

                foreach (var tag in question.Tags)
                {
                    var e4 = xmlDocument.CreateElement("tag");

                    e4.InnerText = tag;

                    e3.AppendChild(e4);
                }

                e2.AppendChild(e3);
                e1.AppendChild(e2);
            }

            xmlDocument.AppendChild(e1);

            xmlDocument.Save(filePath);
        }
    }
}
