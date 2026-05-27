using Microsoft.Win32;
using System.IO;
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

namespace libilabirintus
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Maze maze;
        private Player player;  
        
        public MainWindow()
        {
            InitializeComponent();
            
            Database.Init();
            
            
            //Database.SaveMaze(new Maze("ds"));
            appendMapList(Database.GetAllMazes());
            
        }

        
        
        void drawMap(Maze maze)
        {
            if (maze == null) return;

            citygrid.RowDefinitions.Clear();
            citygrid.ColumnDefinitions.Clear();
            citygrid.Children.Clear();

            char[,] map = maze.Map;

            int rows = maze.Row;
            int columns = maze.Column;

            double maxWidth = 1300;
            double maxHeight = 545;

            double charWidthRatio = 0.55;

            double fontSize = Math.Min(
                maxHeight / rows,
                maxWidth / (columns * charWidthRatio)
            );

            double cellWidth = fontSize * charWidthRatio;
            double cellHeight = fontSize;

            citygrid.Width = columns * cellWidth;
            citygrid.Height = rows * cellHeight;

            for (int row = 0; row < rows; row++)
            {
                citygrid.RowDefinitions.Add(new RowDefinition
                {
                    Height = new GridLength(cellHeight)
                });
            }

            for (int col = 0; col < columns; col++)
            {
                citygrid.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(cellWidth)
                });
            }

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {

                    if (map[col, row] == '.') map[col, row] = ' ';

                    Label label = new()
                    {
                        Content = map[col, row],
                        FontSize = fontSize,
                        FontFamily = new FontFamily("Consolas"),
                        Padding = new Thickness(0),
                        Margin = new Thickness(0),
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        VerticalContentAlignment = VerticalAlignment.Center
                    };

                    Grid.SetRow(label, row);
                    Grid.SetColumn(label, col);

                    citygrid.Children.Add(label);
                }
            }
        }       
        void drawPreMap(Maze maze)
        {
            if (maze == null)
            {
                return;
            }

            preGrid.RowDefinitions.Clear();
            preGrid.ColumnDefinitions.Clear();
            preGrid.Children.Clear();

            char[,] Map = maze.Map;

            int columns = maze.Column;
            int rows = maze.Row;

            double maxWidth = 1000.0;
            double maxHeight = 800.0;

            // en ra nem jottem volna magamtol
            double charWidthRatio = 0.55;

            double fontSizeByHeight = maxHeight / rows;
            double fontSizeByWidth = maxWidth / (columns * charWidthRatio);

            double fontSize = Math.Min(fontSizeByHeight, fontSizeByWidth);

            double cellWidth = fontSize * charWidthRatio;
            double cellHeight = fontSize;

            preGrid.Width = columns * cellWidth;
            preGrid.Height = rows * cellHeight;

            preGrid.HorizontalAlignment = HorizontalAlignment.Center;
            preGrid.VerticalAlignment = VerticalAlignment.Center;

            for (int row = 0; row < rows; row++)
            {
                preGrid.RowDefinitions.Add(new RowDefinition
                {
                    Height = new GridLength(cellHeight)
                });
            }

            for (int col = 0; col < columns; col++)
            {
                preGrid.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(cellWidth)
                });
            }

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    if (Map[col, row] == '.')
                    {
                        continue;
                    }

                    Label label = new()
                    {
                        Content = Map[col, row],
                        FontSize = fontSize,
                        FontFamily = new FontFamily("Consolas"),
                        Padding = new Thickness(0),
                        Margin = new Thickness(0),
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch
                    };

                    Grid.SetRow(label, row);
                    Grid.SetColumn(label, col);

                    preGrid.Children.Add(label);
                }
            }
        }
        
        private void Game_OnKeyDown(object sender, KeyEventArgs e)
        {
            
            if (e.Key == Key.Enter) return;
            if (InGameGrid.Visibility != Visibility.Visible) return;
            if (e.IsRepeat) return;

            Console.WriteLine(e.Key);
            player.Move(e.Key, maze);
           
        }

        private void CreatePlayerButton(object sender, RoutedEventArgs e)
        {
            Database.CreatePlayer(CreateUserNameBox.Text, CreatePasswordBox.Password);
            LoginGrid.Visibility = Visibility.Visible;
            CreatePlayerGrid.Visibility = Visibility.Collapsed;
        }
        
        private void LoginButton(object sender, RoutedEventArgs e)
        {
            if (Database.CheckLogin(LoginUsernameBox.Text, LoginPasswordBox.Password))
            {
                player = Database.GetPlayerByName(LoginUsernameBox.Text);
            
                LoginGrid.Visibility = Visibility.Collapsed;
                SelectorGrid.Visibility = Visibility.Visible;
            }

        }

        private void GoToCreatePlayerButton(object sender, RoutedEventArgs e)
        {
            LoginGrid.Visibility = Visibility.Collapsed;
            CreatePlayerGrid.Visibility = Visibility.Visible;
        }

        void appendMapList(List<Maze> mazes)
        {
            foreach (var maze in mazes)
            {
                SelectionListBox.Items.Add(maze.Name);
            }
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            drawPreMap(Database.getMazeByName(SelectionListBox.SelectedItem.ToString()));
            Console.WriteLine(player.GetPlayer().Name);
        }

        private void SelectEnter(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            string selectedMazeName = SelectionListBox.SelectedItem.ToString();
            Console.WriteLine(selectedMazeName);
            
            InGameGrid.Visibility = Visibility.Visible;
            SelectorGrid.Visibility = Visibility.Collapsed;
            
            drawMap(Database.getMazeByName(selectedMazeName));
        }

    }
}