using System.Windows.Controls;

namespace ClassJournal.Views.EntityEditors
{
    public abstract class PageEntity : Page
    {
        public abstract bool EntitySaved();
        public abstract bool EntityRemoved();
    }
}
