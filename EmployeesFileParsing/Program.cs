using System;
using System.Collections.Generic;
using System.IO;

namespace EmployeesFileParsing
{
    public class Program
    {
        private string EmployeeFileLocation = "Employees/Employees.txt";
        public List<Employee> Employees = new List<Employee>();

        static void Main(string[] args)
        {
            Program program = new Program();
            program.ParseEmployees();
            foreach (Employee e in program.Employees)
            {
                Console.WriteLine(e.CalculateBiWeeklyNetPay());
            }
            Console.ReadLine();
        }

        public void ParseEmployees()
        {
            StreamReader file = new StreamReader(EmployeeFileLocation);
            while (!file.EndOfStream)
            {
                try
                {
                    string line = file.ReadLine();

                    Employee e = new Employee(line);
                    Employees.Add(e);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
            }

        }
    }
}
