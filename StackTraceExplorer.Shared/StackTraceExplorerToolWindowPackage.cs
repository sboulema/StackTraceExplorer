using System;
using System.Runtime.InteropServices;
using System.Threading;
using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using StackTraceExplorer.Helpers;
using Task = System.Threading.Tasks.Task;

namespace StackTraceExplorer
{
    [Guid(PackageGuids.guidStackTraceExplorerToolWindowPackageString)]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideToolWindow(typeof(StackTraceExplorerToolWindow.Pane))]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class StackTraceExplorerToolWindowPackage : ToolkitPackage
    {
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            StackTraceExplorerToolWindow.Initialize(this);

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            await StackTraceExplorerToolWindowCommand.InitializeAsync(this);

            TraceHelper.ComponentModel = await GetServiceAsync(typeof(SComponentModel)) as IComponentModel;
        }
    }
}
