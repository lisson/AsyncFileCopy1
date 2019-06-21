using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.Management.Automation;
using ReactiveUI;

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

        private Process _xcopyProcess;

        public FileCopyContainer()
        {
            _xcopyProcess = new Process();
            _xcopyProcess.StartInfo.UseShellExecute = false;
            _xcopyProcess.StartInfo.CreateNoWindow = true;
            _xcopyProcess.StartInfo.FileName = string.Format(@"{0}\xcopy.exe", Environment.SystemDirectory);
            _xcopyProcess.EnableRaisingEvents = true;
            _xcopyProcess.Exited += new EventHandler(OnExit);
            Status = null;
        }

        public void Copy(string src, string dest)
        {
            Console.Out.WriteLine("In Thread: " + Thread.CurrentThread.ManagedThreadId);
            if (Status == TaskStatus.Running)
            {
                return;
            }
            Console.Out.WriteLine(String.Format("Copying {0} to {1}", src, dest));
            _xcopyProcess.StartInfo.Arguments = string.Format(@"/e/Y/i {0} {1}", src, dest);
            _xcopyProcess.Start();
            Status = TaskStatus.Running;
            Console.Out.WriteLine("Task Started.");
        }

        private void OnExit(object sender, System.EventArgs e)
        {
            Console.Out.WriteLine("Task Completed.");
            Status = TaskStatus.RanToCompletion;
            ExitCode = _xcopyProcess.ExitCode;
        }
    }
}
