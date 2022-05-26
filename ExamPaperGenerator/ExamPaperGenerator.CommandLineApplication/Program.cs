using System;
using System.Linq;
using ExamPaperGenerator;

namespace ExamPaperGenerator.CommandLineApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            var questions = QuestionsImporter.ImportQuestions(@"/Users/benjaminmilnes/Documents/GitHub/NagwaSchemas/exam_profile/alternate_method/questions-geology.xml");
            var exam = ExamImporter.ImportExam(@"/Users/benjaminmilnes/Documents/GitHub/NagwaSchemas/exam_profile/alternate_method/recipe-geology.xml");

            var questionsDatabase = new QuestionsDatabase(questions, new Random(1));
            var paperGenerator = new PaperGenerator(exam, questionsDatabase);

            paperGenerator.GenerateNPapers(21);
            paperGenerator.CheckPapersAgainstExamRules();
            paperGenerator.ExportPapers(@"/Users/benjaminmilnes/Documents/GitHub/NagwaSchemas/exam_profile/alternate_method");

            Console.WriteLine($"{paperGenerator.AllQuestionsUsed().Count()} questions used.");
        }
    }
}
