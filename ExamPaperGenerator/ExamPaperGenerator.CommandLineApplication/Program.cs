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

            var random = new Random(1);

            var questionsDatabase = new QuestionsDatabase(questions, random);
            var paperGenerator = new PaperGenerator(exam, questionsDatabase, random);

            var numberOfTries = 0;
            var maximumNumberOfTries = 2;
            var bestM = 1000;

            while (numberOfTries < maximumNumberOfTries)
            {
                random = new Random(numberOfTries);

                questionsDatabase.Random = random;
                paperGenerator.Random = random;

                paperGenerator.GenerateNPapers(21);
                var m = paperGenerator.CheckPapersAgainstExamRules();

                if (m < bestM)
                {
                    bestM = m;
                }

                Console.WriteLine($"Best case: {bestM}.");

                if (m == 0)
                {
                    break;
                }

                numberOfTries++;
            }


            paperGenerator.ExportPapers(@"/Users/benjaminmilnes/Documents/GitHub/NagwaSchemas/exam_profile/alternate_method");

            Console.WriteLine($"{paperGenerator.AllQuestionsUsed().Count()} questions used.");
            Console.WriteLine($"Best case: {bestM}.");
        }
    }
}
