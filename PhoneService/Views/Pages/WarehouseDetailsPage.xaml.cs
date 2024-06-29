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
    /// Логика взаимодействия для WarehouseDetailsPage.xaml
    /// </summary>
    public partial class WarehouseDetailsPage : Page
    {
        private PhoneServiceEntities _context;

        public WarehouseDetailsPage()
        {
            InitializeComponent();
            _context = new PhoneServiceEntities();
            LoadWarehouseDetails();
            this.Loaded += WarehousePage_Loaded;
        }

        private void WarehousePage_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService != null)
            {
                this.NavigationService.Navigated += OnNavigated;
            }
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            LoadWarehouseDetails();
        }

        private void LoadWarehouseDetails()
        {
            var warehouseDetails = _context.Parts
                .Select(p => new
                {
                    p.PartId,
                    p.PartName,
                    p.StockQuantity,
                    p.Price
                }).ToList();

            WarehouseDetailsDataGrid.ItemsSource = warehouseDetails;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void AddPartButton_Click(object sender, RoutedEventArgs e)
        {
            var partCreateEditPage = new PartCreateEditPage();
            NavigationService.Navigate(partCreateEditPage);
        }

        private void EditPartButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var partId = (int)button.CommandParameter;
            var partCreateEditPage = new PartCreateEditPage(partId);
            NavigationService.Navigate(partCreateEditPage);
        }

        private void DeletePartButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var partId = (int)button.CommandParameter;
            var part = _context.Parts.FirstOrDefault(p => p.PartId == partId);

            if (part != null)
            {
                var result = MessageBox.Show("Вы уверены, что хотите удалить эту деталь?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    _context.Parts.Remove(part);
                    _context.SaveChanges();
                    LoadWarehouseDetails();
                }
            }
        }
    }
}