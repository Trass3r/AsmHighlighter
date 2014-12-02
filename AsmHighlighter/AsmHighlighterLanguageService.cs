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
using System.Runtime.InteropServices;
using AsmHighlighter.Lexer;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;

namespace AsmHighlighter
{
    [Guid(GuidList.guidAsmHighlighterLanguageServiceString)]
    public class AsmHighlighterLanguageService : LanguageService
    {
        private ColorableItem[] m_colorableItems;

        private LanguagePreferences m_preferences;
        private DTE vs;

        public AsmHighlighterLanguageService()
        {
            m_colorableItems = new ColorableItem[]
            {
				// The first 6 items in this list MUST be these default items.
				new AsmHighlighterColorableItem("Keyword", COLORINDEX.CI_BLUE, COLORINDEX.CI_USERTEXT_BK),
				new AsmHighlighterColorableItem("Comment", COLORINDEX.CI_DARKGREEN, COLORINDEX.CI_USERTEXT_BK),
				new AsmHighlighterColorableItem("Identifier", COLORINDEX.CI_USERTEXT_FG, COLORINDEX.CI_USERTEXT_BK),
				new AsmHighlighterColorableItem("String", COLORINDEX.CI_MAROON, COLORINDEX.CI_USERTEXT_BK),
				new AsmHighlighterColorableItem("Number", COLORINDEX.CI_USERTEXT_FG, COLORINDEX.CI_USERTEXT_BK),
//				new AsmHighlighterColorableItem("Text", COLORINDEX.CI_USERTEXT_FG, COLORINDEX.CI_USERTEXT_BK),

				// 6..
//				new AsmHighlighterColorableItem("ASM Instruction", COLORINDEX.CI_BLUE, COLORINDEX.CI_USERTEXT_BK),
//				new AsmHighlighterColorableItem("ASM Comment", COLORINDEX.CI_DARKGREEN, COLORINDEX.CI_USERTEXT_BK),
//				new AsmHighlighterColorableItem("ASM Identifier", COLORINDEX.CI_SYSPLAINTEXT_FG, COLORINDEX.CI_USERTEXT_BK),
//				new AsmHighlighterColorableItem("ASM String", COLORINDEX.CI_RED, COLORINDEX.CI_USERTEXT_BK),
//				new AsmHighlighterColorableItem("ASM Number", COLORINDEX.CI_DARKBLUE, COLORINDEX.CI_USERTEXT_BK),
				new AsmHighlighterColorableItem("ASM Register", COLORINDEX.CI_MAROON, COLORINDEX.CI_USERTEXT_BK, FONTFLAGS.FF_BOLD),
				new AsmHighlighterColorableItem("ASM FpuInstruction", COLORINDEX.CI_AQUAMARINE, COLORINDEX.CI_USERTEXT_BK),
				new AsmHighlighterColorableItem("ASM Directive", COLORINDEX.CI_PURPLE, COLORINDEX.CI_USERTEXT_BK),
				new AsmHighlighterColorableItem("ASM SimdInstruction", COLORINDEX.CI_AQUAMARINE, COLORINDEX.CI_USERTEXT_BK, FONTFLAGS.FF_BOLD)
			};

            vs = Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(DTE)) as DTE;
        }

        public bool IsDebugging()
        {
            bool isDebugging = false;
            Debugger debugger = vs.Debugger;
            if (debugger != null)
            {
                isDebugging = debugger.CurrentMode != dbgDebugMode.dbgDesignMode;
            }
            return isDebugging;
        }

        public override int GetItemCount(out int count)
        {
            count = m_colorableItems.Length;
            return VSConstants.S_OK;
        }

        public override int GetColorableItem(int index, out IVsColorableItem item)
        {
            if (index < 1 || index > m_colorableItems.Length)
                throw new ArgumentOutOfRangeException("index");

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

            AsmHighlighterSource source = (AsmHighlighterSource)GetSource(req.View);
            return new TestAuthoringScope(this, source);
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
                pCodeSpan[0].iStartIndex = 0;
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

        public int ComputeDataTipOnContext(IVsTextLines textLines, int line, int col, ref TextSpan span, out string tipText)
        {
            int result = VSConstants.E_NOTIMPL;

            tipText = "";
            span.iStartLine = line;
            span.iStartIndex = col;
            span.iEndLine = line;
            span.iEndIndex = col;

            if (textLines != null)
            {
                // Parse tokens and search for the token below the selection
                AsmHighlighterScanner scanner = AsmHighlighterScannerFactory.GetScanner(textLines);
                string lineOfCode;
                List<TokenInfo> tokenInfoList = scanner.ParseLine(textLines, line, int.MaxValue, out lineOfCode);

                TokenInfo selectedTokenInfo = null;
                foreach (TokenInfo info in tokenInfoList)
                {
                    if (col >= info.StartIndex && col <= info.EndIndex)
                    {
                        selectedTokenInfo = info;
                        break;
                    }
                }

                // If a valid token was found, handle it
                if (selectedTokenInfo != null)
                {
                    AsmHighlighterToken token = (AsmHighlighterToken)selectedTokenInfo.Token;

                    // Display only tip for REGISTER or IDENTIFIER
                    if ((token & AsmHighlighterToken.IS_REGISTER) != 0 || (token & AsmHighlighterToken.IS_NUMBER) != 0 || token == AsmHighlighterToken.IDENTIFIER)
                    {
                        result = VSConstants.S_OK;

                        tipText = lineOfCode.Substring(selectedTokenInfo.StartIndex,
                                                         selectedTokenInfo.EndIndex - selectedTokenInfo.StartIndex + 1);

                        if ((token & AsmHighlighterToken.IS_REGISTER)!=0)
                        {
                            tipText = tipText.ToLower();
                            if (token == AsmHighlighterToken.REGISTER_FPU)
                            {
                                tipText = tipText.Replace("(", "");
                                tipText = tipText.Replace(")", "");
                            }
                        }

                        span.iStartIndex = selectedTokenInfo.StartIndex;
                        span.iEndIndex = selectedTokenInfo.EndIndex + 1;

                        // If in debugging mode, display the value
                        // (This is a workaround instead of going through the Debugger.ExpressionEvaluator long way...)
                        // TODO: ExpressionEvaluator is not working, this is a workaround to display values
                        if (IsDebugging() && (((token & AsmHighlighterToken.IS_REGISTER) != 0) || token == AsmHighlighterToken.IDENTIFIER))
                        {

                            Expression expression = vs.Debugger.GetExpression(tipText, true, 1000);
                            string valueStr = "";

                            // Make a friendly printable version for float/double and numbers
                            try
                            {
                                if (expression.Type.Contains("double"))
                                {
                                    double value = double.Parse(expression.Value);
                                    valueStr = string.Format("{0:r}", value);
                                }
                                else
                                {
                                    long value = long.Parse(expression.Value);
                                    valueStr = string.Format("0x{0:X8}", value);
                                }
                            }
                            catch
                            {
                            }

                            // Print a printable version only if it's valid
                            if (string.IsNullOrEmpty(valueStr))
                            {
                                tipText = string.Format("{0} = {1}", tipText, expression.Value);
                            }
                            else
                            {
                                tipText = string.Format("{0} = {1} = {2}", tipText, valueStr, expression.Value);
                            }

                        }
                    }
                }
            }
            return result;
        }

        internal class TestAuthoringScope : AuthoringScope
        {
            private AsmHighlighterSource source;
            private AsmHighlighterLanguageService langService;

            public TestAuthoringScope(AsmHighlighterLanguageService langServiceArg, AsmHighlighterSource sourceArg)
            {
                source = sourceArg;
                langService = langServiceArg;
            }
            public override string GetDataTipText(int line, int col, out TextSpan span)
            {
                IVsTextLines textLines = source.GetTextLines();
                string tipText = "";

                span = new TextSpan();
                langService.ComputeDataTipOnContext(textLines, line, col, ref span, out tipText);
                return tipText;
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

        #region Implementation of IVsLanguageTextOps

        /// <summary>
        /// Displays a tip over a span of text when the mouse hovers over this location.
        /// </summary>
        /// <param name="pTextLayer">[in] An <see cref="T:Microsoft.VisualStudio.TextManager.Interop.IVsTextLayer"/> object representing the text file.</param>
        /// <param name="ptsSel">[in] Span of text relevant to the specified text layer. For more information, see <see cref="T:Microsoft.VisualStudio.TextManager.Interop.TextSpan"/>.</param>
        /// <param name="ptsTip">[out] Returns a span of text to center the tip over. For more information, see <see cref="N:Microsoft.VisualStudio.TextManager.Interop"/>.</param>
        /// <param name="pbstrText">[out] Returns the text of the tip to display.</param>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK"/>. If it fails, it returns an error code.
        /// </returns>
        public int GetDataTip(IVsTextLayer pTextLayer, TextSpan[] ptsSel, TextSpan[] ptsTip, out string pbstrText)
        {
            pbstrText = "";
            return VSConstants.E_NOTIMPL;
        }

        public int GetPairExtent(IVsTextLayer pTextLayer, TextAddress ta, TextSpan[] pts)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int GetWordExtent(IVsTextLayer pTextLayer, TextAddress ta, WORDEXTFLAGS flags, TextSpan[] pts)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int Format(IVsTextLayer pTextLayer, TextSpan[] ptsSel)
        {
            return VSConstants.E_NOTIMPL;
        }

        #endregion
    }
}