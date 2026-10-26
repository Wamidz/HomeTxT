using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Linq;
using System.Text.RegularExpressions;

namespace HomeTxT
{
    public class CensorManager
    {
        private readonly MainWindow _window;
        readonly string[] badWords = { "bad" };
        readonly string replacementWord = "[CENSORED]";

        public CensorManager(MainWindow window)
        {
            _window = window;
        }

        public void ValueCheck()
        {
            string userInput = _window.textField.Text;
            string cleanText = userInput;

            foreach (string word in badWords)
            {
                string pattern = $@"\b{Regex.Escape(word)}\b";
                cleanText = Regex.Replace(cleanText, pattern, replacementWord, RegexOptions.IgnoreCase);
            }

            if (cleanText != userInput)
            {
                _window.textField.Text = cleanText;
                _window.textField.CaretIndex = _window.textField.Text.Length;
            }
        }
    }
}
