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

namespace AsmHighlighter
{
    static class GuidList
    {
        public const string guidAsmHighlighterPkgString = "E767EF4D-28FE-4f99-AA6D-4107399D0837";
        public const string guidAsmHighlighterCmdSetString = "B5E15002-E3FB-4f1e-A774-5832740EE2A2";

        public static readonly Guid guidAsmHighlighterCmdSet = new Guid(guidAsmHighlighterCmdSetString);
    };
}