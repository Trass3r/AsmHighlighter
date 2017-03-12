#region Header Licence
//  ---------------------------------------------------------------------
//
//  Copyright (c) 2009 Alexandre Mutel and Microsoft Corporation.
//  All rights reserved.
//
//  This code module is part of AsmHighlighter, a plugin for visual studio
//  to provide syntax highlighting for x86 ASM language (.asm, .inc)
//
//  ------------------------------------------------------------------
//
//  This code is licensed under the Microsoft Public License.
//  See the file License.txt for the license details.
//  More info on: http://asmhighlighter.codeplex.com
//
//  ------------------------------------------------------------------
#endregion
using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace AsmHighlighter
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the registration utility (regpkg.exe) that this class needs
    // to be registered as package.
    [PackageRegistration(UseManagedResourcesOnly = true)]

    //[InstalledProductRegistration(true, null, null, null)]
    // In order be loaded inside Visual Studio in a machine that has not the VS SDK installed,
    // package needs to have a valid load key (it can be requested at
    // http://msdn.microsoft.com/vstudio/extend/). This attributes tells the shell that this
    // package has a load key embedded in its resources.
    [ProvideService(typeof(AsmHighlighterLanguageService), ServiceName = "AsmHighlighter")]
    // [RegisterExpressionEvaluator(typeof(AsmExpressionEvaluator), GuidList.AsmLanguageGuid, GuidList.MicrosoftVendorGuid)]
    [ProvideLanguageService(typeof(AsmHighlighterLanguageService),
                             "ASM Language",
                             0,
                             RequestStockColors = false,
                             EnableCommenting = true,
                             EnableFormatSelection =  true,
                             EnableLineNumbers =  true,
                             QuickInfo = true
                             )]
    [ProvideLanguageExtension(typeof(AsmHighlighterLanguageService), AsmHighlighterSupportedExtensions.ASM)]
    [ProvideLanguageExtension(typeof(AsmHighlighterLanguageService), AsmHighlighterSupportedExtensions.COD)]
    [ProvideLanguageExtension(typeof(AsmHighlighterLanguageService), AsmHighlighterSupportedExtensions.INC)]
    [Guid(GuidList.guidAsmHighlighterPkgString)]
    public sealed class AsmHighlighter : Package, IVsInstalledProduct
    {
        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require
        /// any Visual Studio service because at this point the package object is created but
        /// not sited yet inside Visual Studio environment. The place to do all the other
        /// initialization is the Initialize method.
        /// </summary>
        public AsmHighlighter()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }

        /////////////////////////////////////////////////////////////////////////////
        // Overriden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initilaization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Trace.WriteLine (string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

			// this is unfortunately necessary
			var mgr = GetService(typeof(SVsFontAndColorCacheManager)) as IVsFontAndColorCacheManager;
			if (mgr != null)
			{
				mgr.ClearAllCaches();
				Guid guid = Guid.Empty;
				mgr.RefreshCache(ref guid);
				// refresh text editor category
				guid = new Guid(0xA27B4E24, 0xA735, 0x4d1d, 0xB8, 0xE7, 0x97, 0x16, 0xE1, 0xE3, 0xD8, 0xE0);
				mgr.RefreshCache(ref guid);
			}

			// Proffer the service.
			var serviceContainer = this as IServiceContainer;
#if true
			var langService = new AsmHighlighterLanguageService();
            langService.SetSite(this);
            serviceContainer.AddService(typeof(AsmHighlighterLanguageService), langService, true);
		}
#else

			// Proffer language service on demand
			ServiceCreatorCallback callback = new ServiceCreatorCallback(CreateLanguageService);
			serviceContainer.AddService(typeof(AsmHighlighterLanguageService), callback, true);
        }
		private object CreateLanguageService(IServiceContainer container, Type serviceType)
		{
			if (container == this)
			{
				if (typeof(AsmHighlighterLanguageService) == serviceType)
				{
					AsmHighlighterLanguageService langSvc = new AsmHighlighterLanguageService();
					langSvc.SetSite(this);
					return langSvc;
				}
			}
			return null;
		}
#endif
       #endregion

        #region Implementation of IVsInstalledProduct

        public int IdBmpSplash(out uint pIdBmp)
        {
            pIdBmp = 0;
            return VSConstants.S_OK;
        }

        public int OfficialName(out string pbstrName)
        {
            pbstrName = VSPackageResourceManager.GetString("110");
            return VSConstants.S_OK;
        }

        public int ProductID(out string pbstrPID)
        {
            pbstrPID = AsmHighlighterVersion.VERSION;
            return VSConstants.S_OK;
        }

        public int ProductDetails(out string pbstrProductDetails)
        {
            pbstrProductDetails =  VSPackageResourceManager.GetString("112");
            return VSConstants.S_OK;
        }

        public int IdIcoLogoForAboutbox(out uint pIdIco)
        {
            pIdIco = 400;
            return VSConstants.S_OK;
        }

        private static System.Resources.ResourceManager resourceMan;


        internal static System.Resources.ResourceManager VSPackageResourceManager
        {
            get
            {
                if (ReferenceEquals(resourceMan, null))
                {
                    resourceMan = new System.Resources.ResourceManager("AsmHighlighter.VSPackage", typeof(AsmHighlighter).Assembly);
                }
                return resourceMan;
            }
        }

        #endregion
    }
}