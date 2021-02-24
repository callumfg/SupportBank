using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SupportBank
{
    class Program
    {
        static void Main(string[] args)
        {
            FileStream aFile = new FileStream("./Transactions2014.csv", FileMode.Open);
            StreamReader sr = new StreamReader(aFile);
            // Skip the first line (the column headers)
            string line = sr.ReadLine();
            List<Transaction> Transactions = new List<Transaction>();

            // Read data in line by line
            while ((line = sr.ReadLine()) != null)
            {
                string[] columns = line.Split(',');
                Transactions.Add(new Transaction(columns));
            }
            sr.Close();

            Dictionary<string, Person> Ledger = new Dictionary<string, Person>();

            // For each transaction, add the transaction to the two people's incoming/outgoing transactions,
            // making sure to create an account for a person if it doesn't exist already
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

            if (args != null && args[0] == "List") {
                if (args[1] == "All") {
                // Print out how much each person owes or is owed
                    foreach(var Person in Ledger.Values) {
                        Console.WriteLine("{0,-11}{1,-10}{2,-10}",
                                        Person.Name,
                                        (Person.CalculateNetMoney() > 0 ? "owes" : "is owed"),
                                        $"£{String.Format("{0:0.00}", Math.Abs(Person.CalculateNetMoney()))}");
                    }
                } else if (Ledger.ContainsKey(args[1])) {
                    // Print out all the outgoing transactions for the specified person
                    int ColWidth = Ledger[args[1]].OutgoingTransactions.Max(x => x.Narrative.Length)+3;
                    string ColFormat = $"{{0,-15}} {{1,-{ColWidth}}} {{2,-15}} {{3,10}}";
                    Console.WriteLine(ColFormat,
                                    "Date", "Narrative", "To", "Amount");
                    Console.WriteLine(new String('-', 43+ColWidth));
                    foreach(var Transaction in Ledger[args[1]].OutgoingTransactions) {
                        Console.WriteLine(ColFormat,
                                        Transaction.Date.ToShortDateString(),
                                        Transaction.Narrative,
                                        Transaction.To,
                                        $"£{String.Format("{0:0.00}",Transaction.Amount)}");
                    }                    
                }            
            }                   
        }
    }
}
