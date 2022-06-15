using ClassJournal.Models;
using ClassJournal.ViewModels;
using ClassJournal.Views.EntityCards;
using ClassJournal.Views.EntityEditors;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Threading;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Input;
using System.Diagnostics;
using System.Text;
using Microsoft.Win32;

namespace ClassJournal.Views.Windows
{
    public partial class WindowMain : Window
    {
        private User _user = null;
        private string _contextTableName;
        private string _userPosition;
        private DateTime _contextDate;
        private readonly string _tokenPath = $@"{Environment.CurrentDirectory}/UserToken";

        public WindowMain()
        {
            InitializeComponent();
            this.Hide();

            CheckConnectionServiceStatus();

            if (WindowLoaded())
                this.Show();
            else
                Application.Current.Shutdown();
        }

        private bool WindowLoaded()
        {
            if (TokenExists(_tokenPath))
            {
                string userToken = GetUserToken(_tokenPath);
                _user = UserByToken(userToken);
            }

            if (_user == null)
            {
                _user = AuthUser();
            }

            if (_user != null)
            {
                ConfigureLeftPanel();
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool TokenExists(string tokenPath)
        {
            return File.Exists(tokenPath);
        }

        private string GetUserToken(string tokenPath)
        {
            return File.ReadAllText(tokenPath);
        }

        private User UserByToken(string userToken)
        {
            return DatabaseContext.Database.Users.FirstOrDefault(user => user.Token == userToken);
        }

        private User AuthUser()
        {
            WindowLogin window = new WindowLogin();

            if (window.ShowDialog() == true)
                return window.User;

            return null;
        }

        private void ConfigureLeftPanel()
        {
            cboxRoles.ItemsSource = _user.Employee.Positions.Select(position => position.Title);
            cboxRoles.SelectedIndex = 0;

            if (_user.Employee.Positions.Count == 1)
            {
                stackPanelRole.Visibility = Visibility.Collapsed;
            }

            listViewTables.ItemsSource = SetTableList(cboxRoles.SelectedItem.ToString());
        }

        private List<string> SetTableList(string roleName)
        {
            List<string> tableNames = new List<string>();

            switch (roleName)
            {
                case "Классный руководитель":
                    tableNames = new List<string>
                    {
                        "Преподаватели",
                        "Предметы",
                        "Студенты",
                        "Уроки",
                        "Экспорт"
                    };
                    break;

                case "Завуч":
                    tableNames = new List<string>
                    {
                        "Преподаватели",
                        "Предметы",
                        "Студенты",
                        "Специальности",
                        "Уроки",
                        "Группы",
                        "Сотрудники"
                    };
                    break;

                case "Директор":
                    tableNames = new List<string>
                    {
                        "Преподаватели",
                        "Предметы",
                        "Студенты",
                        "Специальности",
                        "Группы",
                        "Сотрудники"
                    };
                    break;
            }

            tableNames.Add("Выход");
            return tableNames;
        }

        private void CloseApp(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private async void CheckConnectionServiceStatus()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    if (App.ConnectionService.IsConnected)
                    {
                        SetEllipseColor("successConnectionBrush");
                    }
                    else
                    {
                        SetEllipseColor("failedConnectionBrush");
                    }

                    Thread.Sleep(2000);
                }
            });
        }

        private void SetEllipseColor(string brushName)
        {
            try
            {
                this.Dispatcher.Invoke(() =>
                {
                    Brush brush = (Brush)Application.Current.Resources[brushName];
                    this.ellipseConnectionStatus.Fill = brush;
                });
            }
            catch
            {
                return;
            }
        }

        private void UpdateTableList(object sender, SelectionChangedEventArgs e)
        {
            _userPosition = cboxRoles.SelectedItem.ToString();
            listViewMain.ItemsSource = null;
            listViewTables.SelectedItem = null;
            listViewTables.ItemsSource = SetTableList(_userPosition);
        }

        private void SelectedTableChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateMainList();
        }

        private void UpdateMainList()
        {
            _contextTableName = (string)listViewTables.SelectedItem;
            addButton.Visibility = Visibility.Hidden;
            subtractWeek.Visibility = Visibility.Hidden;
            addWeek.Visibility = Visibility.Hidden;
            dataGrid.Visibility = Visibility.Hidden;
            exportButton.Visibility = Visibility.Hidden;
            listViewMain.Visibility = Visibility.Visible;

            switch (_contextTableName)
            {
                case "Преподаватели":
                    SetTableContext(DatabaseContext.Database.Teachers.ToList());
                    EnableButtons();
                    break;

                case "Предметы":
                    SetTableContext(DatabaseContext.Database.Subjects.ToList());
                    EnableButtons();
                    break;

                case "Студенты":
                    SetTableContext(DatabaseContext.Database.Students.ToList());
                    EnableButtons();
                    break;

                case "Специальности":
                    SetTableContext(DatabaseContext.Database.Specialities.ToList());
                    EnableButtons();
                    break;

                case "Уроки":
                    SetTableContext(DatabaseContext.Database.Lessons.ToList());
                    EnableButtons();
                    break;

                case "Группы":
                    SetTableContext(DatabaseContext.Database.Groups.ToList());
                    EnableButtons();
                    break;

                case "Сотрудники":
                    SetTableContext(DatabaseContext.Database.Employees.ToList());
                    EnableButtons();
                    break;

                case "Должности":
                    SetTableContext(DatabaseContext.Database.Employees.ToList());
                    EnableButtons();
                    break;

                case "Экспорт":
                    dataGrid.ItemsSource = DatabaseContext.Database.Students.Where(student => student.GroupID == _user.Employee.GroupID).ToList();

                    dataGrid.Visibility = Visibility.Visible;
                    listViewMain.Visibility = Visibility.Hidden;
                    exportButton.Visibility = Visibility.Visible;
                    break;

                case "Выход":
                    listViewTables.SelectedIndex = -1;
                    ExitUser();
                    break;
            }
        }

        private void SetTableContext(List<Teacher> teachers)
        {
            listViewMain.ItemTemplate = (DataTemplate)Application.Current.Resources["teacherTemplate"];

            if (_userPosition == "Завуч" || _userPosition == "Директор")
            {
                listViewMain.ItemsSource = teachers;
                return;
            }

            if (_userPosition == "Классный руководитель" && _contextTableName == "Преподаватели")
            {
                teachers.Clear();

                foreach(Lesson lesson in DatabaseContext.Database.Lessons.ToList())
                {
                    if(lesson.GroupID == _user.Employee.GroupID && !teachers.Contains(lesson.Teacher))
                    {
                        teachers.Add(lesson.Teacher);
                    }
                }

                listViewMain.ItemsSource = teachers;
                return;
            }
        }

        private void SetTableContext(List<Subject> subjects)
        {
            listViewMain.ItemTemplate = (DataTemplate)Application.Current.Resources["subjectTemplate"];

            if (_userPosition == "Классный руководитель")
            {
                subjects.Clear();

                foreach(Lesson lesson in DatabaseContext.Database.Lessons.ToList())
                {
                    if (lesson.Group == _user.Employee.Group && !subjects.Contains(lesson.Teacher.Subject))
                    {
                        subjects.Add(lesson.Teacher.Subject);
                    }
                }

                listViewMain.ItemsSource = subjects;
            }
            else
            {
                listViewMain.ItemsSource = subjects;
            }
        }

        private void SetTableContext(List<Student> students)
        {
            listViewMain.ItemTemplate = (DataTemplate)Application.Current.Resources["studentTemplate"];
            
            if (_userPosition == "Классный руководитель")
            {
                listViewMain.ItemsSource = students.Where(student => student.Group == _user.Employee.Group);
            }
            else
            {
                listViewMain.ItemsSource = students;
            }
        }

        private void SetTableContext(List<Speciality> specialities)
        {
            listViewMain.ItemTemplate = (DataTemplate)Application.Current.Resources["specialityTemplate"];

            listViewMain.ItemsSource = specialities;
        }

        private void SetTableContext(List<Lesson> lessons)
        {
            if(_userPosition == "Классный руководитель")
            {
                listViewMain.ItemTemplate = (DataTemplate)Application.Current.Resources["dateTemplate"];

                _contextDate = DateTime.Today;

                List<DateTime> dateTimes = GetWeek(_contextDate);

                listViewMain.ItemsSource = dateTimes;
            }
            else
            {
                listViewMain.ItemTemplate = (DataTemplate)Application.Current.Resources["lessonTemplate"];

                listViewMain.ItemsSource = lessons;
            }
        }

        private void SubtractWeek(object sender, RoutedEventArgs e)
        {
            _contextDate = _contextDate.AddDays(-7);

            List<DateTime> dateTimes = GetWeek(_contextDate);

            listViewMain.ItemsSource = dateTimes;
        }

        private void AddWeek(object sender, RoutedEventArgs e)
        {
            _contextDate = _contextDate.AddDays(7);

            List<DateTime> dateTimes = GetWeek(_contextDate);

            listViewMain.ItemsSource = dateTimes;
        }

        private List<DateTime> GetWeek(DateTime selectedDate)
        {
            List<DateTime> dates = new List<DateTime>();

            switch (selectedDate.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    dates.Add(selectedDate);
                    dates.Add(selectedDate.AddDays(1));
                    dates.Add(selectedDate.AddDays(2));
                    dates.Add(selectedDate.AddDays(3));
                    dates.Add(selectedDate.AddDays(4));

                    return dates;
                case DayOfWeek.Tuesday:
                    dates.Add(selectedDate.AddDays(-1));
                    dates.Add(selectedDate);
                    dates.Add(selectedDate.AddDays(1));
                    dates.Add(selectedDate.AddDays(2));
                    dates.Add(selectedDate.AddDays(3));
                    return dates;
                case DayOfWeek.Wednesday:
                    dates.Add(selectedDate.AddDays(-2));
                    dates.Add(selectedDate.AddDays(-1));
                    dates.Add(selectedDate);
                    dates.Add(selectedDate.AddDays(1));
                    dates.Add(selectedDate.AddDays(2));
                    return dates;
                case DayOfWeek.Thursday:
                    dates.Add(selectedDate.AddDays(-3));
                    dates.Add(selectedDate.AddDays(-2));
                    dates.Add(selectedDate.AddDays(-1));
                    dates.Add(selectedDate);
                    dates.Add(selectedDate.AddDays(1));
                    return dates;
                case DayOfWeek.Friday:
                    dates.Add(selectedDate.AddDays(-4));
                    dates.Add(selectedDate.AddDays(-3));
                    dates.Add(selectedDate.AddDays(-2));
                    dates.Add(selectedDate.AddDays(-1));
                    dates.Add(selectedDate);
                    return dates;
                case DayOfWeek.Saturday:
                    dates.Add(selectedDate.AddDays(-4));
                    dates.Add(selectedDate.AddDays(-3));
                    dates.Add(selectedDate.AddDays(-2));
                    dates.Add(selectedDate.AddDays(-1));
                    dates.Add(selectedDate);
                    return dates;
                case DayOfWeek.Sunday:
                    dates.Add(selectedDate.AddDays(-4));
                    dates.Add(selectedDate.AddDays(-3));
                    dates.Add(selectedDate.AddDays(-2));
                    dates.Add(selectedDate.AddDays(-1));
                    dates.Add(selectedDate);
                    return dates;
            }

            return null;
        }

        private void SetTableContext(List<Group> groups)
        {
            listViewMain.ItemTemplate = (DataTemplate)Application.Current.Resources["groupTemplate"];

            listViewMain.ItemsSource = groups;
        }

        private void SetTableContext(List<Employee> employees)
        {
            if(_contextTableName == "Должности")
            {
                listViewMain.ItemTemplate = (DataTemplate)Application.Current.Resources["positionTemplate"];
            }
            else
            {
                listViewMain.ItemTemplate = (DataTemplate)Application.Current.Resources["employeeTemplate"];
            }

            listViewMain.ItemsSource = employees;
        }

        private void EnableButtons()
        {
            if (_userPosition == "Завуч")
            {
                addButton.Visibility = Visibility.Visible;
                return;
            }

            if(_userPosition == "Директор")
            {
                addButton.Visibility = Visibility.Hidden;
                return;
            }

            if (_userPosition == "Классный руководитель" && _contextTableName == "Студенты")
            {
                addButton.Visibility = Visibility.Visible;
                return;
            }

            if (_contextTableName == "Уроки")
            {
                subtractWeek.Visibility = Visibility.Visible;
                addWeek.Visibility = Visibility.Visible;
                return;
            }
        }

        private void AddEntity(object sender, RoutedEventArgs e)
        {
            if (!App.ConnectionService.IsConnected)
            {
                App.ShowErrorMessage("Проверьте подключение к сети!");
                return;
            }

            switch (_contextTableName)
            {
                case "Сотрудники":
                    OpenEntityAdder(new PageEmployeeEntity());
                    break;
                case "Специальности":
                    OpenEntityAdder(new PageSpecialityEntity());
                    break;
                case "Группы":
                    OpenEntityAdder(new PageGroupEntity());
                    break;
                case "Предметы":
                    OpenEntityAdder(new PageSubjectEntity());
                    break;
                case "Преподаватели":
                    OpenEntityAdder(new PageTeacherEntity());
                    break;
                case "Студенты":
                    OpenEntityAdder(new PageStudentEntity(_userPosition, _user));
                    break;
                case "Уроки":
                    OpenEntityAdder(new PageLessonEntity());
                    break;
            }
        }

        private void ExitUser()
        {
            addButton.Visibility = Visibility.Hidden;
            subtractWeek.Visibility = Visibility.Hidden;
            addWeek.Visibility = Visibility.Hidden;
            listViewMain.ItemsSource = null;

            MessageBoxResult result = MessageBox.Show("Вы действительно хотите выйти из аккаунта?",
                "Подтвердите действие", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
                return;

            this.Hide();

            this._user = null;
            RemoveToken();

            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

        private void RemoveToken()
        {
            if (TokenExists(_tokenPath))
                File.Delete(_tokenPath);
        }

        private void EditEntity(object sender, MouseButtonEventArgs e)
        {
            if (listViewMain.SelectedItem == null || _userPosition == "Директор")
            {
                return;
            }

            switch (_contextTableName)
            {
                case "Преподаватели":
                    if (_userPosition == "Классный руководитель")
                    {
                        
                    }

                    Teacher currentTeacher = (Teacher)listViewMain.SelectedItem;
                    ChangeEntity(currentTeacher);

                    break;

                case "Предметы":
                    if (_userPosition == "Классный руководитель")
                    {
                        
                    }

                    Subject currentSubject = (Subject)listViewMain.SelectedItem;
                    ChangeEntity(currentSubject);

                    break;

                case "Студенты":
                    Student currentStudent = (Student)listViewMain.SelectedItem;
                    ChangeEntity(currentStudent);
                    break;

                case "Специальности":
                    Speciality currentSpeciality = (Speciality)listViewMain.SelectedItem;
                    ChangeEntity(currentSpeciality);
                    break;

                case "Уроки":
                    if (_userPosition == "Классный руководитель")
                    {
                        DateTime currentDate = (DateTime)listViewMain.SelectedItem;
                        ChangeEntity(currentDate, DatabaseContext.Database.Lessons.ToList(), _user);
                    }
                    else
                    {
                        Lesson currentLesson = (Lesson)listViewMain.SelectedItem;
                        ChangeEntity(currentLesson);
                    }
                    break;

                case "Группы":
                    Group group = (Group)listViewMain.SelectedItem;
                    ChangeEntity(group);
                    break;

                case "Сотрудники":
                    Employee currentEmployee = (Employee)listViewMain.SelectedItem;
                    ChangeEntity(currentEmployee);
                    break;
            }
        }

        private void ChangeEntity(DateTime currentDate, List<Lesson> lessons, User user)
        {
            PageDayEntity pageEntity = new PageDayEntity(currentDate, lessons, user);

            OpenEntityAdder(pageEntity);
        }

        private void ChangeEntity(Lesson lesson)
        {
            PageLessonEntity pageEntity = new PageLessonEntity(lesson);
            OpenEntityChanger(pageEntity);
        }

        private void ChangeEntity(Employee employee)
        {
            PageEmployeeEntity pageEntity = new PageEmployeeEntity(employee);
            OpenEntityChanger(pageEntity);
        }

        private void ChangeEntity(Teacher teacher)
        {
            PageTeacherEntity pageEntity = new PageTeacherEntity(teacher);
            OpenEntityChanger(pageEntity);
        }

        private void ChangeEntity(Speciality speciality)
        {
            PageSpecialityEntity pageEntity = new PageSpecialityEntity(speciality);
            OpenEntityChanger(pageEntity);
        }

        private void ChangeEntity(Group group)
        {
            PageGroupEntity pageEntity = new PageGroupEntity(group);
            OpenEntityChanger(pageEntity);
        }

        private void ChangeEntity(Subject subject)
        {
            PageSubjectEntity pageEntity = new PageSubjectEntity(subject);
            OpenEntityChanger(pageEntity);
        }

        private void ChangeEntity(Student student)
        {
            PageStudentEntity pageEntity = new PageStudentEntity(student, _userPosition, _user);
            OpenEntityChanger(pageEntity);
        }

        private void OpenEntityChanger(PageEntity page)
        {
            WindowEntityEditor window = new WindowEntityEditor(page);
            window.RemoveButton.Visibility = Visibility.Visible;
            if (window.ShowDialog() == true)
            {
                UpdateMainList();
            }
        }

        private void OpenEntityAdder(PageEntity page)
        {
            WindowEntityEditor window = new WindowEntityEditor(page);
            
            if(window.ShowDialog() == true)
            {
                UpdateMainList();
            }
        }

        private void ExcelExport(object sender, RoutedEventArgs e)
        {
            string filename = "export.csv";

            this.dataGrid.SelectAllCells();
            this.dataGrid.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
            ApplicationCommands.Copy.Execute(null, this.dataGrid);
            this.dataGrid.UnselectAllCells();

            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Filter = "Файлы CSV|*.csv;|Все файлы(*.*)|*.*";
            fileDialog.Title = "Экспорт списка студентов";
            fileDialog.FileName = filename;
            fileDialog.OverwritePrompt = true;

            if(fileDialog.ShowDialog() == true)
            {
                string result = Clipboard.GetText();
                try
                {
                    StreamWriter sw = new StreamWriter(fileDialog.FileName, true, Encoding.UTF8);
                    sw.WriteLine(result);
                    sw.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
