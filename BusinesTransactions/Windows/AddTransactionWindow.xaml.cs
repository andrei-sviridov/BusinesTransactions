using System;
using System.Collections.Generic;
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

namespace BusinesTransactions
{
    /// <summary>
    /// Логика взаимодействия для AddTransactionWindow.xaml
    /// </summary>
    public partial class AddTransactionWindow : Window
    {
        public AddTransactionWindow()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            SqlQuery = new SqlQueryBuilder();

            SetDefault();
            SetComboBoxItems();
        }

        /// <summary>
        /// Построитель SQL запросов
        /// </summary>
        public SqlQueryBuilder SqlQuery { get; set; }

        public void SetDefault()
        {
            SUMMTextBox.Clear();
            COMMENTTextBox.Clear();
            DateDatePicker.SelectedDate = DateTime.Now;
            HourTextBox.Text = DateTime.Now.ToString("HH");
            MinuteTextBox.Text = DateTime.Now.ToString("mm");
            SecondTextBox.Text = "00";
        }

        public void SetComboBoxItems()
        {
            string sqlExpression = @"
                SELECT
	                  Transaction_Object_Id		AS ID
	                , Transaction_Object_Name	AS NAME
                FROM
	                Transaction_Object

            ";
            var list = SqlQuery.DoSelect(sqlExpression);

            if (list != null && list.Count > 0)
            {
                List<CBItem> comboboxItems = new List<CBItem>();

                foreach (var item in list)
                {
                    CBItem cBItem = new CBItem();
                    cBItem.ID = item["ID"];
                    cBItem.NAME = item["NAME"];

                    comboboxItems.Add(cBItem);
                }

                WRITE_OFFComboBox.ItemsSource = comboboxItems;
                RECEIPTComboBox.ItemsSource = comboboxItems;
            }
        }
        
        public void Save()
        {
            if (
                !string.IsNullOrEmpty(SUMMTextBox.Text)
                && !string.IsNullOrEmpty(COMMENTTextBox.Text)
                )
            {
                if (WRITE_OFFComboBox.SelectedItem != null && RECEIPTComboBox.SelectedItem != null)
                {
                    string dt = ((DateTime)DateDatePicker.SelectedDate).ToString("dd.MM.yyyy");
                    string tm = $"{HourTextBox.Text}:{MinuteTextBox.Text}:{SecondTextBox.Text}";

                    string sqlExpression =
                        "INSERT INTO Transaction_Unit " +
                        "(Transaction_Unit_Dttm, Transaction_Unit_Summ, Transaction_Unit_Comment, Transaction_Object_Id_Receipt, Transaction_Object_Id_Write_Off)" +
                        "VALUES " +
                        $"(" +
                        $"  '{dt} {tm}'" +
                        $", {SUMMTextBox.Text}" +
                        $", '{COMMENTTextBox.Text}'" +
                        $", {((BusinesTransactions.CBItem)RECEIPTComboBox.SelectedItem).ID}" +
                        $", {((BusinesTransactions.CBItem)WRITE_OFFComboBox.SelectedItem).ID}" +
                        $")";
                    var i = SqlQuery.DoDML(sqlExpression);

                    if (i > 0)
                    {
                        var informationWindow = new InformationWindow("Успешное добавление новой транзакции", 0);
                        informationWindow.ShowDialog();

                        this.DialogResult = true;
                        CloseWindow();
                    }
                    else
                    {
                        var informationWindow = new InformationWindow($"Ошибка добавления новой транзакции", 0);
                        informationWindow.ShowDialog();
                    }
                }
            }
        }

        public void CloseWindow()
        {
            this.Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow();
        }

        private void HourTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (HourTextBox.Text.Length >= 2 && HourTextBox.SelectedText.Length != HourTextBox.Text.Length)
            {
                e.Handled = true;
            }
        }

        private void MinuteTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (MinuteTextBox.Text.Length >= 2 && MinuteTextBox.SelectedText.Length != MinuteTextBox.Text.Length)
            {
                e.Handled = true;
            }
        }

        private void SecondTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (SecondTextBox.Text.Length >= 2 && SecondTextBox.SelectedText.Length != SecondTextBox.Text.Length)
            {
                e.Handled = true;
            }
        }

        private void SUMMTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == ",")
            {
                int caretIndex = SUMMTextBox.CaretIndex;
                string oldText = SUMMTextBox.Text;
                string newText = oldText.Insert(caretIndex, ".");
                SUMMTextBox.Text = newText;
                SUMMTextBox.CaretIndex = caretIndex + 1;
                e.Handled = true;
            }
        }


        //        string oldText = TB.Text;
        //        int caretIndex = TB.CaretIndex;
        //        string replacedOldText = oldText.Replace(":", null);
        //        string newText = "";
        //        string newFormatedText = "";

        //            if (oldText.Length != caretIndex)
        //            {
        //                int localCaretIndex = -1;
        //        char[] replacedOldTextItems = replacedOldText.ToCharArray();
        //                foreach (char replacedOldTextItem in replacedOldTextItems)
        //                {
        //                    localCaretIndex++;
        //                    if (localCaretIndex == caretIndex)
        //                    {
        //                        newText += e.Text;
        //                    }

        //    newText += replacedOldTextItem;
        //                }
        //            }
        //            else
        //{
        //    newText = $"{replacedOldText}{e.Text}";
        //}

        //if (!string.IsNullOrEmpty(newText))
        //{
        //    int counter = 0;
        //    char[] replacedNewTextItems = newText.ToCharArray();
        //    foreach (var replacedNewTextItem in replacedNewTextItems)
        //    {
        //        newFormatedText = $"{newFormatedText}{replacedNewTextItem}";

        //        counter++;
        //        if (counter % 2 == 0)
        //        {
        //            newFormatedText = $"{newFormatedText}:";
        //        }
        //    }
        //}

        //TB.Text = newFormatedText;
        ////TB.CaretIndex = caretIndex;
        //e.Handled = true;


        //string oldText = TB.Text;
        //string inputText = e.Text;
        //if (!string.IsNullOrEmpty(oldText))
        //{
        //    if (oldText.Length > 7)
        //    {
        //        inputText = null;
        //    }
        //    else
        //    {
        //        string replacedOldText = oldText.Replace(":", null);
        //        if (replacedOldText.Length % 2 != 0 && oldText.Length != 7)
        //        {
        //            inputText = $"{e.Text}:";
        //        }
        //    }
        //}

        //string newText = $"{oldText}{inputText}";
        //TB.Text = newText;
        //TB.CaretIndex = newText.Length;
        //e.Handled = true;
    }
}
