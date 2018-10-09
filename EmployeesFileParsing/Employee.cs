using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeesFileParsing
{
    public class Employee
    {
        public string ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public PayType PayType { get; set; }
        public double Salary { get; set; }
        public DateTime StartDate { get; set; }
        public string State { get; set; }
        public double HoursWorked { get; set; }

        public static double FederalTaxRate = 0.15;
        public static Dictionary<string, double> StateTaxRates = new Dictionary<string, double>() {
            { "UT", 0.05 }, { "WY", 0.05 }, { "NV", 0.05 },
            { "CO", 0.065 }, { "ID", 0.065 }, { "AZ", 0.065 }, { "OR", 0.065 },
            { "WA", 0.07 }, { "NM", 0.07 }, { "TX", 0.07 }
        };

        public Employee(string EmployeeInputLine)
        {
            string[] split = EmployeeInputLine.Split(',');
            ID = split[0];
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
            double grossSalary = CalculateBiWeeklyGrossPay();
            double realSalary = grossSalary;
            realSalary = realSalary - CalculateFederalTax();
            realSalary = realSalary - CalculateStateTax();

            return Math.Round(realSalary, 2, MidpointRounding.AwayFromZero);
        }

        public double CalculateFederalTax()
        {
            double grossSalary = CalculateBiWeeklyGrossPay();
            return Math.Round((grossSalary * FederalTaxRate), 2, MidpointRounding.AwayFromZero);
        }

        public double CalculateStateTax()
        {
            double grossSalary = CalculateBiWeeklyGrossPay();
            return Math.Round((grossSalary * StateTaxRates[State]), 2, MidpointRounding.AwayFromZero);
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

            return Math.Round(grossSalary, 2, MidpointRounding.AwayFromZero);
        }
    }

    public enum PayType
    {
        HOURLY, SALARY
    }
}
