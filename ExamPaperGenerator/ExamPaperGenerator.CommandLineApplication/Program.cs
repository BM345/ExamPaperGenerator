using System;
using System.Linq;
using ExamPaperGenerator;

namespace ExamPaperGenerator.CommandLineApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var questions = QuestionsImporter.ImportQuestions(@"/Users/benjaminmilnes/Documents/GitHub/NagwaSchemas/exam_profile/alternate_method/questions-geology.xml");

            foreach (var question in questions)
            {
                Console.WriteLine($"{question.Id}, {question.LessonId}");

                foreach (var tag in question.Tags)
                {
                    Console.WriteLine($"\t{tag}");
                }
            }

            var exam = ExamImporter.ImportExam(@"/Users/benjaminmilnes/Documents/GitHub/NagwaSchemas/exam_profile/alternate_method/recipe-geology.xml");

            foreach(var rule in exam.Rules)
            {
                Console.WriteLine(rule);
            }

            var questionsDatabase = new QuestionsDatabase(questions, new Random(1));
            var paperGenerator = new PaperGenerator(exam, questionsDatabase);

            paperGenerator.GenerateNPapers(21);
            paperGenerator.CheckPapersAgainstExamRules();
            paperGenerator.ExportPapers(@"/Users/benjaminmilnes/Documents/GitHub/NagwaSchemas/exam_profile/alternate_method");
        }
    }
}
