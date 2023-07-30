using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinesTransactions
{
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
            SEPARATOR = "|";
        }

        public string ID { get; set; }

        public string DTTM { get; set; }

        public string SUMM { get; set; }

        public string COMMENT { get; set; }

        public string WRITE_OFF_ID { get; set; }

        public string WRITE_OFF_NAME { get; set; }

        public string WRITE_OFF_TYPE { get; set; }

        public string WRITE_OFF_BALANCE { get; set; }

        public string RECEIPT_ID { get; set; }

        public string RECEIPT_NAME { get; set; }

        public string RECEIPT_TYPE { get; set; }

        public string RECEIPT_BALANCE { get; set; }

        public string SEPARATOR { get; set; }
    }
}
