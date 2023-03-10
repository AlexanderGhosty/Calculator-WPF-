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
                    MessageBox.Show("Ошибка: Попытка деления на 0", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
        decimal buffer = 0;
        bool historyWritingFlag = true;

        public MainWindow()
        {
            InitializeComponent();
            btnPoint.Content = DecimalSeparator;

            btnAddition.Tag = new Addition();
            btnMultiplication.Tag = new Multiplication();
            btnDivision.Tag = new Division();
            btnSubtraction.Tag = new Substraction();
            RadioButtonOn.IsChecked = true;
            historyWritingFlag = true;
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
            else if (TempTextInput.Text == "")
                TextInput.Text = TextInput.Text.Substring(0, TextInput.Text.Length - 1);
            else if (CurrOperation == null)
            {
                TempTextInput.Text = "";
            }
            else
                TextInput.Text = TextInput.Text.Substring(0, TextInput.Text.Length - 1);
            
            //FirstValue = Convert.ToDecimal(TextInput.Text);
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
            {
                FirstValue = Convert.ToDecimal(TextInput.Text);
                buffer = FirstValue;
            }
            if (CurrOperation != null && TextInput.Text != "")
            {
                if(historyWritingFlag == true)
                {
                    IsEmptyHistoryBox.Text = "";
                    HistoryBox.Text += Math.Round(FirstValue, 9).ToString() + " " + CurrOperation.GetOperation();
                }
                    
                FirstValue = CurrOperation.DoOperation(FirstValue, Convert.ToDecimal(TextInput.Text));
                if (historyWritingFlag == true)
                    HistoryBox.Text += " " + TextInput.Text + " = " + Math.Round(FirstValue, 9).ToString() + "\n";
            }

            CurrOperation = (Operation)((Button)sender).Tag;
            TempTextInput.Text = Math.Round(FirstValue, 9) + CurrOperation.GetOperation();
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

            if (historyWritingFlag == true)
                HistoryBox.Text += FirstValue.ToString() + " " + CurrOperation.GetOperation() + " " + value2 + " = ";
            FirstValue = CurrOperation.DoOperation(FirstValue, value2);
            TextInput.Text = Math.Round(FirstValue, 9).ToString();

            if (historyWritingFlag == true)
                HistoryBox.Text += Math.Round(FirstValue, 9).ToString() + "\n";
            CurrOperation = null;
        }

        private void historyButtonClick(object sender, RoutedEventArgs e)
        {
            if (historyGrid.Visibility == Visibility.Visible)
                historyGrid.Visibility = Visibility.Collapsed;
            else
                historyGrid.Visibility = Visibility.Visible;
        }

        private void historyClearButtonClick(object sender, RoutedEventArgs e)
        {
            if (HistoryBox.Text != "")
            {
                HistoryBox.Text = "";
                IsEmptyHistoryBox.Text = "Журнал операций пуст";
            }
        }
        private void historyBackButtonClick(object sender, RoutedEventArgs e)
        {
            if (HistoryBox.Text != "")
            {
                string str = HistoryBox.Text;
                int lastSlashIndex = str.LastIndexOf('\n');
                if (lastSlashIndex >= 0) // Проверяем, что символ '/' найден
                {
                    str = str.Substring(0, lastSlashIndex); // Получаем подстроку после последнего символа '/'
                    lastSlashIndex = str.LastIndexOf('\n');
                    str = str.Substring(0, lastSlashIndex + 1);
                }
                else
                {
                    str = "";
                }
                HistoryBox.Text = str;
                if (HistoryBox.Text == "")
                    IsEmptyHistoryBox.Text = "Журнал операций пуст";
            }
        }
        private void RadioButtonOff_Checked(object sender, RoutedEventArgs e) 
        {
            historyButton.Visibility = Visibility.Collapsed;
            writeHistoryButton.Visibility = Visibility.Collapsed;
        }
        private void RadioButtonOn_Checked(object sender, RoutedEventArgs e)
        {
            historyButton.Visibility = Visibility.Visible;
            writeHistoryButton.Visibility = Visibility.Visible;
        }

        private void writeHistoryButton_Checked(object sender, RoutedEventArgs e)
        {
            historyWritingFlag = true;
        }
        private void writeHistoryButton_UnChecked(object sender, RoutedEventArgs e)
        {
            historyWritingFlag = false;
        }
    }
}
