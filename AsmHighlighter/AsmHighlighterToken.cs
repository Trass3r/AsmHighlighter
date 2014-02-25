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

namespace AsmHighlighter.Lexer
{
    [Flags]
    public enum AsmHighlighterToken : uint
    {
        EOF = 0,
        UNDEFINED = 1,
        IDENTIFIER = 2,
        INSTRUCTION = 3 | IS_INSTRUCTION,
        DIRECTIVE = 4,
        FPUPROCESSOR = 5 | IS_INSTRUCTION,
        SIMDPROCESSOR = 6 | IS_INSTRUCTION,
        REGISTER = 7 | IS_REGISTER,
        REGISTER_FPU = 8 | IS_REGISTER,
        REGISTER_MMXSSE = 9 | IS_REGISTER,
		REGISTER_AVX = 20 | IS_REGISTER,
		REGISTER_AVX512 = 21 | IS_REGISTER,
        COMMENT_LINE = 10,
        NUMBER = 11 | IS_NUMBER,
        FLOAT = 12 | IS_NUMBER,
        STRING_LITERAL = 13,
        OPERATOR = 14,
        DELIMITER = 15,
        LEFT_PARENTHESIS = 16, 
        RIGHT_PARENTHESIS = 17, 
        LEFT_SQUARE_BRACKET = 18, 
        RIGHT_SQUARE_BRACKET = 19,
		SSE4 = 22,
		AVX2 = 23,
		FMA = 24,

        IS_REGISTER = 0x80000000,
        IS_INSTRUCTION = 0x40000000,
        IS_NUMBER = 0x20000000,
    }
}