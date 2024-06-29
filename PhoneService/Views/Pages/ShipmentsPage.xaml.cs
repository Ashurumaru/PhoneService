using PhoneService.Data;
using PhoneService.Views.Crud;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace PhoneService.Views.Pages
{
    public partial class ShipmentsPage : Page
    {
        private PhoneServiceEntities _context;

        public ShipmentsPage()
        {
            InitializeComponent();
            _context = new PhoneServiceEntities();
            LoadShipments();
            this.Loaded += ShipmentsPage_Loaded;
        }

        private void ShipmentsPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService != null)
            {
                this.NavigationService.Navigated += OnNavigated;
            }
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            LoadShipments(); 
        }

        private void LoadShipments()
        {
            var shipments = _context.Shipments
                .Select(s => new
                {
                    s.ShipmentId,
                    SupplierName = s.Suppliers.SupplierName,
                    s.ShipmentDate,
                    ShipmentDetails = s.ShipmentDetails.Select(d => new
                    {
                        d.PartId,
                        PartName = d.Parts.PartName,
                        d.Quantity,
                        d.Price
                    }).ToList()
                }).ToList();

            ShipmentsItemsControl.ItemsSource = shipments;
        }

        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            DateTime? dateFilter = ShipmentDateFilterPicker.SelectedDate;

            var shipmentsQuery = _context.Shipments.AsQueryable();

            if (dateFilter != null)
            {
                shipmentsQuery = shipmentsQuery.Where(s => s.ShipmentDate == dateFilter);
            }

            var shipments = shipmentsQuery
                .Select(s => new
                {
                    s.ShipmentId,
                    SupplierName = s.Suppliers.SupplierName,
                    s.ShipmentDate,
                    ShipmentDetails = s.ShipmentDetails.Select(d => new
                    {
                        d.PartId,
                        PartName = d.Parts.PartName,
                        d.Quantity,
                        d.Price
                    }).ToList()
                }).ToList();

            ShipmentsItemsControl.ItemsSource = shipments;
        }

        private void AddShipmentButton_Click(object sender, RoutedEventArgs e)
        {
            var shipmentCreateEditPage = new ShipmentCreateEditPage();
            NavigationService.Navigate(shipmentCreateEditPage);
        }

        private void EditShipmentButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var shipmentId = (int)button.CommandParameter;
            NavigationService.Navigate(new ShipmentCreateEditPage(shipmentId));
        }

        private void DeleteShipmentButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var shipmentId = (int)button.CommandParameter;
            var selectedShipment = _context.Shipments.FirstOrDefault(s => s.ShipmentId == shipmentId);

            if (selectedShipment != null)
            {
                var result = MessageBox.Show("Вы уверены, что хотите удалить эту поставку?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    var details = selectedShipment.ShipmentDetails.ToList();
                    foreach (var detail in details)
                    {
                        _context.ShipmentDetails.Remove(detail);
                    }

                    _context.Shipments.Remove(selectedShipment);
                    _context.SaveChanges();
                    LoadShipments();
                }
            }
        }
    }
}
