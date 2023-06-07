using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Cards
{
    /// <summary>
    /// Логика взаимодействия для EditorPage.xaml
    /// </summary>

    public partial class EditorPage : UserControl
    {
        public EditorPage()
        {
            this.Resources.MergedDictionaries.Add(LoginPage.lang);
            InitializeComponent();
            string[] decks = Decks.GetDecks();
            if (decks.Any())
            {
                DecksListView.ItemsSource = decks.Select(x => new ListViewItem() { Content = x });
                DecksListView.SelectedIndex = 0;
                //Card[] cards = Decks.GetCards((string)((ListViewItem)DecksListView.SelectedItem).Content);
                //if(cards.Any())
                //{
                //    CardsListView.ItemsSource = cards.Select(x => new ListViewItem() { Content = $"карта №{x.CardNum}", Tag = x.Id });
                //    CardsListView.SelectedIndex = 0;
                //}
            }
        }
        internal void UpdateDecks()
        {
            string[] decks = Decks.GetDecks();
            if (decks.Length > 1)
            {
                var selectedIndex = DecksListView.SelectedIndex;
                DecksListView.ItemsSource = decks.Select(x => new ListViewItem() { Content = x });
                DecksListView.SelectedIndex = selectedIndex;
                return;
            }
            else
            {
                DecksListView.ItemsSource = decks.Select(x => new ListViewItem() { Content = x });
                DecksListView.SelectedIndex = 0;
            }
        }
        internal void RenameDeck(string previousNameDeck, string newNameDeck)
        {
            foreach (ListViewItem i in DecksListView.Items)
            {
                if ((string)i.Content == previousNameDeck)
                    i.Content = newNameDeck;
            }
        }
        internal void RemoveDeck()
        {
            var selectedindex = DecksListView.SelectedIndex;
            var newIndex = -1;
            if (DecksListView.Items.Count != 0)
            {
                if (selectedindex == 0)
                    newIndex = 0;
                else if (selectedindex == DecksListView.Items.Count - 1)
                    newIndex = DecksListView.Items.Count - 2;
                else
                    newIndex = DecksListView.SelectedIndex - 1;
            }
            string[] decks = Decks.GetDecks();
            CardsListView.ItemsSource = Array.Empty<ListViewItem>();
            DecksListView.ItemsSource = decks.Select(x => new ListViewItem() { Content = x });
            DecksListView.SelectedIndex = newIndex;
            new AnimatedNotificationLabel((string)LoginPage.lang["EditorPage8"], MainGrid).Show();
        }
        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e) => Keyboard.Focus((UserControl)sender);
        private void SaveButton(object sender, RoutedEventArgs e)
        {
            var id = (string)((ListViewItem)CardsListView.SelectedItem).Tag;
            var deck = (string)((ListViewItem)DecksListView.SelectedItem).Content;
            var card = Decks.GetCard(deck, id);
            card.Word = WordTextBox.Text;
            card.Translation = TranslationTextBox.Text;
            Decks.SaveCard(deck, card);
            new AnimatedNotificationLabel((string)LoginPage.lang["EditorPage9"], MainGrid).Show();
        }
        private void CreateDeckButton(object sender, RoutedEventArgs e)
        {
            new CreateDeckControl(this).Show();
        }
        private void EditDeckButton(object sender, RoutedEventArgs e)
        {
            new CreateDeckControl(this, (string)((ListViewItem)DecksListView.SelectedItem).Content).Show();
        }
        private void DeleteDeckButton(object sender, RoutedEventArgs e)
        {
            new DeleteDeckControl(this, (string)((ListViewItem)DecksListView.SelectedItem).Content).Show();
        }
        private void CreateCardButton(object sender, RoutedEventArgs e)
        {
            var deckName = (string)((ListViewItem)DecksListView.SelectedItem).Content;
            var index = Decks.CreateCard(deckName) - 1;
            Card[] cards = Decks.GetCards(deckName);
            CardsListView.ItemsSource = cards.Select(x => new ListViewItem() { Content = $"{((string)LoginPage.lang["EditorPage10"]).ToLower()} №{x.CardNum}", Tag = x.Id });
            CardsListView.SelectedIndex = index;
        }
        private void DeleteCardButton(object sender, RoutedEventArgs e)
        {
            var deckName = (string)((ListViewItem)DecksListView.SelectedItem).Content;
            var cardNum = CardsListView.SelectedIndex + 1;
            Decks.DeleteCard(deckName, cardNum);
            Card[] cards = Decks.GetCards(deckName);

            var selectedindex = CardsListView.SelectedIndex;
            var newIndex = -1;
            if (CardsListView.Items.Count != 0)
            {
                if (selectedindex == 0)
                    newIndex = 0;
                else if (selectedindex == CardsListView.Items.Count - 1)
                    newIndex = CardsListView.Items.Count - 2;
                else
                    newIndex = CardsListView.SelectedIndex - 1;
            }
            CardsListView.ItemsSource = cards.Select(x => new ListViewItem() { Content = $"{((string)LoginPage.lang["EditorPage10"]).ToLower()} №{x.CardNum}", Tag = x.Id });
            CardsListView.SelectedIndex = newIndex;
            new AnimatedNotificationLabel((string)LoginPage.lang["EditorPage8"], MainGrid).Show();
        }
        private void DecksListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DecksListView.SelectedItem is null)
                return;
            var cards = Decks.GetCards((string)((ListViewItem)DecksListView.SelectedItem).Content).Select(x => new ListViewItem() { Content = $"{((string)LoginPage.lang["EditorPage10"]).ToLower()} №{x.CardNum}", Tag = x.Id });
            CardsListView.ItemsSource = cards;
            if (cards.Any())
                CardsListView.SelectedIndex = 0;
        }
        private void CardsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WordTextBox.Clear();
            TranslationTextBox.Clear();
            if (CardsListView.SelectedItem is null)
                return;
            var id = (string)((ListViewItem)CardsListView.SelectedItem).Tag;
            var deck = (string)((ListViewItem)DecksListView.SelectedItem).Content;
            var card = Decks.GetCard(deck, id);
            WordTextBox.Text = card.Word;
            TranslationTextBox.Text = card.Translation;
        }
    }
}
