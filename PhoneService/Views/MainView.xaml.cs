using PhoneService.Models;
using PhoneService.Views.Pages;
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
using System.Windows.Shapes;

namespace PhoneService.Views
{
    /// <summary>
    /// Логика взаимодействия для MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            if (CurrentUser.UserId == 1)
            {
                btnEmpoloyee.Visibility = Visibility.Visible;
            }
            txtNameEmployee.Text = CurrentUser.GetFullName();

        }
        private void NavigateToOrders(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new OrdersPage());
        }

        private void NavigateToShipments(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ShipmentsPage());
        }

        private void NavigateToParts(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new WarehouseDetailsPage());
        }

        private void NavigateToClients(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ClientsPage());
        }

        private void NavigateToEmployees(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new EmployeesPage());
        }



        private void NavigateToReports(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ReportsPage());
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var loginView = new LoginView();
            loginView.Show();
            this.Close();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowState = WindowState.Maximized;
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
