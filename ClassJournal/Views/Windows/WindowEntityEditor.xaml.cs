using ClassJournal.Views.EntityEditors;
using System.Windows;
using System.Threading;
using System.Windows.Media;
using System.Threading.Tasks;

namespace ClassJournal.Views.Windows
{
    public partial class WindowEntityEditor : Window
    {
        private readonly PageEntity _currentEntityPage = null;

        public WindowEntityEditor(PageEntity page)
        {
            InitializeComponent();
            CheckServiceStatus();
            this.Title = page.Title;
            this.MinHeight = page.MinHeight + 100;
            this.MinWidth = page.MinWidth + 100;
            this.Height = page.MinHeight + 70;
            this.Width = page.MinWidth + 70;
            _currentEntityPage = page;
            frameMain.Content = _currentEntityPage;
        }

        private void Back(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void SaveChanges(object sender, RoutedEventArgs e)
        {
            if(!App.ConnectionService.IsConnected)
            {
                App.ShowErrorMessage("Проверьте подключение к сети!");
                return;
            }

            if(_currentEntityPage.EntitySaved())
            {
                this.DialogResult = true;
            }    
        }

        private void RemoveEntity(object sender, RoutedEventArgs e)
        {
            if (!App.ConnectionService.IsConnected)
            {
                App.ShowErrorMessage("Проверьте подключение к сети!");
                return;
            }

            if (_currentEntityPage.EntityRemoved())
            {
                this.DialogResult = true;
            }
        }

        private async void CheckServiceStatus()
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
