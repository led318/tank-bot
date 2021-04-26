using System;
using System.Security.Cryptography.Pkcs;
using System.Text;

namespace FormUICore.Infrastructure
{
    public static class Logger
    {
        private static readonly StringBuilder _stringBuilder = new StringBuilder();

        public static void Append(string line)
        {
            var now = DateTime.Now;
            _stringBuilder.AppendLine($"{now.Hour:00}:{now.Minute:00}:{now.Second:00} - {line}");
        }

        public static string GetLogAndClean()
        {
            var str = _stringBuilder.ToString();
            _stringBuilder.Clear();

            return str;
        }
    }
}
