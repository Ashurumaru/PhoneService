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
    /// Логика взаимодействия для ClientsPage.xaml
    /// </summary>
    public partial class ClientsPage : Page
    {
        private PhoneServiceEntities _context;

        public ClientsPage()
        {
            InitializeComponent();
            _context = new PhoneServiceEntities();
            LoadClients();
            this.Loaded += ClientsPage_Loaded;
        }

        private void ClientsPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService != null)
            {
                this.NavigationService.Navigated += OnNavigated;
            }
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            LoadClients();
        }

        private void LoadClients()
        {
            var clients = _context.Clients
                .Select(c => new
                {
                    c.ClientId,
                    c.FirstName,
                    c.LastName,
                    c.MiddleName,
                    c.PhoneNumber,
                    c.Email
                }).ToList();

            ClientsItemsControl.ItemsSource = clients;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void AddClientButton_Click(object sender, RoutedEventArgs e)
        {
            var clientCreateEditPage = new ClientCreateEditPage();
            NavigationService.Navigate(clientCreateEditPage);
        }

        private void EditClientButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var clientId = (int)button.CommandParameter;
            var clientCreateEditPage = new ClientCreateEditPage(clientId);
            NavigationService.Navigate(clientCreateEditPage);
        }

        private void DeleteClientButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var clientId = (int)button.CommandParameter;
            var client = _context.Clients.FirstOrDefault(c => c.ClientId == clientId);

            if (client != null)
            {
                var result = MessageBox.Show("Вы уверены, что хотите удалить этого клиента?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    _context.Clients.Remove(client);
                    _context.SaveChanges();
                    LoadClients();
                }
            }
        }
    }
}