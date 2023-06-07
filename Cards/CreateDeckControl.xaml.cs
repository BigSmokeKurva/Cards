using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Cards
{
    /// <summary>
    /// Логика взаимодействия для CreateDeckControl.xaml
    /// </summary>
    public partial class CreateDeckControl : UserControl
    {
        private static readonly TimeSpan Duration = TimeSpan.FromSeconds(0.10);
        private readonly EditorPage owner;
        private readonly bool isCreate;
        private readonly string deckName;
        public CreateDeckControl(EditorPage owner)
        {
            this.Resources.MergedDictionaries.Add(LoginPage.lang);
            this.owner = owner;
            this.isCreate = true;
            InitializeComponent();
            // (string)LoginPage.lang["EditorPage9"]
            MainLabel.Content = (string)LoginPage.lang["CreateDeckControl2"];
            MainButton.Content = (string)LoginPage.lang["CreateDeckControl3"];
        }
        public CreateDeckControl(EditorPage owner, string deckName)
        {
            this.owner = owner;
            this.isCreate = false;
            this.deckName = deckName;
            InitializeComponent();
            MainLabel.Content = (string)LoginPage.lang["CreateDeckControl4"];
            MainButton.Content = (string)LoginPage.lang["CreateDeckControl5"];
            NameTextBox.Text = deckName;
        }

        internal void Show()
        {
            owner.MainGrid.Children[0].IsHitTestVisible = false;
            owner.MainGrid.Children.Add(this);
            var animation = new DoubleAnimation()
            {
                From = Opacity,
                To = 1,
                Duration = Duration,
            };
            BeginAnimation(OpacityProperty, animation);
        }

        private void CloseButton(object sender, RoutedEventArgs e)
        {
            var animation = new DoubleAnimation()
            {
                From = Opacity,
                To = 0,
                Duration = Duration,
            };
            animation.Completed += (a, e) =>
            {
                owner.MainGrid.Children[0].IsHitTestVisible = true;
                owner.MainGrid.Children.Remove(this);
            };
            BeginAnimation(OpacityProperty, animation);
        }

        private void SaveButton(object sender, RoutedEventArgs e)
        {
            var deckName = NameTextBox.Text;
            if (isCreate)
            {
                if (Decks.DeckContains(deckName) || string.IsNullOrEmpty(deckName) || string.IsNullOrWhiteSpace(deckName))
                {
                    NameTextBox.BorderThickness = new(1);
                    return;
                }
                Decks.CreateDeck(deckName);
                NameTextBox.BorderThickness = new(0);
                owner.UpdateDecks();
            }
            else
            {
                if (string.IsNullOrEmpty(deckName) || string.IsNullOrWhiteSpace(deckName))
                {
                    NameTextBox.BorderThickness = new(1);
                    return;
                }
                if (deckName != this.deckName)
                {
                    if (Decks.DeckContains(deckName))
                    {
                        NameTextBox.BorderThickness = new(1);
                        return;
                    }
                    owner.RenameDeck(this.deckName, deckName);
                    Decks.RenameDeck(this.deckName, deckName);
                }
            }
            var animation = new DoubleAnimation()
            {
                From = Opacity,
                To = 0,
                Duration = Duration,
            };
            animation.Completed += (a, e) =>
            {
                owner.MainGrid.Children[0].IsHitTestVisible = true;
                owner.MainGrid.Children.Remove(this);
            };
            BeginAnimation(OpacityProperty, animation);
        }

        private void NameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SaveButton(sender, new());
        }
    }
}
