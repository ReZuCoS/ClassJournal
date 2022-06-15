using ClassJournal.Models;
using ClassJournal.ViewModels;
using System;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Text;

namespace ClassJournal.Views.EntityEditors
{
    public partial class PageEmployeeEntity : PageEntity
    {
        private readonly Employee _employee;

        public PageEmployeeEntity()
        {
            InitializeComponent();
            this._employee = new Employee();
            this.Title = "Добавление сотрудника";
            DatabaseContext.Database.Employees.Add(_employee);
        }

        public PageEmployeeEntity(Employee employee)
        {
            InitializeComponent();
            this._employee = employee;
            this.Title = "Изменение сотрудника";
            txtboxSurname.Text = _employee.Surname;
            txtboxName.Text = _employee.Name;
            txtboxPatronymic.Text = _employee.Patronymic;
            txtboxPhone.Text = _employee.Phone;
            txtboxPassport.Text = _employee.Passport;
            txtboxINN.Text = _employee.INN;
            txtboxComment.Text = _employee.Comment;
            txtboxAddress.Text = _employee.Address;
            dateTimeBithday.SelectedDate = _employee.Bithday;
            if (_employee.Image != null)
            {
                employeePhoto.Source = DatabaseImageConverter.ToBitmapImage(_employee.Image);
            }
        }

        private void ChangeImage(object sender, RoutedEventArgs e)
        {
            string fileName = GetFileName();

            if (fileName == null)
            {
                return;
            }

            try
            {
                BitmapImage image = new BitmapImage(new Uri(fileName));
                _employee.Image = DatabaseImageConverter.ToByteArray(image);
                employeePhoto.Source = DatabaseImageConverter.ToBitmapImage(_employee.Image);
            }
            catch
            {
                App.ShowErrorMessage("Неверный формат файла!");
                return;
            }
        }

        private string GetFileName()
        {
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                Title = "Выберите фото",
                InitialDirectory = $@"C:\Users\{Environment.UserName}\Desktop",
                Filter = "Файлы JPG, PNG|*.jpg; *.png;|Все файлы(*.*)|*.*"
            };

            if (fileDialog.ShowDialog() == true)
            {
                return fileDialog.FileName;
            }

            return null;
        }

        public override bool EntitySaved()
        {
            if (!EntityValidated())
                return false;

            AppendEmployeeInfo();

            DatabaseContext.SaveDatabase();
            return true;
        }

        private bool EntityValidated()
        {
            if (!App.ConnectionService.IsConnected)
            {
                App.ShowErrorMessage("Проверьте подключение к сети!");
                return false;
            }

            List<TextBox> onlyTextBoxes = new List<TextBox>()
            {
                txtboxSurname,
                txtboxName
            };

            List<TextBox> onlyDigitsTextBoxes = new List<TextBox>()
            {
                txtboxPhone,
                txtboxINN,
                txtboxPassport
            };

            Tuple<bool, string> tupleText = TextBoxValidator.ValidateMany(onlyTextBoxes, TextBoxValidator.ValidationType.AllowOnlyText);
            Tuple<bool, string> tupleDigits = TextBoxValidator.ValidateMany(onlyDigitsTextBoxes, TextBoxValidator.ValidationType.AllowOnlyDigits);

            if (tupleText.Item1 == false)
            {
                App.ShowErrorMessage(tupleText.Item2);
                return false;
            }

            if (tupleDigits.Item1 == false)
            {
                App.ShowErrorMessage(tupleDigits.Item2);
                return false;
            }

            if (StringValidator.LengthLessThan(txtboxSurname.Text, 3))
            {
                App.ShowErrorMessage("Поле Фамилия имеет длину от 3 до 50 символов!");
                return false;
            }

            if (StringValidator.LengthLessThan(txtboxName.Text, 2))
            {
                App.ShowErrorMessage("Поле Имя имеет длину от 2 до 50 символов!");
                return false;
            }

            if (StringValidator.LengthLessThan(txtboxPassport.Text, 10))
            {
                App.ShowErrorMessage("Поле Паспорт имеет длину 10 символов!");
                return false;
            }

            if (StringValidator.LengthLessThan(txtboxINN.Text, 12))
            {
                App.ShowErrorMessage("Поле ИНН имеет длину 12 символов!");
                return false;
            }

            if (StringValidator.LengthLessThan(txtboxPhone.Text, 11))
            {
                App.ShowErrorMessage("Поле Телефон имеет длину 11 символов!");
                return false;
            }

            int age = DateTime.Now.Year - dateTimeBithday.SelectedDate.Value.Year;

            if(dateTimeBithday.SelectedDate.Value.AddYears(age) > DateTime.Now)
            {
                age--;
            }

            if(age < 18)
            {
                App.ShowErrorMessage("Возраст сотрудника должен быть больше 18 лет!");
                return false;
            }

            if (age >= 95)
            {
                App.ShowErrorMessage("Возраст сотрудника должен быть меньше 95 лет!");
                return false;
            }

            return true;
        }

        private void AppendEmployeeInfo()
        {
            _employee.Surname = txtboxSurname.Text;
            _employee.Name = txtboxName.Text;
            _employee.Patronymic = txtboxPatronymic.Text;
            _employee.Passport = txtboxPassport.Text;
            _employee.Address = txtboxAddress.Text;
            _employee.INN = txtboxINN.Text;
            _employee.Phone = txtboxPhone.Text;
            _employee.Comment = txtboxComment.Text;
            _employee.Bithday = dateTimeBithday.SelectedDate;

            if (_employee.Image == null)
            {
                BitmapImage defaultImage = (BitmapImage)Application.Current.Resources["emptyPhoto"];

                _employee.Image = DatabaseImageConverter.ToByteArray(defaultImage);
            }
        }

        public override bool EntityRemoved()
        {
            MessageBoxResult result = MessageBox.Show($"Вы действительно хотите удалить сотрудника {_employee.Surname} {_employee.Name}?",
                "Удаление сотрудника", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if(result == MessageBoxResult.Yes)
            {
                DatabaseContext.Database.Employees.Remove(_employee);
                DatabaseContext.SaveDatabase();
                return true;
            }

            return false;
        }
    }
}
