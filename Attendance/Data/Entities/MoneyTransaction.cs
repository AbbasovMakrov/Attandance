using Attendance.Data.Entities.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Attendance.Data.Entities
{
    public class MoneyTransaction : Entity, ITimestampableEntity , ICloneable
    {
        public MoneyTransaction()
        {
        }

        public MoneyTransaction(string comment , TransactionType type , int amount)
        {
            Comment = comment;
            Type = type;
            Amount = amount;
        }
        public enum TransactionType
        {
            In,
            Out
        }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public string? Comment { get; set; }

        public Guid EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public object Clone()
        {
            return new MoneyTransaction
            {
                Amount = this.Amount,
                EmployeeId = this.EmployeeId,
            };
        }
    }
}
