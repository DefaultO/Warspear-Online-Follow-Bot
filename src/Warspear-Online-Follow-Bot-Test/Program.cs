using Memory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Console = Colorful.Console;

namespace Warspear_Online_Follow_Bot_Test
{
	class WS
	{
		// Windows structs, different formats of data we'll be receiving
		// LayoutKind.Sequential to store the fields correctly ordered in memory
		// Pack=1 if we need to read a byte at a time
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct SYSTEM_HANDLE_INFORMATION
		{ // Returned data from SystemHandleInformation, a handle
			public int ProcessID;
			public byte ObjectTypeNumber;
			public byte Flags; // 0x01 = PROTECT_FROM_CLOSE, 0x02 = INHERIT
			public ushort Handle;
			public int Object_Pointer;
			public UInt32 GrantedAccess;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct OBJECT_BASIC_INFORMATION
		{ // Information Class 0
			public int Attributes;
			public int GrantedAccess;
			public int HandleCount;
			public int PointerCount;
			public int PagedPoolUsage;
			public int NonPagedPoolUsage;
			public int Reserved1;
			public int Reserved2;
			public int Reserved3;
			public int NameInformationLength;
			public int TypeInformationLength;
			public int SecurityDescriptorLength;
			public System.Runtime.InteropServices.ComTypes.FILETIME CreateTime;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct OBJECT_NAME_INFORMATION
		{ // Information Class 1
			public UNICODE_STRING Name;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct UNICODE_STRING
		{
			public ushort Length;
			public ushort MaximumLength;
			public IntPtr Buffer;
		}
	}

	class Program
	{
		[DllImport("ntdll.dll")]
		// NtQuerySystemInformation gives us data about all the handlers in the system
		private static extern uint NtQuerySystemInformation(uint SystemInformationClass, IntPtr SystemInformation,
			int SystemInformationLength, ref int nLength);

		[DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		// DuplicateHandle duplicates a handle from an external process to ours
		// hSourceProcessHandle is the process we duplicate from, hSourceHandle is the handle we duplicate
		// hTargetProcessHandle is the process we duplicate to, lpTargetHandle is a pointer to a var that receives the new handler
		private static extern bool DuplicateHandle(IntPtr hSourceProcessHandle, ushort hSourceHandle, IntPtr hTargetProcessHandle,
			out IntPtr lpTargetHandle, uint dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, uint dwOptions);

		[DllImport("kernel32.dll")]
		// dwDesiredAccess sets the process access rights (docs.microsoft.com/en-us/windows/desktop/ProcThread/process-security-and-access-rights)
		// if bInheritHandle is true, processes created by this process will inherit the handle (we don't need this, maybe just set it as a bool)
		// dwProcessId is the PID of the process we want to open with those access rights
		public static extern IntPtr OpenProcess(uint dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

		[DllImport("kernel32.dll")]
		// Returns us a handle to the current process
		public static extern IntPtr GetCurrentProcess();

		[DllImport("ntdll.dll")]
		// Retrieves information about an object
		// Handle is the object's handle we're getting information from
		// ObjectInformationClass is the type of information we want; ObjectBasicInformation/ObjectTypeInformation, undocumented ObjectNameInformation?
		// ObjectInformation is the buffer where the data is returned to, ObjectInformationLength is the size of that buffer
		// returnLength is a variable where NtQueryObject writes the size of the information returned to us
		public static extern int NtQueryObject(IntPtr Handle, int ObjectInformationClass, IntPtr ObjectInformation,
			int ObjectInformationLength, ref int returnLength);

		[DllImport("kernel32.dll")]
		// Closes a handle
		public static extern int CloseHandle(IntPtr hObject);

		[DllImport("user32.dll")]
		static extern int SetWindowText(IntPtr hWnd, string text);

		[DllImport("user32.dll", SetLastError = true)]
		static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		[DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
		// Find window by Caption only. Note you must pass IntPtr.Zero as the first parameter.
		static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

		[DllImport("user32.dll", SetLastError = true)]
		static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

		public static Process FindProcess(IntPtr yourHandle)
		{
			foreach (Process p in Process.GetProcesses())
			{
				if (p.Handle == yourHandle)
				{
					return p;
				}
			}

			return null;
		}

		private static bool Is64Bits()
		{
			return Marshal.SizeOf(typeof(IntPtr)) == 8 ? true : false;
		}

		private static void CloseMutex(WS.SYSTEM_HANDLE_INFORMATION handle)
		{
			IntPtr targetHandle;
			// DUPLICATE_CLOSE_SOURCE = 0x1
			// GetCurrentProcess(), out targetHandle ======> Set target process to null for success
			if (!DuplicateHandle(Process.GetProcessById(handle.ProcessID).Handle, handle.Handle, IntPtr.Zero, out targetHandle, 0, false, 0x1))
			{
				// Failed
			}
			Console.WriteLine("Mutex was killed");
		}

		private static string ViewHandleName(WS.SYSTEM_HANDLE_INFORMATION shHandle, int pID)
		{
			// handleInfoStruct is the struct that contains data about our handle
			// targetProcess is the process where the handle resides
			// DUP_HANDLE (0x40) might also work
			IntPtr sourceProcessHandle = OpenProcess(0x1F0FFF, false, pID);
			IntPtr targetHandle = IntPtr.Zero;

			// We create a duplicate of the handle so that we can query more information about it
			if (!DuplicateHandle(sourceProcessHandle, shHandle.Handle, GetCurrentProcess(), out targetHandle, 0, false, 0x2))
			{
				return null;
			}

			// Buffers that the query results get sent to
			IntPtr basicQueryData = IntPtr.Zero;

			// Query result structs
			WS.OBJECT_BASIC_INFORMATION basicInformationStruct = new WS.OBJECT_BASIC_INFORMATION();
			WS.OBJECT_NAME_INFORMATION nameInformationStruct = new WS.OBJECT_NAME_INFORMATION();

			basicQueryData = Marshal.AllocHGlobal(Marshal.SizeOf(basicInformationStruct));

			int nameInfoLength = 0; // Size of information returned to us
			NtQueryObject(targetHandle, 0, basicQueryData, Marshal.SizeOf(basicInformationStruct), ref nameInfoLength);

			// Insert buffer data into a struct and free the buffer
			basicInformationStruct = (WS.OBJECT_BASIC_INFORMATION)Marshal.PtrToStructure(basicQueryData, basicInformationStruct.GetType());
			Marshal.FreeHGlobal(basicQueryData);

			// The basicInformationStruct contains data about the name's length
			// TODO: We could probably skip querying for OBJECT_BASIC_INFORMATION
			nameInfoLength = basicInformationStruct.NameInformationLength;

			// Allocate buffer for the name now that we know its size
			IntPtr nameQueryData = Marshal.AllocHGlobal(nameInfoLength);

			// Object information class: 1
			// If it's incorrect, it returns STATUS_INFO_LENGTH_MISMATCH (0xc0000004)
			int result;
			while ((uint)(result = NtQueryObject(targetHandle, 1, nameQueryData, nameInfoLength, ref nameInfoLength)) == 0xc0000004)
			{
				Marshal.FreeHGlobal(nameQueryData);
				nameQueryData = Marshal.AllocHGlobal(nameInfoLength);
			}
			nameInformationStruct = (WS.OBJECT_NAME_INFORMATION)Marshal.PtrToStructure(nameQueryData, nameInformationStruct.GetType());

			IntPtr handlerName;

			if (Is64Bits())
			{
				handlerName = new IntPtr(Convert.ToInt64(nameInformationStruct.Name.Buffer.ToString(), 10) >> 32);
			}
			else
			{
				handlerName = nameInformationStruct.Name.Buffer;
			}

			if (handlerName != IntPtr.Zero)
			{
				byte[] baTemp2 = new byte[nameInfoLength];
				try
				{
					Marshal.Copy(handlerName, baTemp2, 0, nameInfoLength);
					return Marshal.PtrToStringUni(Is64Bits() ? new IntPtr(handlerName.ToInt64()) : new IntPtr(handlerName.ToInt32()));
				}
				catch (AccessViolationException)
				{
					return null;
				}
				finally
				{
					Marshal.FreeHGlobal(nameQueryData);
					CloseHandle(targetHandle);
				}
			}
			return null;
		}

		// Closes all needed mutant handles in given process
		private static void CloseProcessHandles(int pID)
		{
			// We need to remove handlers from the last process
			Console.WriteLine("Starting handle magic...");
			Console.WriteLine("Querying system handle information...");
			int nLength = 0;
			IntPtr handlePointer = IntPtr.Zero;
			int sysInfoLength = 0x10000; // How much to allocate to returned data
			IntPtr infoPointer = Marshal.AllocHGlobal(sysInfoLength);
			// 0x10 = SystemHandleInformation, an undocumented SystemInformationClass
			uint result; // NtQuerySystemInformation won't give us the correct buffer size, so we guess it
						 // Assign result of NtQuerySystemInformation to this variable and check if the buffer size is correct
						 // If it's incorrect, it returns STATUS_INFO_LENGTH_MISMATCH (0xc0000004)

			while ((result = NtQuerySystemInformation(0x10, infoPointer, sysInfoLength, ref nLength)) == 0xc0000004)
			{
				sysInfoLength = nLength;
				Marshal.FreeHGlobal(infoPointer);
				infoPointer = Marshal.AllocHGlobal(nLength);
			}

			byte[] baTemp = new byte[nLength];
			// Copy the data from unmanaged memory to managed 1-byte uint array
			Marshal.Copy(infoPointer, baTemp, 0, nLength);
			// Do we even need the two statements above??? Look into this later.

			long sysHandleCount = 0; // How many handles there are total
			if (Is64Bits())
			{
				sysHandleCount = Marshal.ReadInt64(infoPointer);
				handlePointer = new IntPtr(infoPointer.ToInt64() + 8); // Points in bits at the start of a handle
			}
			else
			{
				sysHandleCount = Marshal.ReadInt32(infoPointer);
				handlePointer = new IntPtr(infoPointer.ToInt32() + 4); // Ignores 4 first bits instead of 8
			}

			Console.WriteLine("Query received, processing the " + sysHandleCount + " results.");

			WS.SYSTEM_HANDLE_INFORMATION handleInfoStruct; // The struct to hold info about a single handler

			List<WS.SYSTEM_HANDLE_INFORMATION> handles = new List<WS.SYSTEM_HANDLE_INFORMATION>();
			for (long i = 0; i < sysHandleCount; i++)
			{ // Iterate over handle structs in the handle struct list
				handleInfoStruct = new WS.SYSTEM_HANDLE_INFORMATION();
				if (Is64Bits())
				{
					handleInfoStruct = (WS.SYSTEM_HANDLE_INFORMATION)Marshal.PtrToStructure(handlePointer, handleInfoStruct.GetType()); // Convert to struct
					handlePointer = new IntPtr(handlePointer.ToInt64() + Marshal.SizeOf(handleInfoStruct) + 8); // point 8 bits forward to the next handle
				}
				else
				{
					handleInfoStruct = (WS.SYSTEM_HANDLE_INFORMATION)Marshal.PtrToStructure(handlePointer, handleInfoStruct.GetType());
					handlePointer = new IntPtr(handlePointer.ToInt64() + Marshal.SizeOf(handleInfoStruct));
				}

				if (handleInfoStruct.ProcessID != pID)
				{
					// Check if current handler is from Growtopia
					continue; // If it's not from Growtopia, just skip it
				}

				string handleName = ViewHandleName(handleInfoStruct, pID);
				if (handleName != null && handleName.StartsWith(@"\Sessions\") && handleName.EndsWith(@"\BaseNamedObjects\E49C5EC071B44f14B2"))
				{
					handles.Add(handleInfoStruct);
					Console.WriteLine("PID {0,7} Pointer {1,12} Type {2,4} Name {3}", handleInfoStruct.ProcessID.ToString(),
																					  handleInfoStruct.Object_Pointer.ToString(),
																					  handleInfoStruct.ObjectTypeNumber.ToString(),
																					  handleName);
				}
				else
				{
					continue; // This is not a handle we're looking for
				}

			}

			Console.WriteLine("Closing mutexes?");
			foreach (WS.SYSTEM_HANDLE_INFORMATION handle in handles)
			{
				CloseMutex(handle);
			}

			Console.WriteLine("Query finished, " + sysHandleCount + " results processed.");
			Console.WriteLine("Handle closed.");
		}

		class PlayerInfo
		{
			public int Health { get; set; }
			public int maxHealth { get; set; }
			public int Mana { get; set; }
			public int maxMana { get; set; }
			public int posX { get; set; }
			public int posY { get; set; }
			public int desX { get; set; }
			public int desY { get; set; }
			public int cursorFlag { get; set; }
		}

		static int GetListIndex(List<uint> pIDs, uint pid)
		{
			int counter = 0;
			foreach (uint pit in pIDs)
			{
				if (pit == pid)
				{
					return counter;
				}
				counter++;
			}
			return -1;
		}

		static int GetListIndex(List<IntPtr> windowHandles, IntPtr handle)
		{
			int counter = 0;
			foreach (IntPtr hendle in windowHandles)
			{
				if (hendle == handle)
				{
					return counter;
				}
				counter++;
			}
			return -1;
		}

		static Random random = new Random((int)DateTime.Now.Date.Ticks);
		private static void StatusUpdate()
		{
			Console.CursorVisible = false;

			var whiteSpace = new StringBuilder();
			whiteSpace.Append(' ', 10);
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			var randomWord = new string(Enumerable.Repeat(chars, random.Next(10)).Select(s => s[random.Next(s.Length)]).ToArray());
			while (true)
			{
				Console.SetCursorPosition(0, 0);
				var sb = new StringBuilder();
				sb.AppendLine($"Program Status:{whiteSpace}");
				sb.AppendLine("-------------------------------");
				sb.AppendLine($"Last Updated: {DateTime.Now}{whiteSpace}");
				sb.AppendLine($"Random Word: {randomWord}{whiteSpace}");
				sb.AppendLine("-------------------------------");
				Console.Write(sb);
				Thread.Sleep(1000);
			}
		}

		static int GetOwnerIndex(List<Player> players)
		{
			int index = 0;
			foreach (Player player in players)
			{
				if (player.Name == "Uangeu")
				{
					return index;
				}
				index++;
			}
			return -1;
		}

		static List<Player> players = new List<Player>();
		static List<Warspear> GameClients = new List<Warspear>();

		static void Main(string[] args)
		{
			Console.CursorVisible = false;
			Console.WindowHeight = 30;
			Console.WindowWidth = 28 * 2;
			Console.SetBufferSize(28 * 2, 30);

			string warspearPath = @"C:\Users\breit\AppData\Local\Warspear Online\warspear.exe";

			/*Process[] localByName = Process.GetProcessesByName("warspear");
			if (localByName.Length == 5)
            {
				foreach (Process process in localByName)
                {
					Warspear warspear1 = new Warspear()
					{
						Handle = process.MainWindowHandle,
						ProcessId = (uint)process.Id,
						WindowTitle = process.MainWindowTitle,
						IsOwner = false // We identify the Owner by whose Cursor Position changes first.
					};
					GameClients.Add(warspear1);
				}
            }
			else if (localByName.Length < 5)
            {
				for (int id = localByName.Length; id < 5; id++)
				{
					Process warspear = new Process();
					IntPtr foundWindowHandle = IntPtr.Zero;
					uint processID = 0;

					warspear.StartInfo.FileName = warspearPath;
					Process.Start(warspearPath);
					Thread.Sleep(1000);
					foundWindowHandle = FindWindow("Warspear", "Warspear Online");
					SetWindowText(foundWindowHandle, (id + 1).ToString());
					GetWindowThreadProcessId(foundWindowHandle, out processID);

					if (foundWindowHandle != IntPtr.Zero && processID != 0)
					{
						CloseProcessHandles((int)processID);
						Warspear warspear1 = new Warspear()
						{
							Handle = foundWindowHandle,
							ProcessId = processID,
							WindowTitle = (id + 1).ToString(),
							IsOwner = false // We identify the Owner by whose Cursor Position changes first.
						};
						GameClients.Add(warspear1);
					}
				}
			}

			Console.ReadLine();

			// Memory Reading
			Thread memoryReading = new Thread(memoryReadingThread);
			memoryReading.Start();

			Mem mem = new Mem();

			while (true)
			{
				foreach (Warspear client in GameClients)
                {
					if (client.WindowTitle == "1") { }
					else
					{
						mem.OpenProcess((int)client.ProcessId);

						if (mem.ReadByte("warspear.exe+0x19708A") == 0x66)
						{
							/// Patch Cursor Set-back out

							/// Original Bytes: 66 89 41 08
							/// X
							mem.WriteMemory("warspear.exe+0x19708A", "bytes", "90 90 90 90");
							/// Original Bytes: 66 89 41 0A
							/// Y
							mem.WriteMemory("warspear.exe+0x1970B1", "bytes", "90 90 90 90");
						}

						if (GetOwnerIndex(players) != -1)
						{
							Console.WriteLine("Test");
							// Write Coordinates to the Cursor
							mem.WriteMemory(Patchables.CursorX.Get(), "int", players[GetOwnerIndex(players)].PosX.ToString());
							mem.WriteMemory(Patchables.CursorY.Get(), "int", players[GetOwnerIndex(players)].PosX.ToString());

							// Start moving by pressing enter
							Movement.PressKey(client.Handle, Movement.Keys.Enter, 50);
						}
					}
				}
				Thread.Sleep(100);
			}

			// Actual Bot Follow Logic
			/*new Thread(() =>
			{
				while (true)
				{
					System.Threading.Tasks.Parallel.ForEach(GameClients, ws =>
					{
						Mem m = new Mem();
						if (m.OpenProcess((int)ws.ProcessId))
						{

						}
						else
                        {
							Thread.CurrentThread.Abort();
						}
						Thread.Sleep(5);
					});
				}
			}).Start();

			/*List<Warspear> GameClients = new List<Warspear>();
			for (int i = 0; i < 5; i++)
            {
				while (FindWindow("Warspear", "Warspear Online") == IntPtr.Zero)
				{
					try
                    {
                        Process.Start(warspearPath);
						Thread.Sleep(1000);

						IntPtr ptr = FindWindow("Warspear", "Warspear Online");
						uint processId = 0;
						GetWindowThreadProcessId(ptr, out processId);
						if (processId != 0)
						{
							Mem memory = new Mem();
							memory.OpenProcess((int)processId);
							try
							{
								byte[] nameBytes = memory.ReadBytes(Patchables.Name.Get(), 100);
								string nameString = Encoding.Unicode.GetString(nameBytes);
								if (!String.IsNullOrEmpty(nameString))
								{
									SetWindowText(ptr, nameString);
									bool isOwner = false;
									if (GameClients.Count == 0) isOwner = true;
									Warspear warspear = new Warspear()
									{
										Handle = ptr,
										WindowTitle = nameString,
										IsOwner = isOwner
									};
									GameClients.Add(warspear);
								}
								CloseProcessHandles((int)processId);
								Thread.Sleep(1000);
							}
							catch
							{
								// Do Nothing
							}

							// Console.WriteLine(nameString);
						}
					}
					catch
                    {

                    }
					
					Thread.Sleep(100);
				}
			}
			
			IntPtr test = FindWindow("Warspear", "Larewiped");

			uint processID; GetWindowThreadProcessId(test, out processID);
			Mem mem = new Mem();
			if(mem.OpenProcess((int)processID))
            {
				byte[] nameBytes = mem.ReadBytes(Patchables.Name.Get(), 100);
				string nameString = Encoding.Unicode.GetString(nameBytes);
				SetWindowText(test, nameString);

			}

			// Original Bytes: 66 89 41 08
			mem.WriteMemory("warspear.exe+0x19708A", "bytes", "90 90 90 90");
			// Original Bytes: 66 89 41 0A
			mem.WriteMemory("warspear.exe+0x1970B1", "bytes", "90 90 90 90");

			Console.SetCursorPosition(0, 0);

			for (int i = 0; i < 28; i++)
			{
				for (int j = 0; j < 28 * 2; j++)
                {
					Console.Write("0");
                }
            }

			Console.ForegroundColor = System.Drawing.Color.White;
			for (int y = 0; y < 28; y++)
            {
				for (int x = 0; x < 28; x++)
				{
					mem.WriteMemory(Patchables.CursorX.Get(), "int", x.ToString());
					mem.WriteMemory(Patchables.CursorY.Get(), "int", y.ToString());
					Thread.Sleep(50);
					int cursorFlag = mem.ReadInt(Patchables.CursorFlag.Get());
					if (cursorFlag == 15)
					{
						Console.BackgroundColor = System.Drawing.Color.DarkSlateGray;
					}
					else if (cursorFlag == 1 || cursorFlag == 2 || cursorFlag == 3 || cursorFlag == 4)
                    {
						Console.BackgroundColor = System.Drawing.Color.DimGray;
					}
					else if (cursorFlag == 8)
                    {
						Console.BackgroundColor = System.Drawing.Color.Green;
					}
					else
					{
						if (x == mem.Read2Byte(Patchables.PosX.Get()) && y == mem.Read2Byte(Patchables.PosY.Get()))
                        {
							Console.BackgroundColor = System.Drawing.Color.BlueViolet;
						}
						else
                        {
							Console.BackgroundColor = System.Drawing.Color.Black;
						}
					}
					Console.SetCursorPosition(x * 2, y);
					if (cursorFlag < 10)
					{
						Console.Write("0" + cursorFlag);
					}
					else
					{
						Console.Write(cursorFlag);
					}
				}
			}
			mem.WriteMemory("warspear.exe+0x19708A", "bytes", "66 89 41 08");
			mem.WriteMemory("warspear.exe+0x1970B1", "bytes", "66 89 41 0A");*/

			Console.ReadLine();

			// Console.WriteLine("How many game instances do you want to run?");
			int numberOfInstances = 5;

			/*if (!int.TryParse(Console.ReadLine(), out numberOfInstances))
			{
				Console.WriteLine("Failed to convert readline string to int. That's not a number!");
				Console.ReadLine();
				Environment.Exit(0);
			}*/

			List<IntPtr> windowHandles = new List<IntPtr>();
			List<uint> pIDs = new List<uint>();
			for (int id = 0; id < numberOfInstances; id++)
			{
				Console.Clear();
				Process warspear = new Process();
				warspear.StartInfo.FileName = warspearPath;
				Process.Start(warspearPath);
				Thread.Sleep(1000);

				IntPtr foundWindowHandle = IntPtr.Zero;
				foundWindowHandle = FindWindow("Warspear", "Warspear Online");
				windowHandles.Add(foundWindowHandle);
				SetWindowText(foundWindowHandle, (id + 1).ToString());
				uint processID = 0;
				GetWindowThreadProcessId(foundWindowHandle, out processID);
				pIDs.Add(processID);
				CloseProcessHandles((int)processID);
			}

			Console.ReadLine();

			List<PlayerInfo> playerinfo = new List<PlayerInfo>();
			while (true)
			{
				int counter = 0;
				foreach (uint pid in pIDs)
				{
					counter = counter + 1;
					Mem mem = new Mem();
					mem.OpenProcess((int)pid);
					PlayerInfo pi = new PlayerInfo();

					// Console.Clear();
					pi.Health = mem.ReadInt("warspear.exe+0x005A8BF8,0,10,D4,4,B8,4C,9C");
					pi.maxHealth = mem.ReadInt("warspear.exe+0x005A8BF8,0,10,D4,4,B8,4C,A0");
					// Console.WriteLine($"HP: {currentHealth} / {maxHealth}");
					pi.Mana = mem.ReadInt("warspear.exe+0x005A8BF8,0,10,D4,4,B8,4C,A4");
					pi.maxMana = mem.ReadInt("warspear.exe+0x005A8BF8,0,10,D4,4,B8,4C,A8");
					// Console.WriteLine($"MP: {currentMana} / {maxMana}");
					pi.posX = mem.Read2Byte("warspear.exe+0x005A8BF8,0,10,D4,4,B8,4C,B0");
					pi.posY = mem.Read2Byte("warspear.exe+0x005A8BF8,0,10,D4,4,B8,4C,B2");
					// Console.WriteLine($"Position: {posX} | {posY}");
					pi.desX = mem.Read2Byte("warspear.exe+0x005A8BF8,0,10,D4,4,B8,4C,B4");
					pi.desY = mem.Read2Byte("warspear.exe+0x005A8BF8,0,10,D4,4,B8,4C,B6");
					// Console.WriteLine($"Destination: {desX} | {desY}");
					pi.cursorFlag = mem.ReadInt(Patchables.CursorFlag.Get());

					if (playerinfo.Count < counter)
					{
						playerinfo.Add(pi);
					}
					else
					{
						playerinfo[counter - 1] = pi;
					}
				}

				foreach (IntPtr handle in windowHandles)
				{
					if (GetListIndex(windowHandles, handle) == 0) continue;
					Mem mem = new Mem();
					uint processId = 0;
					GetWindowThreadProcessId(handle, out processId);
					mem.OpenProcess((int)processId);
					// Original Bytes: 66 89 41 08
					mem.WriteMemory("warspear.exe+0x19708A", "bytes", "90 90 90 90");
					// Original Bytes: 66 89 41 0A
					mem.WriteMemory("warspear.exe+0x1970B1", "bytes", "90 90 90 90");

					if (mem.ReadInt(Patchables.CursorTimer.Get()) != 0)
					{
						//							     ! Attack															! Pick-Up
						if ((playerinfo[0].cursorFlag == 8 || playerinfo[GetListIndex(windowHandles, handle)].cursorFlag == 10) && (playerinfo[0].posX != playerinfo[0].desX || playerinfo[0].posY != playerinfo[0].desY))
						{
							mem.WriteMemory(Patchables.CursorX.Get(), "int", playerinfo[0].desX.ToString());
							mem.WriteMemory(Patchables.CursorY.Get(), "int", playerinfo[0].desY.ToString());
						}
						//																	   ! Loading (Changing Room)
						else if (playerinfo[GetListIndex(windowHandles, handle)].cursorFlag == 16)
						{
							Thread.Sleep(2000);
						}
						else
						{
							mem.WriteMemory(Patchables.CursorX.Get(), "int", playerinfo[0].posX.ToString());
							mem.WriteMemory(Patchables.CursorY.Get(), "int", playerinfo[0].posY.ToString());
						}
						Movement.PressKey(windowHandles[GetListIndex(windowHandles, handle)], Movement.Keys.Enter, 50);
					}
				}
				Thread.Sleep(15);
			}
		}
	}
}

				/*if (playerinfo[0].desX != playerinfo[0].posX || playerinfo[0].desY != playerinfo[0].posY || playerinfo[0].posX != lastPosX || playerinfo[0].posY != lastPosY)
				{
					foreach (IntPtr handle in windowHandles)
                    {
						if (GetListIndex(windowHandles, handle) == 0) continue;

						int differenceX = playerinfo[0].posX - playerinfo[GetListIndex(windowHandles, handle)].posX;
						int differenceY = playerinfo[0].posY - playerinfo[GetListIndex(windowHandles, handle)].posY;
						if (differenceX > 0)
						{
							// Go Right
							int currentX = playerinfo[GetListIndex(windowHandles, handle)].desX;

							Movement.PressKey(handle, Movement.Keys.VK_RIGHT, 150);
						}
						else if (differenceX == 0)
                        {

                        }
						else
						{
							// Go Left
							Movement.PressKey(handle, Movement.Keys.VK_LEFT, 150);
						}

						if (differenceY > 0)
						{
							// Go Down
							Movement.PressKey(handle, Movement.Keys.VK_DOWN, 150);
						}
						else if (differenceY == 0)
						{

						}
						else
						{
							// Go Up
							Movement.PressKey(handle, Movement.Keys.VK_UP, 150);
						}
						Movement.PressKey(handle, Movement.Keys.Enter);

						lastPosY = playerinfo[0].posY;
						lastPosX = playerinfo[0].posX;
					}
				}
			// Thread.Sleep(10);
		}

		static Mem m = new Mem();
		static void memoryReadingThread()
        {
			while (true)
			{
				foreach (Warspear client in GameClients)
				{
					if (m.OpenProcess((int)client.ProcessId))
					{
						try
						{
							byte[] nameInBytes = m.ReadBytes(Patchables.Name.Get(), 100); // We have to do this, because Memory.dll is pants and can't read Unicode Strings on it's own. If you see this @erfg12, why not add it to the ReadString Function? Also there should be a ReadBool Function too. No Pressure tho!
							string nameAsString = Encoding.Unicode.GetString(nameInBytes);

							Player player = new Player();

							// Player Structure
							player.Name = nameAsString;
							player.HP = m.ReadInt(Patchables.HP.Get());
							player.MaxHP = m.ReadInt(Patchables.MaxHP.Get());
							player.MP = m.ReadInt(Patchables.MP.Get());
							player.MaxMP = m.ReadInt(Patchables.MaxMP.Get());
							player.PosX = m.ReadInt(Patchables.PosX.Get());
							player.PosY = m.ReadInt(Patchables.PosY.Get());
							player.DesX = m.ReadInt(Patchables.DesX.Get());
							player.DesY = m.ReadInt(Patchables.DesY.Get());
							player.DistanceToDes = m.ReadInt(Patchables.DistanceToDestination.Get());
							player.IsMoving = m.ReadInt(Patchables.IsMoving.Get());

							// Cursor Structure
							player.CursorX = m.ReadInt(Patchables.CursorX.Get());
							player.CursorY = m.ReadInt(Patchables.CursorY.Get());
							player.CursorFlag = m.ReadInt(Patchables.CursorFlag.Get());
							player.CursorTImer = m.ReadInt(Patchables.CursorTimer.Get());

							int index = 0;
							if (players.Count == 5)
							{
								foreach (Player plyr in players)
								{
									if (plyr.Name == nameAsString)
									{
										players[index] = player;
									}
									index++;
								}
							}
							else
							{
								players.Add(player);
							}
						}
						catch
						{
							// Do Nothing, shit happens. Keep going Buddy.
							Console.WriteLine("Test2");
						}
					}
					else
					{
						// Process not Running or invalid ProcessId, stop the count!
						Thread.CurrentThread.Abort();
					}
				}
				Thread.Sleep(100);
			}
		}
    }
}
*/