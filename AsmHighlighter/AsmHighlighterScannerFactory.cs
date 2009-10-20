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
using System.IO;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;

namespace AsmHighlighter
{
    public class AsmHighlighterScannerFactory {
        private static AsmHighlighterScanner masmScanner;
        private static AsmHighlighterScanner nasmScanner;
        private static Dictionary<string, AsmHighlighterScanner> mapExtensionToScanner;

        static AsmHighlighterScannerFactory() {

            mapExtensionToScanner = new Dictionary<string, AsmHighlighterScanner>();

            // MASM Scanner
            masmScanner = new AsmHighlighterScanner(new MAsmHighlighterTokenProvider());
            mapExtensionToScanner.Add(AsmHighlighterSupportedExtensions.ASM,masmScanner);
            mapExtensionToScanner.Add(AsmHighlighterSupportedExtensions.COD, masmScanner);
            mapExtensionToScanner.Add(AsmHighlighterSupportedExtensions.INC, masmScanner);
            //// GLSL Scanner
            //nasmScanner = new AsmHighlighterScanner(new GLSLAsmHighlighterTokenProvider());
        }

        public static AsmHighlighterScanner GetScanner(string filepath)
        {
            string ext = Path.GetExtension(filepath).ToLower();
            AsmHighlighterScanner scanner;
            if ( ! mapExtensionToScanner.TryGetValue(ext, out scanner) )
            {
                scanner = masmScanner;
            }
            return scanner;
        }

        public static AsmHighlighterScanner GetScanner(IVsTextLines buffer)
        {
            return GetScanner(FilePathUtilities.GetFilePath(buffer));
        }
    }
}