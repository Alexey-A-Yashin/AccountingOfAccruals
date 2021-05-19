using AccountingOfAccruals.Contexts;
using AccountingOfAccruals.Entities;
using System;

namespace AccountingOfAccruals
{
    class Program
    {
        static void Main(string[] args)
        {
            using (AccrualsContext accrualsContext = new AccrualsContext())
            {
                foreach (Employee employee in accrualsContext.Employees)
                {
                    Console.WriteLine($"Начисления сотрудника {employee.ToString()} равны " +
                        $"{Calculations.GetEmployeeAccrualsForPeriod(employee, new DateTime(2020, 04, 01), new DateTime(2020, 04, 30)) }");

                    Console.WriteLine($"Размер аванса сотрудника сотрудника {employee.ToString()} равен " +
                        $"{Calculations.GetEmployeePrepaidExpense(employee, new DateTime(2020, 04, 25)) }");
                }
            }

            Console.ReadLine();
        }
    }
}
