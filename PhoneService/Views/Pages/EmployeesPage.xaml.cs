using PhoneService.Data;
using PhoneService.Views.Crud;
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
    /// Логика взаимодействия для EmployeesPage.xaml
    /// </summary>
    public partial class EmployeesPage : Page
    {
        private PhoneServiceEntities _context;

        public EmployeesPage()
        {
            InitializeComponent();
            _context = new PhoneServiceEntities();
            LoadEmployees();
            this.Loaded += EmployeesPage_Loaded;
        }

        private void EmployeesPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService != null)
            {
                this.NavigationService.Navigated += OnNavigated;
            }
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            LoadEmployees();
        }

        private void LoadEmployees()
        {
            var employees = _context.Employees
                .Select(e => new
                {
                    e.EmployeeId,
                    e.FirstName,
                    e.LastName,
                    e.MiddleName,
                    e.PhoneNumber,
                    e.Email,
                    RoleName = e.Roles.RoleName
                }).ToList();

            EmployeesItemsControl.ItemsSource = employees;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void AddEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            var employeeCreateEditPage = new EmployeeCreateEditPage();
            NavigationService.Navigate(employeeCreateEditPage);
        }

        private void EditEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var employeeId = (int)button.CommandParameter;
            var employeeCreateEditPage = new EmployeeCreateEditPage(employeeId);
            NavigationService.Navigate(employeeCreateEditPage);
        }

        private void DeleteEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var employeeId = (int)button.CommandParameter;
            var employee = _context.Employees.FirstOrDefault(u => u.EmployeeId == employeeId);

            if (employee != null)
            {
                var result = MessageBox.Show("Вы уверены, что хотите удалить этого сотрудника?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    _context.Employees.Remove(employee);
                    _context.SaveChanges();
                    LoadEmployees();
                }
            }
        }
    }
}