using ClassJournal.Services;
using System.Windows;

namespace ClassJournal
{
    public partial class App : Application
    {
        internal static ConnectionService ConnectionService = new ConnectionService();
    }
}
