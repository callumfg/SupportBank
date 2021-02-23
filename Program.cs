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

            Dictionary<string, Person> Ledger = new Dictionary<string, Person>();

            foreach(var Transaction in Transactions) {
                if (!Ledger.ContainsKey(Transaction.To)) {
                    Ledger.Add(Transaction.To, new Person(Transaction.To));
                }
                Ledger[Transaction.To].IncomingTransactions.Add(Transaction);
                if (!Ledger.ContainsKey(Transaction.From)) {
                    Ledger.Add(Transaction.From, new Person(Transaction.From));
                }
                Ledger[Transaction.From].OutgoingTransactions.Add(Transaction);                
            }           

            foreach(var Person in Ledger.Values) {
                Console.WriteLine($"{Person.Name} {(Person.CalculateNetMoney() > 0 ? "owes" : "is owed")} £{Math.Round(Math.Abs(Person.CalculateNetMoney()), 2)}");
            }
        }
    }
}
