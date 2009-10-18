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
using AsmHighlighter.Lexer;

namespace AsmHighlighter
{
    public class MAsmHighlighterTokenProvider : IAsmHighlighterTokenProvider
    {
        private static EnumMap<AsmHighlighterToken> map;
        
        static MAsmHighlighterTokenProvider()
        {
            map = new EnumMap<AsmHighlighterToken>();
            map.Load("MASMKeywords.map");
        }

        public AsmHighlighterToken GetTokenFromIdentifier(string text)
        {
            AsmHighlighterToken token;
            if ( ! map.TryGetValue(text.ToLower(), out token ) )
            {
                token = AsmHighlighterToken.IDENTIFIER;
            }
            return token;
        }

    }
}