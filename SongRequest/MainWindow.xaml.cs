using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SongRequest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Irc Irc;
        private Bot Bot;
        private const string Path = "info.txt";

        public MainWindow()
        {
            InitializeComponent();
            GetInfoForIrc();
            Bot = new Bot(Irc);
            var UiTimer = new DispatcherTimer();
            UiTimer.Interval = TimeSpan.FromSeconds(1);
            UiTimer.Tick += new EventHandler(UiThread);
            UiTimer.Start();
        }

        private void GetInfoForIrc()
        {

            if (File.Exists(Path))
            {
                using (var file = new StreamReader(Path))
                {
                    var channel = file.ReadLine();
                    var nick = file.ReadLine();
                    var password = file.ReadLine();
                    Irc = new Irc(channel, nick, password);
                }
            }
        }

        private void UiThread(object sender, EventArgs e)
        {
            Bot.Update();
            if(Bot.Requests.Count > 0)
            {
                var request = Bot.Requests.Dequeue();
                var row = new RowDefinition();
                row.Height = new GridLength(25);
                var rowPos = Requests.RowDefinitions.Count;
                Requests.RowDefinitions.Add(row);

                Border border = new Border();
                border.BorderThickness = new Thickness(2);
                border.BorderBrush = Brushes.Black;
                Border border2 = new Border();
                border2.BorderThickness = new Thickness(2);
                border2.BorderBrush = Brushes.Black;

                var song = new TextBlock();
                song.Text = request.Song;
                song.FontSize = 16;
                song.HorizontalAlignment = HorizontalAlignment.Center;
                var user = new TextBlock();
                user.Text = request.User;
                user.FontSize = 16;
                user.HorizontalAlignment = HorizontalAlignment.Center;

                var clear = new Button();
                clear.Content = "Clear";
                clear.Click += ClearOne;

                Requests.Children.Add(clear);
                Grid.SetRow(clear, rowPos);
                Grid.SetColumn(clear, 0);
                Requests.Children.Add(border);
                Grid.SetRow(border, rowPos);
                Grid.SetColumn(border, 1);
                Requests.Children.Add(border2);
                Grid.SetRow(border2, rowPos);
                Grid.SetColumn(border2, 2);
                Requests.Children.Add(user);
                Grid.SetRow(user, rowPos);
                Grid.SetColumn(user, 1);
                Requests.Children.Add(song);
                Grid.SetRow(song, rowPos);
                Grid.SetColumn(song, 2);

            }
        }

        private void ClearOne(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var row = Grid.GetRow(button);
            var stuff = Requests.Children.Cast<UIElement>().Where(x => Grid.GetRow(x) == row && Grid.GetColumn(x) == 1);
            var user = ((TextBlock)stuff.First(x => x is TextBlock)).Text;
            //Bot.RemoveUser(user);
            RemoveRowInfo(row);
            Requests.RowDefinitions.RemoveAt(row);
            //var children = Requests.Children.Cast<UIElement>().Where(x => Grid.GetRow(x) == row);
            //var children2 = children.ToList();
            //Requests.RowDefinitions.RemoveAt(row);
            //var temp = Requests.RowDefinitions;
            //children2.ForEach(x => Requests.Children.Remove(x));
            //var temp = Requests.RowDefinitions[1];
            //Requests.RowDefinitions.Remove(temp);
            //Requests.RowDefinitions.Add(temp);
            UpdateGrid(row);
            Console.WriteLine("test");
        }

        private void UpdateGrid(int row)
        {
            for (int i = row; i < Requests.RowDefinitions.Count; i++)
            {
                var children = Requests.Children.Cast<UIElement>().Where(x => Grid.GetRow(x) == i+1);
                var temp = children.ToList();
                temp.ForEach(x => Grid.SetRow(x, i));

            }
        }

        private void RemoveRowInfo(int row)
        {
            var stuff = Requests.Children.Cast<UIElement>().Where(x => Grid.GetRow(x) == row);
            var temp = stuff.ToList();
            temp.ForEach(x => Requests.Children.Remove(x));
        }

        private void CopyTop_Click(object sender, RoutedEventArgs e)
        {
            if(Requests.RowDefinitions.Count > 1)
            {
                var temp = Requests.Children.Cast<UIElement>().Where(x => Grid.GetRow(x) == 1 && Grid.GetColumn(x) == 2);
                var text = ((TextBlock)temp.First(x => x is TextBlock)).Text;
                Clipboard.SetText(text);
            }
        }

        private void ClearAll_Click(object sender, RoutedEventArgs e)
        {
            //while(Requests.RowDefinitions.Count > 1)
            //{
            for (int i = 1; i < Requests.RowDefinitions.Count; i++)
            {
                RemoveRowInfo(i);
            }
            while(Requests.RowDefinitions.Count > 1)
            {
                Requests.RowDefinitions.RemoveAt(1);
            }
            Bot.ClearUsers();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
            Environment.Exit(0);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
            Environment.Exit(0);
        }
    }
}
