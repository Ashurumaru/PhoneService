using PhoneService.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PhoneService.Views.Crud
{
    public partial class OrderCreateEditPage : Page
    {
        private PhoneServiceEntities _context;
        private Orders _order;

        public OrderCreateEditPage(int orderId = 0)
        {
            InitializeComponent();
            _context = new PhoneServiceEntities();

            if (orderId == 0)
            {
                _order = new Orders { OrderDate = DateTime.Now, Payments = new List<Payments>(), OrderDetails = new List<OrderDetails>() };
                _context.Orders.Add(_order);
            }
            else
            {
                _order = _context.Orders.Include("Payments").Include("OrderDetails").FirstOrDefault(o => o.OrderId == orderId);
                if (_order == null)
                {
                    MessageBox.Show("Заказ не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    NavigationService.GoBack();
                    return;
                }
            }

            LoadData();
            LoadClients();
            LoadEmployees();
            LoadOrderStatuses();
            LoadParts();
            LoadPaymentMethods();
        }

        private void LoadData()
        {
            if (_order.ClientId != 0)
            {
                ClientComboBox.SelectedValue = _order.ClientId;
                var client = _context.Clients.FirstOrDefault(c => c.ClientId == _order.ClientId);
                if (client != null)
                {
                    ClientPhoneTextBox.Text = client.PhoneNumber;
                }
            }

            if (_order.EmployeeId != 0)
            {
                EmployeeComboBox.SelectedValue = _order.EmployeeId;
            }

            if (_order.OrderStatusId != 0)
            {
                OrderStatusComboBox.SelectedValue = _order.OrderStatusId;
            }

            OrderDatePicker.SelectedDate = _order.OrderDate;
            TotalPriceTextBox.Text = _order.OrderDetails.Sum(d => d.Price).ToString();

            if (_order.OrderDetails != null)
            {
                OrderDetailsDataGrid.ItemsSource = _order.OrderDetails.Select(d => new
                {
                    d.OrderDetailId,
                    d.Parts.PartName,
                    d.Price
                }).ToList();
            }

            if (_order.Payments != null)
            {
                PaymentsDataGrid.ItemsSource = _order.Payments.Select(p => new
                {
                    p.PaymentId,
                    p.PaymentDate,
                    p.Amount,
                    MethodName = p.PaymentMethods.MethodName
                }).ToList();

                TotalPaymentTextBlock.Text = _order.Payments.Sum(p => p.Amount).ToString() + " ₽";
                RemainingPaymentTextBlock.Text = (_order.OrderDetails.Sum(d => d.Price) - _order.Payments.Sum(p => p.Amount)).ToString() + " ₽";
            }
        }

        private void LoadClients()
        {
            var clients = _context.Clients.ToList().Select(c => new
            {
                c.ClientId,
                c.PhoneNumber,
                FullName = $"{c.FirstName} {c.LastName}"
            }).ToList();

            ClientComboBox.ItemsSource = clients;
        }

        private void LoadEmployees()
        {
            var employees = _context.Employees.ToList().Select(e => new
            {
                e.EmployeeId,
                FullName = $"{e.FirstName} {e.LastName}"
            }).ToList();

            EmployeeComboBox.ItemsSource = employees;
        }

        private void LoadOrderStatuses()
        {
            OrderStatusComboBox.ItemsSource = _context.OrderStatuses.ToList();
        }

        private void LoadParts()
        {
            PartComboBox.ItemsSource = _context.Parts.ToList();
        }

        private void LoadPaymentMethods()
        {
            PaymentMethodComboBox.ItemsSource = _context.PaymentMethods.ToList();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _order.ClientId = (int)ClientComboBox.SelectedValue;
                _order.EmployeeId = (int)EmployeeComboBox.SelectedValue;
                _order.OrderStatusId = (int)OrderStatusComboBox.SelectedValue;
                _order.OrderDate = OrderDatePicker.SelectedDate ?? DateTime.Now;

                _context.SaveChanges();
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении заказа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void AddPartButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedPart = (Parts)PartComboBox.SelectedItem;
            if (selectedPart != null)
            {
                var orderDetail = new OrderDetails
                {
                    PartId = selectedPart.PartId,
                    Price = selectedPart.Price
                };
                _order.OrderDetails.Add(orderDetail);
                _context.SaveChanges();
                LoadData();
            }
        }

        private void DeleteOrderDetailButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.CommandParameter != null)
            {
                var orderDetailId = (int)button.CommandParameter;
                var orderDetail = _order.OrderDetails.FirstOrDefault(d => d.OrderDetailId == orderDetailId);
                if (orderDetail != null)
                {
                    _order.OrderDetails.Remove(orderDetail);
                    _context.OrderDetails.Remove(orderDetail);
                    _context.SaveChanges();
                    LoadData();
                }
            }
        }

        private void AddPaymentButton_Click(object sender, RoutedEventArgs e)
        {
            if (_order.Payments == null)
            {
                _order.Payments = new List<Payments>();
            }

            var amount = decimal.Parse(PaymentAmountTextBox.Text);
            var paymentMethod = (PaymentMethods)PaymentMethodComboBox.SelectedItem;
            var paymentDate = PaymentDatePicker.SelectedDate ?? DateTime.Now;

            var payment = new Payments
            {
                Amount = amount,
                PaymentMethodId = paymentMethod.PaymentMethodId,
                PaymentDate = paymentDate
            };
            _order.Payments.Add(payment);
            _context.Payments.Add(payment);
            _context.SaveChanges();
            LoadData();
        }

        private void DeletePaymentButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.CommandParameter != null)
            {
                var paymentId = (int)button.CommandParameter;
                var payment = _order.Payments.FirstOrDefault(p => p.PaymentId == paymentId);
                if (payment != null)
                {
                    var result = MessageBox.Show("Вы уверены, что хотите удалить эту оплату?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        _order.Payments.Remove(payment);
                        _context.Payments.Remove(payment);
                        _context.SaveChanges();
                        LoadData();
                    }
                }
            }
        }

        private void ClientComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedClient = ClientComboBox.SelectedItem;
            if (selectedClient != null)
            {
                dynamic client = selectedClient;
                ClientPhoneTextBox.Text = client.PhoneNumber;
            }
        }
    }
}
