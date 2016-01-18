using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace peng04.processing
{
    public class WordLevelProcessor : ILanguageProcessor
    {
        public TextSource Process(IEnumerable<string> documents)
        {
            return Process(documents, string.Empty);
        }

        public TextSource Process(IEnumerable<string> documents, string name)
        {
            TextSource result = new TextSource();

            foreach (var item in documents)
            {
                var doc = new DocumentSource();
                var sItem = item.Replace(".", " . ")
                                .Replace(",", " , ")
                                .Replace("\r\n", " \r##n ")
                                .Replace("\n", " \n ")
                                .Replace(",", " , ")
                                .Replace("##n", "\n");
                // consider punctuation marks/new line as seperate words

                foreach (var segment in sItem.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    doc.LanguageSegments.Add(segment);
                }

                result.Documents.Add(doc);
            }

            result.Name = name;
            return result;
        }
    }
}