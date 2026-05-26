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
            
            dummyListBox();
            
            //int mazeid = Database.SaveMaze(new Maze("ds"));

            
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

        void drawPreMap(char[,] Map, int rows, int columns)
        {
            
            preGrid.RowDefinitions.Clear();
            preGrid.ColumnDefinitions.Clear();
            preGrid.Children.Clear();

            for (int i = 0; i < rows; i++)
            {
                preGrid.RowDefinitions.Add(new RowDefinition());
            }

            for (int i = 0; i < columns; i++)
            {
                preGrid.ColumnDefinitions.Add(new ColumnDefinition());
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

        void dummyListBox()
        {
            for (int i = 0; i < 30; i++)
            {
                SelectionListBox.Items.Add(i);
            }
        }
    }
}