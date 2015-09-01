using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webis.naiveBayes.processing
{
    public interface ILanguageProcessor
    {
        TextSource Process(IEnumerable<string> documents);

        TextSource Process(IEnumerable<string> documents, string name);
    }
}