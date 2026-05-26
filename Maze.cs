using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libilabirintus;

namespace libilabirintus
{
    class Maze
    {
        public string Name { get; private set; }
        public int TreasuryNum { get; private set; }
        public int Row { get; private set; }
        public int Column { get; private set; }
        public char[,] Map { get; private set; }
        public Player Creator { get; private set; }


        public Maze(string name)
        {
            this.Name = name;
            this.Map = getMap();
            this.TreasuryNum = getTreasuryRoomNum();
        }

        char[,] getMap()
        {
            OpenFileDialog ofd = new();
            char[,] map;

            if (ofd.ShowDialog() == false)
            {
                throw new Exception("nem jo a fajl");
            }

            string[] notCharMap = File.ReadAllLines(ofd.FileName);

            map = new char[notCharMap[0].Length, notCharMap.Length];
            this.Row = notCharMap.Length;
            this.Column = notCharMap[0].Length;

            for (int i = 0; i < notCharMap.Length; i++)
            {
                for (int j = 0; j < notCharMap[i].Length; j++)
                {
                    map[j, i] = notCharMap[i][j];
                }
            }

            return map;
        }

        int getTreasuryRoomNum()
        {
            int num = 0;


            return num;
        }
    }
}