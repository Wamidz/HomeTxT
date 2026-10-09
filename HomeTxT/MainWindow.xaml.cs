using System.IO;
using System.Media;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HomeTxT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string filePath = "";
        public MainWindow() 
        {
            InitializeComponent();

        }

        private void importButton_Click(object sender, RoutedEventArgs e)
        {
            var fileExplorer = new Microsoft.Win32.OpenFileDialog();

            if (fileExplorer.ShowDialog() == true)
            {
                filePath = fileExplorer.FileName;
                fileIndicator.Text = filePath;

                textField.Text = File.ReadAllText(filePath);
            }
        }
    }
}