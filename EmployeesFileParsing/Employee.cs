using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeesFileParsing
{
    public class Employee
    {
        public long ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public PayType PayType { get; set; }
        public double Salary { get; set; }
        public DateTime StartDate { get; set; }
        public string State { get; set; }
        public double HoursWorked { get; set; }

        public static double FederalTaxRate = 0.15;
        public static Dictionary<string, double> StateTaxRates = new Dictionary<string, double>() {
            { "UT", 5 }, { "WY", 5 }, { "NV", 5 },
            { "CO", 6.5 }, { "ID", 6.5 }, { "AZ", 6.5 }, { "OR", 6.5 },
            { "WA", 7 }, { "NM", 7 }, { "TX", 7 }
        };

        public Employee(string EmployeeInputLine)
        {
            string[] split = EmployeeInputLine.Split(',');
            ID = long.Parse(split[0]);
            FirstName = split[1];
            LastName = split[2];
            if (split[3].Equals("S"))
            {
                PayType = PayType.SALARY;
            }
            else if (split[3].Equals("H"))
            {
                PayType = PayType.HOURLY;
            }
            else
            {
                throw new Exception("Invalid File Input");
            }

            Salary = double.Parse(split[4]);
            StartDate = DateTime.Parse(split[5]);
            State = split[6];
            HoursWorked = double.Parse(split[7]);

        }

        public double CalculateBiWeeklyNetPay()
        {
            double realSalary = CalculateBiWeeklyGrossPay();
 
            realSalary = realSalary - (Salary * FederalTaxRate);
            realSalary = realSalary - (Salary * StateTaxRates[State]);

            return realSalary;
        }

        public double CalculateBiWeeklyGrossPay()
        {
            double grossSalary = 0;
            if (PayType.Equals(PayType.SALARY))
            {
                // 52 weeks in a year, two week long pay periods
                grossSalary = Salary / (52 / 2);
            }
            else
            {
                // checking for overtime
                if (HoursWorked > 80)
                {
                    grossSalary = 80 * Salary;
                    double remainingHours = HoursWorked - 80;
                    
                    // checking if over ten overtime hours
                    if (remainingHours > 10)
                    {
                        // first ten overtime hours calculated at 150% hourly salary
                        grossSalary += 10 * Salary * 1.5;

                        // remaining overtime hours calculated at 175% hourly salary
                        grossSalary += (remainingHours - 10) * Salary * 1.75;
                    }
                    else
                    {
                        // ten or less overtime hours, valuing at 150% hourly salary.
                        grossSalary += remainingHours * Salary * 1.5;
                    }
                }
                else
                {
                    grossSalary = HoursWorked * Salary;
                }
            }

            return grossSalary;
        }
    }

    public enum PayType
    {
        HOURLY, SALARY
    }
}
