using System;
using System.Collections.Generic;
using System.Linq;

namespace ExamPaperGenerator
{
    public class Question
    {
        public string Id { get; set; }
        public string LessonId { get; set; }
        public IList<string> Tags { get; set; }
        public double Rarity { get; set; }

        public Question()
        {
            Tags = new List<string>();
        }

        public bool HasTag(string tag)
        {
            return tag.IsIn(Tags);
        }

        public IList<string> GetTagsWithPrefix(string prefix)
        {
            return Tags.Where(t => t.StartsWith(prefix)).ToList();
        }

        public string ExclusionGroupTag
        {
            get
            {
                var tags = GetTagsWithPrefix("group:");

                if (tags.Any())
                {
                    return tags.First();
                }
                else
                {
                    return "";
                }
            }
        }

        public bool IsInExclusionGroup()
        {
            return ExclusionGroupTag != "";
        }
    }
}
