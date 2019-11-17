using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.Management.Automation;
using ReactiveUI;
using System.Runtime.InteropServices;

namespace AsyncFileCopy1
{
    class FileCopyContainer : ReactiveObject
    {
        private TaskStatus? _status;

        public TaskStatus? Status
        {
            get { return _status; }
            set { this.RaiseAndSetIfChanged(ref _status, value); }
        }

        private int _exitcode;

        public int ExitCode
        {
            get { return _exitcode; }
            set { this.RaiseAndSetIfChanged(ref _exitcode, value); }
        }

        private long _total;

        public long Total
        {
            get { return _total; }
            set { this.RaiseAndSetIfChanged(ref _total, value); }
        }

        private long _transferred;

        public long Transferred
        {
            get { return _transferred; }
            set { this.RaiseAndSetIfChanged(ref _transferred, value); }
        }



        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CopyFileEx(string lpExistingFileName, string lpNewFileName,
       CopyProgressRoutine lpProgressRoutine, IntPtr lpData, ref Int32 pbCancel,
       CopyFileFlags dwCopyFlags);

        delegate CopyProgressResult CopyProgressRoutine(
        long TotalFileSize,
        long TotalBytesTransferred,
        long StreamSize,
        long StreamBytesTransferred,
        uint dwStreamNumber,
        CopyProgressCallbackReason dwCallbackReason,
        IntPtr hSourceFile,
        IntPtr hDestinationFile,
        IntPtr lpData);

        int pbCancel;

        enum CopyProgressResult : uint
        {
            PROGRESS_CONTINUE = 0,
            PROGRESS_CANCEL = 1,
            PROGRESS_STOP = 2,
            PROGRESS_QUIET = 3
        }

        enum CopyProgressCallbackReason : uint
        {
            CALLBACK_CHUNK_FINISHED = 0x00000000,
            CALLBACK_STREAM_SWITCH = 0x00000001
        }

        [Flags]
        enum CopyFileFlags : uint
        {
            COPY_FILE_FAIL_IF_EXISTS = 0x00000001,
            COPY_FILE_RESTARTABLE = 0x00000002,
            COPY_FILE_OPEN_SOURCE_FOR_WRITE = 0x00000004,
            COPY_FILE_ALLOW_DECRYPTED_DESTINATION = 0x00000008
        }

        private void XCopy(string oldFile, string newFile)
        {
            CopyFileEx(oldFile, newFile, new CopyProgressRoutine(this.CopyProgressHandler), IntPtr.Zero, ref pbCancel, CopyFileFlags.COPY_FILE_RESTARTABLE);
        }

        private CopyProgressResult CopyProgressHandler(long total, long transferred, long streamSize, long StreamByteTrans, uint dwStreamNumber, CopyProgressCallbackReason reason, IntPtr hSourceFile, IntPtr hDestinationFile, IntPtr lpData)
        {
            //Debug.WriteLine("Total: " + total);
            Total = total;
            //Debug.WriteLine("Transferred: " + transferred);
            Transferred = transferred;
            return CopyProgressResult.PROGRESS_CONTINUE;
        }

        public FileCopyContainer()
        {
            Status = null;
            Total = 0;
            Transferred = 0;
        }

        public void Copy(string src, string dest)
        {
            Status = TaskStatus.Running;
            XCopy(src, dest);
            Status = TaskStatus.RanToCompletion;
        }

    }
}
