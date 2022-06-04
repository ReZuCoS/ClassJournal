using ClassJournal.Services;
using System.Windows;

namespace ClassJournal
{
    public partial class App : Application
    {
        internal static ConnectionService ConnectionService = new ConnectionService();

        internal static void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
