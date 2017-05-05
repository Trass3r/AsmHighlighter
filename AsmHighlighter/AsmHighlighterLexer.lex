/*
//  ASMHighlighterMPLexer.lex.
//  Lexical description for MPLex. This file is inspired from http://svn.assembla.com/svn/ppjlab/trunk/scanner.lex
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
*/

%namespace AsmHighlighter.Lexer
%option noparser, verbose, summary, unicode

/**********************************************************************************/
/********************************User Defined Code*********************************/
/**********************************************************************************/

%{
	internal IAsmHighlighterTokenProvider AsmHighlighterTokenProvider = null; // Token provider
%}

/**********************************************************************************/
/**********Start Condition Declarations and Lexical Category Definitions***********/
/**********************************************************************************/


binary              [0-1]
octal               [0-7]
digit               [0-9]
alpha               [a-zA-Z_]
manglingchars       [?@$<>]
exponent            [Ee]("+"|"-")?{digit}+
floatsuffix         [fFhH]
white_space         [ \t\v\n\f\r]
hexdigit            [0-9a-fA-F]
ABStar              [^\*\n]*

/**********************************************************************************/
/**********************************************************************************/
/********************************The Rules Section*********************************/
/**********************************************************************************/
/**********************************************************************************/

%%

/**********************************************************************************/
/************************************Comments**************************************/
/**********************************************************************************/

";"(.)*                 {return (int)AsmHighlighterToken.COMMENT_LINE;}

/**********************************************************************************/
/***********************************Identifier*************************************/
/**********************************************************************************/

/* st(0) - st(7) registers */
"st("{digit}")"             {return (int)AsmHighlighterTokenProvider.GetTokenFromIdentifier(yytext);}
"st"{digit}                 {return (int)AsmHighlighterTokenProvider.GetTokenFromIdentifier(yytext);}

/* directive */
"."({alpha}|{digit})*       {return (int)AsmHighlighterTokenProvider.GetTokenFromIdentifier(yytext);}
"@"({alpha}|{digit})*       {return (int)AsmHighlighterTokenProvider.GetTokenFromIdentifier(yytext);}
"%"({alpha}|{digit})*       {return (int)AsmHighlighterTokenProvider.GetTokenFromIdentifier(yytext);}

/* identifier or keyword */
({alpha}|[$?])({alpha}|{manglingchars}|{digit})*   {return (int)AsmHighlighterTokenProvider.GetTokenFromIdentifier(yytext);}


/**********************************************************************************/
/*************************************Numbers**************************************/
/**********************************************************************************/

(0[bB]{binary}+|{binary}+[bB])           {return (int)AsmHighlighterToken.NUMBER;}
(0[oO]{octal}+|{octal}+[oO])             {return (int)AsmHighlighterToken.NUMBER;}
(0[dD]{digit}+|{digit}+[dD])             {return (int)AsmHighlighterToken.NUMBER;}
(0[xX]{hexdigit}+|{hexdigit}+[hH])       {return (int)AsmHighlighterToken.NUMBER;}
{digit}+                                 {return (int)AsmHighlighterToken.NUMBER;}

/**********************************************************************************/
/**************************************Float***************************************/
/**********************************************************************************/

{digit}+{exponent}          {return (int)AsmHighlighterToken.FLOAT;}
{digit}+"."{digit}*({exponent})?({floatsuffix})?    {return (int)AsmHighlighterToken.FLOAT;}


/**********************************************************************************/
/*************************************String***************************************/
/**********************************************************************************/

\"(\\.|[^\\"])*\"           {return (int)AsmHighlighterToken.STRING_LITERAL;}
\'(\\.|[^\\'])*\'           {return (int)AsmHighlighterToken.STRING_LITERAL;}

/**********************************************************************************/
/***************************Operators And Special Signs****************************/
/**********************************************************************************/

";"                     {return (int)AsmHighlighterToken.DELIMITER;}
","                     {return (int)AsmHighlighterToken.DELIMITER;}
"("                     {return (int)AsmHighlighterToken.LEFT_PARENTHESIS;}
")"                     {return (int)AsmHighlighterToken.RIGHT_PARENTHESIS;}
"["                     {return (int)AsmHighlighterToken.LEFT_SQUARE_BRACKET;}
"]"                     {return (int)AsmHighlighterToken.RIGHT_SQUARE_BRACKET;}
"."                     {return (int)AsmHighlighterToken.OPERATOR;}
"&"                     {return (int)AsmHighlighterToken.OPERATOR;}
"+"                     {return (int)AsmHighlighterToken.OPERATOR;}
"-"                     {return (int)AsmHighlighterToken.OPERATOR;}
"|"                     {return (int)AsmHighlighterToken.OPERATOR;}

/**********************************************************************************/
/****************************White Space & Unrecognized****************************/
/**********************************************************************************/
{white_space}                   {/* Ignore */}
.                   {return (int)AsmHighlighterToken.UNDEFINED;}


/**********************************************************************************/
/**********************************************************************************/
/**************************The User Defined Code Section***************************/
/**********************************************************************************/
/**********************************************************************************/

%%
