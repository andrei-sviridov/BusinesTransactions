using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinesTransactions
{
    public class SqlQueryBuilder
    {
        public SqlQueryBuilder()
        {
            DBConnect();
        }

        /// <summary>
        /// Подключение к базе данных
        /// </summary>
        private SqlConnection _Connection { get; set; }

        ~SqlQueryBuilder()
        {
            Destroy();
        }

        /// <summary>
        /// Деструктор. Закрывает соединение с базой данных
        /// </summary>
        public void Destroy()
        {
            if (_Connection != null)
            {
                _Connection.Close();
            }
        }

        /// <summary>
        /// Создание и проверка подключения к БД
        /// </summary>
        private void DBConnect()
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

        /// <summary>
        /// Проверяем подключение к базе данных
        /// </summary>
        /// <returns></returns>
        public bool CheckConnection()
        {
            bool succesfullConnectionFlag = false;
            string databaseServerVersion = "";

            if (_Connection != null)
            {
                try
                {
                    databaseServerVersion = _Connection.ServerVersion;
                }
                catch (Exception ex)
                {
                    // log
                }
            }

            if (!string.IsNullOrEmpty(databaseServerVersion))
            {
                succesfullConnectionFlag = true;
            }

            return succesfullConnectionFlag;
        }

        /// <summary>
        /// Выполнение SQL команды языка манипулирования данными(DML):
        /// INSERT;
        /// UPDATE;
        /// DELETE.
        /// </summary>
        /// <param name="sqlExpression">Строка содержащая SQL-выражение</param>
        /// <returns>Количество строк, изменённых в результате запроса (При успешном выполнении > 0 )</returns>
        public int DoDML(string sqlExpression)
        {
            int editedRowCount = 0;

            SqlCommand command = new SqlCommand(sqlExpression, _Connection);
            editedRowCount = command.ExecuteNonQuery();

            return editedRowCount;
        }

        /// <summary>
        /// Выполнение SQL команды выборки данных:
        /// SELECT.
        /// </summary>
        /// <param name="sqlExpression">Строка содержащая SQL-выражение</param>
        /// <returns>Лист содержащий построчный ответ в формате Key -- Имя столбца; Value -- Значение в этом столбце для этой строки</returns>
        public List<Dictionary<string, string>> DoSelect(string sqlExpression)
        {
            List<Dictionary<string, string>> queryResultList = new List<Dictionary<string, string>>();

            // Количество столбцов в таблице ответа
            int rowCount = 0;

            SqlCommand command = new SqlCommand(sqlExpression, _Connection);
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                // Получаем количество столбцов таблицы ответа
                rowCount = reader.FieldCount;

                // построчно считываем данные
                // Проходим по всем строкам ответа
                while (reader.Read())
                {
                    // Для каждой строки данных ответа создаём Dictionary<string, string>
                    Dictionary<string, string> dictionary = new Dictionary<string, string>();
                    // Проходим по всем колонкам ответа и получаем её имя и данные, хранящиеся в сотответствующей колонке строки
                    for (int i = 0; i < rowCount; i++)
                    {
                        dictionary.Add(reader.GetName(i), reader.GetValue(i).ToString());
                    }

                    // Заполненную строку ответа заносим в List<Dictionary<string, string>>
                    queryResultList.Add(dictionary);
                }
            }

            // Пока один SqlDataReader не закрыт, другой объект SqlDataReader для одного и того же подключения мы использовать не сможем
            reader.Close();

            return queryResultList;
        }

        /// <summary>
        /// Выполнение SQL команды агрегатной функции:
        /// MIN;
        /// MAX;
        /// COUNT;
        /// SUMM;
        /// AVG.
        /// </summary>
        /// <param name="sqlExpression">Строка содержащая SQL-выражение</param>
        /// <returns>Результат выполнения агрегатной функции, приведённый к типу string</returns>
        public string DoAggregateFunction(string sqlExpression)
        {
            string aggregateFunctionResult = "";

            SqlCommand command = new SqlCommand(sqlExpression, _Connection);
            aggregateFunctionResult = command.ExecuteScalar().ToString();

            return aggregateFunctionResult;
        }
    }
}
