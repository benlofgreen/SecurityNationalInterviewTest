using System;
using System.Collections.Generic;
using System.IO;

namespace EmployeesFileParsing
{
    public class Program
    {
        private string EmployeeFileLocation = "";
        public List<Employee> employees;

        static void Main(string[] args)
        {
            Program program = new Program();
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
                    employees.Add(e);
                }
                catch (Exception e)
                {

                }
            }

        }
    }
}
