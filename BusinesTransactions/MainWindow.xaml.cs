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
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.WindowState = WindowState.Maximized;

            FromDatePicker.SelectedDate = DateTime.Now.AddDays(-7);
            ToDatePicker.SelectedDate = DateTime.Now;

            Logger = new Logger();

            SqlQuery = new SqlQueryBuilder();
            if (!SqlQuery.CheckConnection()) 
            {
                Logger.WriteLog("Ошибка подключения к БД!");

                var informationWindow = new InformationWindow("Ошибка подключения к БД!", 0);
                informationWindow.ShowDialog();
            }

            InitGrid();
            Refresh();
        }

        public Logger Logger { get; set; }

        /// <summary>
        /// Список транзикций, полученных из БД
        /// </summary>
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

            string fromDate = ((DateTime)FromDatePicker.SelectedDate).ToString("dd.MM.yyyy");
            string toDate = ((DateTime)ToDatePicker.SelectedDate).ToString("dd.MM.yyyy");

            string sqlExpression = $@"
                SELECT
	                  tu.Transaction_Unit_Id				AS	ID
	                , tu.Transaction_Unit_Dttm				AS	DTTM
	                , tu.Transaction_Unit_Summ				AS	SUMM
	                , tu.Transaction_Unit_Comment			AS	COMMENT
                    , to1.Transaction_Object_Id			    AS	WRITE_OFF_ID
	                , to1.Transaction_Object_Name			AS	WRITE_OFF_NAME
	                , tot1.Transaction_Object_Type_Name		AS	WRITE_OFF_TYPE
					, tob1.Transaction_Object_Balance_Summ	AS  WRITE_OFF_BALANCE
	                , to2.Transaction_Object_Id			    AS	RECEIPT_ID
	                , to2.Transaction_Object_Name			AS	RECEIPT_NAME
	                , tot2.Transaction_Object_Type_Name		AS	RECEIPT_TYPE
					, tob2.Transaction_Object_Balance_Summ	AS  RECEIPT_BALANCE
                FROM
	                Transaction_Unit tu

	                INNER JOIN Transaction_Object to1
		                ON to1.Transaction_Object_Id = tu.Transaction_Object_Id_Write_Off
	                INNER JOIN Transaction_Object_Type tot1
		                ON tot1.Transaction_Object_Type_Id = to1.Transaction_Object_Type_Id
					LEFT JOIN Transaction_Object_Balance tob1
						ON tob1.Transaction_Object_Id = to1.Transaction_Object_Id
						AND tob1.Transaction_Unit_Id = tu.Transaction_Unit_Id

	                INNER JOIN Transaction_Object to2
		                ON to2.Transaction_Object_Id = tu.Transaction_Object_Id_Receipt
	                INNER JOIN Transaction_Object_Type tot2
		                ON tot2.Transaction_Object_Type_Id = to2.Transaction_Object_Type_Id
					LEFT JOIN Transaction_Object_Balance tob2
						ON tob2.Transaction_Object_Id = to2.Transaction_Object_Id
						AND tob2.Transaction_Unit_Id = tu.Transaction_Unit_Id
				WHERE
					CAST(tu.Transaction_Unit_Dttm AS DATE) >= '{fromDate}'
					AND CAST(tu.Transaction_Unit_Dttm AS DATE) <= '{toDate}'
				ORDER BY
					tu.Transaction_Unit_Dttm

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
                    transaction.WRITE_OFF_ID= item["WRITE_OFF_ID"];
                    transaction.WRITE_OFF_NAME = item["WRITE_OFF_NAME"];
                    transaction.WRITE_OFF_TYPE = item["WRITE_OFF_TYPE"];
                    transaction.WRITE_OFF_BALANCE = item["WRITE_OFF_BALANCE"];
                    transaction.RECEIPT_ID= item["RECEIPT_ID"];
                    transaction.RECEIPT_NAME = item["RECEIPT_NAME"];
                    transaction.RECEIPT_TYPE = item["RECEIPT_TYPE"];
                    transaction.RECEIPT_BALANCE = item["RECEIPT_BALANCE"];

                    transactions.Add(transaction);
                }
            }

            Transactions = transactions;
            MainListView.ItemsSource = Transactions;
        }

        public void FilterItems()
        {
            // Если есть что фильтровать, то фильтруем
            if (MainListView != null && MainListView.ItemsSource != null && Transactions != null && Transactions.Count > 0)
            {
                MainListView.ItemsSource = null;
                List<Transaction> resultItemSource = Transactions;

                // Фильтрация проходит последовательно

                if (WRITE_OFFComboBox.SelectedItem != null)
                {
                    string writeOffId = ((BusinesTransactions.CBItem)WRITE_OFFComboBox.SelectedItem).ID;
                    if (writeOffId != "-1")
                    {
                        resultItemSource = resultItemSource.Where(x => x.WRITE_OFF_ID == writeOffId).ToList();
                    }
                }

                if (RECEIPTComboBox.SelectedItem != null)
                {
                    string receiptId = ((BusinesTransactions.CBItem)RECEIPTComboBox.SelectedItem).ID;
                    if (receiptId != "-1")
                    {
                        resultItemSource = resultItemSource.Where(x => x.RECEIPT_ID == receiptId).ToList();
                    }
                }

                MainListView.ItemsSource = resultItemSource;
            }
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

                // Добавляем элемент для выбора всех значений
                {
                    CBItem cBItem = new CBItem();
                    cBItem.ID = "-1";
                    cBItem.NAME = "Все";

                    comboboxItems.Add(cBItem);
                }

                foreach (var item in list)
                {
                    CBItem cBItem = new CBItem();
                    cBItem.ID = item["ID"];
                    cBItem.NAME = item["NAME"];

                    comboboxItems.Add(cBItem);
                }

                WRITE_OFFComboBox.ItemsSource = comboboxItems;
                WRITE_OFFComboBox.SelectedItem = ((List<CBItem>)WRITE_OFFComboBox.ItemsSource).FirstOrDefault(x => x.ID == "-1");
                RECEIPTComboBox.ItemsSource = comboboxItems;
                RECEIPTComboBox.SelectedItem = ((List<CBItem>)RECEIPTComboBox.ItemsSource).FirstOrDefault(x => x.ID == "-1");
            }
        }

        /// <summary>
        /// Получаем данные по текущему балансу на личных счетах
        /// </summary>
        public void LoadCurrentBalanceData()
        {
            CurrentBankTextBox.Clear();
            CurrentBalanceTextBox.Clear();

            string currentBankText = $"Счёт{Environment.NewLine}---{Environment.NewLine}";
            string currentBalanceText = $"Баланс{Environment.NewLine}---{Environment.NewLine}";

            string sqlExpression = @"
                SELECT
	                  ISNULL(tob.Transaction_Object_Balance_Summ, 0.0)	AS BALANCE_SUMM
	                , to1.Transaction_Object_Id							AS ID
	                , to1.Transaction_Object_Name						AS NAME
                FROM	
	                Transaction_Object to1
	                LEFT JOIN (
		                SELECT	
			                  tob.Transaction_Object_Id
			                , tob.Transaction_Object_Balance_Summ
		                FROM
			                Transaction_Object_Balance tob
			                INNER JOIN (
				                SELECT
					                  MAX(tob1.Transaction_Object_Balance_Dttm)	AS Transaction_Object_Balance_Dttm
					                , tob1.Transaction_Object_Id				AS Transaction_Object_Id
				                FROM
					                Transaction_Object_Balance tob1
				                GROUP BY
					                tob1.Transaction_Object_Id
			                ) tob1
				                ON tob1.Transaction_Object_Id = tob.Transaction_Object_Id
				                AND tob1.Transaction_Object_Balance_Dttm = tob.Transaction_Object_Balance_Dttm
	                ) tob
		                ON tob.Transaction_Object_Id = to1.Transaction_Object_Id
                WHERE
	                to1.Transaction_Object_Type_Id = 1
                ORDER BY
	                to1.Transaction_Object_Id

            ";
            var list = SqlQuery.DoSelect(sqlExpression);

            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    currentBankText = $"{currentBankText}{item["NAME"]}{Environment.NewLine}---{Environment.NewLine}";
                    currentBalanceText = $"{currentBalanceText}{item["BALANCE_SUMM"]}{Environment.NewLine}---{Environment.NewLine}";
                }
            }

            CurrentBankTextBox.Text = currentBankText;
            CurrentBalanceTextBox.Text = currentBalanceText;
        }

        public void Refresh()
        {
            this.LoadData();
            SetComboBoxItems();
            LoadCurrentBalanceData();
        }

        public void AddTransaction()
        {
            var i = new AddTransactionWindow();
            if (i.ShowDialog() == true)
            {
                Refresh();
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

                        Refresh();
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

        /// <summary>
        /// Отчёт по двухнедельным периодам
        /// </summary>
        public void PeriodReport()
        {
            var i = new PeriodReport();
            if (i.ShowDialog() == true)
            {
                //Refresh();
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

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void WRITE_OFFComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterItems();
        }

        private void RECEIPTComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterItems();
        }

        private void AddTransactionMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AddTransaction();
        }

        private void PeriodReportMenuItem_Click(object sender, RoutedEventArgs e)
        {
            PeriodReport();
        }
    }
}
