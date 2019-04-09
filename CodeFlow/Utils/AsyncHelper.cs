using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFlow.Utils
{
    public static class AsyncHelper
    {
        public static R RunSync<R>(Func<R> func)
        {
#pragma warning disable VSTHRD104 // Offer async methods
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            return Microsoft.VisualStudio.Shell.ThreadHelper.JoinableTaskFactory.Run(async () => func());
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
#pragma warning restore VSTHRD104 // Offer async methods
        }
        public static R RunSync<R>(Func<Task<R>> func)
        {
#pragma warning disable VSTHRD104 // Offer async methods
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            return Microsoft.VisualStudio.Shell.ThreadHelper.JoinableTaskFactory.Run(() => func());
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
#pragma warning restore VSTHRD104 // Offer async methods
        }
        public static R RunSyncUI<R>(Func<R> func)
        {
#pragma warning disable VSTHRD104 // Offer async methods
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            return Microsoft.VisualStudio.Shell.ThreadHelper.JoinableTaskFactory.Run(async () => 
            {
                await Microsoft.VisualStudio.Shell.ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                return func();
            });
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
#pragma warning restore VSTHRD104 // Offer async methods
        }
        public static void RunSyncUI(Action action)
        {
#pragma warning disable VSTHRD104 // Offer async methods
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            Microsoft.VisualStudio.Shell.ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                await Microsoft.VisualStudio.Shell.ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                action();
            });
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
#pragma warning restore VSTHRD104 // Offer async methods
        }
    }
}
