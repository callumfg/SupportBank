using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;
using NLog.Config;
using NLog.Targets;
using Newtonsoft.Json;

namespace SupportBank
{
    class Program
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        private static (List<Transaction>, List<string[]>) ReadCsvTransactions(string filename)
        {
            // string filename = "./DodgyTransactions2015.csv";
            List<Transaction> Transactions = new List<Transaction>();
            List<string[]> SkippedTransactions = new List<string[]>();
            FileStream aFile = new FileStream(filename, FileMode.Open);
            StreamReader sr = new StreamReader(aFile);
            // Skip the first line (the column headers)
            string line = sr.ReadLine();

            // Read data in line by line
            while ((line = sr.ReadLine()) != null)
            {
                string[] columns = line.Split(',');

                if (columns.Length == 5 &&
                    DateTime.TryParse(columns[0], out DateTime TestDate) &&
                    Double.TryParse(columns[4], out double TestDouble)
                    )
                {
                    Transactions.Add(new Transaction(columns));
                }
                else
                {
                    SkippedTransactions.Add(columns);
                }
            }
            sr.Close();
            return (Transactions, SkippedTransactions);
        }
        private static List<Transaction> ReadJsonTransactions(string filename)
        {
            // string filename = @"./Transactions2013.json";
            List<Transaction> Transactions = JsonConvert.DeserializeObject<List<Transaction>>(File.ReadAllText(filename));
            return Transactions;
        }
        private static Dictionary<string, Person> PopulateLedger(List<Transaction> Transactions)
        {
            Dictionary<string, Person> Ledger = new Dictionary<string, Person>();
            foreach (var Transaction in Transactions)
            {
                if (!Ledger.ContainsKey(Transaction.To))
                {
                    Ledger.Add(Transaction.To, new Person(Transaction.To));
                }
                Ledger[Transaction.To].IncomingTransactions.Add(Transaction);
                if (!Ledger.ContainsKey(Transaction.From))
                {
                    Ledger.Add(Transaction.From, new Person(Transaction.From));
                }
                Ledger[Transaction.From].OutgoingTransactions.Add(Transaction);
            }
            return Ledger;
        }
        private static void PrintNetMonies(Dictionary<string, Person> Ledger)
        {
            Console.WriteLine("\n");
            // Print out how much each person owes or is owed
            foreach (var Person in Ledger.Values)
            {
                Console.WriteLine("{0,-11}{1,-10}{2,-10}",
                                Person.Name,
                                (Person.CalculateNetMoney() > 0 ? "owes" : "is owed"),
                                $"£{String.Format("{0:0.00}", Math.Abs(Person.CalculateNetMoney()))}");
            }
        }
        private static void PrintOutgoings(Dictionary<string, Person> Ledger, string name)
        {
            int ColWidth = Ledger[name].OutgoingTransactions.Max(x => x.Narrative.Length) + 3;
            string ColFormat = $"{{0,-15}} {{1,-{ColWidth}}} {{2,-15}} {{3,10}}";
            Console.WriteLine($"\n{name}'s Outgoing Transactions:\n");
            Console.WriteLine(ColFormat,
                            "Date", "Narrative", "To", "Amount");
            Console.WriteLine(new String('-', 43 + ColWidth));
            foreach (var Transaction in Ledger[name].OutgoingTransactions)
            {
                Console.WriteLine(ColFormat,
                                Transaction.Date.ToShortDateString(),
                                Transaction.Narrative,
                                Transaction.To,
                                $"£{String.Format("{0:0.00}", Transaction.Amount)}");
            }
        }
        private static void GetAndListAccount(Dictionary<string, Person> Ledger)
        {
            int Counter = 0;
            Console.WriteLine("Enter the number of the person whose account you'd like to view:");
            foreach (var name in Ledger.Keys)
            {
                Console.WriteLine($"{Counter + 1}. {name}");
                Counter++;
            }
            int Option2 = 0;
            while (Option2 < 1 || Option2 > Counter)
            {
                try
                {
                    Option2 = Int16.Parse(Console.ReadLine());
                    if (Option2 < 1 || Option2 > Counter)
                    {
                        Console.WriteLine("Please enter a valid option.");
                    }
                }
                catch
                {
                    Console.WriteLine($"Please enter a positive integer between 1 and {Counter}.");
                }
            }
            PrintOutgoings(Ledger, Ledger.Keys.ToArray()[Option2 - 1]);
        }
        private static void PrintInvalidTransactions(List<string[]> SkippedTransactions)
        {
            if (SkippedTransactions.Count > 0)
            {
                Console.WriteLine("\nThe following transactions were skipped as they were invalid.\n");

                // Calculate column widths here
                int DateColWidth = SkippedTransactions.Max(x => x[0].Length) + 3;
                int NarrColWidth = SkippedTransactions.Max(x => x[3].Length) + 3;
                int AmountColWidth = SkippedTransactions.Max(x => x[4].Length) + 3;
                string ColFormat = $"{{0,-{DateColWidth}}} {{1,-15}} {{2,-15}} {{3,-{NarrColWidth}}} {{4,{AmountColWidth}}}";

                Console.WriteLine(ColFormat,
                                "Date", "From", "To", "Narrative", "Amount");
                Console.WriteLine(new String('-', DateColWidth + NarrColWidth + AmountColWidth + 35));
                foreach (var transaction in SkippedTransactions)
                {
                    Console.WriteLine(ColFormat, transaction[0], transaction[1], transaction[2], transaction[3], transaction[4]);
                }
            }
        }
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

            // Get user to specify file to import
            Console.WriteLine("Please specify the path of the file you want to import:");
            string Filename = Console.ReadLine();
            string FileType = Path.GetExtension(Filename);

            // Read in JSON file and populate Transactions
            if (FileType == ".json")
            {
                Transactions = ReadJsonTransactions(Filename);
            }
            else if (FileType == ".csv")
            {
                (Transactions, SkippedTransactions) = ReadCsvTransactions(Filename);
            }
            else
            {
                Console.WriteLine("Not a valid option");
            }

            // For each transaction, add the transaction to the two people's incoming/outgoing transactions,
            // making sure to create an account for a person if it doesn't exist already
            Ledger = PopulateLedger(Transactions);

            Console.WriteLine("Do you want to List All (1) or List [Account] (2)?");
            string Option1 = Console.ReadLine();
            if (Option1 == "1")
            {
                PrintNetMonies(Ledger);
            }
            else if (Option1 == "2")
            {
                GetAndListAccount(Ledger);
            }
            PrintInvalidTransactions(SkippedTransactions);
        }
    }
}