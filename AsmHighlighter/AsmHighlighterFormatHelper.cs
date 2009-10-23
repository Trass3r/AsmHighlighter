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
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Fasm;
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

        /*
        public class X86TextSpan
        {
            public X86TextSpan(int start, int end)
            {
                Start = start;
                End = end;
            }

            public int Start { get; set; }
            public int End { get; set; }
        }


        public class X86Instruction : X86TextSpan
        {
            public X86Instruction(int start, int end, string name, AsmHighlighterToken type) : base(start, end)
            {
                Name = name;
                Type = type;
            }

            public string Name { get; set; }
            public AsmHighlighterToken Type { get; set; }
        }

        public class X86Code
        {
            public X86Instruction Instruction { get; set; }

        }

        public X86Code Parse(Scanner lexer, string codeToParse)
        {
            lexer.SetSource(codeToParse, 0);
            int state = 0;
            int start, end;

            AsmHighlighterToken token = (AsmHighlighterToken)lexer.GetNext(ref state, out start, out end);
            List<EditSpan> changes = new List<EditSpan>();
            while (token != AsmHighlighterToken.EOF)
            {
                bool isToStrip = false;
                string stripReplace = "";
                string tokenStr = codeToParse.Substring(start, end - start + 1).ToLower();
                switch (token)
                {
                    case AsmHighlighterToken.INSTRUCTION:
                        if (tokenStr == "call" || tokenStr.StartsWith("j"))
                        {
                            string restOfLine = codeToParse.Substring(end + 1, codeToParse.Length - (end + 1)).Trim();
                            // Default call|jmp dword
                            if (!restOfLine.StartsWith("dword") && !restOfLine.StartsWith("short"))
                            {
                                isToStrip = true;
                                stripReplace = tokenStr + " dword";
                            }
                        }
                        break;
                    case AsmHighlighterToken.LEFT_SQUARE_BRACKET:


                        break;
                    case AsmHighlighterToken.RIGHT_SQUARE_BRACKET:


                        break;
                    case AsmHighlighterToken.REGISTER:
                        if (tokenStr.StartsWith("st("))
                        {
                            tokenStr = tokenStr.Replace("(", "");
                            tokenStr = tokenStr.Replace(")", "");
                            isToStrip = true;
                            stripReplace = tokenStr;
                        }
                        break;
                    case AsmHighlighterToken.DIRECTIVE:
                        // strip register
                        if (tokenStr == "ptr")
                        {
                            isToStrip = true;
                            stripReplace = "";
                        }
                        break;

                    case AsmHighlighterToken.IDENTIFIER:
                        // Convert all identifiers to 0 in order to be able to compile the code
                        isToStrip = true;
                        stripReplace = "125125";
                        break;
                }
                if (isToStrip)
                {
                    TextSpan editTextSpan = new TextSpan();
                    editTextSpan.iStartLine = 0;
                    editTextSpan.iEndLine = 0;
                    editTextSpan.iStartIndex = start;
                    editTextSpan.iEndIndex = end + 1;

                    changes.Add(new EditSpan(editTextSpan, stripReplace));
                }
                token = (AsmHighlighterToken)lexer.GetNext(ref state, out start, out end);
            }
            return null;
        }
        */

        public static string ConvertToFasm(Scanner lexer, string codeToFormat, Dictionary<string, string> defines)
        {

            lexer.SetSource(codeToFormat, 0);
            int state = 0;
            int start, end;

            bool isInBracket = false;
            int countRegisterInBracket = 0;
            AsmHighlighterToken token = (AsmHighlighterToken)lexer.GetNext(ref state, out start, out end);
            List<EditSpan> changes = new List<EditSpan>();
            while (token != AsmHighlighterToken.EOF)
            {
                bool isToStrip = false;
                string stripReplace = "";

                string tokenStr = codeToFormat.Substring(start, end - start + 1).ToLower();
                switch (token)
                {
                    case AsmHighlighterToken.INSTRUCTION:
                        if ( tokenStr == "call" || tokenStr.StartsWith("j"))
                        {
                            string restOfLine = codeToFormat.Substring(end + 1, codeToFormat.Length - (end + 1)).Trim();
                            // Set default call|jxx to dword
                            if (!restOfLine.StartsWith("dword") && !restOfLine.StartsWith("short") && !restOfLine.StartsWith("near") && !restOfLine.StartsWith("far"))
                            {
                                isToStrip = true;
                                stripReplace = tokenStr + " dword";
                            }
                        }
                        break;
                    case AsmHighlighterToken.LEFT_SQUARE_BRACKET:
                        isInBracket = true;
                        break;
                    case AsmHighlighterToken.RIGHT_SQUARE_BRACKET:
                        isInBracket = false;
                        countRegisterInBracket = 0;
                        break;
                    case AsmHighlighterToken.REGISTER:
                    case AsmHighlighterToken.REGISTER_FPU:
                    case AsmHighlighterToken.REGISTER_MMXSSE:
                        if (isInBracket)
                        {
                            countRegisterInBracket++;
                        }
                        // Convert st(#) register to st#
                        if (token == AsmHighlighterToken.REGISTER_FPU)
                        {
                            tokenStr = tokenStr.Replace("(", "");
                            tokenStr = tokenStr.Replace(")", "");
                            isToStrip = true;
                            stripReplace = tokenStr;
                        }
                        break;
                    case AsmHighlighterToken.DIRECTIVE:
                        // strip register
                        if (tokenStr == "ptr")
                        {
                            isToStrip = true;
                            stripReplace = "";
                        }
                        break;

                    case AsmHighlighterToken.IDENTIFIER:
                        isToStrip = true;
                        stripReplace  = (defines.ContainsKey(tokenStr)) ? defines[tokenStr] : "4";
                        if (isInBracket)
                        {
                            if ( (lexer.AsmHighlighterTokenProvider.GetTokenFromIdentifier(stripReplace) & AsmHighlighterToken.IS_REGISTER) != 0 )
                            {
                                countRegisterInBracket++;
                            }
                            else if (stripReplace == "4")
                            {
                                // No register before 1st identifier
                                if ( countRegisterInBracket == 0)
                                {
                                    // Fake dword adress if we have mov [IDENTIFIER + ....]
                                    stripReplace = "123123";                                    
                                }                                 
                            }
                        }
                        break;
                }
                if ( isToStrip )
                {
                    TextSpan editTextSpan = new TextSpan();
                    editTextSpan.iStartLine = 0;
                    editTextSpan.iEndLine = 0;
                    editTextSpan.iStartIndex = start;
                    editTextSpan.iEndIndex = end+1;

                    changes.Add(new EditSpan(editTextSpan, stripReplace));
                }
                token = (AsmHighlighterToken)lexer.GetNext(ref state, out start, out end);                
            }

            for (int i = changes.Count - 1; i >= 0; i-- )
            {
                EditSpan editSpan = changes[i];
                codeToFormat = codeToFormat.Substring(0, editSpan.Span.iStartIndex) + editSpan.Text +
                               codeToFormat.Substring(editSpan.Span.iEndIndex, codeToFormat.Length - editSpan.Span.iEndIndex);                                    
            }

            // Force the FASM code to 32 bit
            codeToFormat = "use32\r\n" + codeToFormat;
            return codeToFormat;
        }

        public static Dictionary<string,string> ParseDefineLine(string line)
        {
            line = line.Trim();
            Dictionary<string,string> map = new Dictionary<string, string>();
            if ( line.StartsWith(";!") )
            {
                line = line.Substring(2, line.Length - 2);
                string[] values = Regex.Split(line, @"\s*;\s*");
                foreach (string keyValue in values)
                {
                    string[] keyValueParsed = Regex.Split(keyValue, @"\s*=\s*");
                    if ( keyValueParsed.Length == 2 )
                    {
                        string key = keyValueParsed[0].ToLower();
                        string value = keyValueParsed[1].ToLower();
                        if (!map.ContainsKey(key))
                        {
                            map.Add(key, value);
                        }
                    }
                }
            }
            return map;
        }

        public static List<EditSpan> ReformatCode(IVsTextLines pBuffer, TextSpan span, int tabSize)
        {
            string filePath = FilePathUtilities.GetFilePath(pBuffer);

            // Return dynamic scanner based on file extension
            List<EditSpan> changeList = new List<EditSpan>();

            string codeToFormat;

            int endOfFirstLineIndex;
            // Get 1st line and parse custom define
            pBuffer.GetLengthOfLine(0, out endOfFirstLineIndex);
            pBuffer.GetLineText(0, 0, 0, endOfFirstLineIndex, out codeToFormat);

            Dictionary<string,string> defines = ParseDefineLine(codeToFormat);

            AsmHighlighterScanner scanner = AsmHighlighterScannerFactory.GetScanner(filePath);
            Scanner lexer = scanner.Lexer;


            // Iterate on each line of the selection to format
            for (int line = span.iStartLine; line <= span.iEndLine; line++)
            {
                int lineLength;
                pBuffer.GetLengthOfLine(line, out lineLength);
                pBuffer.GetLineText(line, 0, line, lineLength, out codeToFormat);

                string codeToAssemble =  ConvertToFasm(lexer, codeToFormat, defines);

                lexer.SetSource(codeToFormat, 0);
                int state = 0;
                int start, end;

                bool instructionFound = false, commentFound = false;
                int commentStart = 0;
                AsmHighlighterToken token = (AsmHighlighterToken)lexer.GetNext(ref state, out start, out end);
                while (token != AsmHighlighterToken.EOF )
                {
                    switch (token)
                    {
                        case AsmHighlighterToken.INSTRUCTION:
                        case AsmHighlighterToken.FPUPROCESSOR:
                        case AsmHighlighterToken.SIMDPROCESSOR:
                            instructionFound = true;
                            break;
                        case AsmHighlighterToken.COMMENT_LINE:
                            if (!commentFound)
                            {
                                commentFound = true;
                                commentStart = start;                                
                            }
                            break;
                    }

                    if ( instructionFound && commentFound )
                    {
                        byte[] buffer = null;

                        try
                        {
                            buffer = ManagedFasm.Assemble(codeToAssemble);
                        }
                        catch (Exception ex)
                        {
                            // Unable to parse instruction... skip
                        }
                        if (buffer != null)
                        {

                        }

                        TextSpan editTextSpan = new TextSpan();
                        editTextSpan.iStartLine = line;
                        editTextSpan.iEndLine = line;
                        editTextSpan.iStartIndex = commentStart;
                        editTextSpan.iEndIndex = commentStart+1;
                        if ((codeToFormat.Length - commentStart) > 2 && codeToFormat.Substring(commentStart, 2) == ";#")
                        {
                            editTextSpan.iEndIndex = editTextSpan.iEndIndex + 2;                            
                        }

                        string text = ";#" + ((buffer == null) ? "?" : string.Format("{0:X}",buffer.Length));
                        changeList.Add(new EditSpan(editTextSpan, text));
                        break;
                    }
                    token = (AsmHighlighterToken)lexer.GetNext(ref state, out start, out end);
                }
            }

            return changeList;
        }
    }
}