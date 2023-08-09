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
using System.Windows.Shapes;

namespace BusinesTransactions
{
    /// <summary>
    /// Логика взаимодействия для AddTransactionObjectWindow.xaml
    /// </summary>
    public partial class AddTransactionObjectWindow : Window
    {
        public AddTransactionObjectWindow()
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
            NAMETextBox.Clear();
            COMMENTTextBox.Clear();
        }

        public void SetComboBoxItems()
        {
            string sqlExpression = @"
                SELECT
	                  Transaction_Object_Type_Id	AS ID
	                , Transaction_Object_Type_Name	AS NAME
                FROM
	                Transaction_Object_Type

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

                TYPEComboBox.ItemsSource = comboboxItems;
            }
        }

        public void Save()
        {
            if (!string.IsNullOrEmpty(NAMETextBox.Text) && TYPEComboBox.SelectedItem != null)
            {
                string sqlExpression = $@"
                    INSERT INTO
	                    Transaction_Object
	                    (
		                      Transaction_Object_Name
		                    , Transaction_Object_Comment
		                    , Transaction_Object_Type_Id
	                    )
                    VALUES
	                    (
                              '{NAMETextBox.Text}'
                            , '{COMMENTTextBox.Text}'
                            , {((BusinesTransactions.CBItem)TYPEComboBox.SelectedItem).ID}
	                    )

                ";
                var i = SqlQuery.DoDML(sqlExpression);

                if (i > 0)
                {
                    var informationWindow = new InformationWindow("Успешное добавление нового счёта", 0);
                    informationWindow.ShowDialog();

                    this.DialogResult = true;
                    CloseWindow();
                }
                else
                {
                    var informationWindow = new InformationWindow($"Ошибка добавления нового счёта", 0);
                    informationWindow.ShowDialog();
                }
            }
        }

        public void CloseWindow()
        {
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }
    }
}
