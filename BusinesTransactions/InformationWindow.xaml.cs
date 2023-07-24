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
    /// Логика взаимодействия для InformationWindow.xaml
    /// </summary>
    public partial class InformationWindow : Window
    {
        /// <summary>
        /// Входной параметр typeWindow определяет тип информационного окна:
        /// 0 -- Информационное окно с кнопкой ЗАКРЫТЬ;
        /// 1 -- Окно с выбором действия кнопками ПОДТВЕРДИТЬ/ЗАКРЫТЬ;
        /// 2 -- Окно без кнопок.
        /// </summary>
        /// <param name="typeWindow"></param>
        public InformationWindow(string informationMessage = "", int typeWindow = 0)
        {
            InitializeComponent();

            switch (typeWindow)
            {
                case 0:
                    SaveButton.Visibility = Visibility.Collapsed;
                    break;

                case 1:
                    break;

                case 2:
                    InformationToolbar.Visibility = Visibility.Collapsed;
                    break;

                default:
                    break;
            }

            InformationTextBlock.Text = informationMessage;
        }

        public void CloseWindow()
        {
            this.Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            CloseWindow();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow();
        }
    }
}
