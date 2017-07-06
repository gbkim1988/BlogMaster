using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlogMaster.U
{
    public class AsyncHelperUtil
    {
        public static Task Delay(int millis)
        {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
            Timer timer = new Timer(_ => tcs.SetResult(null), null, millis, Timeout.Infinite);
            tcs.Task.ContinueWith(delegate { timer.Dispose(); });
            return tcs.Task;
        }
    }
}
