using ClassJournal.Models;
using ClassJournal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ClassJournal.Views.EntityEditors
{
    public partial class PageSubjectEntity : PageEntity
    {
        private readonly Subject _subject;

        public PageSubjectEntity()
        {
            InitializeComponent();
            _subject = new Subject();
            this.Title = "Добавление предмета";
            DatabaseContext.Database.Subjects.Add(_subject);
        }

        public PageSubjectEntity(Subject subject)
        {
            InitializeComponent();
            this._subject = subject;
            txtboxTitle.Text = subject.Title;
            this.Title = "Изменение предмета";
        }

        public override bool EntityRemoved()
        {
            MessageBoxResult result = MessageBox.Show($"Вы действительно хотите удалить предмет {_subject.Title}?",
                "Удаление предмета", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                DatabaseContext.Database.Subjects.Remove(_subject);
                DatabaseContext.SaveDatabase();
                return true;
            }

            return false;
        }

        public override bool EntitySaved()
        {
            if (!EntityValidated())
                return false;

            AppendGroupInfo();

            DatabaseContext.SaveDatabase();
            return true;
        }

        private bool EntityValidated()
        {
            if(StringValidator.ContainsDigits(txtboxTitle.Text))
            {
                App.ShowErrorMessage("Наименование предмета не должно содержать цифр!");
                return false;
            }

            if (ValuesExitsts())
            {
                return false;
            }

            return true;
        }

        private bool ValuesExitsts()
        {
            bool subjectExists = IsSubjectExists(DatabaseContext.Database.Subjects.ToList());

            if (subjectExists)
            {
                App.ShowErrorMessage("Предмет уже существует!");
                return true;
            }

            return false;
        }

        private bool IsSubjectExists(List<Subject> subjects)
        {
            return subjects.Exists(subject => subject.Title.ToLower() == txtboxTitle.Text.ToLower());
        }

        private void AppendGroupInfo()
        {
            _subject.Title = txtboxTitle.Text;
        }
    }
}
