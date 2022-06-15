using ClassJournal.Models;
using ClassJournal.ViewModels;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;

namespace ClassJournal.Views.EntityEditors
{
    public partial class PageGroupEntity : PageEntity
    {
        private readonly Group _group;

        public PageGroupEntity()
        {
            InitializeComponent();
            cboxSpeciality.ItemsSource = DatabaseContext.Database.Specialities.ToList();

            this._group = new Group();
            DatabaseContext.Database.Groups.Add(_group);
            this.Title = "Добавление группы";
        }

        public PageGroupEntity(Group group)
        {
            InitializeComponent();
            cboxSpeciality.ItemsSource = DatabaseContext.Database.Specialities.ToList();

            this._group = group;
            txtboxCource.Text = _group.Cource.ToString();
            txtboxNumber.Text = _group.Number.ToString();
            cboxSpeciality.SelectedItem = _group.Speciality;
            this.Title = "Изменение группы";
        }

        public override bool EntityRemoved()
        {
            MessageBoxResult result = MessageBox.Show($"Вы действительно хотите удалить группу {_group.GroupID}?",
                "Удаление группы", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                DatabaseContext.Database.Groups.Remove(_group);
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
            List<TextBox> textBoxes = new List<TextBox>
            {
                txtboxCource,
                txtboxNumber
            };

            Tuple<bool, string> tuple = TextBoxValidator.ValidateMany(textBoxes, TextBoxValidator.ValidationType.SymbolsAvailability);

            if(tuple.Item1 == false)
            {
                App.ShowErrorMessage(tuple.Item2);
                return false;
            }

            if (StringValidator.ContainsLetters(txtboxCource.Text))
            {
                App.ShowErrorMessage("Курс группы не должен содержать букв!");
                return false;
            }

            if (StringValidator.ContainsLetters(txtboxNumber.Text))
            {
                App.ShowErrorMessage("Номер группы не должен содержать букв!");
                return false;
            }

            if(cboxSpeciality.SelectedItem == null)
            {
                App.ShowErrorMessage("Выберите специальность группы!");
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
            bool groupExists = IsGroupIDExists(DatabaseContext.Database.Groups.ToList());

            if (groupExists)
            {
                App.ShowErrorMessage("Группа уже существует!");
                return true;
            }

            return false;
        }

        private bool IsGroupIDExists(List<Group> groups)
        {
            Speciality selectedSpeciality = (Speciality)cboxSpeciality.SelectedItem;

            string groupID = $"{txtboxCource.Text}{txtboxNumber.Text}{selectedSpeciality.Abbreviation}";

            return groups.Exists(group => group.GroupID == groupID);
        }

        private void AppendGroupInfo()
        {
            _group.Cource = int.Parse(txtboxCource.Text);
            _group.Number = int.Parse(txtboxNumber.Text);
            _group.Speciality = (Speciality)cboxSpeciality.SelectedItem;
            
            if(this.Title == "Добавление группы")
            {
                _group.GroupID = $"{_group.Cource}{_group.Number}{_group.Speciality.Abbreviation}";
            }
        }
    }
}
