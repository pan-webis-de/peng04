using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webis.naiveBayes.logic
{
    public static class TakeFromArrayExtensions
    {
        public static string[] GetNGram(this string[] array, int index, int n)
        {
            var newArray = new string[n];
            Array.Copy(array, index, newArray, 0, n);

            return newArray;
        }
    }
}