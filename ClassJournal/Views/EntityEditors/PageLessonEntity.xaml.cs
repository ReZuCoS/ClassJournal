using ClassJournal.Models;
using ClassJournal.ViewModels;
using System;
using System.Linq;
using System.Windows;

namespace ClassJournal.Views.EntityEditors
{
    public partial class PageLessonEntity : PageEntity
    {
        private readonly Lesson _lesson;

        public PageLessonEntity()
        {
            InitializeComponent();

            cboxAudience.ItemsSource = DatabaseContext.Database.Audiences.ToList();
            cboxGroup.ItemsSource = DatabaseContext.Database.Groups.ToList();
            cboxLessonHour.ItemsSource = DatabaseContext.Database.LessonHours.ToList();
            cboxTeacher.ItemsSource = DatabaseContext.Database.Teachers.ToList();

            _lesson = new Lesson();
            this.Title = "Добавление урока";
            DatabaseContext.Database.Lessons.Add(_lesson);
        }

        public PageLessonEntity(Lesson lesson)
        {
            InitializeComponent();

            cboxAudience.ItemsSource = DatabaseContext.Database.Audiences.ToList();
            cboxGroup.ItemsSource = DatabaseContext.Database.Groups.ToList();
            cboxLessonHour.ItemsSource = DatabaseContext.Database.LessonHours.ToList();
            cboxTeacher.ItemsSource = DatabaseContext.Database.Teachers.ToList();

            _lesson = lesson;
            this.Title = "Изменение урока";
            cboxAudience.SelectedItem = _lesson.Audience;
            cboxGroup.SelectedItem = _lesson.Group;
            cboxTeacher.SelectedItem = _lesson.Teacher;
            cboxLessonHour.SelectedItem = _lesson.LessonHour;
            datePickerLesson.SelectedDate = _lesson.Date;
        }

        public override bool EntityRemoved()
        {
            MessageBoxResult result = MessageBox.Show($"Вы действительно хотите удалить урок?",
                "Удаление урока", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                foreach (Attendance attendance in DatabaseContext.Database.Attendances)
                {
                    if(attendance.LessonID == _lesson.LessonID)
                    {
                        DatabaseContext.Database.Attendances.Remove(attendance);
                    }
                }

                DatabaseContext.Database.Lessons.Remove(_lesson);
                DatabaseContext.SaveDatabase();
                return true;
            }

            return false;
        }

        public override bool EntitySaved()
        {
            if (!EntityValidated())
                return false;

            AppendLessonInfo();

            DatabaseContext.SaveDatabase();
            return true;
        }

        private bool EntityValidated()
        {
            if(cboxAudience.SelectedItem == null)
            {
                App.ShowErrorMessage("Выберите аудиторию!");
                return false;
            }

            if(cboxTeacher.SelectedItem == null)
            {
                App.ShowErrorMessage("Выберите преподавателя!");
                return false;
            }

            if(cboxGroup.SelectedItem == null)
            {
                App.ShowErrorMessage("Выберите группу!");
                return false;
            }

            if(cboxLessonHour.SelectedItem == null)
            {
                App.ShowErrorMessage("Выберите номер урока!");
                return false;
            }

            if (datePickerLesson.SelectedDate == null)
            {
                App.ShowErrorMessage("Выберите дату!");
                return false;
            }

            if (ValuesExists())
            {
                App.ShowErrorMessage("Группа уже записана на данный урок в указанную дату!");
                return false;
            }

            return true;
        }

        private bool ValuesExists()
        {
            return DatabaseContext.Database.Lessons.ToList().Exists(lesson =>
            lesson.Date == datePickerLesson.SelectedDate.Value &&
            lesson.LessonHour == cboxLessonHour.SelectedItem &&
            lesson.Group == cboxGroup.SelectedItem);
        }

        private void AppendLessonInfo()
        {
            _lesson.Audience = (Audience)cboxAudience.SelectedItem;
            _lesson.Group = (Group)cboxGroup.SelectedItem;
            _lesson.Teacher = (Teacher)cboxTeacher.SelectedItem;
            _lesson.LessonHour = (LessonHour)cboxLessonHour.SelectedItem;
            _lesson.Date = datePickerLesson.SelectedDate.Value;
        }
    }
}
