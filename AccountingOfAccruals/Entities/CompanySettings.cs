using System;

namespace AccountingOfAccruals.Entities
{
    public class CompanySettings
    {
        private DateTime _date;
        private int _advancePercentage;

        public DateTime Date
        {
            get => _date;
            set => _date = new DateTime(value.Year , value.Month, 1);
        }

        public int AdvancePercentage
        {
            get => _advancePercentage;
            set
            {
                if (value < 0)
                    _advancePercentage = 0;
                else if (value > 100)
                    _advancePercentage = 100;
                else
                    _advancePercentage = value;
            }
        }

        public int MinimumNumberOfDaysWorked { get; set; }

        public bool UseAdvanceDay { get; set; }

        public int DayOfAdvancePayment { get; set; }
    }
}
