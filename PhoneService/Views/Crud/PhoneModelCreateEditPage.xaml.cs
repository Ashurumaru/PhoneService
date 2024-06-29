using PhoneService.Data;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PhoneService.Views.Crud
{
    public partial class PhoneModelCreateEditPage : Page
    {
        private PhoneServiceEntities _context;
        private PhoneModels _phoneModel;

        public PhoneModelCreateEditPage(int phoneModelId = 0)
        {
            InitializeComponent();
            _context = new PhoneServiceEntities();

            if (phoneModelId == 0)
            {
                _phoneModel = new PhoneModels();
                _context.PhoneModels.Add(_phoneModel);
            }
            else
            {
                _phoneModel = _context.PhoneModels.FirstOrDefault(pm => pm.PhoneModelId == phoneModelId);
                if (_phoneModel == null)
                {
                    MessageBox.Show("Модель телефона не найдена!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    NavigationService.GoBack();
                    return;
                }
            }

            LoadData();
        }

        private void LoadData()
        {
            BrandTextBox.Text = _phoneModel.Brand;
            ModelNameTextBox.Text = _phoneModel.ModelName;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _phoneModel.Brand = BrandTextBox.Text;
                _phoneModel.ModelName = ModelNameTextBox.Text;

                _context.SaveChanges();
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении модели телефона: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
