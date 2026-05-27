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

        public void Move(Key e, Maze maze)
        {
            Console.WriteLine("moce");
            int x = 0;
            int y = 0;
            
            switch (e)
            {
                default: 
                    return;
                
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
                
            }

            Console.WriteLine(x + " " + y);
        }
    }
}
