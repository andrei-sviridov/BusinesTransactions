using System;
using System.Collections.Generic;
using System.Configuration;
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
using System.Data.SqlClient;

namespace BusinesTransactions
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Logger = new Logger();

            SqlQuery = new SqlQueryBuilder();
            if (!SqlQuery.CheckConnection()) 
            {
                Logger.WriteLog("Ошибка подключения к БД!");

                var informationWindow = new InformationWindow("Ошибка подключения к БД!", 0);
                informationWindow.ShowDialog();
            }

            InitGrid();
            LoadData();
        }

        public Logger Logger { get; set; }

        public List<Transaction> Transactions { get; set; }

        /// <summary>
        /// Построитель SQL запросов
        /// </summary>
        public SqlQueryBuilder SqlQuery { get; set; }

        /// <summary>
        /// Вся работа по созданию грида
        /// </summary>
        public void InitGrid()
        {

        }

        public void LoadData()
        {
            if (MainListView != null && MainListView.ItemsSource != null)
            {
                MainListView.ItemsSource = null;
            }

            string sqlExpression = @"
                SELECT
	                  tu.Transaction_Unit_Id			AS	ID
	                , tu.Transaction_Unit_Dttm			AS	DTTM
	                , tu.Transaction_Unit_Summ			AS	SUMM
	                , tu.Transaction_Unit_Comment		AS	COMMENT
	                , to1.Transaction_Object_Name		AS	WRITE_OFF_NAME
	                , tot1.Transaction_Object_Type_Name	AS	WRITE_OFF_TYPE
	                , to2.Transaction_Object_Name		AS	RECEIPT_NAME
	                , tot2.Transaction_Object_Type_Name	AS	RECEIPT_TYPE
                FROM
	                Transaction_Unit tu
	                INNER JOIN Transaction_Object to1
		                ON to1.Transaction_Object_Id = tu.Transaction_Object_Id_Write_Off
	                INNER JOIN Transaction_Object_Type tot1
		                ON tot1.Transaction_Object_Type_Id = to1.Transaction_Object_Type_Id
	                INNER JOIN Transaction_Object to2
		                ON to2.Transaction_Object_Id = tu.Transaction_Object_Id_Receipt
	                INNER JOIN Transaction_Object_Type tot2
		                ON tot2.Transaction_Object_Type_Id = to2.Transaction_Object_Type_Id

            ";
            var list = SqlQuery.DoSelect(sqlExpression);

            List<Transaction> transactions = new List<Transaction>();
            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    Transaction transaction = new Transaction();
                    transaction.ID = item["ID"];
                    transaction.DTTM = item["DTTM"];
                    transaction.SUMM = item["SUMM"];
                    transaction.COMMENT = item["COMMENT"];
                    transaction.WRITE_OFF_NAME = item["WRITE_OFF_NAME"];
                    transaction.WRITE_OFF_TYPE = item["WRITE_OFF_TYPE"];
                    transaction.RECEIPT_NAME = item["RECEIPT_NAME"];
                    transaction.RECEIPT_TYPE = item["RECEIPT_TYPE"];

                    transactions.Add(transaction);
                }
            }
            MainListView.ItemsSource = transactions;
        }

        public void AddTransaction()
        {
            var i = new AddTransactionWindow();
            if (i.ShowDialog() == true)
            {
                this.LoadData();
            }
        }

        public void DeleteTransaction()
        { 
            if (MainListView.SelectedItem != null)
            {
                var selected = (BusinesTransactions.Transaction)MainListView.SelectedItem;

                var informationWindow = new InformationWindow($"Транзакция №{selected.ID} {Environment.NewLine}Удалить выбранную транзикцию?", 1);
                if (informationWindow.ShowDialog() == true)
                {
                    string sqlExpression =
                        $@"DELETE FROM
                            Transaction_Unit
                        WHERE
                            Transaction_Unit_Id = {selected.ID}
                        ";
                    var i = SqlQuery.DoDML(sqlExpression);

                    if (i > 0)
                    {
                        informationWindow = new InformationWindow("Успешное удаление выбранной транзакции", 0);
                        informationWindow.ShowDialog();

                        this.LoadData();
                    }
                    else
                    {
                        informationWindow = new InformationWindow("Ошибка удаления выбранной транзакции", 0);
                        informationWindow.ShowDialog();
                    }
                }
            }
            else
            {
                var informationWindow = new InformationWindow("Не выбрана транзакция для удаления", 0);
                informationWindow.ShowDialog();
            }
        }

        private void AddTransactionButton_Click(object sender, RoutedEventArgs e)
        {
            AddTransaction();
        }

        private void DeleteTransactionButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteTransaction();
        }

        private void EditTransactionButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
