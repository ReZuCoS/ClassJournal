using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace ClassJournal.ViewModels
{
    internal static class TextBoxValidator
    {
        /// <summary>
        /// Тип валидации данных
        /// </summary>
        public enum ValidationType
        {
            SymbolsAvailability,
            AllowOnlyText,
            AllowOnlyDigits
        }
        /// <summary>
        /// Проверяет текстовое поле выбранным типом валидации
        /// </summary>
        /// <param name="textBox">Текстовое поле</param>
        /// <param name="validationType">Выбранный тип валидации</param>
        /// <returns>Возвращает Tuple<bool, string>, где bool - состояние валидации, а string - возвращаемое сообщение</returns>
        public static Tuple<bool, string> Validate(TextBox textBox, ValidationType validationType)
        {
            if (StringValidator.IsEmpty(textBox.Text))
            {
                return Tuple.Create(false, $"Заполните поле: {textBox.Tag}!");
            }

            if (validationType == ValidationType.AllowOnlyText)
            {
                if (StringValidator.ContainsDigits(textBox.Text) || StringValidator.ContainsSymbols(textBox.Text))
                {
                    return Tuple.Create(false, $"Поле не должно содержать цифр и символов: {textBox.Tag}!");
                }
            }

            if (validationType == ValidationType.AllowOnlyDigits)
            {
                if (StringValidator.ContainsLetters(textBox.Text) || StringValidator.ContainsSymbols(textBox.Text))
                {
                    return Tuple.Create(false, $"Поле не должно содержать букв и символов: {textBox.Tag}!");
                }
            }

            return Tuple.Create(true, "");
        }
        /// <summary>
        /// Проверяет поле пароля выбранным типом валидации
        /// </summary>
        /// <param name="passwordBox">Текстовое поле</param>
        /// <param name="validationType">Выбранный тип валидации</param>
        /// <returns>Возвращает Tuple<bool, string>, где bool - состояние валидации, а string - возвращаемое сообщение</returns>
        public static Tuple<bool, string> Validate(PasswordBox passwordBox, ValidationType validationType)
        {
            if (StringValidator.IsEmpty(passwordBox.Password))
            {
                return Tuple.Create(false, $"Заполните поле: {passwordBox.Tag}!");
            }

            if (validationType == ValidationType.AllowOnlyText)
            {
                if (StringValidator.ContainsDigits(passwordBox.Password) || StringValidator.ContainsSymbols(passwordBox.Password))
                {
                    return Tuple.Create(false, $"Поле не должно содержать цифр и символов: {passwordBox.Tag}!");
                }
            }

            if (validationType == ValidationType.AllowOnlyDigits)
            {
                if (StringValidator.ContainsLetters(passwordBox.Password) || StringValidator.ContainsSymbols(passwordBox.Password))
                {
                    return Tuple.Create(false, $"Поле не должно содержать букв и символов: {passwordBox.Tag}!");
                }
            }

            return Tuple.Create(true, "");
        }
        /// <summary>
        /// Проверяет список текстовых полей выбранным типом валидации
        /// </summary>
        /// <param name="textBox">Текстовое поле</param>
        /// <param name="validationType">Выбранный тип валидации</param>
        /// <returns>Возвращает Tuple<bool, string>, где bool - состояние валидации, а string - возвращаемое сообщение</returns>
        public static Tuple<bool, string> ValidateMany(List<TextBox> textBoxes, ValidationType validationType)
        {
            List<TextBox> unvalidatedTextBoxes = new List<TextBox>();
            string emptyMessage = "Заполните поля:";
            string notOnlyTextMessage = "Поля не должны содержать цифр и символов:";
            string notOnlyDigitsMessage = "Поля не должны содержать букв и символов:";

            foreach (TextBox textBox in textBoxes)
            {
                if (StringValidator.IsEmpty(textBox.Text))
                {
                    unvalidatedTextBoxes.Add(textBox);
                    emptyMessage += $"\n{textBox.Tag}";
                }
            }

            if(unvalidatedTextBoxes.Count > 0)
            {
                return Tuple.Create(false, emptyMessage);
            }

            if (validationType == ValidationType.AllowOnlyText)
            {
                foreach(TextBox textBox in textBoxes)
                {
                    if (StringValidator.ContainsDigits(textBox.Text) || StringValidator.ContainsSymbols(textBox.Text))
                    {
                        unvalidatedTextBoxes.Add(textBox);
                        notOnlyTextMessage += $"\n{textBox.Tag}";
                    }
                }
            }

            if (unvalidatedTextBoxes.Count > 0)
            {
                return Tuple.Create(false, notOnlyTextMessage);
            }

            if (validationType == ValidationType.AllowOnlyDigits)
            {
                foreach (TextBox textBox in textBoxes)
                {
                    if (StringValidator.ContainsLetters(textBox.Text) || StringValidator.ContainsSymbols(textBox.Text))
                    {
                        unvalidatedTextBoxes.Add(textBox);
                        notOnlyDigitsMessage += $"\n{textBox.Tag}";
                    }
                }
            }

            if (unvalidatedTextBoxes.Count > 0)
            {
                return Tuple.Create(false, notOnlyDigitsMessage);
            }

            return Tuple.Create(true, "");
        }
        /// <summary>
        /// Проверяет список полей пароля выбранным типом валидации
        /// </summary>
        /// <param name="textBox">Текстовое поле</param>
        /// <param name="validationType">Выбранный тип валидации</param>
        /// <returns>Возвращает Tuple<bool, string>, где bool - состояние валидации, а string - возвращаемое сообщение</returns>
        public static Tuple<bool, string> ValidateMany(List<PasswordBox> passwordBoxes, ValidationType validationType)
        {
            List<PasswordBox> unvalidatedTextBoxes = new List<PasswordBox>();
            string emptyMessage = "Заполните поля:";
            string notOnlyTextMessage = "Поля не должны содержать цифр и символов:";
            string notOnlyDigitsMessage = "Поля не должны содержать букв и символов:";

            foreach (PasswordBox passwordBox in passwordBoxes)
            {
                if (StringValidator.IsEmpty(passwordBox.Password))
                {
                    unvalidatedTextBoxes.Add(passwordBox);
                    emptyMessage += $"\n{passwordBox.Tag}";
                }
            }

            if (unvalidatedTextBoxes.Count > 0)
            {
                return Tuple.Create(false, emptyMessage);
            }

            if (validationType == ValidationType.AllowOnlyText)
            {
                foreach (PasswordBox passwordBox in passwordBoxes)
                {
                    if (StringValidator.ContainsDigits(passwordBox.Password) || StringValidator.ContainsSymbols(passwordBox.Password))
                    {
                        unvalidatedTextBoxes.Add(passwordBox);
                        notOnlyTextMessage += $"\n{passwordBox.Tag}";
                    }
                }
            }

            if (unvalidatedTextBoxes.Count > 0)
            {
                return Tuple.Create(false, notOnlyTextMessage);
            }

            if (validationType == ValidationType.AllowOnlyDigits)
            {
                foreach (PasswordBox passwordBox in passwordBoxes)
                {
                    if (StringValidator.ContainsLetters(passwordBox.Password) || StringValidator.ContainsSymbols(passwordBox.Password))
                    {
                        unvalidatedTextBoxes.Add(passwordBox);
                        notOnlyDigitsMessage += $"\n{passwordBox.Tag}";
                    }
                }
            }

            if (unvalidatedTextBoxes.Count > 0)
            {
                return Tuple.Create(false, notOnlyDigitsMessage);
            }

            return Tuple.Create(true, "");
        }
    }
}
