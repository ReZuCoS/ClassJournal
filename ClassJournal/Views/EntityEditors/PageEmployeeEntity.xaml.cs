using System;
using System.Windows.Controls;

namespace ClassJournal.Views.EntityEditors
{
    public partial class PageEmployeeEntity : PageEntity
    {
        public PageEmployeeEntity()
        {
            InitializeComponent();
        }

        public override bool EntitySaved()
        {
            throw new NotImplementedException();
        }
    }
}
