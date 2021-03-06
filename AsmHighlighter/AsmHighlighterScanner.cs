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
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;

namespace AsmHighlighter
{
    public sealed class AsmHighlighterScanner : IScanner
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

        public List<TokenInfo> Parse(string toParse, int maxTokens)
        {
            lex.SetSource(toParse, 0);
            int state = 0;
            int start, end;
            List<TokenInfo> tokenInfos = new List<TokenInfo>();
            AsmHighlighterToken token = (AsmHighlighterToken)lex.GetNext(ref state, out start, out end);
            while (token != AsmHighlighterToken.EOF && --maxTokens >=0 )
            {
                TokenInfo tokenInfo = new TokenInfo();
                tokenInfo.StartIndex = start;
                tokenInfo.EndIndex = end;
                tokenInfo.Token = (int)token;
                tokenInfos.Add(tokenInfo);
                token = (AsmHighlighterToken)lex.GetNext(ref state, out start, out end);
            }
            return tokenInfos;
        }

        public List<TokenInfo> ParseLine(IVsTextLines textLines, int line, int maxTokens, out string lineOut)
        {
            int maxColumn;
            textLines.GetLengthOfLine(line, out maxColumn);
            textLines.GetLineText(line, 0, line, maxColumn, out lineOut);

            return Parse(lineOut, maxTokens);
        }

		public bool ScanTokenAndProvideInfoAboutIt(TokenInfo tokenInfo, ref int state)
		{
			int start, end;
			AsmHighlighterToken token = (AsmHighlighterToken)lex.GetNext(ref state, out start, out end);

			// !EOL and !EOF
			if (token == AsmHighlighterToken.EOF)
				return false;

			tokenInfo.StartIndex = start;
			tokenInfo.EndIndex = end;

			switch (token)
			{
				case AsmHighlighterToken.INSTRUCTION:
					tokenInfo.Color = (TokenColor)AsmTokenColor.Keyword;
					tokenInfo.Type = TokenType.Keyword;
					break;
				case AsmHighlighterToken.COMMENT_LINE:
					tokenInfo.Color = (TokenColor)AsmTokenColor.Comment;
					tokenInfo.Type = TokenType.LineComment;
					break;
				case AsmHighlighterToken.NUMBER:
				case AsmHighlighterToken.FLOAT:
					tokenInfo.Color = (TokenColor)AsmTokenColor.Number;
					tokenInfo.Type = TokenType.Literal;
					break;
				case AsmHighlighterToken.STRING_LITERAL:
					tokenInfo.Color = (TokenColor)AsmTokenColor.String;
					tokenInfo.Type = TokenType.String;
					break;
				case AsmHighlighterToken.REGISTER:
				case AsmHighlighterToken.REGISTER_FPU:
				case AsmHighlighterToken.REGISTER_MMXSSE:
				case AsmHighlighterToken.REGISTER_AVX:
				case AsmHighlighterToken.REGISTER_AVX512:
					tokenInfo.Color = (TokenColor)AsmTokenColor.AsmRegister;
					tokenInfo.Type = TokenType.Identifier;
					break;
				case AsmHighlighterToken.FPUPROCESSOR:
					tokenInfo.Color = (TokenColor)AsmTokenColor.AsmFpuInstruction;
					tokenInfo.Type = TokenType.Identifier;
					break;
				case AsmHighlighterToken.DIRECTIVE:
					tokenInfo.Color = (TokenColor)AsmTokenColor.AsmDirective;
					tokenInfo.Type = TokenType.Keyword;
					break;
				case AsmHighlighterToken.SSE:
				case AsmHighlighterToken.FMA3:
				case AsmHighlighterToken.AVX:
				case AsmHighlighterToken.AVX2:
				case AsmHighlighterToken.AVX512F:
					tokenInfo.Color = (TokenColor)AsmTokenColor.AsmSimdInstruction;
					tokenInfo.Type = TokenType.Keyword;
					break;
				default:
					tokenInfo.Color = TokenColor.Text;
					tokenInfo.Type = TokenType.Text;
					break;
			}
			return true;
		}
	}
}