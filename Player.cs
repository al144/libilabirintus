using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libilabirintus
{
    class Player
    {
        public string Name { get; private set; }
        public List<Maze> PlayedMazes { get; private set; }
        public List<char[,]> Explored {  get; private set; }

        public Player(string Name)
        {
            this.Name = Name;
            PlayedMazes = new List<Maze>();
            Explored = new List<char[,]>();
        }

        public void AlredyPlayed(Maze maze)
        {
            PlayedMazes.Add(maze);
        }
    }
}
