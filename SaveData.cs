namespace libilabirintus
{
    class SaveData
    {
        public int SaveId { get; set; }

        public int PlayerId { get; set; }
        public string PlayerName { get; set; } = "";

        public int MazeId { get; set; }
        public string MazeName { get; set; } = "";

        public int Row { get; set; }
        public int Column { get; set; }

        public char[,] MazeMap { get; set; } = new char[0, 0];
        public char[,] ExploredMap { get; set; } = new char[0, 0];

        public int PlayerX { get; set; }
        public int PlayerY { get; set; }

        public string SavedAt { get; set; } = "";
    }
}