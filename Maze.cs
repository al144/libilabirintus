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
        public int id { get;  set; }
        public string Name { get; set; }
        public int TreasuryNum { get;  set; }
        public int Row { get;  set; }
        public int Column { get; set; }
        public char[,] Map { get; set; }
        

        public Maze(string name)
        {
            this.Name = name;
            this.Map = getMap();
            this.TreasuryNum = getTreasuryRoomNum();
        }

        public Maze(string name, char[,] map, int row, int column)
        {
            this.Name = name;
            this.Map = map;
            this.Row = row;
            this.Column = column;
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

            map = new char[notCharMap.Length, notCharMap[0].Length];
            this.Row =  notCharMap.Length;
            this.Column = notCharMap[0].Length;

            for (int i = 0; i < notCharMap.Length; i++)
            {
                for (int j = 0; j < notCharMap[i].Length; j++)
                {
                    map[i, j] = notCharMap[i][j];
                }
            }

            return map;
        }

        public List<int[]> FindExits()
        {
            List<int[]> exits = new List<int[]>();

            // BAL SZÉL: x = 0, y = row
            for (int row = 0; row < Row; row++)
            {
                if (new char[] { '╬', '═', '╦', '╩', '╣', '╗', '╝' }.Contains(Map[row, 0]))
                {
                    exits.Add(new int[] { 0, row });
                }
            }

            // JOBB SZÉL: x = Column - 1, y = row
            for (int row = 0; row < Row; row++)
            {
                if (new char[] { '╬', '═', '╦', '╩', '╠', '╚', '╔' }.Contains(Map[row, Column - 1]))
                {
                    exits.Add(new int[] { Column - 1, row });
                }
            }

            // FELSŐ SZÉL: x = col, y = 0
            for (int col = 0; col < Column; col++)
            {
                if (new char[] { '╬', '╩', '║', '╣', '╠', '╝', '╚' }.Contains(Map[0, col]))
                {
                    exits.Add(new int[] { col, 0 });
                }
            }

            // ALSÓ SZÉL: x = col, y = Row - 1
            for (int col = 0; col < Column; col++)
            {
                if (new char[] { '╬', '╦', '║', '╣', '╠', '╗', '╔' }.Contains(Map[Row - 1, col]))
                {
                    exits.Add(new int[] { col, Row - 1 });
                }
            }

            return exits;
        }

        int getTreasuryRoomNum()
        {
            int num = 0;


            return num;
        }
    }
}