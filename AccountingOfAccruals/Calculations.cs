using AccountingOfAccruals.Contexts;
using AccountingOfAccruals.Entities;
using System;
using System.Linq;

namespace AccountingOfAccruals
{
    public static class Calculations
    {
        public static int GetEmployeeAccrualsForPeriod(Employee Employee, DateTime BeginningPeriod, DateTime EndPeriod)
        {
            using (AccrualsContext accrualsContext = new AccrualsContext())
            {
                var workingPeriodOfEmployee = accrualsContext.WorkingPeriodsOfEmployees
                    .Where(p => p.EmployeeId == Employee.Id)
                    .Select(p => new { Beginning = p.EmploymentDate, End = p.DismissalDate }).ToArray();

                DateTime beginningPeriodAccruals = BeginningPeriod > workingPeriodOfEmployee.Select(p => p.Beginning).FirstOrDefault()
                    ? BeginningPeriod : workingPeriodOfEmployee.Select(p => p.Beginning).FirstOrDefault();

                DateTime endPeriodAccruals = EndPeriod < workingPeriodOfEmployee.Select(p => p.End).FirstOrDefault()
                    & workingPeriodOfEmployee.Select(p => p.End).FirstOrDefault() != new DateTime(1, 1, 1) 
                    ? workingPeriodOfEmployee.Select(p => p.End).FirstOrDefault() : EndPeriod;

                var daysWorked = accrualsContext.WorkingCalendar
                    .Where(p => p.Date >= beginningPeriodAccruals && p.Date <= endPeriodAccruals && !p.Dayoff)
                    .Select(p => p.Date)
                    .Except(accrualsContext.AbsenceFromWork
                        .Where(p => p.EmployeeId == Employee.Id)
                        .Select(p => p.Date)).ToArray();

                var allPayRateByWorkingDays =
                    (from workedDay in daysWorked
                    from pr in accrualsContext.PayRates
                    .Where(x => x.Date <= workedDay.Date)
                    select(new { workedDay = workedDay.Date, payRatesDate = pr.Date, rate = pr.Rate})).ToArray();

                var actualPayRateByWorkingDays =
                    (from x in allPayRateByWorkingDays
                    group x by x.workedDay into y
                    select new { workedDay = y.Key, payRatesDate = y.Max(z => z.payRatesDate) }).ToArray();

                var allRatesOfEmloyee =
                    (from x in actualPayRateByWorkingDays
                    join y in allPayRateByWorkingDays on x equals new { workedDay = y.workedDay, payRatesDate = y.payRatesDate }
                    select new { workedDay = x.workedDay, payRatesDate = x.payRatesDate, rate = y.rate}).ToArray();

                return allRatesOfEmloyee.Sum(x => x.rate);
            }
        }

        public static int GetEmployeePrepaidExpense(Employee Employee, DateTime DateOfAccrual)
        {
            CompanySettings companySettings = GetCompanySettings(DateTime.Now) as CompanySettings;

            if (companySettings == null)
                return 0;
            if (companySettings.AdvancePercentage == 0)
                return 0;

            DateTime firstDayCurrentMonth = new DateTime(DateOfAccrual.Year,
                                                        DateOfAccrual.Month,
                                                        1);
            DateTime lastDayCurrentMonth = new DateTime(DateOfAccrual.Year,
                                                        DateOfAccrual.Month,
                                                        DateTime.DaysInMonth(DateOfAccrual.Year, DateOfAccrual.Month));
            if (companySettings.UseAdvanceDay)
                lastDayCurrentMonth = new DateTime(DateOfAccrual.Year,
                                                   DateOfAccrual.Month,
                                                   companySettings.DayOfAdvancePayment);

            var employeeWorkPeriod = GetEmployeeWorkPeriod(Employee);
            if (employeeWorkPeriod.DismissalDate <= lastDayCurrentMonth)
                return 0;

            int daysWorkedEmployeeForPeriod = GetDaysWorkedEmployeeForPeriod(Employee, firstDayCurrentMonth, lastDayCurrentMonth);

            if (daysWorkedEmployeeForPeriod < companySettings.MinimumNumberOfDaysWorked)
                return 0;

            int plannedWorkingDaysForPeriod = GetPlannedWorkingDaysForPeriod(firstDayCurrentMonth, lastDayCurrentMonth);

            return Employee.Salary / plannedWorkingDaysForPeriod * daysWorkedEmployeeForPeriod * companySettings.AdvancePercentage / 100;
        }

        public static CompanySettings GetCompanySettings(DateTime DateOfSettings)
        {
            using (AccrualsContext accrualsContext = new AccrualsContext())
            {
                return accrualsContext.CompanySettings.OrderByDescending(x => x.Date <= DateOfSettings).FirstOrDefault();
            }
        }

        public static (DateTime EmploymentDate, DateTime DismissalDate) GetEmployeeWorkPeriod(Employee Employee)
        {
            using (AccrualsContext accrualsContext = new AccrualsContext())
            {
                var workingPeriodOfEmployee =
                    accrualsContext.WorkingPeriodsOfEmployees
                    .Where(p => p.EmployeeId == Employee.Id)
                    .Select(p => new { EmploymentDate = p.EmploymentDate, DismissalDate = p.DismissalDate }).FirstOrDefault();

                return (EmploymentDate: workingPeriodOfEmployee.EmploymentDate,
                        DismissalDate: workingPeriodOfEmployee.DismissalDate == new DateTime(1, 1, 1)
                            ? DateTime.Now : workingPeriodOfEmployee.DismissalDate);
            }
        }

        public static int GetDaysWorkedEmployeeForPeriod(Employee Employee, DateTime BeginningPeriod, DateTime EndPeriod)
        {
            using (AccrualsContext accrualsContext = new AccrualsContext())
            {
                return
                    accrualsContext.WorkingCalendar
                    .Where(p => p.Date >= BeginningPeriod && p.Date <= EndPeriod && !p.Dayoff)
                    .Select(p => p.Date)
                    .Except(accrualsContext.AbsenceFromWork
                        .Where(p => p.EmployeeId == Employee.Id)
                        .Select(p => p.Date)).Count();
            }
        }

        public static int GetPlannedWorkingDaysForPeriod(DateTime BeginningPeriod, DateTime EndPeriod)
        {
            using (AccrualsContext accrualsContext = new AccrualsContext())
            {
                return
                    accrualsContext.WorkingCalendar
                    .Where(p => p.Date >= BeginningPeriod && p.Date <= EndPeriod && !p.Dayoff)
                    .Select(p => p.Date).Count();
            }
        }
    }
}
