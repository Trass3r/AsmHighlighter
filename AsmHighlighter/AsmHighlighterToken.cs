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
namespace AsmHighlighter.Lexer
{
    public enum AsmHighlighterToken
    {
        EOF,
        UNDEFINED,
        IDENTIFIER,
        INSTRUCTION,
        DIRECTIVE,
        FPUPROCESSOR,
        SIMDPROCESSOR,
        REGISTER,
        COMMENT_LINE,
        NUMBER,
        FLOAT,
        STRING_LITERAL,
        OPERATOR,
        DELIMITER,
        LEFT_PARENTHESIS, 
        RIGHT_PARENTHESIS, 
        LEFT_SQUARE_BRACKET, 
        RIGHT_SQUARE_BRACKET
    }
}