using System.IO;
using System.Windows;

namespace HomeTxT
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string filePath = "";
        int egg = 0;

        public MainWindow()
        {
            InitializeComponent();

        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            var fileExplorer = new Microsoft.Win32.OpenFileDialog();

            if (fileExplorer.ShowDialog() == true)
            {
                filePath = fileExplorer.FileName;
                fileIndicator.Text = filePath;

                textField.Text = File.ReadAllText(filePath);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            File.WriteAllText(filePath, textField.Text);
            ShowSavedIndicator();
        }

        private void SaveCopyAsButton_Click(object sender, RoutedEventArgs e)
        {
            var fileExplorer = new Microsoft.Win32.SaveFileDialog();

            if (fileExplorer.ShowDialog() == true)
            {
                string tempFilePath = fileExplorer.FileName;
                File.WriteAllText(tempFilePath, textField.Text);
            }
            ShowSavedIndicator();
        }

        private async void ShowSavedIndicator()
        {
            savedToast.Opacity = 1.0;

            await Task.Delay(1500);

            for (double i = 1.0; i >= 0; i -= 0.1)
            {
                savedToast.Opacity = i;
                await Task.Delay(25);
            }
        }

        private void EasterEgg_Click(object sender, RoutedEventArgs e)
        {
            if (egg >= 10)
            {
                textField.Text = "Was it worth it?";
                egg = 0;
            }
            egg++;
        }
    }
}