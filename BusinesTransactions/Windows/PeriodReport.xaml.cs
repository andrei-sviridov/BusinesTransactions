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
    /// Логика взаимодействия для PeriodReport.xaml
    /// </summary>
    public partial class PeriodReport : Window
    {
        public PeriodReport()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.WindowState = WindowState.Maximized;

            SqlQuery = new SqlQueryBuilder();

            GridInit();
            SetDefault();
            Refresh();
        }

        /// <summary>
        /// Построитель SQL запросов
        /// </summary>
        public SqlQueryBuilder SqlQuery { get; set; }

        public void GridInit()
        {
            GridView MainGridView = new GridView();

            {
                GridViewColumn gridViewColumn = new GridViewColumn();
                gridViewColumn.Header = "Период";
                gridViewColumn.DisplayMemberBinding = new Binding("DTTM");
                MainGridView.Columns.Add(gridViewColumn);
            }

            {
                GridViewColumn gridViewColumn = new GridViewColumn();
                gridViewColumn.Header = "Общее количество транзакций";
                gridViewColumn.DisplayMemberBinding = new Binding("COUNT_TRANSACTION_UNIT");
                MainGridView.Columns.Add(gridViewColumn);
            }

            {
                GridViewColumn gridViewColumn = new GridViewColumn();
                gridViewColumn.Header = "Общее количество поступлений";
                gridViewColumn.DisplayMemberBinding = new Binding("INCOME_COUNT");
                MainGridView.Columns.Add(gridViewColumn);
            }

            {
                GridViewColumn gridViewColumn = new GridViewColumn();
                gridViewColumn.Header = "Сумма поступлений";
                gridViewColumn.DisplayMemberBinding = new Binding("INCOME_SUM");
                MainGridView.Columns.Add(gridViewColumn);
            }

            {
                GridViewColumn gridViewColumn = new GridViewColumn();
                gridViewColumn.Header = "Самое большое поступление";
                gridViewColumn.DisplayMemberBinding = new Binding("INCOME_MAX");
                MainGridView.Columns.Add(gridViewColumn);
            }

            {
                GridViewColumn gridViewColumn = new GridViewColumn();
                gridViewColumn.Header = "Общее количество покупок";
                gridViewColumn.DisplayMemberBinding = new Binding("BUY_COUNT");
                MainGridView.Columns.Add(gridViewColumn);
            }

            {
                GridViewColumn gridViewColumn = new GridViewColumn();
                gridViewColumn.Header = "Сумма покупок";
                gridViewColumn.DisplayMemberBinding = new Binding("BUY_SUM");
                MainGridView.Columns.Add(gridViewColumn);
            }

            {
                GridViewColumn gridViewColumn = new GridViewColumn();
                gridViewColumn.Header = "Самая дорогая покупка";
                gridViewColumn.DisplayMemberBinding = new Binding("BUY_MAX");
                MainGridView.Columns.Add(gridViewColumn);
            }

            MainListView.View = MainGridView;
        }

        public void LoadData()
        {
            if (MainListView != null && MainListView.ItemsSource != null)
            {
                MainListView.ItemsSource = null;
            }

            // Лист с данными для заполнения грида
            List<RepItem> RepItemList = new List<RepItem>();
            // Начальная дата для алгоритма. Константа. Нужна для определения самой первой даты, с которой начнётся отчёт.
            DateTime startDateTime = DateTime.Parse("20.07.2023");
            // Конечная дата для алгоритма. Константа. Нужна для определения шага отчётных периодов.
            DateTime endDateTime = DateTime.Parse("07.08.2023");
            // Счётчик отчётных периодов. Начинает отсчёт с 0.
            int counter = 0;

            while (endDateTime <= DateTime.Now.Date)
            {
                //для первого отчётного периода оставляем даты как есть, для остальных расчитываем
                if (counter > 0)
                {
                    DateTime tempDateTime = startDateTime;
                    startDateTime = endDateTime;
                    endDateTime = tempDateTime.AddMonths(1);
                }
                counter += 1;

                // Выполняем запрос с полученными датами
                string fromDate = startDateTime.ToString("dd.MM.yyyy");
                string toDate = endDateTime.ToString("dd.MM.yyyy");
                // Текст запроса
                string sqlExpression = $@"
				    SELECT
					      buy.BUY_SUM					AS BUY_SUM	
					    , buy.BUY_COUNT					AS BUY_COUNT	
					    , buy.BUY_MAX					AS BUY_MAX	
					    , income.INCOME_SUM				AS INCOME_SUM		
					    , income.INCOME_COUNT			AS INCOME_COUNT	
					    , income.INCOME_MAX				AS INCOME_MAX		
					    , cnt.COUNT_TRANSACTION_UNIT	AS COUNT_TRANSACTION_UNIT
				    FROM
					    (
						    -- Траты с личных счетов на покупки
						    SELECT
							      SUM(tu.Transaction_Unit_Summ)		AS BUY_SUM
							    , COUNT(tu.Transaction_Unit_Summ)	AS BUY_COUNT
							    , MAX(tu.Transaction_Unit_Summ)		AS BUY_MAX
						    FROM
							    Transaction_Unit tu
							    INNER JOIN Transaction_Object to1
								    ON to1.Transaction_Object_Id = tu.Transaction_Object_Id_Receipt
							    INNER JOIN Transaction_Object to2
								    ON to2.Transaction_Object_Id = tu.Transaction_Object_Id_Write_Off
						    WHERE
							    to1.Transaction_Object_Type_Id = 2
							    AND to2.Transaction_Object_Type_Id = 1
							    AND CAST(tu.Transaction_Unit_Dttm AS DATE) >= '{fromDate}'
							    AND CAST(tu.Transaction_Unit_Dttm AS DATE) <= '{toDate}'
					    ) buy
					    ,
					    (
						    -- Поступления с внешних средств на личные счета
						    SELECT
							      SUM(tu.Transaction_Unit_Summ)		AS INCOME_SUM
							    , COUNT(tu.Transaction_Unit_Summ)	AS INCOME_COUNT
							    , MAX(tu.Transaction_Unit_Summ)		AS INCOME_MAX
						    FROM
							    Transaction_Unit tu
							    INNER JOIN Transaction_Object to1
								    ON to1.Transaction_Object_Id = tu.Transaction_Object_Id_Receipt
							    INNER JOIN Transaction_Object to2
								    ON to2.Transaction_Object_Id = tu.Transaction_Object_Id_Write_Off
							    WHERE
								    to1.Transaction_Object_Type_Id = 1
								    AND to2.Transaction_Object_Type_Id = 3
								    AND CAST(tu.Transaction_Unit_Dttm AS DATE) >= '{fromDate}'
								    AND CAST(tu.Transaction_Unit_Dttm AS DATE) <= '{toDate}'
					    ) income
					    ,
					    (
						    SELECT
							    COUNT(tu.Transaction_Unit_Id)	AS COUNT_TRANSACTION_UNIT
						    FROM
							    Transaction_Unit tu
						    WHERE
							    CAST(tu.Transaction_Unit_Dttm AS DATE) >= '{fromDate}'
							    AND CAST(tu.Transaction_Unit_Dttm AS DATE) <= '{toDate}'
					    ) cnt

                ";
                var list = SqlQuery.DoSelect(sqlExpression);

                // Заполняем лист с данными для грида
                if (list != null && list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        RepItem RepItem = new RepItem();
                        RepItem.COUNT_TRANSACTION_UNIT = item["COUNT_TRANSACTION_UNIT"];
                        RepItem.BUY_COUNT = item["BUY_COUNT"];
                        RepItem.BUY_SUM = item["BUY_SUM"];
                        RepItem.BUY_MAX = item["BUY_MAX"];
                        RepItem.INCOME_COUNT = item["INCOME_COUNT"];
                        RepItem.INCOME_SUM = item["INCOME_SUM"];
                        RepItem.INCOME_MAX = item["INCOME_MAX"];
                        RepItem.START_DTTM = fromDate;
                        RepItem.END_DTTM = toDate;
                        RepItem.DTTM = $"{fromDate} - {toDate}";

                        RepItemList.Add(RepItem);
                    }
                }
            }

            // Заполняем грид полученными данными
            MainListView.ItemsSource = RepItemList;
        }

        public void SetDefault()
        {

        }

        public void Refresh()
        {
            this.LoadData();
        }

        public void CloseWindow()
        {
            this.Close();
        }

        private void ShowButton_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow();
        }

        /// <summary>
        /// Строка данных грида
        /// </summary>
        public class RepItem
        {
            public RepItem()
            {

            }

            public string COUNT_TRANSACTION_UNIT { get; set; }

            public string BUY_COUNT { get; set; }

            public string BUY_SUM { get; set; }

            public string BUY_MAX { get; set; }

            public string INCOME_COUNT { get; set; }

            public string INCOME_SUM { get; set; }

            public string INCOME_MAX { get; set; }

            public string START_DTTM { get; set; }

            public string END_DTTM { get; set; }

            public string DTTM { get; set; }

        }
    }
}
