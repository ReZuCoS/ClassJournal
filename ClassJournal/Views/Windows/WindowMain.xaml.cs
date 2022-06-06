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

namespace ClassJournal.Views.Windows
{
    public partial class WindowMain : Window
    {
        private User _user = null;
        private string _contextTableName;
        private string _userPosition;
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
                        "Недельный табель"
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
                        "Группы"
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

        private void UpdateMainList(object sender, SelectionChangedEventArgs e)
        {
            _contextTableName = (string)listViewTables.SelectedItem;
            addButton.Visibility = Visibility.Hidden;
            subtractWeek.Visibility = Visibility.Hidden;
            addWeek.Visibility = Visibility.Hidden;

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

                case "Выход":
                    listViewTables.SelectedIndex = -1;
                    ExitUser();
                    break;
            }
        }

        private void SetTableContext(List<Teacher> teachers)
        {
            listViewMain.ItemTemplate = (DataTemplate)Application.Current.Resources["teacherTemplate"];

            if (_userPosition == "Завуч")
            {
                listViewMain.ItemsSource = teachers;
                return;
            }

            if (_userPosition == "Классный руководитель" && _contextTableName == "Преподаватели")
            {
                teachers.Clear();

                foreach(Lesson lesson in DatabaseContext.Database.Lessons.ToList())
                {
                    if(lesson.GroupID == _user.Employee.GroupID)
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
            //throw new NotImplementedException();
        }

        private void SetTableContext(List<Student> students)
        {
            //throw new NotImplementedException();
        }

        private void SetTableContext(List<Speciality> specialities)
        {
            //throw new NotImplementedException();
        }

        private void SetTableContext(List<Lesson> lessons)
        {
            //throw new NotImplementedException();
        }

        private void SetTableContext(List<Group> groups)
        {
            //throw new NotImplementedException();
        }

        private void SetTableContext(List<Employee> employees)
        {
            listViewMain.ItemTemplate = (DataTemplate)Application.Current.Resources["employeeTemplate"];

            listViewMain.ItemsSource = employees;
        }

        private void EnableButtons()
        {
            if (_userPosition == "Завуч")
            {
                addButton.Visibility = Visibility.Visible;
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
            switch (_contextTableName)
            {
                case "Сотрудники":
                    OpenEntityEditor(new PageEmployeeEntity());
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

        private void OpenEntityEditor(PageEntity page)
        {
            WindowEntityEditor window = new WindowEntityEditor(page);
            window.ShowDialog();
        }
    }
}
