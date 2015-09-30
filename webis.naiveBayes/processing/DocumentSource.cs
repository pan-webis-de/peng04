using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using webis.naiveBayes.logic;

namespace webis.naiveBayes.processing
{
    public class DocumentSource
    {
        public DocumentSource()
        {
            LanguageSegments = new List<string>();
        }

        public List<string> LanguageSegments { get; private set; }
        
        public int FindOccurrences(IEnumerable<string> ngram)
        {   
            int result = 0;

            string startSegment = ngram.First();
            string[] segments = LanguageSegments.ToArray();

            for (int i = 0; i < segments.Length; i++)
            {
                if (segments[i] == startSegment)
                {
                    int j = i;
                    bool foundMatch = true;

                    foreach (var item in ngram)
                    {
                        if (j >= segments.Length || segments[j] != item)
                        {
                            foundMatch = false;
                            break;
                        }

                        j++;
                    }

                    if (foundMatch) result++;
                }
            }

            return result;
        }
    }
}