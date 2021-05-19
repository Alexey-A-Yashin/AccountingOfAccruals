using System;

namespace AccountingOfAccruals.Entities
{
    public class WorkingPeriodsOfEmployees
    {
        public int EmployeeId { get; set; }

        public DateTime EmploymentDate { get; set; }

        public DateTime DismissalDate { get; set; }
    }
}
