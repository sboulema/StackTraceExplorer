using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using StackTraceExplorer.Helpers;

namespace StackTraceExplorer
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(StackTraceExplorerToolWindow))]
    [Guid(PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    public sealed class StackTraceExplorerToolWindowPackage : AsyncPackage
    {
        public const string PackageGuidString = "0485ea98-864e-461f-945f-3c8f9c994842";
        public StackTraceExplorerToolWindowPackage()
        {
        }

        #region Package Members

        protected override async System.Threading.Tasks.Task InitializeAsync(System.Threading.CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            EnvDteHelper.ComponentModel = await GetServiceAsync(typeof(SComponentModel)) as IComponentModel;
            EnvDteHelper.Dte = await GetServiceAsync(typeof(DTE)) as DTE;

            StackTraceExplorerToolWindowCommand.Initialize(this);
            await base.InitializeAsync(cancellationToken, progress);
        }

        #endregion
    }
}
