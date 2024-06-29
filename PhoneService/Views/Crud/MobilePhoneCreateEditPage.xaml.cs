using PhoneService.Data;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PhoneService.Views.Crud
{
    public partial class MobilePhoneCreateEditPage : Page
    {
        private PhoneServiceEntities _context;
        private MobilePhones _mobilePhone;

        public MobilePhoneCreateEditPage(int mobilePhoneId = 0)
        {
            InitializeComponent();
            _context = new PhoneServiceEntities();

            if (mobilePhoneId == 0)
            {
                _mobilePhone = new MobilePhones { ReceivedDate = DateTime.Now };
                _context.MobilePhones.Add(_mobilePhone);
            }
            else
            {
                _mobilePhone = _context.MobilePhones.FirstOrDefault(mp => mp.MobilePhoneId == mobilePhoneId);
                if (_mobilePhone == null)
                {
                    MessageBox.Show("Телефон не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    NavigationService.GoBack();
                    return;
                }
            }

            LoadData();
            LoadClients();
            LoadPhoneModels();
        }

        private void LoadData()
        {
            if (_mobilePhone.ClientId != 0)
            {
                ClientComboBox.SelectedValue = _mobilePhone.ClientId;
            }

            if (_mobilePhone.PhoneModelId != 0)
            {
                PhoneModelComboBox.SelectedValue = _mobilePhone.PhoneModelId;
            }

            IMEITextBox.Text = _mobilePhone.IMEI;
            ReceivedDatePicker.SelectedDate = _mobilePhone.ReceivedDate;
            IssueDescriptionTextBox.Text = _mobilePhone.IssueDescription;
        }

        private void LoadClients()
        {
            var clients = _context.Clients.ToList().Select(c => new
            {
                c.ClientId,
                FullName = $"{c.FirstName} {c.LastName}"
            }).ToList();

            ClientComboBox.ItemsSource = clients;
        }

        private void LoadPhoneModels()
        {
            var phoneModels = _context.PhoneModels.ToList();
            PhoneModelComboBox.ItemsSource = phoneModels;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _mobilePhone.ClientId = (int)ClientComboBox.SelectedValue;
                _mobilePhone.PhoneModelId = (int)PhoneModelComboBox.SelectedValue;
                _mobilePhone.IMEI = IMEITextBox.Text;
                _mobilePhone.ReceivedDate = ReceivedDatePicker.SelectedDate ?? DateTime.Now;
                _mobilePhone.IssueDescription = IssueDescriptionTextBox.Text;

                _context.SaveChanges();
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении телефона: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
