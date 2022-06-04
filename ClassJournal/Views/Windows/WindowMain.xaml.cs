using ClassJournal.Models;
using ClassJournal.ViewModels;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Threading;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ClassJournal.Views.Windows
{
    public partial class WindowMain : Window
    {
        private readonly string _tokenPath = $@"{Environment.CurrentDirectory}/UserToken.txt";
        private User _user = null;

        public WindowMain()
        {
            InitializeComponent();
            this.Hide();

            CheckConnectionServiceStatus();

            if (WindowLoaded())
                this.Show();
            else
                Application.Current.Shutdown();


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

        private bool WindowLoaded()
        {
            if (TokenExists(_tokenPath))
            {
                string userToken = GetUserToken(_tokenPath);
                _user = UserByToken(userToken);
            }

            if (_user == null)
            {
                _user = AuthUser();
            }

            if (_user != null)
            {
                //List<string> tableNames = GetTableNames(_user.Employee.Positions.ToList());

                //SetTableList(tableNames);

                return true;
            }
            else
            {
                return false;
            }
        }

        private bool TokenExists(string tokenPath)
        {
            return File.Exists(tokenPath);
        }

        private string GetUserToken(string tokenPath)
        {
            return File.ReadAllText(tokenPath);
        }

        private User UserByToken(string userToken)
        {
            return DatabaseContext.Database.Users.FirstOrDefault(user => user.Token == userToken);
        }

        private User AuthUser()
        {
            WindowLogin window = new WindowLogin();

            if (window.ShowDialog() == true)
                return null; // window.User;

            return null;
        }
    }
}
