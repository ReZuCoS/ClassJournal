using System.Text;
using System.Security.Cryptography;

namespace ClassJournal.ViewModels
{
    internal static class HashGenerator
    {
        /// <summary>
        /// Генерирует SHA256 из заданной строки
        /// </summary>
        /// <param name="text">Строка</param>
        /// <returns></returns>
        internal static string GetSHA256(string text)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));

                StringBuilder builder = new StringBuilder();

                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }
    }
}
