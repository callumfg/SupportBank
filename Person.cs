using System.Collections.Generic;
using System.Linq;

namespace SupportBank {   
    class Person {
        public string Name { get; set; }
        public List<Transaction> IncomingTransactions { get; private set; }
        public List<Transaction> OutgoingTransactions { get; private set; }

        // Constructor 
        public Person(string name) {
            Name = name;
            IncomingTransactions = new List<Transaction>();
            OutgoingTransactions = new List<Transaction>();
        }

        public double CalculateNetMoney() {             
            double IncomingSum = IncomingTransactions.Sum(item => item.Amount);
            double OutgoingSum = OutgoingTransactions.Sum(item => item.Amount);
            return OutgoingSum - IncomingSum;
            // A positive number means the person owes money!!!!!
        }
    }    
}