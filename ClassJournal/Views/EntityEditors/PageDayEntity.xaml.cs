using ClassJournal.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using ClassJournal.Views.Windows;
using ClassJournal.ViewModels;

namespace ClassJournal.Views.EntityEditors
{
    public partial class PageDayEntity : PageEntity
    {
        private readonly DateTime _date;
        private readonly User _user;
        private List<Lesson> _lessons;

        public PageDayEntity(DateTime date, List<Lesson> lessons, User user)
        {
            InitializeComponent();
            _date = date;
            _user = user;
            _lessons = lessons.Where(lesson =>
                lesson.Date == date &&
                lesson.Group == user.Employee.Group).ToList();

            if (_lessons.Count > 0)
                listViewLessons.ItemsSource = _lessons.OrderBy(lesson => lesson.LessonNumber);
            else
                errorLabel.Visibility = Visibility.Visible;
        }

        public override bool EntityRemoved()
        {
            return true;
        }

        public override bool EntitySaved()
        {
            return true;
        }

        private void ShowLessonInfo(object sender, MouseButtonEventArgs e)
        {
            if (listViewLessons.SelectedItem == null)
            {
                return;
            }

            Lesson selectedLesson = (Lesson)listViewLessons.SelectedItem;

            PageAttendanceEntity pageEntity = new PageAttendanceEntity(selectedLesson);
            OpenEntityChanger(pageEntity);
        }

        private void OpenEntityChanger(PageEntity page)
        {
            WindowEntityEditor window = new WindowEntityEditor(page);
            window.ShowDialog();
        }
    }
}
