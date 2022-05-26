using System;
using System.Collections.Generic;

namespace ExamPaperGenerator
{
    public class Exam
    {
        public string Id { get; set; }
        public string Board { get; set; }
        public IList<Rule> Rules { get; set; }

        public Exam()
        {
            Rules = new List<Rule>();
        }
    }
}
