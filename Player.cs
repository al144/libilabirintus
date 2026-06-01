using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace libilabirintus
{
    class Player
    {
        public string Name { get; set; }

        public Player(string name)
        {
            this.Name = name;
        }

        public Player GetPlayer()
        {
            return this;
        }


        public int[] Move(Key e, Maze maze, int[] playerPos)
        {
            int x = playerPos[0];
            int y = playerPos[1];

            int newX = x;
            int newY = y;

            switch (e)
            {
                case Key.W:
                    newY--;
                    break;

                case Key.A:
                    newX--;
                    break;

                case Key.S:
                    newY++;
                    break;

                case Key.D:
                    newX++;
                    break;

                default:
                    return playerPos;
            }

            if (newY > maze.Row - 1 || newX > maze.Column - 1 || newY < 0 || newX < 0)
            {
                return playerPos;
            }

            char current = maze.Map[y, x];
            char next = maze.Map[newY, newX];

            // Tanarur nem mondott semmit a kod szepsegerol es a feladatba sem szerepel hogy ezert pontlevonas jarna!!
            if (
                e == Key.W &&
                (
                    current == '╬' || current == '║' || current == '╩' ||
                    current == '╣' || current == '╠' || current == '╝' ||
                    current == '╚' || current == '█'
                ) &&
                (
                    next == '╬' || next == '║' || next == '╦' ||
                    next == '╣' || next == '╠' || next == '╗' ||
                    next == '╔' || next == '█'
                )
                ||
                e == Key.A &&
                (
                    current == '╬' || current == '═' || current == '╦' ||
                    current == '╩' || current == '╣' || current == '╗' ||
                    current == '╝' || current == '█'
                ) &&
                (
                    next == '╬' || next == '═' || next == '╦' ||
                    next == '╩' || next == '╠' || next == '╚' ||
                    next == '╔' || next == '█'
                )
                ||
                e == Key.S &&
                (
                    current == '╬' || current == '║' || current == '╦' ||
                    current == '╣' || current == '╠' || current == '╗' ||
                    current == '╔' || current == '█'
                ) &&
                (
                    next == '╬' || next == '║' || next == '╩' ||
                    next == '╣' || next == '╠' || next == '╝' ||
                    next == '╚' || next == '█'
                )
                ||
                e == Key.D &&
                (
                    current == '╬' || current == '═' || current == '╦' ||
                    current == '╩' || current == '╠' || current == '╚' ||
                    current == '╔' || current == '█'
                ) &&
                (
                    next == '╬' || next == '═' || next == '╦' ||
                    next == '╩' || next == '╣' || next == '╗' ||
                    next == '╝' || next == '█'
                )
            )
            {
                return new[] { newX, newY };
            }

            return playerPos;
        }
        
    }
}
