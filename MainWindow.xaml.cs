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

namespace WpfApp1
{
    
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnStudent(object sender, RoutedEventArgs e)
        {
            students students = new students();
            students.Show();
            this.Hide();
        }

        private void btnTeacher(object sender, RoutedEventArgs e)
        {
            login login = new login();
            login.Show();
            this.Hide();
        }
    }
}
