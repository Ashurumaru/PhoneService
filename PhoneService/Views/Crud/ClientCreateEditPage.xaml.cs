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
    /// Логика взаимодействия для ClientCreateEditPage.xaml
    /// </summary>
    public partial class ClientCreateEditPage : Page
    {
        private PhoneServiceEntities _context;
        private Clients _client;

        public ClientCreateEditPage(int clientId = 0)
        {
            InitializeComponent();
            _context = new PhoneServiceEntities();

            if (clientId == 0)
            {
                _client = new Clients();
                _context.Clients.Add(_client);
            }
            else
            {
                _client = _context.Clients.FirstOrDefault(c => c.ClientId == clientId);
                if (_client == null)
                {
                    MessageBox.Show("Клиент не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    NavigationService.GoBack();
                    return;
                }

                FirstNameTextBox.Text = _client.FirstName;
                LastNameTextBox.Text = _client.LastName;
                MiddleNameTextBox.Text = _client.MiddleName;
                PhoneNumberTextBox.Text = _client.PhoneNumber;
                EmailTextBox.Text = _client.Email;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _client.FirstName = FirstNameTextBox.Text;
                _client.LastName = LastNameTextBox.Text;
                _client.PhoneNumber = PhoneNumberTextBox.Text;
                _client.Email = EmailTextBox.Text;
                _client.MiddleName = MiddleNameTextBox.Text;

                _context.SaveChanges();
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении клиента: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}