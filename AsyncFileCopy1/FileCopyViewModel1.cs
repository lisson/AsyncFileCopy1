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
    class FileCopyViewModel1 : ReactiveObject
    {
        private TaskManager _tmanager;
        public ReactiveCommand<Unit, bool> CopyCommand { get; set; }

        public TaskManager TManager
        {
            get { return _tmanager; }
        }

        public FileCopyViewModel1()
        {
            _tmanager = new TaskManager();
            CopyCommand = ReactiveCommand.CreateFromTask(_tmanager.CopyTask);
        }
    }
}
