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

        void drawMap(char[,] Map, int rows, int columns)
        {
            
            citygrid.RowDefinitions.Clear();
            citygrid.ColumnDefinitions.Clear();
            citygrid.Children.Clear();

            for (int i = 0; i < rows; i++)
            {
                citygrid.RowDefinitions.Add(new RowDefinition());
            }

            for (int i = 0; i < columns; i++)
            {
                citygrid.ColumnDefinitions.Add(new ColumnDefinition());
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
                        FontSize = 100,
                        FontFamily = new FontFamily("Consolas"),
                        Padding = new Thickness(0),
                        Margin = new Thickness(0),
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

            // Nálad a tömb első dimenziója az oszlop,
            // a második dimenziója a sor.
            int columns = Map.GetLength(0);
            int rows = Map.GetLength(1);

            double maxWidth = 1000.0;
            double maxHeight = 800.0;

            // Ezt állítgasd, ha túl nagy vagy túl kicsi a vízszintes távolság.
            // Ha túl nagy a rés: csökkentsd, pl. 0.50
            // Ha összecsúszik: növeld, pl. 0.65
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
        
        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (InGameGrid.Visibility == Visibility.Visible)
            {
                if (e.IsRepeat) return;
                player.Move(e, maze);
            }
           
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
                Console.WriteLine(Database.GetSavesForPlayer(LoginUsernameBox.Text));
            }
            
            LoginGrid.Visibility = Visibility.Collapsed;
            InGameGrid.Visibility = Visibility.Visible;

            SaveData test = Database.GetSavesForPlayer(LoginUsernameBox.Text).First();
            
            drawMap(test.MazeMap, test.Row, test.Column);
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
                Console.WriteLine("ka");
            }
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            drawPreMap(Database.getMazeByName(SelectionListBox.SelectedItem.ToString())); 
        }
    }
}