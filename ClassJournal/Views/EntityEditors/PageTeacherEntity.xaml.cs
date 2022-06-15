using ClassJournal.Models;
using ClassJournal.ViewModels;
using System.Linq;
using System.Windows;
using System.Collections.Generic;

namespace ClassJournal.Views.EntityEditors
{
    public partial class PageTeacherEntity : PageEntity
    {
        private readonly Teacher _teacher;

        public PageTeacherEntity()
        {
            InitializeComponent();
            cboxEmployee.ItemsSource = DatabaseContext.Database.Employees.ToList();
            cboxSubject.ItemsSource = DatabaseContext.Database.Subjects.ToList();

            this._teacher = new Teacher();
            DatabaseContext.Database.Teachers.Add(_teacher);
            this.Title = "Добавление преподавателя";
        }

        public PageTeacherEntity(Teacher teacher)
        {
            InitializeComponent();
            cboxEmployee.ItemsSource = DatabaseContext.Database.Employees.ToList();
            cboxSubject.ItemsSource = DatabaseContext.Database.Subjects.ToList();

            this._teacher = teacher;
            cboxEmployee.SelectedItem = _teacher.Employee;
            cboxSubject.SelectedItem = _teacher.Subject;
            this.Title = "Изменение преподавателя";
        }

        public override bool EntityRemoved()
        {
            MessageBoxResult result = MessageBox.Show($"Вы действительно хотите удалить преподавателя " +
                $"{_teacher.Employee.Surname} {_teacher.Employee.Name} {_teacher.Employee.Patronymic}?",
                "Удаление преподавателя", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                DatabaseContext.Database.Teachers.Remove(_teacher);
                DatabaseContext.SaveDatabase();
                return true;
            }

            return false;
        }

        public override bool EntitySaved()
        {
            if (!EntityValidated())
                return false;

            AppendTeacherInfo();

            DatabaseContext.SaveDatabase();
            return true;
        }

        private bool EntityValidated()
        {
            if(cboxEmployee.SelectedItem == null)
            {
                App.ShowErrorMessage("Выберите преподавателя!");
                return false;
            }

            if (cboxSubject.SelectedItem == null)
            {
                App.ShowErrorMessage("Выберите предмет!");
                return false;
            }

            if (ValuesExists())
            {
                return false;
            }

            return true;
        }

        private bool ValuesExists()
        {
            bool teacherExists = IsTeacherExists(DatabaseContext.Database.Teachers.ToList());

            if (teacherExists)
            {
                App.ShowErrorMessage("Информация о данном преподавателе уже существует в системе!");
                return true;
            }

            return false;
        }

        private bool IsTeacherExists(List<Teacher> teachers)
        {
            Employee selectedEmployee = (Employee)cboxEmployee.SelectedItem;
            Subject selectedSubject = (Subject)cboxSubject.SelectedItem;

            return teachers.Exists(teacher =>
            teacher.Employee == selectedEmployee &&
            teacher.Subject == selectedSubject);
        }

        private void AppendTeacherInfo()
        {
            _teacher.Employee = (Employee)cboxEmployee.SelectedItem;
            _teacher.Subject = (Subject)cboxSubject.SelectedItem;
        }
    }
}
