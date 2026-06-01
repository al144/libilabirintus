using Microsoft.Win32;
using System.IO;
using System.Security.Cryptography.X509Certificates;
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
        private Char[,] explored;
        private int[] currPos;
        private bool normalGame = true;
        private int points = 0;
        private int steps = 0;
        
        public MainWindow()
        {
            InitializeComponent();
            
            Database.Init();
        
            // Database.SaveMaze(new Maze("osdasp"));

        }
        
        
        
        void DrawMap(Maze maze, int currX, int currY)
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

                    if (map[row, col] == '.') map[row, col] = ' ';
                    

                    Label label = new()
                    {
                        Content = map[row, col],
                        FontSize = fontSize,
                        FontFamily = new FontFamily("Consolas"),
                        Padding = new Thickness(0),
                        Margin = new Thickness(0),
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        VerticalContentAlignment = VerticalAlignment.Center
                    };
                    
                    if (currX == col && currY == row)
                    {
                        label.Background = new  SolidColorBrush(Colors.Blue);
                    }

                    Grid.SetRow(label, row);
                    Grid.SetColumn(label, col);

                    citygrid.Children.Add(label);
                }
            }
        }       
        
        void DrawHiddenMap(Maze maze, int currX, int currY)
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
                    if (map[row, col] == '.') map[row, col] = ' ';

                    Grid cellGrid = new()
                    {
                        Margin = new Thickness(0)
                    };

                    Label label = new()
                    {
                        Content = map[row, col],
                        FontSize = fontSize,
                        Foreground = Brushes.Gray,
                        FontFamily = new FontFamily("Consolas"),
                        Padding = new Thickness(0),
                        Margin = new Thickness(0),
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch
                    };

                    if (currX == col && currY == row)
                    {
                        cellGrid.Background = new SolidColorBrush(Colors.Blue);
                    }

                    if (explored[row, col] == map[row, col])
                    {
                        label.Foreground = Brushes.Black;
                    }

                    cellGrid.Children.Add(label);

                    string arrow = GetMoveArrowForCell(maze, currX, currY, col, row);

                    if (arrow != "")
                    {
                        Label arrowLabel = new()
                        {
                            Content = arrow,
                            FontSize = fontSize * 0.45,
                            Foreground = Brushes.Gold,
                            FontWeight = FontWeights.Bold,
                            FontFamily = new FontFamily("Consolas"),
                            Padding = new Thickness(0),
                            Margin = new Thickness(0),
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            VerticalAlignment = VerticalAlignment.Stretch,
                            IsHitTestVisible = false
                        };

                        SetArrowPosition(arrowLabel, arrow);

                        Panel.SetZIndex(arrowLabel, 10);
                        cellGrid.Children.Add(arrowLabel);
                    }

                    Grid.SetRow(cellGrid, row);
                    Grid.SetColumn(cellGrid, col);

                    citygrid.Children.Add(cellGrid);
                }
            }
        }

        string GetMoveArrowForCell(Maze maze, int currX, int currY, int col, int row)
        {
            int[] playerPos = { currX, currY };

            int[] up = player.Move(Key.W, maze, playerPos);
            if (
                !(up[0] == currX && up[1] == currY) &&
                up[0] == col &&
                up[1] == row
            )
            {
                return "↑";
            }

            int[] left = player.Move(Key.A, maze, playerPos);
            if (
                !(left[0] == currX && left[1] == currY) &&
                left[0] == col &&
                left[1] == row
            )
            {
                return "←";
            }

            int[] down = player.Move(Key.S, maze, playerPos);
            if (
                !(down[0] == currX && down[1] == currY) &&
                down[0] == col &&
                down[1] == row
            )
            {
                return "↓";
            }

            int[] right = player.Move(Key.D, maze, playerPos);
            if (
                !(right[0] == currX && right[1] == currY) &&
                right[0] == col &&
                right[1] == row
            )
            {
                return "→";
            }

            return "";
        }

        void SetArrowPosition(Label arrowLabel, string arrow)
        {
            switch (arrow)
            {
                case "↑":
                    arrowLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
                    arrowLabel.VerticalContentAlignment = VerticalAlignment.Top;
                    break;

                case "←":
                    arrowLabel.HorizontalContentAlignment = HorizontalAlignment.Left;
                    arrowLabel.VerticalContentAlignment = VerticalAlignment.Center;
                    break;

                case "↓":
                    arrowLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
                    arrowLabel.VerticalContentAlignment = VerticalAlignment.Bottom;
                    break;

                case "→":
                    arrowLabel.HorizontalContentAlignment = HorizontalAlignment.Right;
                    arrowLabel.VerticalContentAlignment = VerticalAlignment.Center;
                    break;
            }
        }      
        
        void DrawPreMap(Maze maze)
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
                    if (Map[row, col] == '.')
                    {
                        continue;
                    }

                    Label label = new()
                    {
                        Content = Map[row, col],
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

            if (e.Key == Key.Escape)
            {
                ShowScene(PauseGrid);
                return;
            }

            if (maze == null || player == null || currPos == null || explored == null)
            {
                return;
            }

            int oldX = currPos[0];
            int oldY = currPos[1];

            int[] newPos = player.Move(e.Key, maze, currPos);

            int newX = newPos[0];
            int newY = newPos[1];

            if (oldX == newX && oldY == newY)
            {
                return;
            }

            currPos = newPos;

            explored[newY, newX] = maze.Map[newY, newX];

            CoordinateLabel.Content = $"Koordinatak row:{newY} ; column{newX}";

            if (maze.Map[newY, newX] == '█')
            {
                points++;
                PointLabel.Content = $"Pontok: {points}";

                maze.Map[newY, newX] = '╬';
                explored[newY, newX] = '╬';
            }

            steps++;
            StepsLabel.Content = $"Lépések: {steps}";

            Database.SaveGame(
                player.Name,
                maze,
                explored,
                currPos[0],
                currPos[1]
            );

            StartGame(normalGame);
        }
        
        
        void ShowScene(Grid selectedGrid)
        {
            foreach (var child in MainContainer.Children)
            {
                if (child is Grid grid)
                {
                    grid.Visibility = Visibility.Collapsed;
                }
            }

            selectedGrid.Visibility = Visibility.Visible;
        }

        private void CreatePlayerButton(object sender, RoutedEventArgs e)
        {
            Database.CreatePlayer(CreateUserNameBox.Text, CreatePasswordBox.Password);
            ShowScene(LoginGrid);
        }
        
        private void LoginButton(object sender, RoutedEventArgs e)
        {
            if (Database.CheckLogin(LoginUsernameBox.Text, LoginPasswordBox.Password))
            {
                player = Database.GetPlayerByName(LoginUsernameBox.Text);
            
                ShowScene(GameModeGrid);
                AppendMapList(Database.GetAllMazes());
            }

        }

        private void GoToCreatePlayerButton(object sender, RoutedEventArgs e)
        {
            ShowScene(CreatePlayerGrid);
        }

        void AppendMapList(List<Maze> mazes)
        {
            foreach (var maze in mazes)
            {
                SelectionListBox.Items.Add(maze.Name);
            }
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            QuestionGrid.Visibility = Visibility.Collapsed;
            if (!normalGame)
            {
                QuestionGrid.Visibility = Visibility.Visible;
                return;
            }
            DrawPreMap(Database.GetMazeByName(SelectionListBox.SelectedItem.ToString())); 
        }

        private void SelectEnter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SelectionListBox.SelectedItem == null)
                {
                    return;
                }

                string selectedMazeName = SelectionListBox.SelectedItem.ToString()!;

                maze = Database.GetMazeByName(selectedMazeName);

                if (maze == null)
                {
                    return;
                }

                SaveData? save = Database.GetSave(player.Name, maze.Name);

                if (save != null)
                {
                    currPos = new[] { save.PlayerX, save.PlayerY };
                    explored = save.ExploredMap;
                }
                else
                {
                    explored = new char[maze.Row, maze.Column];

                    List<int[]> exits = maze.FindExits();

                    if (exits.Count == 0)
                    {
                        return;
                    }

                    currPos = exits[Random.Shared.Next(exits.Count)];

                    explored[currPos[1], currPos[0]] = maze.Map[currPos[1], currPos[0]];

                    Database.SaveGame(
                        player.Name,
                        maze,
                        explored,
                        currPos[0],
                        currPos[1]
                    );
                }

                StartGame(normalGame);
            }

            if (e.Key == Key.Escape)
            {
                ShowScene(GameModeGrid);
            }
        }

        private void BackToInGame(object sender, RoutedEventArgs e)
        {
            ShowScene(GameModeGrid);
        }

        private void SaveAndToSelector(object sender, RoutedEventArgs e)
        {
            Database.SaveGame(player.Name, maze, explored, currPos[0], currPos[1]);
            ShowScene(SelectorGrid);
        }

        private void NormalStart(object sender, RoutedEventArgs e)
        {
            normalGame = true;
            ShowScene(SelectorGrid);
        }

        private void HiddenStart(object sender, RoutedEventArgs e)
        {
            normalGame = false;
            ShowScene(SelectorGrid);
        }

        void StartGame(bool normal)
        {
            if (maze == null || currPos == null)
            {
                return;
            }

            ShowScene(InGameGrid);
                

            if (normal)
            {
                DrawMap(maze, currPos[0], currPos[1]);
                return;
            }

            DrawHiddenMap(maze, currPos[0], currPos[1]);
        }

        private void GoToGamemode(object sender, RoutedEventArgs e)
        {
            ShowScene(GameModeGrid);
        }
    }
}