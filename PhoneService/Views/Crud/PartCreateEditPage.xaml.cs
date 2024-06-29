using PhoneService.Data;
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

namespace PhoneService.Views.Crud
{
    /// <summary>
    /// Логика взаимодействия для PartCreateEditPage.xaml
    /// </summary>
    public partial class PartCreateEditPage : Page
    {
        private PhoneServiceEntities _context;
        private Parts _part;

        public PartCreateEditPage(int partId = 0)
        {
            InitializeComponent();
            _context = new PhoneServiceEntities();

            if (partId == 0)
            {
                _part = new Parts();
                _context.Parts.Add(_part);
            }
            else
            {
                _part = _context.Parts.FirstOrDefault(p => p.PartId == partId);
                if (_part == null)
                {
                    MessageBox.Show("Деталь не найдена!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    NavigationService.GoBack();
                    return;
                }

                PartNameTextBox.Text = _part.PartName;
                StockQuantityTextBox.Text = _part.StockQuantity.ToString();
                PriceTextBox.Text = _part.Price.ToString();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _part.PartName = PartNameTextBox.Text;
                _part.StockQuantity = int.Parse(StockQuantityTextBox.Text);
                _part.Price = decimal.Parse(PriceTextBox.Text);

                _context.SaveChanges();
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении детали: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}