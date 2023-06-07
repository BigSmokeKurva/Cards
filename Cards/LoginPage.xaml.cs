using System;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace Cards
{
    /// <summary>
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : UserControl
    {
        private readonly MainWindow mainWindow;
        internal static ResourceDictionary lang;
        public LoginPage(MainWindow mainWindow)
        {
            if (!File.Exists("configuration.json"))
                File.WriteAllText("configuration.json", "{\"lang\":\"En\"}");
            var configuration = JsonSerializer.Deserialize<Configuration>(File.ReadAllText("configuration.json"));
            lang = new ResourceDictionary() { Source = new Uri($"..\\Languages\\{configuration.lang}.xaml", UriKind.Relative) };
            this.Resources.MergedDictionaries.Add(lang);

            this.mainWindow = mainWindow;
            InitializeComponent();
            foreach (var i in LangComboBox.Items)
            {
                if ((string)((ComboBoxItem)i).Tag == configuration.lang)
                {
                    LangComboBox.SelectedItem = i;
                    break;
                }
            }

        }

        private void GoToNextPage(object sender, RoutedEventArgs e)
        {
            var pageName = (string)((Button)sender).Tag;
            mainWindow.GoToPage(pageName);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.Resources.MergedDictionaries.Remove(lang);
            var configuration = new Configuration()
            {
                lang = ((ComboBoxItem)((ComboBox)sender).SelectedItem).Content switch
                {
                    "Russian" => "Ru",
                    "English" => "En"
                }
            };
            File.WriteAllText("configuration.json", JsonSerializer.Serialize(configuration));
            lang = new ResourceDictionary() { Source = new Uri($"..\\Languages\\{configuration.lang}.xaml", UriKind.Relative) };
            this.Resources.MergedDictionaries.Add(lang);

        }
    }
}
