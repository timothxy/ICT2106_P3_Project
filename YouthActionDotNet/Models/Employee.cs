using System.ComponentModel.DataAnnotations.Schema;

namespace YouthActionDotNet.Models
{
    public class Employee: User
    {
        [ForeignKey("UserId")]
        public string EmployeeId { get; set; }

        public string EmployeeNationalId { get; set; }

        public string BankName { get; set; }

        public string BankAccountNumber { get; set; }

        public string PAYE { get; set; }

        public string DateJoined { get; set; }

        public string EmployeeType { get; set; }

        public string EmployeeRole { get; set; }
    }
}