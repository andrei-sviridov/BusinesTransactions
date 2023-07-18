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
            DBConnect();

            InitGrid();
        }

        public List<Transaction> Transactions { get; set; }


        public void InitGrid()
        {
            LoadColumns();
            LoadData();
        }

        public void LoadColumns()
        {
            MainGrid.AutoGenerateColumns = false;
            MainGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "ИД",
                Binding = new Binding("Id")
            });
            MainGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Дата",
                Binding = new Binding("DateTime")
            });
            MainGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Сбербанк",
                Binding = new Binding("Sberbank")
            });
            MainGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "--",
                Binding = new Binding("Vector1")
            });
            MainGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Тинькофф",
                Binding = new Binding("Tincoff")
            });
            MainGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "--",
                Binding = new Binding("Vector2")
            });
            MainGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "ВТБ",
                Binding = new Binding("VTB")
            });
            MainGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Комментарий",
                Binding = new Binding("Comment")
            });
        }

        public void LoadData()
        {
            Transactions = new List<Transaction>();

            {
                Transaction transaction = new Transaction();
                transaction.Id = 1;
                transaction.DateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
                transaction.Sberbank = "1234";
                transaction.Vector1 = "->";
                transaction.Tincoff = "1234";
                transaction.Vector2 = "";
                transaction.VTB = "1234";
                transaction.Comment = "test";

                Transactions.Add(transaction);
            }


            MainGrid.ItemsSource = Transactions;
        }

        /// <summary>
        /// Строка данных таблицы
        /// </summary>
        public class Transaction
        {
            /// <summary>
            /// Конструктор строки данных таблицы
            /// </summary>
            public Transaction()
            {

            }

            public int Id { get; set; }

            /// <summary>
            /// Дата и время транзакции
            /// </summary>
            public string DateTime { get; set; }

            public string Sberbank { get; set; }

            public string Vector1 { get; set; }

            public string Tincoff { get; set; }

            public string Vector2 { get; set; }

            public string VTB { get; set; }

            public string Comment { get; set; }

        }
    }
}
