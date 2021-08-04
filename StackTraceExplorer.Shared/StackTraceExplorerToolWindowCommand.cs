using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace StackTraceExplorer
{
    [Command(PackageGuids.guidStackTraceExplorerToolWindowPackageCmdSetString, PackageIds.StackTraceExplorerToolWindowCommandId)]
    internal sealed class StackTraceExplorerToolWindowCommand : BaseCommand<StackTraceExplorerToolWindowCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
            => await StackTraceExplorerToolWindow.ShowAsync();
    }
}
