using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace libilabirintus
{
    class Database
    {
        
        
        static string dbPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            $"..\\..\\..\\labirintus_.Sav"
        );

        static string connectionString =
            $"Data Source={dbPath}";

        private static SqliteConnection OpenConnection()
        {
            SqliteConnection connection = new(connectionString);
            connection.Open();

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = "PRAGMA foreign_keys = ON;";
            command.ExecuteNonQuery();

            return connection;
        }

        public static void Init()
        {
            using SqliteConnection connection = OpenConnection();

            string sql = """
            CREATE TABLE IF NOT EXISTS maze (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                name TEXT NOT NULL UNIQUE,
                "row" INTEGER NOT NULL,
                "column" INTEGER NOT NULL,
                map TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS player (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                username TEXT NOT NULL UNIQUE,
                password TEXT NOT NULL DEFAULT ''
            );

            CREATE TABLE IF NOT EXISTS savedMap (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                player_id INTEGER NOT NULL,
                maze_id INTEGER NOT NULL,
                explored TEXT NOT NULL,
                player_x INTEGER NOT NULL,
                player_y INTEGER NOT NULL,
                saved_at TEXT NOT NULL,

                FOREIGN KEY (player_id) REFERENCES player(id) ON DELETE CASCADE,
                FOREIGN KEY (maze_id) REFERENCES maze(id) ON DELETE CASCADE,

                UNIQUE(player_id, maze_id)
            );
            """;

            using SqliteCommand command = new(sql, connection);
            command.ExecuteNonQuery();
        }

        public static int CreatePlayer(string username, string password)
        {
            using SqliteConnection connection = OpenConnection();

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = """
            INSERT INTO player (username, password)
            VALUES ($username, $password);
            """;

            command.Parameters.AddWithValue("$username", username);
            command.Parameters.AddWithValue("$password", HashPassword(password));

            command.ExecuteNonQuery();

            return GetPlayerId(connection, username);
        }

        public static int GetOrCreatePlayer(string username)
        {
            using SqliteConnection connection = OpenConnection();

            using SqliteCommand insertCommand = connection.CreateCommand();
            insertCommand.CommandText = """
            INSERT OR IGNORE INTO player (username, password)
            VALUES ($username, '');
            """;

            insertCommand.Parameters.AddWithValue("$username", username);
            insertCommand.ExecuteNonQuery();

            return GetPlayerId(connection, username);
        }

        public static bool CheckLogin(string username, string password)
        {
            using SqliteConnection connection = OpenConnection();

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = """
            SELECT password
            FROM player
            WHERE username = $username;
            """;

            command.Parameters.AddWithValue("$username", username);

            object? result = command.ExecuteScalar();

            if (result == null)
            {
                return false;
            }

            string storedPasswordHash = result.ToString()!;
            string typedPasswordHash = HashPassword(password);

            return storedPasswordHash == typedPasswordHash;
        }

        public static int SaveMaze(Maze maze)
        {
            using SqliteConnection connection = OpenConnection();

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = """
            INSERT INTO maze (name, "row", "column", map)
            VALUES ($name, $row, $column, $map)
            ON CONFLICT(name) DO UPDATE SET
                "row" = $row,
                "column" = $column,
                map = $map;
            """;

            command.Parameters.AddWithValue("$name", maze.Name);
            command.Parameters.AddWithValue("$row", maze.Row);
            command.Parameters.AddWithValue("$column", maze.Column);
            command.Parameters.AddWithValue("$map", CharArrayToString(maze.Map));

            command.ExecuteNonQuery();

            return GetMazeId(connection, maze.Name);
        }

        public static void SaveGame(string playerName, Maze maze, char[,] exploredMap, int playerX, int playerY)
        {
            int playerId = GetOrCreatePlayer(playerName);
            int mazeId = SaveMaze(maze);
            
            using SqliteConnection connection = OpenConnection();

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = """
            INSERT INTO savedMap 
            (
                player_id,
                maze_id,
                explored,
                player_x,
                player_y,
                saved_at
            )
            VALUES 
            (
                $player_id,
                $maze_id,
                $explored,
                $player_x,
                $player_y,
                $saved_at
            )
            ON CONFLICT(player_id, maze_id) DO UPDATE SET
                explored = $explored,
                player_x = $player_x,
                player_y = $player_y,
                saved_at = $saved_at;
            """;

            command.Parameters.AddWithValue("$player_id", playerId);
            command.Parameters.AddWithValue("$maze_id", mazeId);
            command.Parameters.AddWithValue("$explored", CharArrayToString(exploredMap));
            command.Parameters.AddWithValue("$player_x", playerX);
            command.Parameters.AddWithValue("$player_y", playerY);
            command.Parameters.AddWithValue("$saved_at", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            command.ExecuteNonQuery();
        }

        public static List<SaveData> GetSavesForPlayer(string username)
        {
            List<SaveData> saves = new();

            using SqliteConnection connection = OpenConnection();

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = """
            SELECT 
                savedMap.id,
                player.id,
                player.username,
                maze.id,
                maze.name,
                maze."row",
                maze."column",
                maze.map,
                savedMap.explored,
                savedMap.player_x,
                savedMap.player_y,
                savedMap.saved_at
            FROM savedMap
            INNER JOIN player ON savedMap.player_id = player.id
            INNER JOIN maze ON savedMap.maze_id = maze.id
            WHERE player.username = $username
            ORDER BY savedMap.saved_at DESC;
            """;

            command.Parameters.AddWithValue("$username", username);

            using SqliteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                SaveData save = new()
                {
                    SaveId = reader.GetInt32(0),
                    PlayerId = reader.GetInt32(1),
                    PlayerName = reader.GetString(2),
                    MazeId = reader.GetInt32(3),
                    MazeName = reader.GetString(4),
                    Row = reader.GetInt32(5),
                    Column = reader.GetInt32(6),
                    MazeMap = StringToCharArray(reader.GetString(7)),
                    ExploredMap = StringToCharArray(reader.GetString(8)),
                    PlayerX = reader.GetInt32(9),
                    PlayerY = reader.GetInt32(10),
                    SavedAt = reader.GetString(11)
                };

                saves.Add(save);
            }

            return saves;
        }

        public static SaveData? GetSave(string username, string mazeName)
        {
            using SqliteConnection connection = OpenConnection();

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = """
            SELECT 
                savedMap.id,
                player.id,
                player.username,
                maze.id,
                maze.name,
                maze."row",
                maze."column",
                maze.map,
                savedMap.explored,
                savedMap.player_x,
                savedMap.player_y,
                savedMap.saved_at
            FROM savedMap
            INNER JOIN player ON savedMap.player_id = player.id
            INNER JOIN maze ON savedMap.maze_id = maze.id
            WHERE player.username = $username
            AND maze.name = $mazeName;
            """;

            command.Parameters.AddWithValue("$username", username);
            command.Parameters.AddWithValue("$mazeName", mazeName);

            using SqliteDataReader reader = command.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            return new SaveData
            {
                SaveId = reader.GetInt32(0),
                PlayerId = reader.GetInt32(1),
                PlayerName = reader.GetString(2),
                MazeId = reader.GetInt32(3),
                MazeName = reader.GetString(4),
                Row = reader.GetInt32(5),
                Column = reader.GetInt32(6),
                MazeMap = StringToCharArray(reader.GetString(7)),
                ExploredMap = StringToCharArray(reader.GetString(8)),
                PlayerX = reader.GetInt32(9),
                PlayerY = reader.GetInt32(10),
                SavedAt = reader.GetString(11)
            };
        }

        public static void DeleteSave(int saveId)
        {
            using SqliteConnection connection = OpenConnection();

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = """
            DELETE FROM savedMap
            WHERE id = $saveId;
            """;

            command.Parameters.AddWithValue("$saveId", saveId);
            command.ExecuteNonQuery();
        }

        private static int GetPlayerId(SqliteConnection connection, string username)
        {
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = """
            SELECT id
            FROM player
            WHERE username = $username;
            """;

            command.Parameters.AddWithValue("$username", username);

            object? result = command.ExecuteScalar();

            if (result == null)
            {
                throw new Exception("Player not found.");
            }

            return Convert.ToInt32(result);
        }

        public static Player? GetPlayerByName(string playerName)
        {
            using SqliteConnection connection = OpenConnection();

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = """
                                  SELECT username
                                  FROM player
                                  WHERE username = $playerName;
                                  """;

            command.Parameters.AddWithValue("$playerName", playerName);

            using SqliteDataReader reader = command.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            return new Player(reader.GetString(0));
        }

        static public Maze GetMazeByName(string mazeName)
        {
            using SqliteConnection connection = OpenConnection();
            
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = """
                                  SELECT id, name, row, column, map
                                  FROM maze
                                  WHERE name = $mazeName;
                                  """;
            
            command.Parameters.AddWithValue("$mazeName", mazeName);
            
            using SqliteDataReader reader = command.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            };

            return new Maze(
               name: reader.GetString(1),
               map: StringToCharArray(reader.GetString(4)),
               row: reader.GetInt32(2),
               column: reader.GetInt32(3)
            );
        }
        
        public static List<Maze> GetAllMazes()
        {
            List<Maze> mazes = new List<Maze>();

            using SqliteConnection connection = OpenConnection();

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = """
                                  SELECT id, name, row, column, map
                                  FROM maze;
                                  """;

            using SqliteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Maze maze = new Maze(
                    name: reader.GetString(1),
                    map: StringToCharArray(reader.GetString(4)),
                    row: reader.GetInt32(2),
                    column: reader.GetInt32(3)
                );

                mazes.Add(maze);
            }

            return mazes;
        }

        private static int GetMazeId(SqliteConnection connection, string mazeName)
        {
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = """
            SELECT id
            FROM maze
            WHERE name = $mazeName;
            """;

            command.Parameters.AddWithValue("$mazeName", mazeName);

            object? result = command.ExecuteScalar();

            if (result == null)
            {
                throw new Exception("Maze not found.");
            }

            return Convert.ToInt32(result);
        }

        public static string CharArrayToString(char[,] map)
        {
            int columns = map.GetLength(0);
            int rows = map.GetLength(1);

            StringBuilder sb = new();

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    sb.Append(map[x, y]);
                }

                sb.Append('\n');
            }

            return sb.ToString();
        }

        public static char[,] StringToCharArray(string text)
        {
            string[] lines = text
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.TrimEnd('\r'))
                .ToArray();

            int rows = lines.Length;
            int columns = lines[0].Length;

            char[,] map = new char[columns, rows];

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    map[x, y] = lines[y][x];
                }
            }

            return map;
        }

        private static string HashPassword(string password)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(password);
            byte[] hash = SHA256.HashData(bytes);

            return Convert.ToHexString(hash);
        }
    }
}