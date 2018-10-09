using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EmployeesFileParsing
{
    public class Program
    {
        private string EmployeeFileLocation = "Employees/Employees.txt";
        Dictionary<string, Employee> Employees;
        static void Main(string[] args)
        {
            Program program = new Program();
            Console.WriteLine("Beginning Operation");
            program.Employees = program.ParseEmployees().ToDictionary(x => x.ID);
            List<Employee> employees = program.Employees.Select(x => x.Value).ToList();

            CalculateAllPaychecks(employees);

            CalculateTopFifteenPercent(employees);

            CalculateStateStatistics(employees);

            Console.WriteLine("Operations Complete");
            Console.ReadLine();
        }

        public static void CalculateAllPaychecks(List<Employee> Employees)
        {
            DateTime startTime = DateTime.Now;

            Employees = Employees.OrderByDescending(x => x.CalculateBiWeeklyGrossPay()).ToList();
            List<string> EmployeesOutput = new List<string>();

            Employees = Employees.OrderByDescending(x => x.CalculateBiWeeklyGrossPay()).ToList();

            List<Employee> TopFifteenPercent = Employees.Take((15 * Employees.Count) / 100)
                .OrderByDescending(x => CalculateYearsWorked(x)).ThenBy(x => x.LastName).ThenBy(x => x.FirstName).ToList();

            foreach (Employee e in Employees)
            {
                EmployeesOutput.Add(e.ID + "," + e.FirstName + "," + e.LastName + "," + e.CalculateBiWeeklyGrossPay()
                    + "," + e.CalculateFederalTax() + "," + e.CalculateStateTax() + "," + e.CalculateBiWeeklyNetPay());
            }

            System.IO.File.WriteAllLines("Employees/AllEmployeesOutput.txt", EmployeesOutput);

            DateTime endTime = DateTime.Now;
            TimeSpan diff = endTime - startTime;
            double milliseconds = diff.TotalMilliseconds;

            Console.WriteLine("All Paychecks calculated, process took " + milliseconds + " Milliseconds");
        }

        public static void CalculateTopFifteenPercent(List<Employee> Employees)
        {
            DateTime startTime = DateTime.Now;
            List<Employee> TopFifteenPercent = Employees.Take((15 * Employees.Count) / 100)
                .OrderByDescending(x => CalculateYearsWorked(x)).ThenBy(x => x.LastName).ThenBy(x => x.FirstName)
                .ToList();

            List<string> TopFifteenPercentEmployeesOutput = new List<string>();

            foreach (Employee e in TopFifteenPercent)
            {

                double grossPay = e.CalculateBiWeeklyGrossPay();
                TopFifteenPercentEmployeesOutput.Add(e.FirstName + " " + e.LastName + ", Years Worked: " + CalculateYearsWorked(e) + ": Gross Pay: " + grossPay);
            }
            System.IO.File.WriteAllLines("Employees/Top15PercentOutput.txt", TopFifteenPercentEmployeesOutput);

            DateTime endTime = DateTime.Now;
            TimeSpan diff = endTime - startTime;
            double milliseconds = diff.TotalMilliseconds;

            Console.WriteLine("All Top Fifteen Percent Paychecks calculated, process took " + milliseconds + " Milliseconds");
        }

        public static void CalculateStateStatistics(List<Employee> Employees)
        {
            DateTime startTime = DateTime.Now;

            List<string> StatesOutput = new List<string>();

            foreach (KeyValuePair<string, double> state in Employee.StateTaxRates)
            {
                StateStatistics stateStats = new StateStatistics();
                stateStats.State = state.Key;

                List<Employee> stateEmployees = Employees.Where(x => x.State.Equals(state.Key)).ToList();

                // All median calculations round on even length lists, rather than calculate the median as the average between the two middle entries.
                if (stateEmployees.Count > 0)
                {
                    stateStats.MedianNetPay = stateEmployees.OrderBy(x => x.CalculateBiWeeklyNetPay()).ToArray()[stateEmployees.Count / 2].CalculateBiWeeklyNetPay();
                    stateStats.MedianTimeWorked = stateEmployees.OrderBy(x => x.HoursWorked).ToArray()[stateEmployees.Count / 2].CalculateBiWeeklyNetPay();
                    stateStats.TotalStateTaxesPaid = Math.Round((stateEmployees.Sum(x => x.CalculateStateTax())), 2, MidpointRounding.AwayFromZero);
                }
                StatesOutput.Add(stateStats.State + "," + stateStats.MedianTimeWorked + "," + stateStats.MedianNetPay + "," + stateStats.TotalStateTaxesPaid);
            }

            System.IO.File.WriteAllLines("Employees/StatesOutput.txt", StatesOutput);

            DateTime endTime = DateTime.Now;
            TimeSpan diff = endTime - startTime;
            double milliseconds = diff.TotalMilliseconds;

            Console.WriteLine("All State statistics calculated, process took " + milliseconds + " Milliseconds");
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

        public Employee FindEmployee(string id)
        {
            return Employees[id];
        }

        public static int CalculateYearsWorked(Employee e)
        {
            DateTime now = DateTime.Today;
            int yearsWorked = now.Year - e.StartDate.Year;
            if (e.StartDate > now.AddYears(-yearsWorked)) yearsWorked--;
            return yearsWorked;
        }
    }
}
