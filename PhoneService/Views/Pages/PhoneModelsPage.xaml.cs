using PhoneService.Data;
using PhoneService.Views.Crud;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace PhoneService.Views.Pages
{
    public partial class PhoneModelsPage : Page
    {
        private PhoneServiceEntities _context;

        public PhoneModelsPage()
        {
            InitializeComponent();
            _context = new PhoneServiceEntities();
            LoadPhoneModels();
            this.Loaded += PhoneModelsPage_Loaded;
        }

        private void PhoneModelsPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService != null)
            {
                this.NavigationService.Navigated += OnNavigated;
            }
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            LoadPhoneModels();
        }

        private void LoadPhoneModels()
        {
            var phoneModels = _context.PhoneModels
                .Select(pm => new
                {
                    pm.PhoneModelId,
                    pm.Brand,
                    pm.ModelName
                }).ToList();

            PhoneModelsItemsControl.ItemsSource = phoneModels;
        }

        private void AddPhoneModelButton_Click(object sender, RoutedEventArgs e)
        {
            var phoneModelCreateEditPage = new PhoneModelCreateEditPage();
            NavigationService.Navigate(phoneModelCreateEditPage);
        }

        private void EditPhoneModelButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var phoneModelId = (int)button.CommandParameter;
            NavigationService.Navigate(new PhoneModelCreateEditPage(phoneModelId));
        }

        private void DeletePhoneModelButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var phoneModelId = (int)button.CommandParameter;
            var selectedPhoneModel = _context.PhoneModels.FirstOrDefault(pm => pm.PhoneModelId == phoneModelId);

            if (selectedPhoneModel != null)
            {
                var result = MessageBox.Show("Вы уверены, что хотите удалить эту модель телефона?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    _context.PhoneModels.Remove(selectedPhoneModel);
                    _context.SaveChanges();

                    LoadPhoneModels();
                }
            }
        }
    }
}
