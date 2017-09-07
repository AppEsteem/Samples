using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;

namespace SRCL
{
    sealed class Init : IDisposable
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate int SRCLINIT();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void SRCLTERM();

        static Init()
        {
            try
            {
                dllPath = Path.GetTempFileName();
                string[] resourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
                string dllResourceName = "";
                if (IntPtr.Size == 4)
                {
                    // 32 bit
                    foreach (string name in resourceNames)
                    {
                        // The dll name must be srcl100.dll or srcl110.dll or srcl120.dll or srcl140.dll or srcl141.dll
                        if (name.Length <= 11 + 2) // 11 is strlen("srclXXX.dll") plus 2 because of 2 '.' for project and folder
                            continue;
                        if (!name.EndsWith(".dll") || name.Substring(name.Length - 12, 5) != ".srcl")
                            continue;
                        string dll_version = name.Substring(name.Length - 7, 3);
                        if (dll_version != "100" && dll_version != "110" && dll_version != "120" && dll_version != "140" && dll_version != "141")
                            continue;
                        dllResourceName = name;
                        break;
                    }
                }
                else
                {
                    // 64 bit
                    foreach (string name in resourceNames)
                    {
                        // The dll name must be srcl10064.dll or srcl11064.dll or srcl12064.dll or srcl14064.dll or srcl14164.dll
                        if (name.Length <= 13 + 2) // 11 is strlen("srclXXX64.dll") plus 2 because of 2 '.' for project and folder
                            continue;
                        if (!name.EndsWith("64.dll") || name.Substring(name.Length - 14, 5) != ".srcl")
                            continue;
                        string dll_version = name.Substring(name.Length - 9, 3);
                        if (dll_version != "100" && dll_version != "110" && dll_version != "120" && dll_version != "140" && dll_version != "141")
                            continue;
                        dllResourceName = name;
                        break;
                    }
                }

                if (dllResourceName.Length != 0)
                {
                    using (var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(dllResourceName))
                    {
                        using (var file = new FileStream(dllPath, FileMode.OpenOrCreate, FileAccess.Write))
                        {
                            resource.CopyTo(file);
                        }
                    }

                    dll = NativeMethods.LoadLibraryW(dllPath);
                    if (dll != IntPtr.Zero)
                    {
                        IntPtr pInit = NativeMethods.GetProcAddress(dll, "srcl_1");
                        if (pInit != IntPtr.Zero)
                        {
                            SRCLINIT srcl_init = (SRCLINIT)Marshal.GetDelegateForFunctionPointer(pInit, typeof(SRCLINIT));
                            srcl_init(); // best effort
                        }
                    }
                }
            }
            catch (Exception) { }
        }

        ~Init()
        {
            Dispose();
        }

        // Implement IDisposable
        public void Dispose()
        {
            if (dll != IntPtr.Zero)
            {
                try
                {
                    IntPtr pTerm = NativeMethods.GetProcAddress(dll, "srcl_2");
                    if (pTerm != IntPtr.Zero)
                    {
                        SRCLTERM srcl_term = (SRCLTERM)Marshal.GetDelegateForFunctionPointer(pTerm, typeof(SRCLTERM));
                        srcl_term();
                    }
                    NativeMethods.FreeLibrary(dll);
                    dll = IntPtr.Zero;
                }
                catch (Exception) { }
            }

            try { File.Delete(dllPath); }
            catch (Exception) { }

            // Prevent further finalization
            GC.SuppressFinalize(this);
        }

        private static IntPtr dll = IntPtr.Zero;
        private static string dllPath;
    }

    internal static class NativeMethods
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LoadLibraryW([MarshalAs(UnmanagedType.LPWStr)] string dllToLoad);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)] string procedureName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool FreeLibrary(IntPtr hModule);
    }
}
