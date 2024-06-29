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
    /// Логика взаимодействия для RevenueReportPage.xaml
    /// </summary>
    public partial class RevenueReportPage : Page
    {
        private PhoneServiceEntities _context;

        public RevenueReportPage()
        {
            InitializeComponent();
            _context = new PhoneServiceEntities();
            LoadRevenueData();
        }

        private void LoadRevenueData()
        {
            DateTime? startDate = StartDatePicker.SelectedDate;
            DateTime? endDate = EndDatePicker.SelectedDate;

            var revenues = _context.Payments
                .Join(_context.PaymentMethods, p => p.PaymentMethodId, pm => pm.PaymentMethodId, (p, pm) => new { p, pm })
                .Join(_context.Orders, p_pm => p_pm.p.OrderId, o => o.OrderId, (p_pm, o) => new { p_pm, o })
                .Join(_context.Clients, p_pm_o => p_pm_o.o.ClientId, c => c.ClientId, (p_pm_o, c) => new RevenueReportModel
                {
                    Date = p_pm_o.p_pm.p.PaymentDate,
                    CustomerName = c.FirstName + " " + c.LastName,
                    Description = "Доход от заказа",
                    Amount = (decimal)p_pm_o.p_pm.p.Amount,
                    PaymentMethod = p_pm_o.p_pm.pm.MethodName
                });

            if (startDate.HasValue)
            {
                revenues = revenues.Where(r => r.Date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                revenues = revenues.Where(r => r.Date <= endDate.Value);
            }

            var revenueList = revenues.OrderBy(r => r.Date).ToList();
            RevenueDataGrid.ItemsSource = revenueList;
            CalculateTotalRevenue(revenueList);
        }

        private void CalculateTotalRevenue(List<RevenueReportModel> revenueList)
        {
            var totalRevenue = revenueList.Sum(r => r.Amount);
            TotalRevenueTextBox.Text = $"{totalRevenue:C}";
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadRevenueData();
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Word Document (*.docx)|*.docx",
                Title = "Сохранить отчет о доходах",
                FileName = $"RevenueReport_{DateTime.Now:yyyy-MM-dd}.docx"
            };

            if (saveFileDialog.ShowDialog() != true) return;

            var filePath = saveFileDialog.FileName;
            using (var doc = DocX.Create(filePath))
            {
                var title = doc.InsertParagraph("Отчет о доходах")
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

                var revenues = _context.Payments
                    .Join(_context.PaymentMethods, p => p.PaymentMethodId, pm => pm.PaymentMethodId, (p, pm) => new { p, pm })
                    .Join(_context.Orders, p_pm => p_pm.p.OrderId, o => o.OrderId, (p_pm, o) => new { p_pm, o })
                    .Join(_context.Clients, p_pm_o => p_pm_o.o.ClientId, c => c.ClientId, (p_pm_o, c) => new RevenueReportModel
                    {
                        Date = p_pm_o.p_pm.p.PaymentDate,
                        CustomerName = c.FirstName + " " + c.LastName,
                        Description = "Доход от заказа",
                        Amount = (decimal)p_pm_o.p_pm.p.Amount,
                        PaymentMethod = p_pm_o.p_pm.pm.MethodName
                    });

                if (startDate.HasValue)
                {
                    revenues = revenues.Where(r => r.Date >= startDate.Value);
                }

                if (endDate.HasValue)
                {
                    revenues = revenues.Where(r => r.Date <= endDate.Value);
                }

                var revenueList = revenues.OrderBy(r => r.Date).ToList();

                if (revenueList.Any())
                {
                    var table = doc.AddTable(revenueList.Count + 1, 5);
                    table.Rows[0].Cells[0].Paragraphs.First().Append("Дата").Bold();
                    table.Rows[0].Cells[1].Paragraphs.First().Append("Имя клиента").Bold();
                    table.Rows[0].Cells[2].Paragraphs.First().Append("Описание").Bold();
                    table.Rows[0].Cells[3].Paragraphs.First().Append("Сумма").Bold();
                    table.Rows[0].Cells[4].Paragraphs.First().Append("Метод оплаты").Bold();

                    for (int i = 0; i < revenueList.Count; i++)
                    {
                        var revenue = revenueList[i];
                        table.Rows[i + 1].Cells[0].Paragraphs.First().Append(revenue.Date.ToString());
                        table.Rows[i + 1].Cells[1].Paragraphs.First().Append(revenue.CustomerName);
                        table.Rows[i + 1].Cells[2].Paragraphs.First().Append(revenue.Description);
                        table.Rows[i + 1].Cells[3].Paragraphs.First().Append($"{revenue.Amount:F2} руб.");
                        table.Rows[i + 1].Cells[4].Paragraphs.First().Append(revenue.PaymentMethod);
                    }

                    doc.InsertTable(table);
                    table.Design = TableDesign.TableGrid;
                    table.Alignment = Alignment.center;
                    table.AutoFit = AutoFit.Contents;

                    var totalRevenue = revenueList.Sum(r => r.Amount);
                    doc.InsertParagraph($"\nОбщая выручка за указанный период: {totalRevenue:F2} руб.")
                        .FontSize(12)
                        .SpacingAfter(10)
                        .Bold();
                }
                else
                {
                    doc.InsertParagraph("За выбранный период доходы отсутствуют.")
                        .FontSize(12);
                }

                doc.Save();
            }
            Process.Start("explorer.exe", filePath);
            MessageBox.Show("Отчет успешно сохранен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
