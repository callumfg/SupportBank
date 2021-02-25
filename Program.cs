using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;
using NLog.Config;
using NLog.Targets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SupportBank
{
    class Program
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            
            // Stuff for using NLog
            var config = new LoggingConfiguration();
            var target = new FileTarget { FileName = @"C:\Users\emipat\Documents\Training\logs\SupportBank.log", Layout = @"${longdate} ${level} - ${logger}: ${message}" };
            config.AddTarget("File Logger", target);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, target));
            LogManager.Configuration = config;
            
            List<Transaction> Transactions = new List<Transaction>();
            List<string[]> SkippedTransactions = new List<string[]>();
            Dictionary<string, Person> Ledger = new Dictionary<string, Person>();

            // CHANGE - Get which type of file from user input
            bool jsonfile = true;
            bool csvfile = false;

            // Read in JSON file and populate Transactions
            if (jsonfile) {
                Transactions = JsonConvert.DeserializeObject<List<Transaction>>(File.ReadAllText(@"./Transactions2013.json"));
            }

            if (csvfile) {
                // FileStream aFile = new FileStream("./Transactions2014.csv", FileMode.Open);
                FileStream aFile = new FileStream("./DodgyTransactions2015.csv", FileMode.Open);
                
                StreamReader sr = new StreamReader(aFile);
                // Skip the first line (the column headers)
                string line = sr.ReadLine();                

                // Read data in line by line
                while ((line = sr.ReadLine()) != null)
                {
                    bool ColumnsIsValid;
                    string[] columns = line.Split(',');

                    if (columns.Length == 5 && 
                        DateTime.TryParse(columns[0], out DateTime TestDate) && 
                        Double.TryParse(columns[4], out double TestDouble)
                        ) {
                        ColumnsIsValid = true;
                    } else {
                        ColumnsIsValid = false;
                    }

                    if (ColumnsIsValid) {
                        Transactions.Add(new Transaction(columns));
                    } else {
                        SkippedTransactions.Add(columns);
                    }              
                }
                sr.Close();
            }            

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
                Console.WriteLine("\n");
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
                // print out SkippedTransactions 
                if (csvfile) {
                    if (SkippedTransactions.Count > 0) {
                        Console.WriteLine("\nThe following transactions were skipped as they were invalid.\n");

                        // Calculate column widths here
                        int DateColWidth = SkippedTransactions.Max(x => x[0].Length)+3;
                        int NarrColWidth = SkippedTransactions.Max(x => x[3].Length)+3;
                        int AmountColWidth = SkippedTransactions.Max(x => x[4].Length)+3;
                        string ColFormat = $"{{0,-{DateColWidth}}} {{1,-15}} {{2,-15}} {{3,-{NarrColWidth}}} {{4,{AmountColWidth}}}";

                        Console.WriteLine(ColFormat,
                                        "Date", "From", "To", "Narrative", "Amount");
                        Console.WriteLine(new String('-', DateColWidth + NarrColWidth + AmountColWidth + 35));
                        foreach(var transaction in SkippedTransactions) {
                            Console.WriteLine(ColFormat, transaction[0], transaction[1], transaction[2], transaction[3], transaction[4]);
                        }                
                    }      
                }                              
            }                   
        }
    }
}
