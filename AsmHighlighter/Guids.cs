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

        public const string guidAsmHighlighterLanguageServiceString = "44c6cef2-a3cd-37b5-8120-1b241659eac7";

        /// <summary>Identifies vendor Micorosft (used by debugger)</summary>
        public const string MicrosoftVendorGuid = "994B45C4-E6E9-11D2-903F-00C04FA302A1";
        /// <summary>Identifies Asm language (used by debugger)</summary>
        public const string AsmLanguageGuid = "23E44B4E-BF25-4bc1-8ACA-229471ABAAD4";

        public static readonly Guid guidAsmHighlighterCmdSet = new Guid(guidAsmHighlighterCmdSetString);
    };
}