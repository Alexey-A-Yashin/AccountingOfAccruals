using AccountingOfAccruals.Entities;
using System;
using System.Data.Entity;

namespace AccountingOfAccruals.Contexts
{
    public class AccrualsContextInitializer : DropCreateDatabaseAlways<AccrualsContext> // CreateDatabaseIfNotExists
    {
        protected override void Seed(AccrualsContext accrualsContext)
        {
            Employee employeeIII = new Employee { FullName = "Иванов Иван Иванович", Salary = 100 };
            Employee employeePPP = new Employee { FullName = "Петров Пётр Петрович", Salary = 150 };
            
            accrualsContext.Employees.Add(employeeIII);
            accrualsContext.Employees.Add(employeePPP);
            accrualsContext.SaveChanges();

            accrualsContext.WorkingPeriodsOfEmployees.Add(
                new WorkingPeriodsOfEmployees { EmployeeId = employeeIII.Id,
                    EmploymentDate = new DateTime(2020, 04, 03)
                });
            accrualsContext.WorkingPeriodsOfEmployees.Add(
                new WorkingPeriodsOfEmployees { EmployeeId = employeePPP.Id,
                    EmploymentDate = new DateTime(2020, 04, 01)
                });

            accrualsContext.AbsenceFromWork.Add( new AbsenceFromWork { Date = new DateTime(2020, 04, 06), EmployeeId = employeeIII.Id});
            accrualsContext.AbsenceFromWork.Add( new AbsenceFromWork { Date = new DateTime(2020, 04, 07), EmployeeId = employeeIII.Id});
            accrualsContext.AbsenceFromWork.Add( new AbsenceFromWork { Date = new DateTime(2020, 04, 08), EmployeeId = employeeIII.Id});
            accrualsContext.AbsenceFromWork.Add( new AbsenceFromWork { Date = new DateTime(2020, 04, 09), EmployeeId = employeeIII.Id});
            accrualsContext.AbsenceFromWork.Add( new AbsenceFromWork { Date = new DateTime(2020, 04, 10), EmployeeId = employeeIII.Id});
            accrualsContext.AbsenceFromWork.Add( new AbsenceFromWork { Date = new DateTime(2020, 04, 11), EmployeeId = employeeIII.Id});
            accrualsContext.AbsenceFromWork.Add( new AbsenceFromWork { Date = new DateTime(2020, 04, 12), EmployeeId = employeeIII.Id});

            DateTime currentDate = DateTime.Now;
            DateTime tempDate = new DateTime(currentDate.Year - 1, 1, 1);
            while (tempDate <= currentDate)
            {
                accrualsContext.WorkingCalendar.Add(
                    new WorkingCalendar { Date = tempDate,
                        Dayoff = tempDate.DayOfWeek == DayOfWeek.Sunday || tempDate.DayOfWeek == DayOfWeek.Saturday
                    });
                tempDate = tempDate.AddDays(1);
            }

            accrualsContext.PayRates.Add(new PayRate { Date = new DateTime(1900, 01, 01), Rate = 200 });
            accrualsContext.PayRates.Add(new PayRate { Date = new DateTime(2020, 04, 20), Rate = 300 });

            accrualsContext.CompanySettings.Add(
                new CompanySettings { Date = new DateTime(2020, 04, 01),
                    AdvancePercentage = 30,
                    MinimumNumberOfDaysWorked = 15,
                    UseAdvanceDay = false,
                    DayOfAdvancePayment = 20
                });

            accrualsContext.SaveChanges();
            base.Seed(accrualsContext);
        }
    }
}
