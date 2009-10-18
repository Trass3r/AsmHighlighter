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
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;
using AsmHighlighter.Lexer;

namespace AsmHighlighter
{
    /// <summary>
    /// Alpha version for reformatting. After some test (more particularly with prepropressor directives), 
    /// we definitely need a fully implemented lexical-parser in order to perform a correct reformatting.
    /// </summary>
    public class AsmHighlighterFormatHelper
    {
        static AsmHighlighterFormatHelper()
        {
        }

        public static List<EditSpan> ReformatCode(IVsTextLines pBuffer, TextSpan span, int tabSize)
        {
            return null;
        }
    }
}