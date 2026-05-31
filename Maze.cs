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
            
            for (int i = 0; i < this.Row; i++)
            {
                if(new char[]{ '╬', '═', '╦', '╩', '╣', '╗', '╝'}.Contains(Map[i, 0])) exits.Add(new int[] { i, 0 } );
            }
            for (int i = 0; i < Row; i++)
            {
                if(new char[]{ '╬', '═', '╦', '╩', '╠', '╚', '╔'}.Contains(Map[i, Column-1])) exits.Add(new int[] { i, Column-1} );
            }
            for (int i = 0; i < Column-1; i++)
            {
                if(new char[]{ '╬', '╩', '║', '╣', '╠',  '╝', '╚'}.Contains(Map[0, i])) exits.Add(new int[] { 0, i} );
            }
            for (int i = 0; i < Column-1; i++)
            {
                if(new char[]{ '╬', '╦', '║', '╣', '╠', '╗', '╔'}.Contains(Map[Row-1, i])) exits.Add(new int[] {Row-1, i } );
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