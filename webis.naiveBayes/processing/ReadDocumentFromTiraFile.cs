using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webis.naiveBayes.processing
{
    public class ReadDocumentFromTiraFile
    {
        public string ReadDocumentText(string fileName, Encoding encoding, CultureInfo cultureInfo)
        {
            string fullText = string.Empty;
            using (StreamReader sr = new StreamReader(fileName, encoding)) // Encoding.GetEncoding(1253)
            {
                fullText = sr.ReadToEnd();
            }

            var result = fullText.ToLower(cultureInfo);
            return result;
        }
    }
}