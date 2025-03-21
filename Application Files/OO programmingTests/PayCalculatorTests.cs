using Microsoft.VisualStudio.TestTools.UnitTesting;
using OO_programming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OO_programming.Tests
{
    [TestClass()]
    public class PayCalculatorTests
    {
        [TestMethod]
        public void TestGrossPayCalculation()
        {
            var paySlip = new PaySlip { hourlyRate = 25 };
            var calculator = new PayCalculator(paySlip);

            calculator.GetGross(40);

            Assert.AreEqual(1000, paySlip.grossPay, "Gross pay calculation is incorrect.");
        }

        [TestMethod]
        public void TestSuperannuationCalculation()
        {
            var paySlip = new PaySlip { hourlyRate = 25 };
            var calculator = new PayCalculator(paySlip);
            calculator.GetGross(30);

            calculator.GetSuper();

            Assert.AreEqual(82.50, calculator.super, "Superannuation calculation is incorrect.");
        }
    }
    [TestClass()]
    public class PayCalculatorTestsWithThresh
    {
        [TestInitialize]
        public void Setup()
        {
            PayCalculatorWithThreshold.ReadTaxData();
        }

        [TestMethod]
        public void TestTaxCalculation_WithThreshold()
        {
            var paySlip = new PaySlip { hourlyRate = 25, taxThreshold = "Y" };
            var calculator = new PayCalculatorWithThreshold(paySlip);
            calculator.GetGross(20);

            calculator.GetTax();

            Assert.AreEqual(32.81, Math.Round(calculator.tax, 2), "Tax calculation with threshold is incorrect.");
        }

        [TestMethod]
        public void TestNetPayCalculation_WithThreshold()
        {
            var paySlip = new PaySlip { hourlyRate = 25, taxThreshold = "Y" };
            var calculator = new PayCalculatorWithThreshold(paySlip);
            calculator.GetGross(20);
            calculator.GetTax();

            calculator.GetNetPay();

            Assert.AreEqual(467.19, Math.Round(calculator.netPay, 2), "Net pay calculation with threshold is incorrect.");
        }
    }
    [TestClass()]
    public class PayCalculatorTestsNoThresh
    {
        [TestInitialize]
        public void Setup()
        {
            PayCalculatorNoThreshold.ReadTaxData();
        }

        [TestMethod]
        public void TestTaxCalculation_NoThreshold()
        {
            var paySlip = new PaySlip { hourlyRate = 25, taxThreshold = "N" };
            var calculator = new PayCalculatorNoThreshold(paySlip);
            calculator.GetGross(20);

            calculator.GetTax();

            Assert.AreEqual(111.40, Math.Round(calculator.tax, 2), "Tax calculation without threshold is incorrect.");
        }

        [TestMethod]
        public void TestNetPayCalculation_NoThreshold()
        {
            var paySlip = new PaySlip { hourlyRate = 25, taxThreshold = "N" };
            var calculator = new PayCalculatorNoThreshold(paySlip);
            calculator.GetGross(20);
            calculator.GetTax();

            calculator.GetNetPay();

            Assert.AreEqual(388.60, Math.Round(calculator.netPay, 2), "Net pay calculation without threshold is incorrect.");
        }

    }
}