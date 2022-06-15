using ClassJournal.Models;
using ClassJournal.ViewModels;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Input;

namespace ClassJournal.Views.EntityEditors
{
    public partial class PageAttendanceEntity : PageEntity
    {
        private Lesson _lesson;
        private List<Attendance> _attendances = new List<Attendance>();

        public PageAttendanceEntity(Lesson lesson)
        {
            InitializeComponent();

            _lesson = lesson;

            _attendances = DatabaseContext.Database.Attendances.Where(attendance => attendance.LessonID == _lesson.LessonID).ToList();

            if (_attendances.Count < DatabaseContext.Database.Students.Where(student => student.GroupID == _lesson.GroupID).Count())
            {
                DatabaseContext.Database.Attendances.RemoveRange(_attendances);

                foreach (Student student in lesson.Group.Students)
                {
                    Attendance attendance = new Attendance
                    {
                        LessonID = lesson.LessonID,
                        StudentID = student.StudentID,
                        IsVisited = false
                    };

                    DatabaseContext.Database.Attendances.Add(attendance);
                }

                DatabaseContext.Database.SaveChanges();
                _attendances = DatabaseContext.Database.Attendances.Where(attendance => attendance.LessonID == _lesson.LessonID).ToList();
            }

            listViewStudentsAttendance.ItemsSource = _attendances;
        }

        public override bool EntityRemoved()
        {
            return true;
        }

        public override bool EntitySaved()
        {
            DatabaseContext.SaveDatabase();
            return true;
        }

        private void ChangeVisitedStatus(object sender, MouseButtonEventArgs e)
        {
            if (listViewStudentsAttendance.SelectedItem == null)
            {
                return;
            }

            Attendance attendance = (Attendance)listViewStudentsAttendance.SelectedItem;

            attendance.IsVisited = !attendance.IsVisited;

            _attendances = DatabaseContext.Database.Attendances.Where(atten => atten.LessonID == _lesson.LessonID).ToList();

            listViewStudentsAttendance.ItemsSource = _attendances;
        }
    }
}
