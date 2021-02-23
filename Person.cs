using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SupportBank {   
    class Person {
        public string Name { get; set; }
        public List<Transaction> IncomingTransactions { get; set; }
        public List<Transaction> OutgoingTransactions { get; set; }

        // Constructor 
        public Person(string name, List<Transaction> transactions) {
            Name = name;
            IncomingTransactions = transactions.Where(x => x.To == name).ToList();
            OutgoingTransactions = transactions.Where(x => x.From == name).ToList();
        }

        public double CalculateNetMoney() {             
            double IncomingSum = IncomingTransactions.Sum(item => item.Amount);
            double OutgoingSum = OutgoingTransactions.Sum(item => item.Amount);
            return OutgoingSum - IncomingSum;
            // A positive number means the person owes money!!!!!
        }
    }    
}