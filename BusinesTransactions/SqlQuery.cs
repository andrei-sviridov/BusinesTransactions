using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinesTransactions
{
    public class SqlQuery
    {
        public SqlQuery()
        {
            DBConnect();
        }


        private SqlConnection _Connection { get; set; }

        /// <summary>
        /// Создание и проверка подключения к БД
        /// </summary>
        public void DBConnect()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
            // Создание подключения
            _Connection = new SqlConnection(connectionString);
            try
            {
                // Открываем подключение
                _Connection.Open();
            }
            catch (SqlException ex)
            {
                // log
            }
        }

        public int DoInsert(string sqlExpression)
        {
            int editedRowCount = 0;

            SqlCommand command = new SqlCommand(sqlExpression, _Connection);
            editedRowCount = command.ExecuteNonQuery();

            return editedRowCount;
        }
    }
}
