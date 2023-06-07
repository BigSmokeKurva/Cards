using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Animation;

namespace Cards;

public class AnimatedSliderBorder : Border
{
    private static readonly TimeSpan Duration = TimeSpan.FromSeconds(0.15);
    public void AnimatedMove(Thickness newMargin)
    {
        var animation = new ThicknessAnimation
        {
            From = Margin,
            To = newMargin,
            Duration = Duration
        };
        BeginAnimation(MarginProperty, animation);
    }
}
public class AnimatedNotificationLabel : Label
{
    private readonly Grid grid;
    private static readonly TimeSpan Duration = TimeSpan.FromSeconds(0.10);
    private static readonly TimeSpan BeginTime = TimeSpan.FromSeconds(1.5);
    private static readonly Style style = (Style)Application.Current.FindResource("NotificationLabel");
    public AnimatedNotificationLabel(string content, Grid grid)
    {
        this.grid = grid;
        Style = style;
        Content = content;
        Margin = new Thickness(0, 0, 0, -Height);
    }
    public void Show()
    {
        grid.Children.Add(this);

        var animation = new ThicknessAnimation
        {
            From = Margin,
            To = new Thickness(0, 0, 0, 35),
            Duration = Duration
        };
        animation.Completed += (a, e) =>
        {
            var animation = new ThicknessAnimation
            {
                From = Margin,
                To = new Thickness(0, 0, 0, -Height),
                Duration = Duration,
                BeginTime = BeginTime
            };
            animation.Completed += (a, e) => grid.Children.Remove(this);
            BeginAnimation(MarginProperty, animation);
        };
        BeginAnimation(MarginProperty, animation);
    }
}
internal class Card
{
    internal string Word;
    internal string Translation;
    internal bool IsFavorite;
    internal string Id;
    internal int CardNum;
}
internal class Decks
{
    private static readonly Dictionary<string, List<Card>> decks = new();
    private static SqliteConnection connection;
    private static readonly Random rnd = new();
    internal static void ConnectDB()
    {
        //SqliteCommand cmd;
        connection = new SqliteConnection($"Data Source=db.db");
        connection.Open();
        //cmd = connection.CreateCommand();
        //cmd.CommandText = "CREATE TABLE IF NOT EXISTS accounts (token TEXT PRIMARY KEY, login TEXT, password TEXT, streamers TEXT DEFAULT '{}');";
        //cmd.ExecuteNonQuery();
    }
    internal static void ParseDB()
    {
        decks.Clear();
        SqliteCommand cmd;
        SqliteCommand cmd1;
        cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table';";
        cmd.ExecuteNonQuery();
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                string table = reader.GetString(0);
                cmd1 = connection.CreateCommand();
                cmd1.CommandText = $"SELECT * FROM '{table}';";
                cmd1.ExecuteNonQuery();
                decks[table] = new();
                using (var reader1 = cmd1.ExecuteReader())
                {
                    while (reader1.Read())
                    {
                        decks[table].Add(new()
                        {
                            Word = reader1.GetString(0),
                            Translation = reader1.GetString(1),
                            IsFavorite = reader1.GetBoolean(2),
                            Id = reader1.GetString(3),
                            CardNum = reader1.GetInt32(4)
                        });
                    }
                }
            }
        }
    }
    internal static string[] GetDecks() => decks.Keys.ToArray();
    internal static Card[] GetCards(string key) => decks[key].ToArray();
    internal static bool DeckContains(string key) => decks.ContainsKey(key);
    internal static void CreateDeck(string key)
    {
        SqliteCommand cmd;
        decks[key] = new();
        cmd = connection.CreateCommand();
        cmd.CommandText = $"CREATE TABLE '{key}' (word TEXT DEFAULT '', translation TEXT DEFAULT '', isfavorite BOOL DEFAULT false, id TEXT NOT NULL, cardnum INTEGER NOT NULL);";
        cmd.ExecuteNonQuery();
    }
    internal static void RenameDeck(string previousNameDeck, string newNameDeck)
    {
        SqliteCommand cmd;
        decks[newNameDeck] = new();
        decks[newNameDeck].AddRange(decks[previousNameDeck]);
        decks.Remove(previousNameDeck);
        cmd = connection.CreateCommand();
        cmd.CommandText = $"ALTER TABLE '{previousNameDeck}' RENAME TO '{newNameDeck}';";
        cmd.ExecuteNonQuery();
    }
    internal static void DeleteDeck(string key)
    {
        SqliteCommand cmd;
        decks.Remove(key);
        cmd = connection.CreateCommand();
        cmd.CommandText = $"DROP TABLE '{key}';";
        cmd.ExecuteNonQuery();
    }
    internal static int CreateCard(string key)
    {
        SqliteCommand cmd;
        var cardNum = decks[key].Count + 1;
        string id;
        do
        {
            id = rnd.Next(0, 1000000000).ToString();
        } while (decks.Any(x => x.Value.Any(x => x.Id == id)));
        Card card = new()
        {
            Word = string.Empty,
            Translation = string.Empty,
            IsFavorite = false,
            Id = id,
            CardNum = cardNum,
        };
        decks[key].Add(card);
        cmd = connection.CreateCommand();
        cmd.CommandText = $"INSERT INTO '{key}' (id, cardnum) VALUES ('{id}', {cardNum});";
        cmd.ExecuteNonQuery();
        return cardNum;
    }
    internal static void DeleteCard(string key, int cardNum)
    {
        SqliteCommand cmd;

        decks[key].RemoveAt(cardNum - 1);
        for (var i = cardNum - 1; i < decks[key].Count; i++)
            decks[key][i].CardNum--;

        cmd = connection.CreateCommand();
        cmd.CommandText =
            $"DELETE FROM '{key}' WHERE cardnum = {cardNum};" +
            $"UPDATE '{key}' SET cardnum = cardnum - 1 WHERE cardnum > {cardNum};";
        cmd.ExecuteNonQuery();
    }
    internal static Card GetCard(string key, string id) => decks[key].First(x => x.Id == id);
    internal static Card GetCard(string id)
    {
        foreach (var deck in decks.Values)
        {
            foreach (var card in deck)
            {
                if (card.Id == id)
                    return card;
            }
        }
        return null;

    }
    internal static Card GetCard(string key, int num) => decks[key].First(x => x.CardNum == num);
    internal static void SaveCard(string key, Card card)
    {
        SqliteCommand cmd;
        for (var i = 0; i < decks[key].Count; i++)
        {
            if (decks[key][i].Id == card.Id)
            {
                decks[key][i] = card;
                break;
            }
        }
        cmd = connection.CreateCommand();
        cmd.CommandText =
            $"UPDATE '{key}' SET " +
            $"word = '{card.Word}'," +
            $"translation = '{card.Translation}'," +
            $"isfavorite = {card.IsFavorite}," +
            $"cardnum = {card.CardNum} " +
            $"WHERE id = '{card.Id}';";
        cmd.ExecuteNonQuery();
    }
    internal static void SaveCard(Card card)
    {
        SqliteCommand cmd;
        string key = null;
        foreach (var deck in decks.Keys)
        {
            foreach (var _card in decks[deck])
            {
                if (_card.Id == card.Id)
                {
                    key = deck;
                    break;
                }
            }
        }
        cmd = connection.CreateCommand();
        cmd.CommandText =
            $"UPDATE '{key}' SET " +
            $"word = '{card.Word}'," +
            $"translation = '{card.Translation}'," +
            $"isfavorite = {card.IsFavorite}," +
            $"cardnum = {card.CardNum} " +
            $"WHERE id = '{card.Id}';";
        cmd.ExecuteNonQuery();
    }

    internal static Card[] GetFavorites()
    {
        var cards = new List<Card>();
        foreach (var deck in decks.Values)
        {
            cards.AddRange(deck.Where(x => x.IsFavorite));
        }
        return cards.ToArray();
    }
}
internal class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => ((bool)value) ? Visibility.Visible : Visibility.Hidden;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => DependencyProperty.UnsetValue;
}
internal class NullOrNumberToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var flag = (parameter as string) is not null ? (string)parameter switch
        {
            "Visibility.Visible" => Visibility.Visible,
            "Visibility.Hidden" => Visibility.Hidden,
            "Visibility.Collapsed" => Visibility.Collapsed
        } : Visibility.Hidden;
        return value is null || (int)value == -1 ? flag : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => DependencyProperty.UnsetValue;
}
internal class ListViewItemToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is null)
            return string.Empty;
        var content = (string)((ListViewItem)value).Content;
        content = char.ToUpper(content[0]) + content[1..];
        return content;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => DependencyProperty.UnsetValue;
}
public class Configuration
{
    [JsonPropertyName("lang")]
    public string lang { get; set; }
}