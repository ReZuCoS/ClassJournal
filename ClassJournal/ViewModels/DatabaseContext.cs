using ClassJournal.Models;
using System;

namespace ClassJournal.ViewModels
{
    internal static class DatabaseContext
    {
        internal static ModelClassJournal Database = new ModelClassJournal();

        internal static void SaveDatabase()
        {
            try
            {
                Database.SaveChanges();
            }
            catch (Exception exception)
            {
                App.ShowErrorMessage("Ошибка:\n" + exception.Message);
            }
        }
    }
}
