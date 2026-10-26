using System;
using System.IO;
using System.Windows;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Collections.Generic;

namespace HomeTxT
{
    public class CensorManager(MainWindow window)
    {
        private readonly MainWindow _window = window;
        private readonly string[] badWords = LoadBadWords();
        private readonly string[] kindWords = LoadKindWords();
        private readonly Random _random = new();

        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true
        };

        private static string[] LoadBadWords()
        {
            string folderName = "Languages";
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folderName);
            List<string> masterWordList = [];

            try
            {
                if (Directory.Exists(folderPath))
                {
                    string[] jsonFiles = Directory.GetFiles(folderPath, "*.json");

                    foreach (string filePath in jsonFiles)
                    {
                        string jsonContent = File.ReadAllText(filePath);
                        string[] words = JsonSerializer.Deserialize<string[]>(jsonContent, SerializerOptions) ?? [];
                        masterWordList.AddRange(words);
                    }

                    return [.. masterWordList.Distinct()];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading language files: {ex.Message}");
            }

            return [];
        }

        private static string[] LoadKindWords()
        {
            string fileName = "kind_words.json";
            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            string[] standardFallback = ["splendid", "fiddlesticks", "fluffernutter"];

            try
            {
                if (File.Exists(fullPath))
                {
                    string jsonContent = File.ReadAllText(fullPath).Trim();

                    if (!string.IsNullOrWhiteSpace(jsonContent))
                    {
                        string[] words = JsonSerializer.Deserialize<string[]>(jsonContent, SerializerOptions) ?? [];

                        if (words.Length > 0)
                        {
                            return words;
                        }
                    }
                }
            }
            catch (JsonException jsonEx)
            {
                System.Diagnostics.Debug.WriteLine($"JSON Parsing issue encountered: {jsonEx.Message}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"General file IO exception encountered: {ex.Message}");
            }

            return standardFallback;
        }

        public void ValueCheck()
        {
            string userInput = _window.textField.Text;
            string cleanText = userInput;

            foreach (string word in badWords)
            {
                string pattern = $@"\b{Regex.Escape(word)}\b";

                cleanText = Regex.Replace(cleanText, pattern, match =>
                {
                    int randomIndex = _random.Next(kindWords.Length);
                    return $"<{kindWords[randomIndex]}>";
                }, RegexOptions.IgnoreCase);
            }

            if (cleanText != userInput)
            {
                _window.textField.Text = cleanText;
                _window.textField.CaretIndex = _window.textField.Text.Length;
            }
        }
    }
}
