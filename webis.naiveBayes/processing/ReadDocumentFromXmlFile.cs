using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webis.naiveBayes.processing
{
    public class ReadDocumentFromXmlFile
    {
        public string ReadDocumentText(string fileName, Encoding encoding, CultureInfo cultureInfo)
        {
            string fullText = string.Empty;
            using (StreamReader sr = new StreamReader(fileName, encoding)) // Encoding.GetEncoding(1253)
            {
                fullText = sr.ReadToEnd();
            }

            int startIndex = fullText.IndexOf("<TEXT>", StringComparison.OrdinalIgnoreCase) + 6;
            int stopIndex = fullText.IndexOf("</TEXT>", StringComparison.OrdinalIgnoreCase);

            if (startIndex > stopIndex || stopIndex < 0 || startIndex < 0) throw new ArgumentException("file structure invalid");
            
            var result = new string(fullText.Skip(startIndex).Take(stopIndex - startIndex).ToArray())
                .ToLower(cultureInfo);

            return result;
        }
    }
}