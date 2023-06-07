using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Cards
{
    /// <summary>
    /// Логика взаимодействия для EducationPage.xaml
    /// </summary>
    public partial class EducationPage : UserControl
    {
        private readonly List<string> historyCards = new();
        private Card[] cards = Array.Empty<Card>();
        private string? activeCard = null;
        private bool randomCard = false;
        private readonly static Random rnd = new();
        private readonly static BrushConverter bc = new();
        public EducationPage()
        {
            this.Resources.MergedDictionaries.Add(LoginPage.lang);
            InitializeComponent();
            List<ListViewItem> decks = new()
            {
                new() { Content = (string)LoginPage.lang["EducationPage7"], Tag = "F" }
            };
            decks.AddRange(Decks.GetDecks().Select(x => new ListViewItem() { Content = x }));
            EducationList.ItemsSource = decks;
        }
        private Card GetCardNext()
        {
            if (activeCard is not null && historyCards.Contains(activeCard) && historyCards.IndexOf(activeCard) != historyCards.Count - 1)
            {
                activeCard = historyCards[historyCards.IndexOf(activeCard) + 1];
                return this.cards.First(x => x.Id == activeCard);
            }
            if (historyCards.Count == this.cards.Length)
                return null;
            if (activeCard is null)
            {
                var card = this.cards[0];
                activeCard = card.Id;
                return card;
            }
            historyCards.Add(activeCard);
            var cards = Array.FindAll(this.cards, x => !historyCards.Contains(x.Id));
            if (!cards.Any())
                return null;
            activeCard = randomCard ? cards[rnd.Next(0, cards.Length)].Id : cards.First().Id;
            return cards.First(x => x.Id == activeCard);
        }
        private Card GetCardBack()
        {
            if (activeCard is not null && !historyCards.Contains(activeCard))
            {
                historyCards.Add(activeCard);
            }
            if (historyCards.IndexOf(activeCard) == 0)
                return null;
            activeCard = historyCards[historyCards.IndexOf(activeCard) - 1];
            return this.cards.First(x => x.Id == activeCard);

        }
        private void Burger_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            btn.Tag = (string)btn.Tag == "rolled" ? "deployed" : "rolled";
            DoubleAnimation animation;
            if (BurgerBorder.Visibility == Visibility.Hidden)
            {
                BurgerBorder.Visibility = Visibility.Visible;
                animation = new DoubleAnimation()
                {
                    From = BurgerBorder.Opacity,
                    To = 1,
                    Duration = TimeSpan.FromSeconds(0.10),
                };
            }
            else
            {
                animation = new DoubleAnimation()
                {
                    From = BurgerBorder.Opacity,
                    To = 0,
                    Duration = TimeSpan.FromSeconds(0.10),
                };
                animation.Completed += (s, e) => BurgerBorder.Visibility = Visibility.Hidden;
            }
            BurgerBorder.BeginAnimation(OpacityProperty, animation);
        }
        private void EducationList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = EducationList.SelectedItem as ListViewItem;
            historyCards.Clear();
            activeCard = null;
            cards = "F" != (string)item.Tag ? cards = Decks.GetCards((string)item.Content) : cards = Decks.GetFavorites();

            var animation = new DoubleAnimation()
            {
                From = CardBorder.Opacity,
                To = 0,
                Duration = TimeSpan.FromSeconds(0.10),
            };
            if (!cards.Any())
            {
                var animation1 = new DoubleAnimation()
                {
                    From = NoCardsBorder.Opacity,
                    To = 1,
                    Duration = TimeSpan.FromSeconds(0.10),
                };
                NoCardsBorder.BeginAnimation(OpacityProperty, animation1);
            }
            else
            {
                var animation1 = new DoubleAnimation()
                {
                    From = NoCardsBorder.Opacity,
                    To = 0,
                    Duration = TimeSpan.FromSeconds(0.10),
                };
                NoCardsBorder.BeginAnimation(OpacityProperty, animation1);
                var card = GetCardNext();
                animation.Completed += (s, e) =>
                {
                    CardNumLabel.Content = $"№{card.CardNum}";
                    CardFavoritePath.Fill = card.IsFavorite ? (Brush)bc.ConvertFrom("#FFFB7401") : Brushes.Transparent;
                    CardContentTextBlock.Text = card.Word;
                    var animation = new DoubleAnimation()
                    {
                        From = CardBorder.Opacity,
                        To = 1,
                        Duration = TimeSpan.FromSeconds(0.10),
                    };
                    CardBorder.BeginAnimation(OpacityProperty, animation);
                };
            }
            CardBorder.BeginAnimation(OpacityProperty, animation);

        }
        private void RandomCard_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (randomCard)
            {
                btn.Tag = "False";
                randomCard = false;
                return;
            }
            btn.Tag = "True";
            randomCard = true;
        }
        private void NextCard_Click(object sender, RoutedEventArgs e)
        {
            if (!cards.Any())
                return;
            var card = GetCardNext();
            if (card is null)
                return;

            var animation = new DoubleAnimation()
            {
                From = CardBorder.Opacity,
                To = 0,
                Duration = TimeSpan.FromSeconds(0.10),
            };
            animation.Completed += (s, e) =>
            {
                CardNumLabel.Content = $"№{card.CardNum}";
                CardFavoritePath.Fill = card.IsFavorite ? (Brush)bc.ConvertFrom("#FFFB7401") : Brushes.Transparent;
                CardContentTextBlock.Text = card.Word;
                var animation = new DoubleAnimation()
                {
                    From = CardBorder.Opacity,
                    To = 1,
                    Duration = TimeSpan.FromSeconds(0.10),
                };
                CardBorder.BeginAnimation(OpacityProperty, animation);
            };

            CardBorder.BeginAnimation(OpacityProperty, animation);
        }
        private void BackCard_Click(object sender, RoutedEventArgs e)
        {
            if (!this.cards.Any() || activeCard is null)
                return;
            var card = GetCardBack();
            if (card is null)
                return;

            var animation = new DoubleAnimation()
            {
                From = CardBorder.Opacity,
                To = 0,
                Duration = TimeSpan.FromSeconds(0.10),
            };
            animation.Completed += (s, e) =>
            {
                CardNumLabel.Content = $"№{card.CardNum}";
                CardFavoritePath.Fill = card.IsFavorite ? (Brush)bc.ConvertFrom("#FFFB7401") : Brushes.Transparent;
                CardContentTextBlock.Text = card.Word;
                var animation = new DoubleAnimation()
                {
                    From = CardBorder.Opacity,
                    To = 1,
                    Duration = TimeSpan.FromSeconds(0.10),
                };
                CardBorder.BeginAnimation(OpacityProperty, animation);
            };

            CardBorder.BeginAnimation(OpacityProperty, animation);

        }
        private void FavoriteCard_Click(object sender, RoutedEventArgs e)
        {
            if (!this.cards.Any() || activeCard is null)
                return;
            var card = cards.First(x => x.Id == activeCard);
            card.IsFavorite = !card.IsFavorite;
            Decks.SaveCard(card);
            CardFavoritePath.Fill = card.IsFavorite ? (Brush)bc.ConvertFrom("#FFFB7401") : Brushes.Transparent;

        }
        private void FlipCard_Click(object sender, RoutedEventArgs e)
        {
            if (!this.cards.Any() || activeCard is null)
                return;
            var card = cards.First(x => x.Id == activeCard);
            var animation = new DoubleAnimation()
            {
                From = CardContentTextBlock.Opacity,
                To = 0,
                Duration = TimeSpan.FromSeconds(0.10),
            };
            animation.Completed += (s, e) =>
            {
                CardContentTextBlock.Text = CardContentTextBlock.Text == card.Word ? card.Translation : card.Word;
                var animation = new DoubleAnimation()
                {
                    From = CardContentTextBlock.Opacity,
                    To = 1,
                    Duration = TimeSpan.FromSeconds(0.10),
                };
                CardContentTextBlock.BeginAnimation(OpacityProperty, animation);
            };
            CardContentTextBlock.BeginAnimation(OpacityProperty, animation);

        }
    }
}
