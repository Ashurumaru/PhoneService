using PhoneService.Views.ReportsPages;
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

namespace PhoneService.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для ReportsPage.xaml
    /// </summary>
    public partial class ReportsPage : Page
    {
        public ReportsPage()
        {
            InitializeComponent();
            ReportFrame.Navigate(new BasePage());
        }

        private void OrderReportButton_Click(object sender, RoutedEventArgs e)
        {
            ReportFrame.Navigate(new OrderReportPage());
        }

        private void RevenueReportButton_Click(object sender, RoutedEventArgs e)
        {
            ReportFrame.Navigate(new RevenueReportPage());
        }

        private void EmployeeReportButton_Click(object sender, RoutedEventArgs e)
        {
            ReportFrame.Navigate(new EmployeeReportPage());
        }
    
}
}
