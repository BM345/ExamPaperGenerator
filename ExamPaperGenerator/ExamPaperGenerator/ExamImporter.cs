using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace ExamPaperGenerator
{
    public class ExamImporter
    {
        protected static string GetXMLAttribute(XmlElement xmlElement, string attributeName, string defaultValue)
        {
            if (xmlElement.HasAttribute(attributeName))
            {
                var attributeValue = xmlElement.GetAttribute(attributeName);

                return attributeValue;
            }

            return defaultValue;
        }

        protected static int GetXMLAttribute(XmlElement xmlElement, string attributeName, int defaultValue)
        {
            if (xmlElement.HasAttribute(attributeName))
            {
                var attributeValue = int.Parse(xmlElement.GetAttribute(attributeName));

                return attributeValue;
            }

            return defaultValue;
        }

        public static Exam ImportExam(string filePath)
        {
            var xmlDocument = new XmlDocument();

            xmlDocument.Load(filePath);

            var exam = new Exam();

            var a5 = GetXMLAttribute(xmlDocument.DocumentElement, "no_questions", 0);

            var totalCountRule = new TotalCountRule(a5, a5);

            exam.Rules.Add(totalCountRule);

            foreach (var xmlElement1 in xmlDocument.SelectNodes("/paper/questions/question").OfType<XmlElement>())
            {
                var tagRule = new TagRule();

                var a1 = GetXMLAttribute(xmlElement1, "category", "");
                var a2 = GetXMLAttribute(xmlElement1, "value", "");
                var a3 = GetXMLAttribute(xmlElement1, "min", 0);
                var a4 = GetXMLAttribute(xmlElement1, "max", -1);

                if (a1 == "figures" && a2 == "none")
                {
                    tagRule.Tag = "shallow:does_not_have_figure";
                }
                if (a1 == "figures" && a2 == "diagram")
                {
                    tagRule.Tag = "shallow:has_diagram";
                }
                if (a1 == "figures" && a2 == "plot")
                {
                    tagRule.Tag = "shallow:has_plot";
                }
                if (a1 == "figures" && a2 == "image")
                {
                    tagRule.Tag = "shallow:has_image";
                }

                if (a1 == "skill" && a2 == "recall")
                {
                    tagRule.Tag = "bloom:recall";
                }
                if (a1 == "skill" && a2 == "apply")
                {
                    tagRule.Tag = "bloom:apply";
                }
                if (a1 == "skill" && a2 == "analyze")
                {
                    tagRule.Tag = "bloom:analyse";
                }

                if (a1 == "concept" && a2 == "single")
                {
                    tagRule.Tag = "subjective:single_concept";
                }
                if (a1 == "concept" && a2 == "multiple")
                {
                    tagRule.Tag = "subjective:multiple_concept";
                }

                tagRule.MinimumAllowedNumberOfQuestions = a3;
                tagRule.MaximumAllowedNumberOfQuestions = a4;

                exam.Rules.Add(tagRule);
            }

            foreach (var xmlElement1 in xmlDocument.SelectNodes("/paper/lessons/cluster").OfType<XmlElement>())
            {
                var lessonGroupRule = new LessonGroupRule();

                var a1 = GetXMLAttribute(xmlElement1, "min", 0);
                var a2 = GetXMLAttribute(xmlElement1, "max", -1);

                foreach (var xmlElement2 in xmlElement1.SelectNodes("./lesson").OfType<XmlElement>())
                {
                    var a3 = GetXMLAttribute(xmlElement2, "id", "");

                    lessonGroupRule.LessonIds.Add(a3);
                }

                lessonGroupRule.MinimumAllowedNumberOfQuestions = a1;
                lessonGroupRule.MaximumAllowedNumberOfQuestions = a2;

                exam.Rules.Add(lessonGroupRule);
            }

            return exam;
        }
    }
}
