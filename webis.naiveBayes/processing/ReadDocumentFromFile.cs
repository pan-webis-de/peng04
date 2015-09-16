using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webis.naiveBayes.processing
{
    public class ReadDocumentFromFile
    {
        public string ReadDocumentText(string fileName)
        {
            string fullText = string.Empty;
            using (StreamReader sr = new StreamReader(fileName, Encoding.GetEncoding(1253)))
            {
                fullText = sr.ReadToEnd();
            }

            int startIndex = fullText.IndexOf("<TEXT>", StringComparison.OrdinalIgnoreCase) + 6;
            int stopIndex = fullText.IndexOf("</TEXT>", StringComparison.OrdinalIgnoreCase);

            if (startIndex > stopIndex || stopIndex < 0 || startIndex < 0) throw new ArgumentException("file structure invalid");

            var result = new string(fullText.Skip(startIndex).Take(stopIndex - startIndex).ToArray())
                .Replace("\r\n", " ")
                .Replace("\n", " ")
                .Replace(",", " ")
                .Replace(".", " ")
                .ToLower(new CultureInfo("el-GR"));

            return result;
        }
    }
}