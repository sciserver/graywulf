using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;

namespace Jhu.Graywulf.IO.Tasks
{
    public partial class AsyncFileCopy
    {
        static class Native
        {
            private const string DllKernel32 = "KERNEL32";

            public const int FILE_FLAG_NO_BUFFERING = unchecked((int)0x20000000);
            public const int FILE_FLAG_OVERLAPPED = unchecked((int)0x40000000);
            public const int FILE_FLAG_SEQUENTIAL_SCAN = unchecked((int)0x08000000);

            [DllImport(DllKernel32, SetLastError = true, CharSet = CharSet.Auto, BestFitMapping = false)]
            public static extern SafeFileHandle CreateFile(
                String fileName,
                int desiredAccess,
                System.IO.FileShare shareMode,
                IntPtr securityAttrs,
                System.IO.FileMode creationDisposition,
                int flagsAndAttributes,
                IntPtr templateFile);
        }

    }
}
