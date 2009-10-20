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
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;

namespace AsmHighlighter
{
    public class AsmHighlighterSource : Source
    {
        public AsmHighlighterSource(LanguageService service, IVsTextLines textLines, Colorizer colorizer) : base(service, textLines, colorizer)
        {
        }

        private void DoFormatting(EditArray mgr, TextSpan span)
        {
            // Make sure there is one space after every comma unless followed
            // by a tab or comma is at end of line.
            IVsTextLines pBuffer = GetTextLines();
            if (pBuffer != null)
            {
                List<EditSpan> changeList = AsmHighlighterFormatHelper.ReformatCode(pBuffer, span, LanguageService.GetLanguagePreferences().TabSize);
                if (changeList != null)
                {
                    foreach (EditSpan editSpan in changeList)
                    {
                        // Add edit operation
                        mgr.Add(editSpan);
                    }
                }
            }
        }

        public override CommentInfo GetCommentFormat()
        {
            CommentInfo info = new CommentInfo();
            info.LineStart = ";";
            info.BlockStart = "";
            info.BlockEnd = "";
            info.UseLineComments = true;
            return info;
        }

        public override void ReformatSpan(EditArray mgr, TextSpan span)
        {
            string description = "Reformat code";
            CompoundAction ca = new CompoundAction(this, description);
            using (ca)
            {
                // ca.FlushEditActions();      // Flush any pending edits
                DoFormatting(mgr, span);    // Format the span
            }
        }
    }
}