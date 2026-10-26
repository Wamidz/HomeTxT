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
        private readonly HashSet<string> appropriateWords = LoadAppropriateWords(LoadBadWords());
        private readonly Random _random = new();

        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true
        };

        private static string GetProjectRootPath()
        {
            // Navigates up from bin/Debug/net10.0-windows to the project root folder
            return Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\"));
        }

        private static string[] LoadBadWords()
        {
            string folderName = "Languages";
            string folderPath = Path.Combine(GetProjectRootPath(), folderName);
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
            string fullPath = Path.Combine(GetProjectRootPath(), fileName);
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

        private static HashSet<string> LoadAppropriateWords(string[] restrictedWords)
        {
            string fileName = "mit_words.json";
            string fullPath = Path.Combine(GetProjectRootPath(), fileName);
            HashSet<string> safeWords = new(StringComparer.OrdinalIgnoreCase);

            try
            {
                if (File.Exists(fullPath))
                {
                    string jsonContent = File.ReadAllText(fullPath);
                    string[] words = JsonSerializer.Deserialize<string[]>(jsonContent, SerializerOptions) ?? [];
                    safeWords = new HashSet<string>(words, StringComparer.OrdinalIgnoreCase);

                    foreach (string bad in restrictedWords)
                    {
                        safeWords.Remove(bad.Trim());
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading appropriate words: {ex.Message}");
            }

            return safeWords;
        }

        private static string NormalizeText(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            string text = input.ToLower();

            text = text.Replace("0", "o")
                       .Replace("1", "i")
                       .Replace("3", "e")
                       .Replace("4", "a")
                       .Replace("5", "s")
                       .Replace("7", "t")
                       .Replace("8", "b")
                       .Replace("@", "a")
                       .Replace("$", "s")
                       .Replace("!", "i")
                       .Replace("z", "s")
                       .Replace("ph", "f")
                       .Replace("ck", "k");

            text = Regex.Replace(text, @"[^a-z0-9]", "");

            if (string.IsNullOrEmpty(text)) return text;

            System.Text.StringBuilder collapsed = new();
            collapsed.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (text[i] != text[i - 1])
                {
                    collapsed.Append(text[i]);
                }
            }
            return collapsed.ToString();
        }

        private static string RunEasterEggs(string input)
        {
            string pattern = @"\b(vim|neovim|nano|emacs|notepad\+\+|sublime(?:\s*text)?|visual\s*studio(?:\s*code)?|vscode|atom|eclipse|intellij|notepad|wordpad|text\s+editor|ide)\b";
            return Regex.Replace(input, pattern, "HomeTxT", RegexOptions.IgnoreCase);
        }

        private static int GetLevenshteinDistance(string source, string target)
        {
            if (string.IsNullOrEmpty(source)) return target?.Length ?? 0;
            if (string.IsNullOrEmpty(target)) return source.Length;

            int[,] distance = new int[source.Length + 1, target.Length + 1];

            for (int i = 0; i <= source.Length; distance[i, 0] = i++) { }
            for (int j = 0; j <= target.Length; distance[0, j] = j++) { }

            for (int i = 1; i <= source.Length; i++)
            {
                for (int j = 1; j <= target.Length; j++)
                {
                    int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;
                    distance[i, j] = Math.Min(
                        Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1),
                        distance[i - 1, j - 1] + cost);
                }
            }
            return distance[source.Length, target.Length];
        }

        public void ValueCheck()
        {
            string userInput = _window.textField.Text;
            string cleanText = RunEasterEggs(userInput);

            string[] tokens = Regex.Split(cleanText, @"(\s+)");

            for (int i = 0; i < tokens.Length; i++)
            {
                string token = tokens[i];
                string strippedToken = Regex.Replace(token, @"[^\w]", "").Trim();

                if (string.IsNullOrWhiteSpace(strippedToken)) continue;

                if (appropriateWords.Count > 0 &&
                    (appropriateWords.Contains(strippedToken) || appropriateWords.Contains(NormalizeText(strippedToken))))
                {
                    continue;
                }

                string normalizedToken = NormalizeText(token);
                if (string.IsNullOrEmpty(normalizedToken)) continue;

                bool shouldCensor = false;

                foreach (string word in badWords)
                {
                    string targetWord = word.Trim().ToLower();
                    if (string.IsNullOrWhiteSpace(targetWord)) continue;

                    string normalizedTarget = NormalizeText(targetWord);
                    if (string.IsNullOrEmpty(normalizedTarget)) continue;

                    bool isMatch = (normalizedToken == normalizedTarget);

                    if (!isMatch && normalizedTarget.Length >= 4 && normalizedToken.Length >= 4)
                    {
                        if (Math.Abs(normalizedToken.Length - normalizedTarget.Length) <= 1)
                        {
                            isMatch = GetLevenshteinDistance(normalizedToken, normalizedTarget) <= 1;
                        }
                    }

                    string pattern = $@"\b{string.Join(@"[\s\-_*\.,!]*", targetWord.Select(c => Regex.Escape(c.ToString())))}?\b";

                    if (Regex.IsMatch(token, pattern, RegexOptions.IgnoreCase) || isMatch)
                    {
                        shouldCensor = true;
                        break;
                    }
                }

                if (shouldCensor)
                {
                    int randomIndex = _random.Next(kindWords.Length);
                    tokens[i] = $"<{kindWords[randomIndex]}>";
                }
            }

            cleanText = string.Concat(tokens);

            if (cleanText != userInput)
            {
                _window.textField.Text = cleanText;
                _window.textField.CaretIndex = _window.textField.Text.Length;
            }
        }
    }
}