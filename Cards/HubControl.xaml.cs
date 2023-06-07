using System.Windows;
using System.Windows.Controls;

namespace Cards
{
    /// <summary>
    /// Логика взаимодействия для HubControl.xaml
    /// </summary>
    public partial class HubControl : UserControl
    {
        private EditorPage? editorPage = null;
        public HubControl(string page)
        {
            this.Resources.MergedDictionaries.Add(LoginPage.lang);
            InitializeComponent();
            Decks.ConnectDB();
            Decks.ParseDB();
            switch (page)
            {
                case "EditorPage":
                    EditorButton_Click(EditorButton, new());
                    break;
                case "EducationPage":
                    EducationButton_Click(EducationButton, new());
                    break;
            }
        }

        private void EducationButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            btn.IsEnabled = false;
            EditorButton.IsEnabled = true;
            SliderBorder.AnimatedMove(new(-12, -8, 81, -9));
            NavigationControl.Content = new EducationPage();
        }

        private void EditorButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            btn.IsEnabled = false;
            EducationButton.IsEnabled = true;
            SliderBorder.AnimatedMove(new(81, -8, -12, -9));
            editorPage ??= new();
            NavigationControl.Content = editorPage;
        }
    }
}
