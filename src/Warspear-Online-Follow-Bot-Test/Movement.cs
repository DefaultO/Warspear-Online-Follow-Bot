using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace Warspear_Online_Follow_Bot_Test
{
    class Movement
    {
        public enum Keys : int
        {
            //Hexadecimal values of the inputs...

            //Letter Keys
            A = 0x41,
            B = 0x42,
            C = 0x43,
            D = 0x44,
            E = 0x45,
            F = 0x46,
            G = 0x47,
            H = 0x48,
            I = 0x49,
            J = 0x4A,
            K = 0x4B,
            L = 0x4C,
            M = 0x4D,
            N = 0x4E,
            O = 0x4F,
            P = 0x50,
            Q = 0x51,
            R = 0x52,
            S = 0x53,
            T = 0x54,
            U = 0x55,
            V = 0x56,
            W = 0x57,
            X = 0x58,
            Y = 0x59,
            Z = 0x59,

            //Num Keys
            Cero = 0x30,
            One = 0x31,
            Two = 0x32,
            Three = 0x33,
            Four = 0x34,
            Five = 0x35,
            Six = 0x36,
            Seven = 0x37,
            Eight = 0x38,
            Nine = 0x39,

            //Special Keys
            Space = 0x20,
            Enter = 0x0D,
            TAB = 0x09,
            ESC = 0x1B,
            LShift = 0xA0,
            RShift = 0xA1,
            LControl = 0xA2,
            RControl = 0xA3,

            VK_LEFT = 0x25,
            VK_UP = 0x26,
            VK_RIGHT = 0x27,
            VK_DOWN = 0x28
        }
        //InputStatesEnum
        public enum InputState : UInt32
        {
            //Hexadecimal values of the input states...

            //Key States
            WM_KEYDOWN = 0x0100,
            WM_KEYUP = 0x0101,

            //Mouse States
            MOUSEEVENTF_LEFTDOWN = 0x0201,
            MOUSEEVENTF_LEFTUP = 0x0202
        }
        public struct Point
        {
            public Point(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
            public int x { get; }
            public int y { get; }

            public override String ToString()
            {
                return "" + x + "-" + y;
            }

            //transform point into hex value for LParam.
            public int ToLParam()
            {
                return (x & 0xffff) + ((y & 0xffff) << 16);
            }

        }
        struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public int winH() { return Right - Left; }
            public int winW() { return Bottom - Top; }
            public Rectangle ToRectangle()
            {
                int winH = Right - Left;
                int winW = Bottom - Top;
                return new Rectangle(Left, Top, winH, winW);
            }
        }

        //Dll functions
        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        static extern bool SendMessage(IntPtr hWnd, UInt32 Msg, int wParam, int lParam);

        //Press and release desired key defined by time
        public static void PressKey(IntPtr mainWindowHandle, Keys key, int time = 500)
        {
            if (mainWindowHandle != IntPtr.Zero)
            {
                PostMessage(mainWindowHandle, (UInt32)InputState.WM_KEYDOWN, (int)key, 0);
                Thread.Sleep(time);
                PostMessage(mainWindowHandle, (UInt32)InputState.WM_KEYUP, (int)key, 0);
            }
            else
            {
                Console.WriteLine("Invalid Process; Cannot Execute Function.");
            }
        }

        //Press key down
        public static void PressKeyDown(IntPtr mainWindowHandle, Keys key)
        {
            if (mainWindowHandle != IntPtr.Zero)
            {
                PostMessage(mainWindowHandle, (UInt32)InputState.WM_KEYDOWN, (int)key, 0);
            }
            else
            {
                Console.WriteLine("Invalid Process; Cannot Execute Function.");
            }
        }

        //Release Key
        public static void KeyUp(IntPtr mainWindowHandle, Keys key)
        {
            if (mainWindowHandle != IntPtr.Zero)
            {
                PostMessage(mainWindowHandle, (UInt32)InputState.WM_KEYUP, (int)key, 0);
            }
            else
            {
                Console.WriteLine("Invalid Process; Cannot Execute Function.");
            }
        }

        //Type given string...
        public static void Type(IntPtr mainWindowHandle, string str, int time = 500)
        {
            if (mainWindowHandle != IntPtr.Zero)
            {
                foreach (char letter in str)
                {
                    Console.WriteLine(letter);
                    if (char.IsLower(letter))
                    {
                        Keys key = (Keys)Enum.Parse(typeof(Keys), char.ToUpper(letter).ToString());
                        PressKeyDown(mainWindowHandle, Keys.LShift);
                        PressKey(mainWindowHandle, key, time);
                        KeyUp(mainWindowHandle, Keys.LShift);
                    }
                    else if (char.IsUpper(letter))
                    {
                        Keys key = (Keys)Enum.Parse(typeof(Keys), letter.ToString());
                        PressKey(mainWindowHandle, key, time);
                    }
                    else if (char.IsDigit(letter))
                    {
                        switch (letter)
                        {
                            case '0':
                                PressKey(mainWindowHandle, Keys.Cero);
                                break;
                            case '1':
                                PressKey(mainWindowHandle, Keys.One);
                                break;
                            case '2':
                                PressKey(mainWindowHandle, Keys.Two);
                                break;
                            case '3':
                                PressKey(mainWindowHandle, Keys.Three);
                                break;
                            case '4':
                                PressKey(mainWindowHandle, Keys.Four);
                                break;
                            case '5':
                                PressKey(mainWindowHandle, Keys.Five);
                                break;
                            case '6':
                                PressKey(mainWindowHandle, Keys.Six);
                                break;
                            case '7':
                                PressKey(mainWindowHandle, Keys.Seven);
                                break;
                            case '8':
                                PressKey(mainWindowHandle, Keys.Eight);
                                break;
                            case '9':
                                PressKey(mainWindowHandle, Keys.Nine);
                                break;
                        }

                    }
                    else if (char.IsWhiteSpace(letter))
                    {
                        PressKey(mainWindowHandle, Keys.Space);
                    }
                    else
                    {
                        Console.WriteLine("Character not supported...");
                    }
                }
            }
            else
            {
                Console.WriteLine("Invalid Process; Cannot Execute Function.");
            }
        }

        //Release All KEys
        public static void AllKeysUp(IntPtr mainWindowHandle)
        {
            if (mainWindowHandle != IntPtr.Zero)
            {
                var values = Enum.GetValues(typeof(Keys)).Cast<Keys>();
                foreach (var key in values)
                {
                    KeyUp(mainWindowHandle, key);
                }
            }
            else
            {
                Console.WriteLine("Invalid Process; Cannot Execute Function.");
            }
        }

        //Click Inputs
        public static void Click(IntPtr mainWindowHandle, Point position, int time = 500)
        {
            if (mainWindowHandle != IntPtr.Zero)
            {
                PostMessage(mainWindowHandle, (UInt32)InputState.MOUSEEVENTF_LEFTDOWN, 1, position.ToLParam());
                Thread.Sleep(time);
                PostMessage(mainWindowHandle, (UInt32)InputState.MOUSEEVENTF_LEFTUP, 1, position.ToLParam());
            }
            else
            {
                Console.WriteLine("Invalid Process; Cannot Execute Function.");
            }
        }

        public static void LeftMouseButtonDown(IntPtr mainWindowHandle, Point position)
        {
            if (mainWindowHandle != IntPtr.Zero)
            {
                PostMessage(mainWindowHandle, (UInt32)InputState.MOUSEEVENTF_LEFTDOWN, 1, position.ToLParam());
            }
            else
            {
                Console.WriteLine("Invalid Process; Cannot Execute Function.");
            }
        }

        public static void LeftMouseButtonUp(IntPtr mainWindowHandle, Point position)
        {
            if (mainWindowHandle != IntPtr.Zero)
            {
                PostMessage(mainWindowHandle, (UInt32)InputState.MOUSEEVENTF_LEFTUP, 1, position.ToLParam());
            }
            else
            {
                Console.WriteLine("Invalid Process; Cannot Execute Function.");
            }
        }
    }
}
