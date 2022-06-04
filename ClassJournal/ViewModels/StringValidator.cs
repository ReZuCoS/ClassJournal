using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassJournal.ViewModels
{
    internal static class StringValidator
    {
        /// <summary>
        /// Проверяет условие, при котором длина строки больше указанного параметра
        /// </summary>
        /// <param name="text">Строка</param>
        /// <param name="minLength">Минимальная длина строки</param>
        /// <returns>Возвращает значение true, если условие истинно</returns>
        internal static bool LengthLessThan(string text, int minLength)
        {
            if (text.Length <= minLength - 1)
                return true;

            return false;
        }
        /// <summary>
        /// Проверяет, является ли строка пустой
        /// </summary>
        /// <param name="text">Строка</param>
        /// <returns>Возвращает значение true, если условие истинно</returns>
        internal static bool IsEmpty(string text)
        {
            if (text == "")
                return true;

            return false;
        }
        /// <summary>
        /// Проверяет наличие символов в строке
        /// </summary>
        /// <param name = "text" > Строка </ param >
        /// < returns >Возвращает значение true, если условие истинно</ returns >
        internal static bool ContainsSymbols(string text)
        {
            foreach (char c in text)
            {
                if (!char.IsLetterOrDigit(c))
                    return true;
            }

            return false;
        }
        /// <summary>
        /// Проверяет наличие цифр в строке
        /// </summary>
        /// <param name="text">Строка</param>
        /// <returns>Возвращает значение true, если условие истинно</returns>
        internal static bool ContainsDigits(string text)
        {
            bool flag = false;

            foreach (char c in text)
            {
                if (char.IsDigit(c))
                    return true;
            }

            return flag;
        }
        /// <summary>
        /// Проверяет наличие букв в строке
        /// </summary>
        /// <param name="text">Строка</param>
        /// <returns>Возвращает значение true, если условие истинно</returns>
        internal static bool ContainsLetters(string text)
        {
            bool flag = false;

            foreach (char c in text)
            {
                if (char.IsLetter(c))
                    return true;
            }

            return flag;
        }
    }
}
