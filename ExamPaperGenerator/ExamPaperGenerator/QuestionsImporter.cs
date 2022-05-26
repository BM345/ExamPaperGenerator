using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace ExamPaperGenerator
{
    public class QuestionsImporter
    {
        public QuestionsImporter()
        {
        }

        protected static string GetXMLAttribute(XmlElement xmlElement, string attributeName, string defaultValue)
        {
            if (xmlElement.HasAttribute(attributeName))
            {
                var attributeValue = xmlElement.GetAttribute(attributeName);

                return attributeValue;
            }

            return defaultValue;
        }

        protected static Question GetQuestion(XmlElement xmlElement, string lessonId)
        {
            var question = new Question();

            question.LessonId = lessonId;
            question.Id = GetXMLAttribute(xmlElement, "id", "");

            var a1 = GetXMLAttribute(xmlElement, "format", "");
            var a2 = GetXMLAttribute(xmlElement, "figures", "");
            var a3 = GetXMLAttribute(xmlElement, "concept", "");
            var a4 = GetXMLAttribute(xmlElement, "skill", "");

            if (a1 == "qm")
            {
                question.Tags.Add("shallow:has_question_mark");
            }
            if (a1 == "fib")
            {
                question.Tags.Add("shallow:has_fillable_space");
            }
            if (a2 == "none")
            {
                question.Tags.Add("shallow:does_not_have_figure");
            }

            if (a2 == "diagram")
            {
                question.Tags.Add("shallow:has_figure");
                question.Tags.Add("shallow:has_diagram");
            }
            if (a2 == "plot")
            {
                question.Tags.Add("shallow:has_figure");
                question.Tags.Add("shallow:has_plot");
            }
            if (a2 == "image")
            {
                question.Tags.Add("shallow:has_figure");
                question.Tags.Add("shallow:has_image");
            }

            if (a3 == "single")
            {
                question.Tags.Add("subjective:single_concept");
            }
            if (a3 == "multiple")
            {
                question.Tags.Add("subjective:multiple_concept");
            }

            if (a4 == "recall")
            {
                question.Tags.Add("bloom:recall");
            }
            if (a4 == "apply")
            {
                question.Tags.Add("bloom:apply");
            }
            if (a4 == "analyze")
            {
                question.Tags.Add("bloom:analyse");
            }

            return question;
        }

        public static IList<Question> ImportQuestions(string filePath)
        {
            var xmlDocument = new XmlDocument();

            xmlDocument.Load(filePath);

            var questions = new List<Question>();

            var groupNumber = 0;

            foreach (var xmlElement1 in xmlDocument.SelectNodes("/lessons/lesson").OfType<XmlElement>())
            {
                var lessonId = GetXMLAttribute(xmlElement1, "id", "");

                foreach (var xmlElement2 in xmlElement1.SelectNodes("./question").OfType<XmlElement>())
                {
                    questions.Add(GetQuestion(xmlElement2, lessonId));
                }

                foreach (var xmlElement3 in xmlElement1.SelectNodes("./group").OfType<XmlElement>())
                {
                    groupNumber++;

                    foreach (var xmlElement4 in xmlElement3.SelectNodes("./question").OfType<XmlElement>())
                    {
                        var question = GetQuestion(xmlElement4, lessonId);

                        question.Tags.Add($"group:{groupNumber}");

                        questions.Add(question);
                    }
                }
            }

            return questions;
        }
    }
}
