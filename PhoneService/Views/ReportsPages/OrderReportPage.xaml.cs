using Microsoft.Win32;
using PhoneService.Data;
using PhoneService.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace PhoneService.Views.ReportsPages
{
    /// <summary>
    /// Логика взаимодействия для OrderReportPage.xaml
    /// </summary>
    public partial class OrderReportPage : Page
    {
        private PhoneServiceEntities _context;

        public OrderReportPage()
        {
            InitializeComponent();
            _context = new PhoneServiceEntities();
            LoadOrderData();
        }

        private void LoadOrderData()
        {
            DateTime? startDate = StartDatePicker.SelectedDate;
            DateTime? endDate = EndDatePicker.SelectedDate;

            var orders = _context.Orders
                .Select(o => new OrderReportModel
                {
                    OrderDate = o.OrderDate,
                    CustomerName = o.Clients.FirstName + " " + o.Clients.LastName,
                    TotalPrice = o.OrderDetails.Sum(od => (decimal?)od.Price) ?? 0,
                    Payments = o.Payments.Sum(p => (decimal?)p.Amount) ?? 0
                });

            if (startDate.HasValue)
            {
                orders = orders.Where(o => o.OrderDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                orders = orders.Where(o => o.OrderDate <= endDate.Value);
            }

            var orderList = orders.OrderBy(o => o.OrderDate).ToList();
            OrderDataGrid.ItemsSource = orderList;
            CalculateTotalOrderPrice(orderList);
        }

        private void CalculateTotalOrderPrice(List<OrderReportModel> orderList)
        {
            var totalOrderPrice = orderList.Sum(o => o.TotalPrice);
            TotalOrderPriceTextBox.Text = $"{totalOrderPrice:C}";
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadOrderData();
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Word Document (*.docx)|*.docx",
                Title = "Сохранить отчет по заказам",
                FileName = $"OrderReport_{DateTime.Now:yyyy-MM-dd}.docx"
            };

            if (saveFileDialog.ShowDialog() != true) return;

            var filePath = saveFileDialog.FileName;
            using (var doc = DocX.Create(filePath))
            {
                var title = doc.InsertParagraph("Отчет по заказам")
                               .FontSize(16)
                               .Bold()
                               .Alignment = Alignment.center;
                doc.InsertParagraph("\n");

                DateTime? startDate = StartDatePicker.SelectedDate;
                DateTime? endDate = EndDatePicker.SelectedDate;

                var periodInfo = doc.InsertParagraph($"Отчетный период: с {startDate:dd.MM.yyyy} по {endDate:dd.MM.yyyy}")
                                   .FontSize(12)
                                   .Alignment = Alignment.both;
                doc.InsertParagraph("\n");

                var orders = _context.Orders
                    .Select(o => new OrderReportModel
                    {
                        OrderDate = o.OrderDate,
                        CustomerName = o.Clients.FirstName + " " + o.Clients.LastName,
                        TotalPrice = o.OrderDetails.Sum(od => (decimal?)od.Price) ?? 0,
                        Payments = o.Payments.Sum(p => (decimal?)p.Amount) ?? 0
                    });

                if (startDate.HasValue)
                {
                    orders = orders.Where(o => o.OrderDate >= startDate.Value);
                }

                if (endDate.HasValue)
                {
                    orders = orders.Where(o => o.OrderDate <= endDate.Value);
                }

                var orderList = orders.OrderBy(o => o.OrderDate).ToList();

                if (orderList.Any())
                {
                    var table = doc.AddTable(orderList.Count + 1, 4);
                    table.Rows[0].Cells[0].Paragraphs.First().Append("Дата").Bold();
                    table.Rows[0].Cells[1].Paragraphs.First().Append("Имя клиента").Bold();
                    table.Rows[0].Cells[2].Paragraphs.First().Append("Общая стоимость").Bold();
                    table.Rows[0].Cells[3].Paragraphs.First().Append("Оплаты").Bold();

                    for (int i = 0; i < orderList.Count; i++)
                    {
                        var order = orderList[i];
                        table.Rows[i + 1].Cells[0].Paragraphs.First().Append(order.OrderDate.ToString());
                        table.Rows[i + 1].Cells[1].Paragraphs.First().Append(order.CustomerName);
                        table.Rows[i + 1].Cells[2].Paragraphs.First().Append($"{order.TotalPrice:F2} руб.");
                        table.Rows[i + 1].Cells[3].Paragraphs.First().Append($"{order.Payments:F2} руб.");
                    }

                    doc.InsertTable(table);
                    table.Design = TableDesign.TableGrid;
                    table.Alignment = Alignment.center;
                    table.AutoFit = AutoFit.Contents;

                    var totalOrderPrice = orderList.Sum(o => o.TotalPrice);
                    doc.InsertParagraph($"\nОбщая стоимость заказов за указанный период: {totalOrderPrice:F2} руб.")
                        .FontSize(12)
                        .SpacingAfter(10)
                        .Bold();
                }
                else
                {
                    doc.InsertParagraph("За выбранный период заказы отсутствуют.")
                        .FontSize(12);
                }

                doc.Save();
            }
            Process.Start("explorer.exe", filePath);
            MessageBox.Show("Отчет успешно сохранен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
