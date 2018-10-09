using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EmployeesFileParsing
{
    public class Program
    {
        private string EmployeeFileLocation = "Employees/Employees.txt";

        static void Main(string[] args)
        {
            Program program = new Program();
            List<Employee> Employees = program.ParseEmployees();
            Employees = Employees.OrderByDescending(x => x.CalculateBiWeeklyGrossPay()).ToList();

            List<Employee> TopFifteenPercent = Employees.Take((15 * Employees.Count) / 100)
                .OrderByDescending(x => x.StartDate.Year).ThenBy(x => x.LastName).ThenBy(x => x.FirstName)
                .ToList();

            foreach (Employee e in Employees)
            {
                Console.WriteLine(e.ID + "," + e.FirstName + "," + e.LastName + "," + e.CalculateBiWeeklyGrossPay() 
                    + "," + e.CalculateFederalTax() + "," + e.CalculateStateTax() + "," + e.CalculateBiWeeklyNetPay());
            }

            foreach (Employee e in TopFifteenPercent)
            {
                DateTime now = DateTime.Today;
                int yearsWorked = now.Year - e.StartDate.Year;
                if (e.StartDate > now.AddYears(-yearsWorked)) yearsWorked--;

                double grossPay = e.CalculateBiWeeklyGrossPay();
                Console.WriteLine(e.FirstName + " " + e.LastName + ", Years Worked: " + yearsWorked + ": Gross Pay: " + grossPay);
            }

            Console.ReadLine();
        }

        public List<Employee> ParseEmployees()
        {
            StreamReader file = new StreamReader(EmployeeFileLocation);
            List<Employee> employees = new List<Employee>();
            while (!file.EndOfStream)
            {
                try
                {
                    string line = file.ReadLine();

                    Employee e = new Employee(line);
                    employees.Add(e);
                }
                catch (Exception e)
                {
                    // Need better handler
                    Console.WriteLine(e.StackTrace);
                }
            }
            return employees;
        }
    }
}
