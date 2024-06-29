using PhoneService.Data;
using PhoneService.Views.Crud;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace PhoneService.Views.Pages
{
    public partial class MobilePhonesPage : Page
    {
        private PhoneServiceEntities _context;

        public MobilePhonesPage()
        {
            InitializeComponent();
            _context = new PhoneServiceEntities();
            LoadMobilePhones();
            this.Loaded += MobilePhonesPage_Loaded;
        }

        private void MobilePhonesPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService != null)
            {
                this.NavigationService.Navigated += OnNavigated;
            }
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            LoadMobilePhones();
        }

        private void LoadMobilePhones(string searchText = "")
        {
            var mobilePhones = _context.MobilePhones
                .Where(mp => mp.Clients.FirstName.Contains(searchText) ||
                             mp.Clients.LastName.Contains(searchText) ||
                             mp.PhoneModels.Brand.Contains(searchText) ||
                             mp.PhoneModels.ModelName.Contains(searchText) ||
                             mp.IMEI.Contains(searchText) ||
                             mp.IssueDescription.Contains(searchText))
                .Select(mp => new
                {
                    mp.MobilePhoneId,
                    ClientName = mp.Clients.FirstName + " " + mp.Clients.LastName,
                    PhoneModelName = mp.PhoneModels.Brand + " " + mp.PhoneModels.ModelName,
                    mp.IMEI,
                    mp.ReceivedDate,
                    mp.IssueDescription
                }).ToList();

            MobilePhonesItemsControl.ItemsSource = mobilePhones;
        }

        private void AddMobilePhoneButton_Click(object sender, RoutedEventArgs e)
        {
            var mobilePhoneCreateEditPage = new MobilePhoneCreateEditPage();
            NavigationService.Navigate(mobilePhoneCreateEditPage);
        }

        private void EditMobilePhoneButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var mobilePhoneId = (int)button.CommandParameter;
            NavigationService.Navigate(new MobilePhoneCreateEditPage(mobilePhoneId));
        }

        private void DeleteMobilePhoneButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var mobilePhoneId = (int)button.CommandParameter;
            var selectedMobilePhone = _context.MobilePhones.FirstOrDefault(mp => mp.MobilePhoneId == mobilePhoneId);

            if (selectedMobilePhone != null)
            {
                var result = MessageBox.Show("Вы уверены, что хотите удалить этот телефон и все связанные с ним записи?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    _context.MobilePhones.Remove(selectedMobilePhone);
                    _context.SaveChanges();

                    LoadMobilePhones();
                }
            }
        }

        private void ManagePhoneModelsButton_Click(object sender, RoutedEventArgs e)
        {
            var phoneModelsPage = new PhoneModelsPage();
            NavigationService.Navigate(phoneModelsPage);
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            LoadMobilePhones(SearchTextBox.Text);
        }

        private void SearchTextBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                LoadMobilePhones(SearchTextBox.Text);
            }
        }
    }
}
