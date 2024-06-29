using PhoneService.Data;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace PhoneService.Views.Crud
{
    public partial class ShipmentCreateEditPage : Page
    {
        private PhoneServiceEntities _context;
        private Shipments _shipment;
        public ObservableCollection<ShipmentDetails> ShipmentDetails { get; set; }
        public ObservableCollection<Parts> Parts { get; set; }

        public ShipmentCreateEditPage(int shipmentId = 0)
        {
            InitializeComponent();
            _context = new PhoneServiceEntities();
            LoadSuppliers();
            LoadParts();

            if (shipmentId == 0)
            {
                _shipment = new Shipments { ShipmentDate = DateTime.Now };
                ShipmentDetails = new ObservableCollection<ShipmentDetails>();
            }
            else
            {
                _shipment = _context.Shipments.Include("ShipmentDetails").FirstOrDefault(s => s.ShipmentId == shipmentId);
                if (_shipment != null)
                {
                    SupplierComboBox.SelectedValue = _shipment.SupplierId;
                    ShipmentDatePicker.SelectedDate = _shipment.ShipmentDate;
                    ShipmentDetails = new ObservableCollection<ShipmentDetails>(_shipment.ShipmentDetails);
                }
            }

            ShipmentDetailsDataGrid.ItemsSource = ShipmentDetails;
            DataContext = this;
        }

        private void LoadSuppliers()
        {
            var suppliers = _context.Suppliers.ToList();
            SupplierComboBox.ItemsSource = suppliers;
        }

        private void LoadParts()
        {
            Parts = new ObservableCollection<Parts>(_context.Parts.ToList());
            NewPartComboBox.ItemsSource = Parts;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _shipment.SupplierId = (int)SupplierComboBox.SelectedValue;
                _shipment.ShipmentDate = ShipmentDatePicker.SelectedDate ?? DateTime.Now;

                if (_shipment.ShipmentId == 0)
                {
                    _context.Shipments.Add(_shipment);
                }

                _context.SaveChanges();

                foreach (var detail in ShipmentDetails)
                {
                    if (detail.ShipmentDetailId == 0)
                    {
                        detail.ShipmentId = _shipment.ShipmentId;
                        _context.ShipmentDetails.Add(detail);
                    }
                    else
                    {
                        var existingDetail = _context.ShipmentDetails.FirstOrDefault(d => d.ShipmentDetailId == detail.ShipmentDetailId);
                        if (existingDetail != null)
                        {
                            existingDetail.PartId = detail.PartId;
                            existingDetail.Quantity = detail.Quantity;
                            existingDetail.Price = detail.Price;
                        }
                    }
                }

                _context.SaveChanges();
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении поставки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void AddDetailButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedPart = NewPartComboBox.SelectedItem as Parts;
            if (selectedPart != null && int.TryParse(NewPartQuantityTextBox.Text, out int quantity) && decimal.TryParse(NewPartPriceTextBox.Text, out decimal price))
            {
                ShipmentDetails.Add(new ShipmentDetails { Parts = selectedPart, PartId = selectedPart.PartId, Quantity = quantity, Price = price });
                NewPartComboBox.SelectedIndex = -1;
                NewPartQuantityTextBox.Clear();
                NewPartPriceTextBox.Clear();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите деталь и введите корректные данные для количества и цены.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteDetailButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var detail = button.CommandParameter as ShipmentDetails;

            if (detail != null)
            {
                if (detail.ShipmentDetailId != 0)
                {
                    _context.ShipmentDetails.Remove(detail);
                }
                ShipmentDetails.Remove(detail);
            }
        }
    }
}