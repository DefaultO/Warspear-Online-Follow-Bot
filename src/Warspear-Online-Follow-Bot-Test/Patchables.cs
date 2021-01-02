using System;

namespace Warspear_Online_Follow_Bot_Test
{
    /// <summary>
    /// Class containing all the latest working Pointers that are neccessary for this bot to work perfectly fine.
    /// </summary>
    public static class Patchables
    {
        /// <summary>
        /// The Name of your localplayer
        /// </summary>
        public static Pointer Name = new Pointer
        {
            Module = "warspear.exe",
            Offset = "0x005A8BF8",
            Offsets = new string[] { "0", "10", "D4", "4", "B8", "4C", "0" }
        };

        /// <summary>
        /// The current Health your localplayer has
        /// </summary>
        public static Pointer HP = new Pointer
        {
            Module = "warspear.exe",
            Offset = "0x005A8BF8",
            Offsets = new string[] { "0", "10", "D4", "4", "B8", "4C", "9C" }
        };

        /// <summary>
        /// The max Health your localplayer can have
        /// </summary>
        public static Pointer MaxHP = new Pointer
        {
            Module = "warspear.exe",
            Offset = "0x005A8BF8",
            Offsets = new string[] { "0", "10", "D4", "4", "B8", "4C", "A0" }
        };

        /// <summary>
        /// The current Mana your localplayer has
        /// </summary>
        public static Pointer MP = new Pointer
        {
            Module = "warspear.exe",
            Offset = "0x005A8BF8",
            Offsets = new string[] { "0", "10", "D4", "4", "B8", "4C", "A4" }
        };

        /// <summary>
        /// The max Mana your localplayer can have
        /// </summary>
        public static Pointer MaxMP = new Pointer
        {
            Module = "warspear.exe",
            Offset = "0x005A8BF8",
            Offsets = new string[] { "0", "10", "D4", "4", "B8", "4C", "A8" }
        };

        /// <summary>
        /// Localplayer X Position
        /// </summary>
        public static Pointer PosX = new Pointer
        {
            Module = "warspear.exe",
            Offset = "0x005A8BF8",
            Offsets = new string[] { "0", "10", "D4", "4", "B8", "4C", "B0" }
        };

        /// <summary>
        /// Localplayer Y Position
        /// </summary>
        public static Pointer PosY = new Pointer
        {
            Module = "warspear.exe",
            Offset = "0x005A8BF8",
            Offsets = new string[] { "0", "10", "D4", "4", "B8", "4C", "B2" }
        };

        /// <summary>
        /// Cursor-set Destination X Position
        /// </summary>
        public static Pointer DesX = new Pointer
        {
            Module = "warspear.exe",
            Offset = "0x005A8BF8",
            Offsets = new string[] { "0", "10", "D4", "4", "B8", "4C", "B4" }
        };

        /// <summary>
        /// Cursor-set Destination Y Position
        /// </summary>
        public static Pointer DesY = new Pointer
        {
            Module = "warspear.exe",
            Offset = "0x005A8BF8",
            Offsets = new string[] { "0", "10", "D4", "4", "B8", "4C", "B6" }
        };

        /// <summary>
        /// I think that's the in-games Distance to your Destination. Even though it's off / not working properply when it's only 1 Tile away.
        /// </summary>
        public static Pointer DistanceToDestination = new Pointer
        {
            Module = "warspear.exe",
            Offset = "0x005A8BF8",
            Offsets = new string[] { "0", "10", "D4", "4", "B8", "4C", "1D8" }
        };

        /// <summary>
        /// 'Play Player Animation?' returns 0 or 1
        /// </summary>
        public static Pointer IsMoving = new Pointer
        {
            Module = "warspear.exe",
            Offset = "0x005A8BF8",
            Offsets = new string[] { "0", "10", "D4", "4", "B8", "4C", "D0" }
        };

        /// <summary>
        /// Cursor X Position
        /// </summary>
        public static Pointer CursorX = new Pointer
        {
            Module = "warspear.exe",
            Offset = "0x005A45B0",
            Offsets = new string[] { "24", "10", "4", "21C", "8" }
        };

        /// <summary>
        /// Cursor Y Position
        /// </summary>
        public static Pointer CursorY = new Pointer
        {
            Module = "warspear.exe",
            Offset = "0x005A45B0",
            Offsets = new string[] { "24", "10", "4", "21C", "A" }
        };

        /// <summary>
        /// Cursor Icon Flag
        /// </summary>
        public static Pointer CursorFlag = new Pointer
        {
            Module = "warspear.exe",
            Offset = "0x005A45B0",
            Offsets = new string[] { "24", "10", "4", "21C", "6C" }
        };

        /// <summary>
        /// Ingame Timer that starts running when the cursor is above your char. Used for making ´the Cursor transparent.
        /// </summary>
        public static Pointer CursorTimer = new Pointer
        {
            Module = "warspear.exe",
            Offset = "0x005A45B0",
            Offsets = new string[] { "24", "10", "4", "21C", "84" }
        };
    }

    /// <summary>
    /// Basic Pointer Structure fit to the memory.dll Library. Inspired by <see href="https://github.com/Azukee">Azuki's</see> Memory Class.
    /// </summary>
    public class Pointer
    {
        public string Module;
        public string Offset;
        public string[] Offsets;

        /// <summary>
        /// Returns every part of the Pointer into a single String useable by the memory.dll Library.
        /// </summary>
        public string Get()
        {
            string pointer = String.Format("{0}+{1}", this.Module, Offset);
            foreach (string offset in this.Offsets)
            {
                pointer = String.Format("{0},{1}", pointer, offset);
            }
            return pointer;
        }
    }
}
