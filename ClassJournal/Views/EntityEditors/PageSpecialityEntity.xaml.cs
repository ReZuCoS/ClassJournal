using ClassJournal.Models;
using ClassJournal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ClassJournal.Views.EntityEditors
{
    public partial class PageSpecialityEntity : PageEntity
    {
        private readonly Speciality _speciality;

        public PageSpecialityEntity()
        {
            InitializeComponent();
            this.Title = "Добавление специальности";
            this._speciality = new Speciality();
            DatabaseContext.Database.Specialities.Add(_speciality);
        }

        public PageSpecialityEntity(Speciality speciality)
        {
            InitializeComponent();
            this.Title = "Изменение специальности";
            this._speciality = speciality;
            txtboxCode.Text = _speciality.SpecialityCode;
            txtboxTitle.Text = _speciality.Title;
            txtboxAbbreviation.Text = _speciality.Abbreviation;
        }

        public override bool EntityRemoved()
        {
            MessageBoxResult result = MessageBox.Show($"Вы действительно хотите удалить специальность {_speciality.Title}?",
                "Удаление специальности", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                DatabaseContext.Database.Specialities.Remove(_speciality);
                DatabaseContext.SaveDatabase();
                return true;
            }

            return false;
        }

        public override bool EntitySaved()
        {
            if (!EntityValidated())
                return false;

            AppendSpecialityInfo();

            DatabaseContext.SaveDatabase();
            return true;
        }

        private bool EntityValidated()
        {
            if (!App.ConnectionService.IsConnected)
            {
                App.ShowErrorMessage("Проверьте подключение к сети!");
                return false;
            }

            List<TextBox> textBoxes = new List<TextBox>()
            {
                txtboxCode,
                txtboxTitle,
                txtboxAbbreviation
            };

            Tuple<bool, string> tuple = TextBoxValidator.ValidateMany(textBoxes, TextBoxValidator.ValidationType.SymbolsAvailability);

            if (tuple.Item1 == false)
            {
                App.ShowErrorMessage(tuple.Item2);
                return false;
            }

            if (StringValidator.ContainsLetters(txtboxCode.Text))
            {
                App.ShowErrorMessage("Код специальности не должен содержать букв!");
                return false;
            }

            if (StringValidator.ContainsDigits(txtboxTitle.Text))
            {
                App.ShowErrorMessage("Наименование специальности не должно содержать цифр!");
                return false;
            }

            if (StringValidator.ContainsDigits(txtboxAbbreviation.Text) || StringValidator.ContainsSymbols(txtboxAbbreviation.Text))
            {
                App.ShowErrorMessage("Аббривеатура специальности не должна содержать цифр или символов!");
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
            bool codeExists = IsСodeExists(DatabaseContext.Database.Specialities.ToList());

            if (codeExists)
            {
                App.ShowErrorMessage("Специальность с таким кодом уже существует!\nВведите другой код");
                return true;
            }

            bool abbreviationExists = IsAbbreviationExists(DatabaseContext.Database.Specialities.ToList());

            if (abbreviationExists)
            {
                App.ShowErrorMessage("Специальность с такой аббревиатурой уже существует!\nВведите другую аббревиатуру");
                return true;
            }

            return false;
        }

        private bool IsСodeExists(List<Speciality> specialities)
        {
            return specialities.Exists(speciality => speciality.SpecialityCode == txtboxCode.Text);
        }

        private bool IsAbbreviationExists(List<Speciality> specialities)
        {
            return specialities.Exists(speciality => speciality.Abbreviation == txtboxAbbreviation.Text);
        }

        private void AppendSpecialityInfo()
        {
            _speciality.SpecialityCode = txtboxCode.Text;
            _speciality.Title = txtboxTitle.Text;
            _speciality.Abbreviation = txtboxAbbreviation.Text;
        }
    }
}
