using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;

namespace OO_programming
{

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }


    /// <summary>
    /// Class a capture details accociated with an employee's pay slip record
    /// </summary>
    public class PaySlip
    {
        public int employeeID { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public double hourlyRate { get; set; }
        public string taxThreshold { get; set; }
        public double hours { get; set; }
        public double grossPay { get; set; }


    }


    /// <summary>
    /// Base class to hold all Pay calculation functions
    /// Default class behaviour is tax calculated with tax threshold applied
    /// </summary>
    public class PayCalculator
    {
        public PaySlip paySlip;
        public double tax;
        public double super;
        public double netPay;
        public string taxBrac;

        public static int[] _minPay;
        public static int[] _maxPay;
        public static double[] _taxRateA;
        public static double[] _taxRateB;

        public PayCalculator(PaySlip a)
        {
            paySlip = a;
        }

        /// <summary>
        /// Calculates gross pay
        /// </summary>
        /// <param name="hours"></param>
        public void GetGross(double hours)
        {
            paySlip.grossPay = hours * paySlip.hourlyRate;
        }
        /// <summary>
        /// Calculates tax and finds tax bracket
        /// </summary>
        public virtual void GetTax()
        {
            for (int i = 0; i < _minPay.Length; i++)
            {
                if (paySlip.grossPay >= _minPay[i] && paySlip.grossPay <= _maxPay[i])
                {
                    tax = paySlip.grossPay * _taxRateA[i] - _taxRateB[i];
                    taxBrac = $" {_minPay[i]} - {_maxPay[i]}";
                    break;
                }
            }
        }
        /// <summary>
        /// Calculates the super
        /// </summary>
        public void GetSuper()
        {
            super = paySlip.grossPay * 0.11;
        }
        /// <summary>
        /// Calculates net pay
        /// </summary>
        public void GetNetPay()
        {
            netPay = paySlip.grossPay - tax;
        }
    }


    /// <summary>
    /// Extends PayCalculator class handling No tax threshold
    /// </summary>
    public class PayCalculatorNoThreshold : PayCalculator
    {
        public PayCalculatorNoThreshold(PaySlip b) : base(b) { }

        private static string filePath = "../../../taxrate-nothreshold.csv";

        /// <summary>
        /// reads csv data and stores the data into arrays
        /// </summary>
        public static void ReadTaxData()
        {
            List<int> minPay = new List<int>();
            List<int> maxPay = new List<int>();
            List<double> taxRateA = new List<double>();
            List<double> taxRateB = new List<double>();

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false
            };
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, config))
                while (csv.Read())
                {
                    minPay.Add(csv.GetField<int>(0));
                    maxPay.Add(csv.GetField<int>(1));
                    taxRateA.Add(csv.GetField<double>(2));
                    taxRateB.Add(csv.GetField<double>(3));
                }

            _minPay = minPay.ToArray();
            _maxPay = maxPay.ToArray();
            _taxRateA = taxRateA.ToArray();
            _taxRateB = taxRateB.ToArray();

        }

    }

    /// <summary>
    /// Extends PayCalculator class handling With tax threshold
    /// </summary>
    public class PayCalculatorWithThreshold : PayCalculator
    {
        public PayCalculatorWithThreshold(PaySlip b) : base(b) { }
        private static string filePath = "../../../taxrate-withthreshold.csv";

        /// <summary>
        /// reads csv data and stores the data into arrays
        /// </summary>
        public static void ReadTaxData()
        {
            List<int> minPay = new List<int>();
            List<int> maxPay = new List<int>();
            List<double> taxRateA = new List<double>();
            List<double> taxRateB = new List<double>();

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false
            };
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, config))
                while (csv.Read())
                {
                    minPay.Add(csv.GetField<int>(0));
                    maxPay.Add(csv.GetField<int>(1));
                    taxRateA.Add(csv.GetField<double>(2));
                    taxRateB.Add(csv.GetField<double>(3));
                }

            _minPay = minPay.ToArray();
            _maxPay = maxPay.ToArray();
            _taxRateA = taxRateA.ToArray();
            _taxRateB = taxRateB.ToArray();

        }
    }
}

