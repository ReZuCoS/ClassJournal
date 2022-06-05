using ClassJournal.Models;
using ClassJournal.ViewModels;
using ClassJournal.Views.EntityEditors;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Threading;
using System.Windows.Media;
using System.Threading.Tasks;

namespace ClassJournal.Views.Windows
{
    public partial class WindowLogin : Window
    {
        public User User { get; private set; }

        public WindowLogin()
        {
            InitializeComponent();
            CheckConnectionServiceStatus();
        }

        private void OpenCodeInsertWindow(object sender, RoutedEventArgs e)
        {
            if (!App.ConnectionService.IsConnected)
            {
                App.ShowErrorMessage("Проверьте подключение к сети!");
                return;
            }

            WindowCode window = new WindowCode();
            window.ShowDialog();

            if (window.DialogResult == true)
            {
                OpenWindowUserEntity(window.User);
            }
        }

        private void OpenWindowUserEntity(User user)
        {
            this.Hide();

            PageUserEntity page = new PageUserEntity(user);

            WindowEntityEditor windowNewUser = new WindowEntityEditor(page);
            windowNewUser.ShowDialog();

            this.ShowDialog();
        }

        private void Login(object sender, RoutedEventArgs e)
        {
            if (!WindowValidated())
            {
                return;
            }

            User = SearchUser(txtboxLogin.Text);

            if (UserDataValidated(User))
            {
                if (btnRememberUser.IsChecked.Value)
                {
                    string path = $@"{Environment.CurrentDirectory}\UserToken";

                    File.Delete(path);
                    File.AppendAllText(path, User.Token.ToString());
                }

                this.DialogResult = true;
            }
        }

        private bool WindowValidated()
        {
            if (!App.ConnectionService.IsConnected)
            {
                App.ShowErrorMessage("Проверьте подключение к сети!");
                return false;
            }

            if (StringValidator.IsEmpty(txtboxLogin.Text) ||
                StringValidator.IsEmpty(txtboxLogin.Text))
            {
                App.ShowErrorMessage("Заполните все текстовые поля!");
                return false;
            }

            if (StringValidator.LengthLessThan(txtboxLogin.Text, 6))
            {
                App.ShowErrorMessage("Поле Логин имеет длину от 6 до 12 символов!");
                return false;
            }

            if (StringValidator.LengthLessThan(txtboxPassword.Password, 8))
            {
                App.ShowErrorMessage("Поле Пароль имеет длину от 8 до 25 символов!");
                return false;
            }

            return true;
        }

        private User SearchUser(string login)
        {
            User foundUser = null;
            foundUser = DatabaseContext.Database.Users.FirstOrDefault(user => user.Login == login);

            return foundUser;
        }

        private bool UserDataValidated(User user)
        {
            string passwordHash = HashGenerator.GetSHA256(txtboxPassword.Password);

            if (user == null || user.Password != passwordHash)
            {
                App.ShowErrorMessage("Пользователь не найден, проверьте правильность введённых данных!");
                return false;
            }

            return true;
        }

        private void PasswordInputHandler(object sender, RoutedEventArgs e)
        {
            if (txtboxPassword.Password.Length > 0)
                watermarkPassword.Visibility = Visibility.Collapsed;
            else
                watermarkPassword.Visibility = Visibility.Visible;
        }

        private async void CheckConnectionServiceStatus()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    if (App.ConnectionService.IsConnected)
                    {
                        SetEllipseColor("successConnectionBrush");
                    }
                    else
                    {
                        SetEllipseColor("failedConnectionBrush");
                    }

                    Thread.Sleep(2000);
                }
            });
        }

        private void SetEllipseColor(string brushName)
        {
            try
            {
                this.Dispatcher.Invoke(() =>
                {
                    Brush brush = (Brush)Application.Current.Resources[brushName];
                    this.ellipseConnectionStatus.Fill = brush;
                });
            }
            catch
            {
                return;
            }
        }
    }
}
