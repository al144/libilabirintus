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
        public int Id {get; private set;}
        public string Name { get; private set; }

        public Player(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }
        
        public Player GetPlayer()
        {
            return this;
        }

        public void Move(KeyEventArgs e, Maze maze)
        {
            int x = 0;
            int y = 0;
            
            switch (e.Key)
            {
                case Key.W:
                    y = 1;
                    break;
                case Key.A:
                    x = -1;
                    break;
                case Key.S:
                    y = -1;
                    break;
                case Key.D:
                    x = 1;
                    break;
                
                default:
                    return;
            }

            Console.WriteLine(x + " " + y);
        }
    }
}
