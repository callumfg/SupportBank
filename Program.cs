using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SupportBank
{
    class Program
    {
        static void Main(string[] args)
        {
            FileStream aFile = new FileStream("./Transactions2014.csv", FileMode.Open);
            StreamReader sr = new StreamReader(aFile);
            // skip the first line (the column headers)
            string line = sr.ReadLine();
            List<string> Transactions = new List<string>();

            // read data in line by line
            while ((line = sr.ReadLine()) != null)
            {
                // Console.WriteLine(line);
                Transactions.Add(line);
            }
            sr.Close();

            Transactions.ForEach(Console.WriteLine);
        }
    }
}
