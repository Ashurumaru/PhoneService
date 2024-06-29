using PhoneService.Data;
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

namespace PhoneService.Views.Crud
{
    /// <summary>
    /// Логика взаимодействия для EmployeeCreateEditPage.xaml
    /// </summary>
    public partial class EmployeeCreateEditPage : Page
    {
        private PhoneServiceEntities _context;
        private Employees _employee;

        public EmployeeCreateEditPage(int employeeId = 0)
        {
            InitializeComponent();
            _context = new PhoneServiceEntities();
            LoadRoles();

            if (employeeId == 0)
            {
                _employee = new Employees();
            }
            else
            {
                _employee = _context.Employees.FirstOrDefault(e => e.EmployeeId == employeeId);
                if (_employee != null)
                {
                    FirstNameTextBox.Text = _employee.FirstName;
                    LastNameTextBox.Text = _employee.LastName;
                    MiddleNameTextBox.Text = _employee.MiddleName;
                    PhoneNumberTextBox.Text = _employee.PhoneNumber;
                    EmailTextBox.Text = _employee.Email;
                    UsernameTextBox.Text = _employee.Username;
                    RoleComboBox.SelectedValue = _employee.RoleId;
                }
            }
        }

        private void LoadRoles()
        {
            RoleComboBox.ItemsSource = _context.Roles.ToList();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _employee.FirstName = FirstNameTextBox.Text;
                _employee.LastName = LastNameTextBox.Text;
                _employee.MiddleName = MiddleNameTextBox.Text;
                _employee.PhoneNumber = PhoneNumberTextBox.Text;
                _employee.Email = EmailTextBox.Text;
                _employee.Username = UsernameTextBox.Text;
                _employee.Password = PasswordTextBox.Password;
                _employee.RoleId = (int)RoleComboBox.SelectedValue;

                if (_employee.EmployeeId == 0)
                {
                    _context.Employees.Add(_employee);
                }

                _context.SaveChanges();
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении сотрудника: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}