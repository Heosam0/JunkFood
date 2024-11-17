using JunkFood.AppData;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JunkFood
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AppFrame.frameMain = FrmMain;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            db Postgres = new db();
            string str = Postgres.Initialize(login.Text, pass.Password);
            if (str != "Successful")
            {

                MessageBox.Show(str, "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            else
            {
                Window a = new ManagementWindow(login.Text, pass.Password);
                a.Show();
                this.Close();
            }
            Postgres.connection.Close();
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {

        }
    }
}

