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
using AsmHighlighter.Lexer;
using Microsoft.VisualStudio.Package;

namespace AsmHighlighter
{
    public class AsmHighlighterScanner : IScanner
    {
        private Scanner lex;

        public AsmHighlighterScanner(IAsmHighlighterTokenProvider tokenProvider)
        {
            lex = new Scanner();
            lex.AsmHighlighterTokenProvider = tokenProvider;            
        }

        public void SetSource(string source, int offset)
        {
            lex.SetSource(source, offset);
        }

        public Scanner Lexer
        {
            get
            {
                return lex;
            }
        }

        public bool ScanTokenAndProvideInfoAboutIt(TokenInfo tokenInfo, ref int state)
        {

            int start, end;
            AsmHighlighterToken token = (AsmHighlighterToken)lex.GetNext(ref state, out start, out end);

            // !EOL and !EOF
            if (token != AsmHighlighterToken.EOF)
            {
                tokenInfo.StartIndex = start;
                tokenInfo.EndIndex = end;

                switch (token)
                {
                    case AsmHighlighterToken.INSTRUCTION:
                        tokenInfo.Color = TokenColor.Keyword;
                        tokenInfo.Type = TokenType.Keyword;
                        break;
                    case AsmHighlighterToken.COMMENT_LINE:
                        tokenInfo.Color = TokenColor.Comment;
                        tokenInfo.Type = TokenType.LineComment;
                        break;
                    case AsmHighlighterToken.NUMBER:
                    case AsmHighlighterToken.FLOAT:
                        tokenInfo.Color = TokenColor.Number;
                        tokenInfo.Type = TokenType.Literal;
                        break;
                    case AsmHighlighterToken.STRING_LITERAL:
                        tokenInfo.Color = TokenColor.String;
                        tokenInfo.Type = TokenType.String;
                        break;
                    case AsmHighlighterToken.REGISTER:
                        // hugly. TODO generate a AsmHighlighterTokenColor to keep tracks of 6-7-8 TokenColors
                        tokenInfo.Color = (TokenColor)6;
                        tokenInfo.Type = TokenType.Identifier;
                        break;
                    case AsmHighlighterToken.FPUPROCESSOR:
                        tokenInfo.Color = (TokenColor)7;
                        tokenInfo.Type = TokenType.Identifier;
                        break;
                    case AsmHighlighterToken.DIRECTIVE:
                        tokenInfo.Color = (TokenColor)8;
                        tokenInfo.Type = TokenType.Keyword;
                        break;
                    case AsmHighlighterToken.SIMDPROCESSOR:
                        tokenInfo.Color = (TokenColor)9;
                        tokenInfo.Type = TokenType.Keyword;
                        break;
                    default:
                        tokenInfo.Color = TokenColor.Text;
                        tokenInfo.Type = TokenType.Text;
                        break;
                }
                return true;
            }
            return false;
        }
    }
}