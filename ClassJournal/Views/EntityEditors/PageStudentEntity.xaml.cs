using ClassJournal.Models;
using ClassJournal.ViewModels;
using System;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace ClassJournal.Views.EntityEditors
{
    public partial class PageStudentEntity : PageEntity
    {
        private readonly Student _student;
        private readonly User _user;
        private readonly string _userPosition;

        public PageStudentEntity(string userPosition, User user)
        {
            InitializeComponent();
            _user = user;
            _userPosition = userPosition;
            if(userPosition == "Классный руководитель")
            {
                commentgrid.Visibility = Visibility.Visible;
                cboxGroup.Visibility = Visibility.Collapsed;
            }
            
            cboxGroup.ItemsSource = DatabaseContext.Database.Groups.ToList();
            _student = new Student();
            DatabaseContext.Database.Students.Add(_student);
            this.Title = "Добавление студента";
        }

        public PageStudentEntity(Student student, string userPosition, User user)
        {
            InitializeComponent();
            _user = user;
            _userPosition = userPosition;
            if (userPosition == "Классный руководитель")
            {
                commentgrid.Visibility = Visibility.Visible;
                cboxGroup.Visibility = Visibility.Collapsed;
            }

            cboxGroup.ItemsSource = DatabaseContext.Database.Groups.ToList();
            this._student = student;
            this.Title = "Изменение студента";

            txtboxName.Text = _student.Name;
            txtboxSurname.Text = _student.Surname;
            txtboxPatronymic.Text = _student.Patronymic;
            cboxGroup.SelectedItem = _student.Group;
            txtboxComment.Text = _student.Comment;
            txtboxAddress.Text = _student.Address;
            txtboxEmail.Text = _student.Email;
            txtboxPhone.Text = _student.Phone;
            txtboxStudentTicketID.Text = _student.StudentTicketID.ToString();
            cboxGroup.SelectedItem = _student.Group;
            dateTimeEnrollment.SelectedDate = _student.EnrollmentDate;
            if (_student.Image != null)
            {
                studentPhoto.Source = DatabaseImageConverter.ToBitmapImage(_student.Image);
            }
        }

        public override bool EntityRemoved()
        {
            MessageBoxResult result = MessageBox.Show($"Вы действительно хотите удалить студента " +
                $"{_student.Surname} {_student.Name} {_student.Patronymic}?",
                "Удаление студента", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                DatabaseContext.Database.Students.Remove(_student);
                DatabaseContext.SaveDatabase();
                return true;
            }

            return false;
        }

        public override bool EntitySaved()
        {
            if (!EntityValidated())
                return false;

            AppendStudentInfo();

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

            if (_userPosition == "Классный руководитель")
            {
                cboxGroup.SelectedItem = _user.Employee.Group;
            }

            List<TextBox> onlyTextBoxes = new List<TextBox>()
            {
                txtboxSurname,
                txtboxName
            };

            List<TextBox> onlyDigitsTextBoxes = new List<TextBox>()
            {
                txtboxPhone,
                txtboxStudentTicketID
            };

            Tuple<bool, string> tupleText = TextBoxValidator.ValidateMany(onlyTextBoxes, TextBoxValidator.ValidationType.AllowOnlyText);
            Tuple<bool, string> tupleDigits = TextBoxValidator.ValidateMany(onlyDigitsTextBoxes, TextBoxValidator.ValidationType.AllowOnlyDigits);

            if (tupleText.Item1 == false)
            {
                App.ShowErrorMessage(tupleText.Item2);
                return false;
            }

            if (tupleDigits.Item1 == false)
            {
                App.ShowErrorMessage(tupleDigits.Item2);
                return false;
            }

            if (StringValidator.LengthLessThan(txtboxSurname.Text, 3))
            {
                App.ShowErrorMessage("Поле Фамилия имеет длину от 3 до 50 символов!");
                return false;
            }

            if (StringValidator.LengthLessThan(txtboxName.Text, 2))
            {
                App.ShowErrorMessage("Поле Имя имеет длину от 2 до 50 символов!");
                return false;
            }

            if (StringValidator.LengthLessThan(txtboxPhone.Text, 11))
            {
                App.ShowErrorMessage("Поле Телефон имеет длину 11 символов!");
                return false;
            }

            if (dateTimeEnrollment.SelectedDate > DateTime.Today)
            {
                App.ShowErrorMessage("Дата зачисления не может быть позже сегодняшней даты!");
                return false;
            }

            if (cboxGroup.SelectedItem == null)
            {
                App.ShowErrorMessage("Выберите группу студента!");
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
            bool ticketExists = IsStudentTicketExists(DatabaseContext.Database.Students.ToList());

            if (ticketExists)
            {
                App.ShowErrorMessage("Номер билета уже зарегистрирован в системе!");
                return true;
            }

            bool phoneExists = IsStudentPhoneExists(DatabaseContext.Database.Students.ToList());

            if (phoneExists)
            {
                App.ShowErrorMessage("Номер телефона уже зарегистрирован в системе!");
                return true;
            }

            return false;
        }

        private bool IsStudentTicketExists(List<Student> students)
        {
            return students.Exists(student => student.StudentTicketID.ToString() == txtboxStudentTicketID.Text);
        }

        private bool IsStudentPhoneExists(List<Student> students)
        {
            return students.Exists(student => student.Phone == txtboxPhone.Text);
        }

        private void ChangeImage(object sender, RoutedEventArgs e)
        {
            string fileName = GetFileName();

            if (fileName == null)
            {
                return;
            }

            try
            {
                BitmapImage image = new BitmapImage(new Uri(fileName));
                _student.Image = DatabaseImageConverter.ToByteArray(image);
                studentPhoto.Source = DatabaseImageConverter.ToBitmapImage(_student.Image);
            }
            catch
            {
                App.ShowErrorMessage("Неверный формат файла!");
                return;
            }
        }

        private string GetFileName()
        {
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                Title = "Выберите фото",
                InitialDirectory = $@"C:\Users\{Environment.UserName}\Desktop",
                Filter = "Файлы JPG, PNG|*.jpg; *.png;|Все файлы(*.*)|*.*"
            };

            if (fileDialog.ShowDialog() == true)
            {
                return fileDialog.FileName;
            }

            return null;
        }

        private void AppendStudentInfo()
        {
            _student.Surname = txtboxSurname.Text;
            _student.Name = txtboxName.Text;
            _student.Patronymic = txtboxPatronymic.Text;
            _student.EnrollmentDate = dateTimeEnrollment.SelectedDate.Value;
            _student.StudentTicketID = int.Parse(txtboxStudentTicketID.Text);
            _student.Phone = txtboxPhone.Text;
            _student.Address = txtboxAddress.Text;
            _student.Email = txtboxEmail.Text;
            _student.Comment = txtboxComment.Text;
            _student.Group = (Group)cboxGroup.SelectedItem;

            if (_student.Image == null)
            {
                BitmapImage defaultImage = (BitmapImage)Application.Current.Resources["emptyPhoto"];

                _student.Image = DatabaseImageConverter.ToByteArray(defaultImage);
            }
        }
    }
}
