using System;
using System.Collections.Generic;
using Contract;
using AlexDataAnalyser;
using System.Linq;

namespace CSharpBasic
{
    class Program
    {
        static void Main(string[] args)
        {
            string folderPath = @"C:\Data";
            List<IDataAnalyser> Analysers = new List<IDataAnalyser>();
            Analysers.Add(new NoahAnalyser(folderPath));
            foreach (IDataAnalyser analyser in Analysers)
            {
                Console.WriteLine($"Author is {analyser.Author} ");
                foreach (string str in analyser.GetTopTenStrings(analyser.Path))
                {
                    Console.WriteLine(str);
                }
            }
            Console.WriteLine("Press any ken to exit.");
            Console.ReadKey();
        }
    }
}
