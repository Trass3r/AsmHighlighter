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
using System.Diagnostics;
using System.IO;

namespace AsmHighlighter
{
    internal sealed class EnumMap<T> : Dictionary<string, T>
    {
        public void Load(string resource)
        {
            Stream file = typeof(T).Assembly.GetManifestResourceStream(typeof(T).Assembly.GetName().Name + "." + resource);
            TextReader textReader = new StreamReader(file);
            string line;
            while ((line = textReader.ReadLine()) != null )
            {
                int indexEqu = line.IndexOf('=');
                if (indexEqu <= 0)
                    continue;

                string enumName = line.Substring(0, indexEqu);
                string value = line.Substring(indexEqu + 1, line.Length - indexEqu-1).Trim();
                string[] values = value.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                T enumValue = (T)Enum.Parse(typeof(T), enumName);
                foreach (string token in values)
                {
                    Debug.Assert(token == token.ToLowerInvariant());
                    if (!ContainsKey(token))
                        Add(token, enumValue);
                    else
                        Trace.WriteLine(string.Format("Warning: token {0} for enum {1} already added for {2}", token, enumValue, this[token]));
                }
            }
        }
    }
}