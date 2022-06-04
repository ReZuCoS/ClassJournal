using System;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Threading;
using ClassJournal.Models;

namespace ClassJournal.Views.Windows
{
    public partial class WindowLogin : Window
    {
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

            //WindowNewUser windowNewUser = new WindowNewUser(user);
            //windowNewUser.ShowDialog();

            this.ShowDialog();
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
