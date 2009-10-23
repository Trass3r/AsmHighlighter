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
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;
using Constants=Microsoft.VisualStudio.Debugger.Interop.Constants;

namespace AsmHighlighter
{

    public class AsmDebugParsedExpression : IDebugParsedExpression
    {
        private string _name;
        private DTE vs;

        public AsmDebugParsedExpression(DTE vsArg, string name)
        {
            _name = name;
            vs = vsArg;
        }

        #region Implementation of IDebugParsedExpression

        public int EvaluateSync(uint dwEvalFlags, uint dwTimeout, IDebugSymbolProvider pSymbolProvider, IDebugAddress pAddress, IDebugBinder pBinder, string bstrResultType, out IDebugProperty2 ppResult)
        {
            ppResult = new AsmDebugProperty(vs, _name);
            return VSConstants.S_OK;
        }

        #endregion
    }

    public class AsmDebugProperty : IDebugProperty2
    {
        private string _expression;
        private DTE vs;
        public AsmDebugProperty(DTE vsArg, string expression)
        {
            vs = vsArg;
            _expression = expression;
            
        }
        #region Implementation of IDebugProperty2

        public int GetPropertyInfo(uint dwFields, uint dwRadix, uint dwTimeout, IDebugReference2[] rgpArgs, uint dwArgCount, DEBUG_PROPERTY_INFO[] pPropertyInfo)
        {
            if (pPropertyInfo.Length > 0)
            {
                Expression debugExpr = vs.Debugger.GetExpression(_expression, true, 1000);

                pPropertyInfo[0].dwAttrib = (long)Constants.DBG_ATTRIB_ACCESS_ALL;
                pPropertyInfo[0].dwAttrib |= (long)Constants.DBG_ATTRIB_TYPE_VOLATILE;
                pPropertyInfo[0].dwAttrib |= (long) Constants.DBG_ATTRIB_STORAGE_REGISTER;
                
                pPropertyInfo[0].dwFields = dwFields;

                if ((dwFields & (uint)enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_FULLNAME) != 0)
                {
                    pPropertyInfo[0].bstrFullName = _expression;
                }

                if ((dwFields & (uint)enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_NAME) != 0)
                {
                    pPropertyInfo[0].bstrName = debugExpr.Name;
                }

                if ((dwFields & (uint)enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_TYPE) != 0)
                {
                    pPropertyInfo[0].bstrType = debugExpr.Type;
                }

                if ((dwFields & (uint)enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_VALUE) != 0
                    || (dwFields & (uint)enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_VALUE_RAW) != 0)
                {
                    pPropertyInfo[0].bstrValue = debugExpr.Value;
                }
            }
            return VSConstants.S_OK;
        }

        public int SetValueAsString(string pszValue, uint dwRadix, uint dwTimeout)
        {
            return VSConstants.S_OK;
        }

        public int SetValueAsReference(IDebugReference2[] rgpArgs, uint dwArgCount, IDebugReference2 pValue, uint dwTimeout)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int EnumChildren(uint dwFields, uint dwRadix, ref Guid guidFilter, ulong dwAttribFilter, string pszNameFilter, uint dwTimeout, out IEnumDebugPropertyInfo2 ppEnum)
        {
            ppEnum = null;
            return VSConstants.E_NOTIMPL;
        }

        public int GetParent(out IDebugProperty2 ppParent)
        {
            ppParent = null;
            return VSConstants.E_NOTIMPL;
        }

        public int GetDerivedMostProperty(out IDebugProperty2 ppDerivedMost)
        {
            ppDerivedMost = null;
            return VSConstants.E_NOTIMPL;
        }

        public int GetMemoryBytes(out IDebugMemoryBytes2 ppMemoryBytes)
        {
            ppMemoryBytes = null;
            return VSConstants.E_NOTIMPL;
        }

        public int GetMemoryContext(out IDebugMemoryContext2 ppMemory)
        {
            ppMemory = null;
            return VSConstants.E_NOTIMPL;
        }

        public int GetSize(out uint pdwSize)
        {
            pdwSize = 0;
            return VSConstants.E_NOTIMPL;
        }

        public int GetReference(out IDebugReference2 ppReference)
        {
            ppReference = null;
            return VSConstants.E_NOTIMPL;
        }

        public int GetExtendedInfo(ref Guid guidExtendedInfo, out object pExtendedInfo)
        {
            pExtendedInfo = null;
            return VSConstants.E_NOTIMPL;
        }

        #endregion
    }
}