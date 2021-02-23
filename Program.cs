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
            List<Transaction> Transactions = new List<Transaction>();

            // read data in line by line
            while ((line = sr.ReadLine()) != null)
            {
                string[] columns = line.Split(',');
                Transactions.Add(new Transaction(columns));
            }
            sr.Close();

            // string[] People = the unique values of all the names in the From and To of Transactions

            // foreach (var Transaction in Transactions) {
            //     Console.WriteLine(Transaction.To);
            // }
        }
    }
}
