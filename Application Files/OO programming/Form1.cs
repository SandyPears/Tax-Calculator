using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;

namespace OO_programming
{
    public partial class Form1 : Form
    {
        List<PaySlip> employees;
        public Form1()
        {
            InitializeComponent();
            employees = new List<PaySlip>();
            // Add code below to complete the implementation to populate the listBox
            // by reading the employee.csv file into a List of PaySlip objects, then binding this to the ListBox.
            // CSV file format: <employee ID>, <first name>, <last name>, <hourly rate>,<taxthreshold>
            try
            {

                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = false
                };
                using (var reader = new StreamReader("../../../employee.csv"))
                using (var csv = new CsvReader(reader, config))


                    while (csv.Read())
                    {
                        var employee = new PaySlip
                        {
                            employeeID = csv.GetField<int>(0),
                            firstName = csv.GetField<string>(1),
                            lastName = csv.GetField<string>(2),
                            hourlyRate = csv.GetField<double>(3),
                            taxThreshold = csv.GetField<string>(4)
                        };
                        employees.Add(employee);
                        listBox1.Items.Add($"{employee.employeeID} - {employee.firstName} {employee.lastName}");
                    }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Add code below to complete the implementation to populate the
            // payment summary (textBox2) using the PaySlip and PayCalculatorNoThreshold
            // and PayCalculatorWithThresholds classes object and methods.

            if (listBox1.SelectedIndex == -1)
            {
                MessageBox.Show("You must select an Employee");
                return;
            }

            var selectedEmployeeInfo = listBox1.SelectedItem.ToString();
            var selectedEmployeeID = int.Parse(selectedEmployeeInfo.Split(' ')[0]);
            var selectedEmployee = employees.Find(emp => emp.employeeID == selectedEmployeeID);

            if (selectedEmployee == null)
            {
                MessageBox.Show("Selected employee not found");
                return;
            }

            if (!double.TryParse(hoursWorked.Text, out double hourWorked) || hourWorked < 0)
            {
                MessageBox.Show("Please enter a valid number of hours worked");
                return;
            }

            PayCalculator calculator;
            string YN;
            if (selectedEmployee.taxThreshold == "N")
            {
                PayCalculatorNoThreshold.ReadTaxData();
                calculator = new PayCalculatorNoThreshold(selectedEmployee);
                YN = "(Without Threshold)";
            }
            else
            {
                PayCalculatorWithThreshold.ReadTaxData();
                calculator = new PayCalculatorWithThreshold(selectedEmployee);
                YN = "(With Threshold)";
            }

            calculator.GetGross(hourWorked);
            calculator.GetTax();
            calculator.GetSuper();
            calculator.GetNetPay();

            string newLine = Environment.NewLine;
            textBox2.Text = $"{selectedEmployee.employeeID} - {selectedEmployee.firstName} {selectedEmployee.lastName}" +
                newLine + $"Hours Worked: {hourWorked}" +
                newLine + $"Hourly Rate: {selectedEmployee.hourlyRate:c} per hour" +
                newLine + $"Tax Threshold: {calculator.taxBrac} {YN}" +
                newLine + $"Gross Pay: {calculator.paySlip.grossPay:C}" +
                newLine + $"Tax: {calculator.tax:C}" +
                newLine + $"Net Pay: {calculator.netPay:C}" +
                newLine + $"Super: {calculator.super:C}";
        }


        private void button2_Click(object sender, EventArgs e)
        {
            // Add code below to complete the implementation for saving the
            // calculated payment data into a csv file.
            // File naming convention: Pay_<full name>_<datetimenow>.csv
            // Data fields expected - EmployeeId, Full Name, Hours Worked, Hourly Rate, Tax Threshold, Gross Pay, Tax, Net Pay, Superannuation
            // Check if an employee is selected
            if (listBox1.SelectedIndex == -1)
            {
                MessageBox.Show("You must select an Employee");
                return;
            }

            var selectedEmployeeInfo = listBox1.SelectedItem.ToString();
            var selectedEmployeeID = int.Parse(selectedEmployeeInfo.Split(' ')[0]);
            var selectedEmployee = employees.Find(emp => emp.employeeID == selectedEmployeeID);

            // Check if the selected employee exists
            if (selectedEmployee == null)
            {
                MessageBox.Show("Selected employee not found");
                return;
            }

            // Check if hours worked is valid
            if (!double.TryParse(hoursWorked.Text, out double hoursWorkedValue) || hoursWorkedValue < 0)
            {
                MessageBox.Show("Please enter a valid number of hours worked");
                return;
            }

            // Calculate payment summary
            PayCalculator calculator;
            string taxThresholdInfo;
            if (selectedEmployee.taxThreshold == "N")
            {
                PayCalculatorNoThreshold.ReadTaxData();
                calculator = new PayCalculatorNoThreshold(selectedEmployee);
                taxThresholdInfo = "(Without Threshold)";
            }
            else
            {
                PayCalculatorWithThreshold.ReadTaxData();
                calculator = new PayCalculatorWithThreshold(selectedEmployee);
                taxThresholdInfo = "(With Threshold)";
            }

            calculator.GetGross(hoursWorkedValue);
            calculator.GetTax();
            calculator.GetSuper();
            calculator.GetNetPay();

            // Generate file name
            string fileName = $"Pay_{selectedEmployee.firstName}_{selectedEmployee.lastName}_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

            try
            {
                // Write payment summary to CSV file using CsvHelper
                using (var writer = new StreamWriter(fileName))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    var records = new[]
                    {
                        new
                        {
                            EmployeeID = selectedEmployee.employeeID,
                            FullName = $"{selectedEmployee.firstName} {selectedEmployee.lastName}",
                            HoursWorked = hoursWorkedValue,
                            HourlyRate = selectedEmployee.hourlyRate,
                            TaxThreshold = selectedEmployee.taxThreshold,
                            GrossPay = calculator.paySlip.grossPay,
                            Tax = calculator.tax,
                            NetPay = calculator.netPay,
                            Super = calculator.super,
                        }
                    };
                    csv.WriteRecords(records);
                }

                MessageBox.Show($"Payment data saved to {fileName}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }

        }


    }
}
