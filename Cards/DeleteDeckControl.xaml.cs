using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Cards
{
    /// <summary>
    /// Логика взаимодействия для DeleteControl.xaml
    /// </summary>
    public partial class DeleteDeckControl : UserControl
    {
        private static readonly TimeSpan Duration = TimeSpan.FromSeconds(0.10);
        private readonly string deckName;
        private readonly EditorPage owner;

        public DeleteDeckControl(EditorPage owner, string deckName)
        {
            this.Resources.MergedDictionaries.Add(LoginPage.lang);
            this.deckName = deckName;
            this.owner = owner;
            InitializeComponent();
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
        private void YesButton(object sender, RoutedEventArgs e)
        {
            Decks.DeleteDeck(deckName);
            owner.RemoveDeck();
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
    }
}
