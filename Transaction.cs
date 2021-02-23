using System;
using System.Collections.Generic;
using System.IO;

namespace SupportBank {   
    class Transaction {
        public DateTime Date { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Narrative { get; set; }
        public double Amount { get; set; }

        // Constructor
        public Transaction(string[] args) {
            Date = Convert.ToDateTime(args[0]); // needs converting to DateTime
            From = args[1];
            To = args[2];
            Narrative = args[3];
            Amount = Convert.ToDouble(args[4]);
        }
    }
}