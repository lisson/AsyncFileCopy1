using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using System.Reactive;
using System.Threading;

namespace AsyncFileCopy1
{
    class TaskManager : ReactiveObject
    {
        private FileCopyContainer _fp;
        
        public TaskManager()
        {
            _fp = new FileCopyContainer();
            _fp.WhenAnyValue(x => x.Status).Subscribe(s =>
            {
                this.Status = s;
            });

        }

        private TaskStatus? _taskstatus;

        public TaskStatus? Status
        {
            get { return _taskstatus; }
            set { this.RaiseAndSetIfChanged(ref _taskstatus, value); }
        }

        public void CopyTask()
        {
            Console.Out.WriteLine("In Thread: " + Thread.CurrentThread.ManagedThreadId);
            string src = @"C:\";
            string dest = @"D:\";
            _fp.Copy(src, dest);
        }
    }
}
