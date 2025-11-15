
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace BadCalc_VeryBad
{

    // variables globales para toda la aplicacion
    public class GloabalsVariables
    {
        // historial de operaciones
        public ArrayList Historial { get; } = [];
        // contador de operaciones
        public int Counter { get; set; } = 0;
        // variable miscelanea
        public string Misc { get; set; }
    }



    static class Program
    {
        public static readonly GloabalsVariables globals = new();

        static void Main()
        {
            RunCalculator();
        }

        private static void RunCalculator()
        {
            bool exitRequested = false;

            while (!exitRequested)
            {
                ShowMenu();
                var option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        ProcessBinaryOperation("+", (x, y) => x + y);
                        break;
                    case "2":
                        ProcessBinaryOperation("-", (x, y) => x - y);
                        break;
                    case "3":
                        ProcessBinaryOperation("*", (x, y) => x * y);
                        break;
                    case "4":
                        ProcessDivision();
                        break;
                    case "5":
                        ProcessBinaryOperation("^", (x, y) => Math.Pow(x, y));
                        break;
                    case "6":
                        ProcessBinaryOperation("%", (x, y) => x % y);
                        break;
                    case "7":
                        ProcessSqrt();
                        break;
                    case "8":
                        ShowHistory();
                        break;
                    case "0":
                        exitRequested = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }

        private static void ShowMenu()
        {
            Console.WriteLine("Calculadora arreglada");
            Console.WriteLine("1) add  2) sub  3) mul  4) div  5) pow  6) mod  7) sqrt  8) hist  0) exit");
            Console.Write("opt: ");
        }

        private static bool TryReadTwoOperands(out string a, out string b, out double x, out double y)
        {
            a = GetNumericInput("a: ");
            b = GetNumericInput("b: ");

            if (!double.TryParse(a, NumberStyles.Float, CultureInfo.InvariantCulture, out x) ||
                !double.TryParse(b, NumberStyles.Float, CultureInfo.InvariantCulture, out y))
            {
                Console.WriteLine("Invalid numeric input.");
                y = 0;
                return false;
            }

            return true;
        }

        private static void ProcessBinaryOperation(string op, Func<double, double, double> operation)
        {
            if (!TryReadTwoOperands(out var a, out var b, out var x, out var y))
            {
                return;
            }

            var result = operation(x, y);
            SaveAndPrint(a, b, op, result);
        }

        private static void ProcessDivision()
        {
            if (!TryReadTwoOperands(out var a, out var b, out var x, out var y))
            {
                return;
            }

            if (Math.Abs(y) < 1e-8)
            {
                Console.WriteLine("Error: Division by zero.");
                return;
            }

            var result = x / y;
            SaveAndPrint(a, b, "/", result);
        }

        private static void ProcessSqrt()
        {
            var a = GetNumericInput("a: ");

            if (!double.TryParse(a, NumberStyles.Float, CultureInfo.InvariantCulture, out var x))
            {
                Console.WriteLine("Invalid numeric input.");
                return;
            }

            if (x < 0)
            {
                Console.WriteLine("Error: Cannot compute square root of negative number.");
                return;
            }

            var result = Math.Sqrt(x);
            SaveAndPrint(a, "0", "sqrt", result);
        }

        private static void ShowHistory()
        {
            foreach (var item in globals.Historial)
            {
                Console.WriteLine(item);
            }

            Thread.Sleep(100);
        }

        private static void SaveAndPrint(string a, string b, string op, double res)
        {
            SaveOperation(a, b, op, res);
            Console.WriteLine("= " + res.ToString(CultureInfo.InvariantCulture));
            globals.Counter++;
            Thread.Sleep(new Random().Next(0, 2));
        }

        static string GetNumericInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }

        static void SaveOperation(string a, string b, string op, double res)
        {
            try
            {
                var line = a + "|" + b + "|" + op + "|" + res.ToString("0.###############", CultureInfo.InvariantCulture);
                globals.Historial.Add(line);
                globals.Misc = line;
                File.AppendAllText("history.txt", line + Environment.NewLine);
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not write history.txt: " + e.Message);
            }
        }
    }
}
