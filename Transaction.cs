using System;
using NLog;
using Newtonsoft.Json;

namespace SupportBank {   
    class Transaction {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        [JsonProperty]
        public DateTime Date { get; set; }
        [JsonProperty("FromAccount")]
        public string From { get; set; }
        [JsonProperty("ToAccount")]
        public string To { get; set; }
        [JsonProperty]
        public string Narrative { get; set; }
        [JsonProperty]
        public double Amount { get; set; }

        // Constructor
        // public Transaction(string[] args) {
        //      try {
        //         Date = Convert.ToDateTime(args[0]);
        //     } 
        //     catch (Exception ) {
        //         Logger.Error($"{args[0]} is not a valid date.");                
        //     } 
        //     From = args[1];
        //     To = args[2];
        //     Narrative = args[3];
        //     try {
        //         Amount = Convert.ToDouble(args[4]);
        //     } 
        //     catch (Exception) {
        //         Logger.Error($"{args[4]} is not a valid amount.");
        //     }            
        // }
    }
}