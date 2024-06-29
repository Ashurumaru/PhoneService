using PhoneService.Data;
using PhoneService.Views.Crud;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace PhoneService.Views.Pages
{
    public partial class OrdersPage : Page
    {
        private PhoneServiceEntities _context;

        public OrdersPage()
        {
            InitializeComponent();
            _context = new PhoneServiceEntities();
            LoadCustomerFilter();
            LoadRepairOrders();
            LoadStatusFilter();
            this.Loaded += ManagementPage_Loaded;
        }

        private void ManagementPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCustomerFilter();
            if (this.NavigationService != null)
            {
                this.NavigationService.Navigated += OnNavigated;
            }
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            LoadRepairOrders(); 
        }
        private void LoadStatusFilter()
        {
            StatusFilterComboBox.Items.Add("Все");
            var statuses = _context.OrderStatuses
                .Select(s => s.StatusName)
                .Distinct()
                .ToList();

            foreach (var status in statuses)
            {
                StatusFilterComboBox.Items.Add(status);
            }
            StatusFilterComboBox.SelectedIndex = 0;
        }

        private void LoadCustomerFilter()
        {
            CustomerFilterComboBox.Items.Add("Все");
            var customers = _context.Orders
                .Select(o => o.Clients.FirstName + " " + o.Clients.LastName)
                .Distinct()
                .ToList();

            foreach (var customer in customers)
            {
                CustomerFilterComboBox.Items.Add(customer);
            }
            CustomerFilterComboBox.SelectedIndex = 0;
        }

        private void LoadRepairOrders()
        {
            var repairOrders = _context.Orders
                .Select(o => new
                {
                    o.OrderId,
                    CustomerName = o.Clients.FirstName + " " + o.Clients.LastName,
                    CustomerPhone = o.Clients.PhoneNumber,
                    o.OrderDate,
                    TotalPrice = o.OrderDetails.Sum(d => (decimal?)d.Price) ?? 0,
                    OrderStatus = o.OrderStatuses.StatusName,
                    EmployeeName = o.Employees.FirstName + " " + o.Employees.LastName
                }).ToList()
                .Select(o => new
                {
                    o.OrderId,
                    o.CustomerName,
                    o.CustomerPhone,
                    o.OrderDate,
                    TotalPrice = $"{o.TotalPrice}",
                    o.OrderStatus,
                    o.EmployeeName
                }).ToList();

            RepairOrdersItemsControl.ItemsSource = repairOrders;
        }

        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            string customerFilter = CustomerFilterComboBox.SelectedItem as string;
            string statusFilter = StatusFilterComboBox.SelectedItem as string;

            var repairOrdersQuery = _context.Orders.AsQueryable();

            if (customerFilter != "Все")
            {
                repairOrdersQuery = repairOrdersQuery.Where(o => (o.Clients.FirstName + " " + o.Clients.LastName) == customerFilter);
            }

            if (statusFilter != "Все")
            {
                repairOrdersQuery = repairOrdersQuery.Where(o => o.OrderStatuses.StatusName == statusFilter);
            }

            var repairOrders = repairOrdersQuery
                .Select(o => new
                {
                    o.OrderId,
                    CustomerName = o.Clients.FirstName + " " + o.Clients.LastName,
                    CustomerPhone = o.Clients.PhoneNumber,
                    o.OrderDate,
                    TotalPrice = o.OrderDetails.Sum(d => (decimal?)d.Price) ?? 0,
                    OrderStatus = o.OrderStatuses.StatusName,
                    EmployeeName = o.Employees.FirstName + " " + o.Employees.LastName
                }).ToList()
                .Select(o => new
                {
                    o.OrderId,
                    o.CustomerName,
                    o.CustomerPhone,
                    o.OrderDate,
                    TotalPrice = $"{o.TotalPrice}",
                    o.OrderStatus,
                    o.EmployeeName
                }).ToList();

            RepairOrdersItemsControl.ItemsSource = repairOrders;
        }


        private void AddRepairOrderButton_Click(object sender, RoutedEventArgs e)
        {
            var repairOrderCreateEditPage = new OrderCreateEditPage();
            NavigationService.Navigate(repairOrderCreateEditPage);
        }

        private void EditRepairOrderButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var orderId = (int)button.CommandParameter;
            NavigationService.Navigate(new OrderCreateEditPage(orderId));

        }

        private void DeleteRepairOrderButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var orderId = (int)button.CommandParameter;
            var selectedRepairOrder = _context.Orders.FirstOrDefault(o => o.OrderId == orderId);

            if (selectedRepairOrder != null)
            {
                var result = MessageBox.Show("Вы уверены, что хотите удалить этот заказ и все связанные с ним записи?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    var details = selectedRepairOrder.OrderDetails.ToList();
                    foreach (var detail in details)
                    {
                        _context.OrderDetails.Remove(detail);
                    }

                    var payments = selectedRepairOrder.Payments.ToList();
                    foreach (var payment in payments)
                    {
                        _context.Payments.Remove(payment);
                    }

                    _context.Orders.Remove(selectedRepairOrder);
                    _context.SaveChanges();

                    LoadRepairOrders();
                }
            }
        }

        private void RepairOrdersItemsControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void ManagementRepairOrderButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void ManagementMobilPhonesButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MobilePhonesPage());
        }
    }
}
