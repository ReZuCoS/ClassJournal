using ClassJournal.Models;
using ClassJournal.ViewModels;
using System.Windows;

namespace ClassJournal.Views.Windows
{
    public partial class WindowCode : Window
    {
        public User User { get; private set; } = null;

        public WindowCode()
        {
            InitializeComponent();
        }

        private void Back(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void SearchCode(object sender, RoutedEventArgs e)
        {
            if (!CodeValidated(txtboxCode.Text))
            {
                return;
            }

            foreach (User user in DatabaseContext.Database.Users)
            {
                string userCode = HashGenerator.GetSHA256(txtboxCode.Text);
                string databaseCode = HashGenerator.GetSHA256(user.Login + user.Password);

                if (userCode == databaseCode)
                {
                    User = user;
                    this.DialogResult = true;
                }
            }

            if (User == null)
            {
                App.ShowErrorMessage("Код пользователя недействителен!");
                return;
            }
        }

        private bool CodeValidated(string text)
        {
            if (StringValidator.IsEmpty(text))
            {
                App.ShowErrorMessage("Введите код пользователя!");
                return false;
            }

            return true;
        }
    }
}
