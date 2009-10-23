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
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;

namespace AsmHighlighter
{      
    /// <summary>
    /// TODO : NOT USED. THIS IS NOT WORKING
    /// 
    /// ASM Expression evaluator implementation.  This is the original entry point into 
    /// the expression evaluator
    /// </summary>
//    [Guid("E6A6B6A4-4DEF-4526-9F16-D0F7F28B407F")]
//    [ComVisible(true)]
    public class AsmExpressionEvaluator : IDebugExpressionEvaluator2
    {
        private DTE vs;
        public AsmExpressionEvaluator()
        {
            vs = Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(DTE)) as DTE;
        }
        #region IDebugExpressionEvaluator2 Members

        public int GetMethodLocationProperty(string upstrFullyQualifiedMethodPlusOffset, IDebugSymbolProvider pSymbolProvider, IDebugAddress pAddress, IDebugBinder pBinder, out IDebugProperty2 ppProperty)
        {
            ppProperty = null;
            return VSConstants.E_NOTIMPL;
        }

        public int GetMethodProperty(IDebugSymbolProvider pSymbolProvider, IDebugAddress pAddress, IDebugBinder pBinder, int fIncludeHiddenLocals, out IDebugProperty2 ppProperty)
        {
            ppProperty = null;
            return VSConstants.E_NOTIMPL;
        }

        public int GetService(Guid uid, out object ppService)
        {
            ppService = null;
            return VSConstants.E_NOTIMPL;
        }

        public int Parse(string upstrExpression, uint dwFlags, uint nRadix, out string pbstrError, out uint pichError, out IDebugParsedExpression ppParsedExpression)
        {
            pbstrError = null;
            pichError = 0;
            if (string.IsNullOrEmpty(upstrExpression))
            {
                ppParsedExpression = null;
                return VSConstants.E_INVALIDARG;
            }
            ppParsedExpression = new AsmDebugParsedExpression(vs, upstrExpression);
            return VSConstants.S_OK;

        }

        public int PreloadModules(IDebugSymbolProvider pSym)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int SetCallback(IDebugSettingsCallback2 pCallback)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int SetCorPath(string pcstrCorPath)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int SetIDebugIDECallback(IDebugIDECallback pCallback)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int SetLocale(ushort wLangID)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int SetRegistryRoot(string ustrRegistryRoot)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int Terminate()
        {
            return VSConstants.E_NOTIMPL;
        }

        #endregion
    }
}