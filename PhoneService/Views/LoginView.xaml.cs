using PhoneService.Data;
using PhoneService.Models;
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

namespace PhoneService.Views
{
    /// <summary>
    /// Логика взаимодействия для LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        private readonly PhoneServiceEntities _context;

        public LoginView()
        {
            InitializeComponent();
            _context = new PhoneServiceEntities();
        }

        private void LogIn_Click(object sender, RoutedEventArgs e)
        {
            var login = LoginTextBox.Text;
            var password = PasswordBox.Password;

            var user = _context.Employees.SingleOrDefault(u => u.Username == login && u.Password == password);
            if (user != null)
            {
                CurrentUser.FirstName = user.FirstName;
                CurrentUser.MiddleName = user.MiddleName;
                CurrentUser.SecondName = user.LastName;
                CurrentUser.UserId = user.EmployeeId;
                var mainWindow = new MainView();
                mainWindow.Show();
                this.Close();

            }
            else
            {
                LoginErrorMessage.Text = "Неправильный логин или пароль";
                LoginErrorMessage.Visibility = Visibility.Visible;
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            var username = RegisterUsernameTextBox.Text;
            var password = RegisterPasswordBox.Password;
            var firstName = RegisterFirstNameTextBox.Text;
            var lastName = RegisterLastNameTextBox.Text;
            var email = RegisterEmailTextBox.Text;

            var existingUser = _context.Employees.SingleOrDefault(u => u.Username == username);
            if (existingUser == null)
            {
                var newUser = new Employees
                {
                    Username = username,
                    Password = password,
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    RoleId = 2 
                };

                _context.Employees.Add(newUser);
                _context.SaveChanges();

                RegisterErrorMessage.Text = "Пользователь успешно зарегистрирован";
                RegisterErrorMessage.Foreground = new SolidColorBrush(Colors.Green);
                RegisterErrorMessage.Visibility = Visibility.Visible;
            }
            else
            {
                RegisterErrorMessage.Text = "Пользователь с таким логином уже существует";
                RegisterErrorMessage.Visibility = Visibility.Visible;
            }
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}