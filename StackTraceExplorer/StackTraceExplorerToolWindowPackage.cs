using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace StackTraceExplorer
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(StackTraceExplorerToolWindow))]
    [Guid(PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    public sealed class StackTraceExplorerToolWindowPackage : Package
    {
        public const string PackageGuidString = "0485ea98-864e-461f-945f-3c8f9c994842";
        public StackTraceExplorerToolWindowPackage()
        {
            EnvDteHelper.Dte = GetGlobalService(typeof(DTE)) as DTE;
        }

        public void TestStackTrace()
        {
            EnvDteHelper.TestStackTrace();
        }

        #region Package Members

        protected override void Initialize()
        {
            StackTraceExplorerToolWindowCommand.Initialize(this);
            base.Initialize();
        }

        #endregion
    }
}
