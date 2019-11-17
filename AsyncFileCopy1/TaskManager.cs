using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using System.Reactive;
using System.Threading;
using System.Diagnostics;

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
                Debug.WriteLine("Status: " + Status);
            });

            _fp.WhenAnyValue(x => x.Total).Subscribe(x =>
            {
                this.Total = x;
                Debug.WriteLine("Total: " + Total);
            });

            _fp.WhenAnyValue(x => x.Transferred).Subscribe(x =>
            {
                this.Transferred = x;
                Debug.WriteLine("Transferred: " + Transferred);
            });

            Total = 1;
            Transferred = 0;
        }

        private TaskStatus? _taskstatus;

        public TaskStatus? Status
        {
            get { return _taskstatus; }
            set { this.RaiseAndSetIfChanged(ref _taskstatus, value); }
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

        public async Task<bool> CopyTask()
        {
            Console.Out.WriteLine("In Thread: " + Thread.CurrentThread.ManagedThreadId);
            string src = @"G:\OpenMapChest_Canada_2018.01.24.zip";
            string dest = @"G:\temp1";
            await Task.Run(() =>
                _fp.Copy(src, dest)
            );
            return true;
        }
    }
}
