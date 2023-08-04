using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinesTransactions
{
    /// <summary>
    /// Главный логер.
    /// 
    /// Будет отвечать за:
    /// Запись логов;
    /// Запись ошибок.
    /// </summary>
    public class Logger
    {
        /// <summary>
        /// Конструктор главного логера
        /// </summary>
        public Logger()
        {

        }

        public void WriteLog(string log)
        {
            Console.WriteLine(log);
        }
    }
}
