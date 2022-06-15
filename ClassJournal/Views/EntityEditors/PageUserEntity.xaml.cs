using ClassJournal.Models;
using ClassJournal.ViewModels;
using System;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace ClassJournal.Views.EntityEditors
{
    public partial class PageUserEntity : PageEntity
    {
        private bool _imageChanged = false;
        private readonly User _user;
        
        public PageUserEntity(User user)
        {
            InitializeComponent();
             this._user = user;

            txtboxSurname.Text = _user.Employee.Surname;
            txtboxName.Text = _user.Employee.Name;
            txtboxPatronymic.Text = _user.Employee.Patronymic;
            txtboxBithday.Text = _user.Employee.Bithday.Value.Date.ToShortDateString();
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
                _user.Employee.Image = DatabaseImageConverter.ToByteArray(image);
                employeePhoto.Source = DatabaseImageConverter.ToBitmapImage(_user.Employee.Image);
                _imageChanged = true;
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

            AppendUserInfo();

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

            List<TextBox> textBoxes = new List<TextBox>()
            {
                txtboxLogin,
                txtboxEmail
            };

            List<PasswordBox> passwordBoxes = new List<PasswordBox>()
            {
                txtboxPassword,
                txtboxRepeatPassword
            };

            Tuple<bool, string> tuple1 = TextBoxValidator.ValidateMany(textBoxes, TextBoxValidator.ValidationType.SymbolsAvailability);
            Tuple<bool, string> tuple2 = TextBoxValidator.ValidateMany(passwordBoxes, TextBoxValidator.ValidationType.SymbolsAvailability);

            if (tuple1.Item1 == false)
            {
                App.ShowErrorMessage(tuple1.Item2);
                return false;
            }

            if (tuple2.Item1 == false)
            {
                App.ShowErrorMessage(tuple2.Item2);
                return false;
            }

            if (StringValidator.LengthLessThan(txtboxLogin.Text, 6))
            {
                App.ShowErrorMessage("Поле Логин имеет длину от 6 до 12 символов!");
                return false;
            }

            if (StringValidator.LengthLessThan(txtboxPassword.Password, 8) ||
                StringValidator.LengthLessThan(txtboxRepeatPassword.Password, 8))
            {
                App.ShowErrorMessage("Поле Пароль и Повторите пароль имеют длину от 8 до 25 символов!");
                return false;
            }

            if (txtboxPassword.Password != txtboxRepeatPassword.Password)
            {
                App.ShowErrorMessage("Пароли должны совпадать!");
                return false;
            }

            if (StringValidator.LengthLessThan(txtboxEmail.Text, 6))
            {
                App.ShowErrorMessage("Поле E-mail имеет длину от 6 до 50 символов!");
                return false;
            }

            if (ValuesExists())
            {
                return false;
            }

            return true;
        }

        private void AppendUserInfo()
        {
            _user.Login = txtboxLogin.Text;
            _user.Password = HashGenerator.GetSHA256(txtboxPassword.Password);
            _user.Email = txtboxEmail.Text;
            _user.Token = $"{HashGenerator.GetSHA256(txtboxLogin.Text)}{HashGenerator.GetSHA256(txtboxPassword.Password)}";

            if (!_imageChanged)
            {
                BitmapImage defaultImage = (BitmapImage)Application.Current.Resources["emptyPhoto"];

                _user.Employee.Image = DatabaseImageConverter.ToByteArray(defaultImage);
            }
        }

        private bool ValuesExists()
        {
            bool loginExists = IsLoginExists(DatabaseContext.Database.Users.ToList());

            if (loginExists)
            {
                App.ShowErrorMessage("Пользователь с таким логином уже существует!\nВведите другой логин");
                return true;
            }
            
            bool emailExists = IsEmailExists(DatabaseContext.Database.Users.ToList());

            if (emailExists)
            {
                App.ShowErrorMessage("Пользователь с таким адресом эл. почты уже существует!\nВведите другой адрес");
                return true;
            }

            return false;
        }

        private bool IsLoginExists(List<User> users)
        {
            return users.Exists(user => user.Login == txtboxLogin.Text);
        }

        private bool IsEmailExists(List<User> users)
        {
            return users.Exists(user => user.Email == txtboxEmail.Text);
        }

        private void PasswordInputHandler(object sender, RoutedEventArgs e)
        {
            if (txtboxPassword.Password.Length > 0)
                watermarkPassword.Visibility = Visibility.Collapsed;
            else
                watermarkPassword.Visibility = Visibility.Visible;
        }

        private void RepeatPasswordInputHandler(object sender, RoutedEventArgs e)
        {
            if (txtboxPassword.Password.Length > 0)
                watermarkRepeatPassword.Visibility = Visibility.Collapsed;
            else
                watermarkRepeatPassword.Visibility = Visibility.Visible;
        }

        public override bool EntityRemoved()
        {
            return false;
        }
    }
}
