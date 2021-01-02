using System;

namespace Warspear_Online_Follow_Bot_Test
{
    public class Warspear
    {
        public IntPtr Handle { get; set; }
        public uint ProcessId { get; set; }
        public string WindowTitle { get; set; }
        public bool IsOwner { get; set; }
    }
}
