using System;
using NLog;

namespace SupportBank {   
    class Transaction {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        public DateTime Date { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Narrative { get; set; }
        public double Amount { get; set; }

        // Constructor
        public Transaction(string[] args) {
             try {
                Date = Convert.ToDateTime(args[0]);
            } 
            catch (Exception ) {
                Logger.Error($"{args[0]} is not a valid date.");
                
            } 
            From = args[1];
            To = args[2];
            Narrative = args[3];
            try {
                Amount = Convert.ToDouble(args[4]);
            } 
            catch (Exception) {
                Logger.Error($"{args[4]} is not a valid amount.");
            }            
        }
    }
}