using Memory;
using System;
using System.Text;

namespace Warspear_Online_Bot
{
    class Program
    {
        static void Main(string[] args)
        {
            Mem m = new Mem();
            if (m.OpenProcess("warspear"))
            {
                Console.WriteLine($"Open Handle: {m.pHandle.ToString("X")}");

                long entityListAdr = m.ReadLong("warspear.exe+5F0330,10,38,0");
                Console.WriteLine($"Entity List located at: {entityListAdr.ToString("X")}\n");

                // My Brain is smoking trying to implement a red-black list reader or it's because of the hot air here in Germany that is burning my energy right now.
                // So I just hardcode the addresses. It's basically like counting up binary but with offsets, and I couldn't make a function that does that.
                // Will have to sleep over it, as this is a bad habbit, just to see progression.

                string[] addr = {
                    "warspear.exe+5F0330,10,38,0,14",
                    "warspear.exe+5F0330,10,38,0,4,14",
                    "warspear.exe+5F0330,10,38,0,8,14",
                    "warspear.exe+5F0330,10,38,0,4,4,14",
                    "warspear.exe+5F0330,10,38,0,4,8,14",
                    "warspear.exe+5F0330,10,38,0,8,4,14",
                    "warspear.exe+5F0330,10,38,0,8,8,14",
                    "warspear.exe+5F0330,10,38,0,4,4,4,14",
                    "warspear.exe+5F0330,10,38,0,4,4,8,14",
                    "warspear.exe+5F0330,10,38,0,4,8,4,14",
                    "warspear.exe+5F0330,10,38,0,4,8,8,14",
                    "warspear.exe+5F0330,10,38,0,8,4,4,14",
                    "warspear.exe+5F0330,10,38,0,8,4,8,14",
                    "warspear.exe+5F0330,10,38,0,8,8,4,14",
                    "warspear.exe+5F0330,10,38,0,8,8,8,14",
                    "warspear.exe+5F0330,10,38,0,4,4,4,4,14",
                    "warspear.exe+5F0330,10,38,0,4,4,4,8,14",
                    "warspear.exe+5F0330,10,38,0,4,4,8,4,14",
                    "warspear.exe+5F0330,10,38,0,4,4,8,8,14",
                    "warspear.exe+5F0330,10,38,0,4,8,4,4,14",
                    "warspear.exe+5F0330,10,38,0,4,8,4,8,14",
                    "warspear.exe+5F0330,10,38,0,4,8,8,4,14",
                    "warspear.exe+5F0330,10,38,0,4,8,8,8,14",
                    "warspear.exe+5F0330,10,38,0,8,4,4,4,14",
                    "warspear.exe+5F0330,10,38,0,8,4,4,8,14",
                    "warspear.exe+5F0330,10,38,0,8,4,8,4,14",
                    "warspear.exe+5F0330,10,38,0,8,4,8,8,14",
                    "warspear.exe+5F0330,10,38,0,8,8,4,4,14",
                    "warspear.exe+5F0330,10,38,0,8,8,4,8,14",
                    "warspear.exe+5F0330,10,38,0,8,8,8,4,14",
                    "warspear.exe+5F0330,10,38,0,8,8,8,8,14",
                    "warspear.exe+5F0330,10,38,0,4,4,4,4,8,14",
                    "warspear.exe+5F0330,10,38,0,4,4,4,8,4,14",
                    "warspear.exe+5F0330,10,38,0,4,4,4,8,8,14",
                    "warspear.exe+5F0330,10,38,0,4,4,8,4,4,14",
                    "warspear.exe+5F0330,10,38,0,4,4,8,4,8,14",
                    "warspear.exe+5F0330,10,38,0,4,4,8,8,4,14",
                    "warspear.exe+5F0330,10,38,0,4,4,8,8,8,14",
                    "warspear.exe+5F0330,10,38,0,4,8,4,4,4,14",
                    "warspear.exe+5F0330,10,38,0,4,8,4,4,8,14",
                    "warspear.exe+5F0330,10,38,0,4,8,4,8,4,14",
                    "warspear.exe+5F0330,10,38,0,4,8,4,8,8,14",
                    "warspear.exe+5F0330,10,38,0,4,8,8,4,4,14",
                    "warspear.exe+5F0330,10,38,0,4,8,8,4,8,14",
                    "warspear.exe+5F0330,10,38,0,4,8,8,8,4,14",
                    "warspear.exe+5F0330,10,38,0,4,8,8,8,8,14",
                    "warspear.exe+5F0330,10,38,0,8,4,4,4,4,14",
                    "warspear.exe+5F0330,10,38,0,8,4,4,4,8,14",
                    "warspear.exe+5F0330,10,38,0,8,4,4,8,4,14",
                    "warspear.exe+5F0330,10,38,0,8,4,4,8,8,14",
                    "warspear.exe+5F0330,10,38,0,8,4,8,4,4,14",
                    "warspear.exe+5F0330,10,38,0,8,4,8,4,8,14",
                    "warspear.exe+5F0330,10,38,0,8,4,8,8,4,14",
                    "warspear.exe+5F0330,10,38,0,8,4,8,8,8,14",
                    "warspear.exe+5F0330,10,38,0,8,8,4,4,4,14",
                    "warspear.exe+5F0330,10,38,0,8,8,4,4,8,14",
                    "warspear.exe+5F0330,10,38,0,8,8,4,8,4,14",
                    "warspear.exe+5F0330,10,38,0,8,8,4,8,8,14",
                    "warspear.exe+5F0330,10,38,0,8,8,8,4,4,14",
                    "warspear.exe+5F0330,10,38,0,8,8,8,4,8,14",
                    "warspear.exe+5F0330,10,38,0,8,8,8,8,4,14",
                    "warspear.exe+5F0330,10,38,0,8,8,8,8,8,14"
                };

                /*
                    [ENABLE]
                    "warspear.exe"+1A82FA:  // X
                    db 90 90 90 90          // Nop
                    "warspear.exe"+1A8321:  // Y
                    db 90 90 90 90          // Nop
                */
                byte NOP = 0x90; // Byte representing "no operation"
                if (m.ReadBytes("warspear.exe+1A82FA", 1)[0] != NOP && m.ReadBytes("warspear.exe+1A8321", 1)[0] != NOP)
                {
                    m.WriteMemory("warspear.exe+1A82FA", "", "90 90 90 90"); // X
                    m.WriteMemory("warspear.exe+1A8321", "bytes", "90 90 90 90"); // Y
                }

                // Setting up Input Class
                Input.Input.process = m.theProc;

                while (!m.theProc.HasExited)
                {
                    foreach (string address in addr)
                    {
                        int nameLength = m.ReadInt($"{address},54");
                        byte[] nameAsBytes = m.ReadBytes($"{address},58", nameLength * 2);
                        string nameAsString = Encoding.Unicode.GetString(nameAsBytes);

                        if (String.IsNullOrEmpty(nameAsString) || nameAsString != "Uangeu") continue;
                        int currentHP = m.ReadInt($"{address},F4");

                        // Use heal on your main account when you get lower than a set value
                        if (currentHP < 1000)
                        {
                            /*
                            (hex)58		entity name
                            (hex)F4		health
                            (hex)F8		max health
                            (hex)FC		mana
                            (hex)100	max mana
                            (hex)108	pos x
                            (hex)10A	pos y
                            (hex)10C 	des x
                            (hex)10E 	des y
                            */
                            int x = m.Read2Byte($"{address},108");
                            int y = m.Read2Byte($"{address},10A");


                            // Check if we have enough Mana, my Healing Spirit costs 22 Mana, change this with your Skill Cost.
                            int skillCost = 22;
                            if (m.ReadInt("warspear.exe+5F49B8,0,10,D4,4,B8,4C,A4") > skillCost)
                            {
                                Input.Input.PressKey(Input.Input.Keys.Two);

                                // You can find writememory types here: https://github.com/erfg12/memory.dll/blob/2f349ae4ebc1c98d681ecf373ebaba6a99148632/Memory/memory.cs#L1059
                                // Yes, this is retarded when you don't know them and that they are basically equal to the read methods.
                                // Better would be accepting both, the string (because it's faster to type) and some sort of enum that lists you all options <- or just accept custom types.
                                m.WriteMemory("warspear.exe+5F0330,24,10,4,21C,8", "2bytes", x.ToString()); // X
                                m.WriteMemory("warspear.exe+5F0330,24,10,4,21C,A", "2bytes", y.ToString()); // Y

                                Input.Input.PressKey(Input.Input.Keys.Enter);
                            }
                        }
                    }
                }

                /*
                    [DISABLE]
                    "warspear.exe"+1A82FA:  // X
                    db 66 89 41 08          // Original Bytes
                    "warspear.exe"+1A8321:  // Y
                    db 66 89 41 0A          // Original Bytes
                */
                if (m.ReadBytes("warspear.exe+1A82FA", 1)[0] == NOP && m.ReadBytes("warspear.exe+1A8321", 1)[0] == NOP)
                {
                    m.WriteMemory("warspear.exe+1A82FA", "bytes", "66 89 41 08"); // X
                    m.WriteMemory("warspear.exe+1A8321", "bytes", "66 89 41 0A"); // Y
                }
            }
        }
    }
}
