using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Calculator.MainWindow;
using static System.Net.Mime.MediaTypeNames;

namespace Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string DecimalSeparator => CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator;

        public interface Operation
        {
            public string GetOperation();
            decimal DoOperation(decimal value1, decimal value2);
        }
        public class Addition : Operation
        {
            public string GetOperation() { return "+"; }
            public decimal DoOperation(decimal value1, decimal value2) => value1 + value2;
        }
        public class Multiplication : Operation
        {
            public string GetOperation() { return "*"; }
            public decimal DoOperation(decimal value1, decimal value2) => value1 * value2;
        }
        public class Division : Operation
        {
            public string GetOperation() { return "/"; }
            public decimal DoOperation(decimal value1, decimal value2) //=> value1 / value2;
            {
                if (value2 == 0)
                {
                    return 0;
                }
                return value1 / value2;
            }
        }
        public class Substraction : Operation
        {
            public string GetOperation() { return "-"; }
            public decimal DoOperation(decimal value1, decimal value2) => value1 - value2;
        }

        Operation CurrOperation;
        decimal FirstValue { get; set; }
        string oper = "";

        public MainWindow()
        {
            InitializeComponent();
            btnPoint.Content = DecimalSeparator;

            btnAddition.Tag = new Addition();
            btnMultiplication.Tag = new Multiplication();
            btnDivision.Tag = new Division();
            btnSubtraction.Tag = new Substraction();    
        }

        private void regButtonClick(object sender, RoutedEventArgs e)
        {
            if (TextInput.Text == "0")
                TextInput.Text = "";
            TextInput.Text = $"{TextInput.Text}{((Button)sender).Content}";
        }
        private void btnPoint_Click(object sender, RoutedEventArgs e)
        {
            if (TextInput.Text.Contains(this.DecimalSeparator))
                return;
            regButtonClick(sender, e);
        }
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (TextInput.Text == "0")
                return;
            if (TextInput.Text.Length == 1 || TextInput.Text.Length == 0)
                TextInput.Text = "0";
            else
                TextInput.Text = TextInput.Text.Substring(0, TextInput.Text.Length - 1);
            
            FirstValue = Convert.ToDecimal(TextInput.Text);
            if (TextInput.Text == "")
            {
                TextInput.Text = "0";
                FirstValue = Convert.ToDecimal(TextInput.Text);
            } 
        }
        private void btnClearEntry_Click(object sender, RoutedEventArgs e)
        {
            TextInput.Text = "0";
        }

        private void btnClearAll_Click(object sender, RoutedEventArgs e)
        {
            TextInput.Text = "0";
            FirstValue = Convert.ToDecimal(TextInput.Text);
            TempTextInput.Text = "";
            CurrOperation = null;
        }
        private void btnOperation(object sender, RoutedEventArgs e)
        {
            if (CurrOperation == null)
                FirstValue = Convert.ToDecimal(TextInput.Text);
            CurrOperation = (Operation)((Button)sender).Tag;

            TempTextInput.Text = TextInput.Text + CurrOperation.GetOperation();
            TextInput.Text = "";

        }
        private void btnAllClear_Click(object sender, RoutedEventArgs e)
        {
            FirstValue = 0;
            CurrOperation = null;
            TextInput.Text = "0";
        }

        private void btnEquals_Click(object sender, RoutedEventArgs e)
        {
            if (CurrOperation == null)
                return;
            if (TextInput.Text == "")
                return;
            decimal value2 = Convert.ToDecimal(TextInput.Text);
            TempTextInput.Text += value2;
            TempTextInput.Text += "=";
            TextInput.Text = CurrOperation.DoOperation(FirstValue, value2).ToString();
            FirstValue = Convert.ToDecimal(TextInput.Text);
            CurrOperation = null;
        }
    }
}
