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
using AsmHighlighter.Lexer;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;

namespace AsmHighlighter
{
    public class AsmHighlighterLanguageService : LanguageService
    {
        private ColorableItem[] m_colorableItems;

        private LanguagePreferences m_preferences;

        public AsmHighlighterLanguageService()
        {
            m_colorableItems = new ColorableItem[]
                                   {
                                        /* 1 */
                                        new AsmHighlighterColorableItem("ASM Language - Instruction", "ASM Language - Instruction", COLORINDEX.CI_BLUE, COLORINDEX.CI_USERTEXT_BK),
                                        /* 2 */
                                        new AsmHighlighterColorableItem("ASM Language - Comment", "ASM Language - Comment", COLORINDEX.CI_DARKGREEN, COLORINDEX.CI_USERTEXT_BK),
                                        /* 3 */
                                        new AsmHighlighterColorableItem("ASM Language - Identifier", "ASM Language - Identifier", COLORINDEX.CI_SYSPLAINTEXT_FG, COLORINDEX.CI_USERTEXT_BK),
                                        /* 4 */
                                        new AsmHighlighterColorableItem("ASM Language - String", "ASM Language - String", COLORINDEX.CI_RED, COLORINDEX.CI_USERTEXT_BK),
                                        /* 5 */
                                        new AsmHighlighterColorableItem("ASM Language - Number", "ASM Language - Number", COLORINDEX.CI_DARKBLUE, COLORINDEX.CI_USERTEXT_BK),
                                        /* 6 */
                                        new AsmHighlighterColorableItem("ASM Language - Register", "ASM Language - Register", COLORINDEX.CI_MAROON, COLORINDEX.CI_USERTEXT_BK, FONTFLAGS.FF_BOLD),
                                        /* 7 */
                                        new AsmHighlighterColorableItem("ASM Language - FpuInstruction", "ASM Language - FpuInstruction", COLORINDEX.CI_AQUAMARINE, COLORINDEX.CI_USERTEXT_BK),
                                        /* 8 */
                                        new AsmHighlighterColorableItem("ASM Language - Directive", "ASM Language - Directive", COLORINDEX.CI_PURPLE, COLORINDEX.CI_USERTEXT_BK),
                                        /* 9 */
                                        new AsmHighlighterColorableItem("ASM Language - SimdInstruction", "ASM Language - SimdInstruction", COLORINDEX.CI_AQUAMARINE, COLORINDEX.CI_USERTEXT_BK, FONTFLAGS.FF_BOLD),
                                   };
        }


        public override int GetItemCount(out int count)
        {
            count = m_colorableItems.Length;
            return VSConstants.S_OK;
        }

        public override int GetColorableItem(int index, out IVsColorableItem item)
        {
            if (index < 1)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            item = m_colorableItems[index-1];
            return VSConstants.S_OK;
        }

        public override LanguagePreferences GetLanguagePreferences()
        {
            if (m_preferences == null)
            {
                m_preferences = new LanguagePreferences(this.Site,
                                                        typeof(AsmHighlighterLanguageService).GUID,
                                                        this.Name);
                m_preferences.Init();
            }
            return m_preferences;
        }

        public override IScanner GetScanner(IVsTextLines buffer)
        {
            string filePath = FilePathUtilities.GetFilePath(buffer);
            // Return dynamic scanner based on file extension
            return AsmHighlighterScannerFactory.GetScanner(filePath);
        }

        public override Source CreateSource(IVsTextLines buffer)
        {
            return new AsmHighlighterSource(this, buffer, GetColorizer(buffer));
        }

        public override AuthoringScope ParseSource(ParseRequest req)
        {
            // req.FileName
            return new TestAuthoringScope();
        }

        public override string GetFormatFilterList()
        {
            return "";
        }

        public override string Name
        {
            get { return "ASM Language"; }
        }

        /// <summary>
        /// Validates the breakpoint location.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="line">The line.</param>
        /// <param name="col">The col.</param>
        /// <param name="pCodeSpan">The TextSpans to update.</param>
        /// <returns></returns>
        public override int ValidateBreakpointLocation(IVsTextBuffer buffer, int line, int col, TextSpan[] pCodeSpan)
        {
            // Return noimpl by default
            int retval = VSConstants.E_NOTIMPL;

            if (pCodeSpan != null)
            {
                // Make sure the span is set to at least the current
                // position by default.
                pCodeSpan[0].iStartLine = line;
                pCodeSpan[0].iStartIndex = col;
                pCodeSpan[0].iEndLine = line;
                pCodeSpan[0].iEndIndex = col;
            }
            
            if (buffer != null)
            {
                IVsTextLines textLines = buffer as IVsTextLines;
                if (textLines != null)
                {
                    AsmHighlighterScanner scanner = AsmHighlighterScannerFactory.GetScanner(textLines);
                    Scanner lexer = scanner.Lexer;

                    int maxColumn;
                    textLines.GetLengthOfLine(line, out maxColumn);
                    string lineToParse;
                    textLines.GetLineText(line, 0, line, maxColumn, out lineToParse);
                    
                    // Setup token scanner
                    lexer.SetSource(lineToParse, 0);

                    int state = 0;
                    int start, end;

                    AsmHighlighterToken token = (AsmHighlighterToken)lexer.GetNext(ref state, out start, out end);

                    // Set Not a valid breakpoint
                    retval = VSConstants.S_FALSE;
                    switch (token)
                    {
                        case AsmHighlighterToken.INSTRUCTION:
                        case AsmHighlighterToken.FPUPROCESSOR:
                        case AsmHighlighterToken.SIMDPROCESSOR:
                            if (pCodeSpan != null)
                            {
                                // Breakpoint covers the whole line (including comment)
                                pCodeSpan[0].iEndIndex = maxColumn;
                            }
                            // Set valid breakpoint
                            retval = VSConstants.S_OK;
                            break;
                    }
                }
            }
            return retval;
        }

        //private bool isHelpContextSet = false;
        public override void UpdateLanguageContext(LanguageContextHint hint, IVsTextLines buffer, TextSpan[] ptsSelection, Microsoft.VisualStudio.Shell.Interop.IVsUserContext context)
        {
            // WOULD HAVE BEEN NICE TO HAVE HELP, BUT SEEMS TO BE A LONG WAY TO CREATE AN HXS HELP FILE
            //if (isHelpContextSet)
            //{
            //    context.RemoveAttribute("keyword", "mov");
            //    isHelpContextSet = false;
            //}
            //else
            //{
            //    context.AddAttribute(VSUSERCONTEXTATTRIBUTEUSAGE.VSUC_Usage_LookupF1, "keyword", "mov");
            //    isHelpContextSet = true;
            //}
        }
        
        internal class TestAuthoringScope : AuthoringScope
        {
            public override string GetDataTipText(int line, int col, out TextSpan span)
            {
                span = new TextSpan();
                return null;
            }

            public override Declarations GetDeclarations(IVsTextView view,
                                                         int line,
                                                         int col,
                                                         TokenInfo info,
                                                         ParseReason reason)
            {
                return null;
            }

            public override Methods GetMethods(int line, int col, string name)
            {
                return null;
            }

            public override string Goto(VSConstants.VSStd97CmdID cmd, IVsTextView textView, int line, int col, out TextSpan span)
            {
                span = new TextSpan();
                return null;
            }
        }

    }
}