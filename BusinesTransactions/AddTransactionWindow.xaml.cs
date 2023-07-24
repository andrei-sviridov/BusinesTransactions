﻿using System;
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
            DTTMTextBox.Text = $"{DateTime.Now.ToString("dd.MM.yyyy")} {"12:00:00"}";
            SUMMTextBox.Clear();
            COMMENTTextBox.Clear();
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
                !string.IsNullOrEmpty(DTTMTextBox.Text)
                && !string.IsNullOrEmpty(SUMMTextBox.Text)
                && !string.IsNullOrEmpty(COMMENTTextBox.Text)
                )
            {
                if (WRITE_OFFComboBox.SelectedItem != null && RECEIPTComboBox.SelectedItem != null)
                {
                    string sqlExpression =
                        "INSERT INTO Transaction_Unit " +
                        "(Transaction_Unit_Dttm, Transaction_Unit_Summ, Transaction_Unit_Comment, Transaction_Object_Id_Receipt, Transaction_Object_Id_Write_Off)" +
                        "VALUES " +
                        $"(" +
                        $"  '{DTTMTextBox.Text}'" +
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
    }
}
