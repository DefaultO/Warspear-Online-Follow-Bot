using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warspear_Online_Follow_Bot_Test
{
    public class Player
    {
        public string Name { get; set; }
        public int HP { get; set; }
        public int MaxHP { get; set; }
        public int MP { get; set; }
        public int MaxMP { get; set; }
        public int PosX { get; set; }
        public int PosY { get; set; }
        public int DesX { get; set; }
        public int DesY { get; set; }
        public int DistanceToDes { get; set; }
        public int CursorX { get; set; }
        public int CursorY { get; set; }
        public int CursorTImer { get; set; }
        public int CursorFlag { get; set; }
        public int IsMoving { get; set; }
    }
}
