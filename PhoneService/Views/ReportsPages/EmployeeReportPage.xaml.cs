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
    /// Логика взаимодействия для EmployeeReportPage.xaml
    /// </summary>
    public partial class EmployeeReportPage : Page
    {
        private PhoneServiceEntities _context;

        public EmployeeReportPage()
        {
            InitializeComponent();
            _context = new PhoneServiceEntities();
            LoadEmployeeData();
        }

        private void LoadEmployeeData()
        {
            var employees = _context.Employees
                .Join(_context.Roles, e => e.RoleId, r => r.RoleId, (e, r) => new EmployeeReportModel
                {
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    MiddleName = e.MiddleName,
                    PhoneNumber = e.PhoneNumber,
                    Email = e.Email,
                    RoleName = r.RoleName
                }).ToList();

            EmployeeDataGrid.ItemsSource = employees;
            CalculateTotalEmployees(employees);
        }

        private void CalculateTotalEmployees(List<EmployeeReportModel> employeeList)
        {
            var totalEmployees = employeeList.Count;
            TotalEmployeesTextBox.Text = $"{totalEmployees}";
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Word Document (*.docx)|*.docx",
                Title = "Сохранить отчет о сотрудниках",
                FileName = $"EmployeeReport_{DateTime.Now:yyyy-MM-dd}.docx"
            };

            if (saveFileDialog.ShowDialog() != true) return;

            var filePath = saveFileDialog.FileName;
            using (var doc = DocX.Create(filePath))
            {
                var title = doc.InsertParagraph("Отчет о сотрудниках")
                               .FontSize(16)
                               .Bold()
                               .Alignment = Alignment.center;
                doc.InsertParagraph("\n");

                var employees = _context.Employees
                    .Join(_context.Roles, u => u.RoleId, r => r.RoleId, (u, r) => new EmployeeReportModel
                    {
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        MiddleName = u.MiddleName,
                        PhoneNumber = u.PhoneNumber,
                        Email = u.Email,
                        RoleName = r.RoleName
                    }).ToList();

                if (employees.Any())
                {
                    var table = doc.AddTable(employees.Count + 1, 6);
                    table.Rows[0].Cells[0].Paragraphs.First().Append("Имя").Bold();
                    table.Rows[0].Cells[1].Paragraphs.First().Append("Фамилия").Bold();
                    table.Rows[0].Cells[2].Paragraphs.First().Append("Отчество").Bold();
                    table.Rows[0].Cells[3].Paragraphs.First().Append("Телефон").Bold();
                    table.Rows[0].Cells[4].Paragraphs.First().Append("Email").Bold();
                    table.Rows[0].Cells[5].Paragraphs.First().Append("Роль").Bold();

                    for (int i = 0; i < employees.Count; i++)
                    {
                        var employee = employees[i];
                        table.Rows[i + 1].Cells[0].Paragraphs.First().Append(employee.FirstName);
                        table.Rows[i + 1].Cells[1].Paragraphs.First().Append(employee.LastName);
                        table.Rows[i + 1].Cells[2].Paragraphs.First().Append(employee.MiddleName);
                        table.Rows[i + 1].Cells[3].Paragraphs.First().Append(employee.PhoneNumber);
                        table.Rows[i + 1].Cells[4].Paragraphs.First().Append(employee.Email);
                        table.Rows[i + 1].Cells[5].Paragraphs.First().Append(employee.RoleName);
                    }

                    doc.InsertTable(table);
                    table.Design = TableDesign.TableGrid;
                    table.Alignment = Alignment.center;
                    table.AutoFit = AutoFit.Contents;

                    doc.InsertParagraph($"\nОбщее количество сотрудников: {employees.Count}")
                        .FontSize(12)
                        .SpacingAfter(10)
                        .Bold();
                }
                else
                {
                    doc.InsertParagraph("Нет сотрудников для отображения.")
                        .FontSize(12);
                }

                doc.Save();
            }
            Process.Start("explorer.exe", filePath);
            MessageBox.Show("Отчет успешно сохранен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
