using System;
using System.Collections.Generic;

using VisCPU.Compiler.Parser.Tokens;
using VisCPU.Utility;
using VisCPU.Utility.Events;
using VisCPU.Utility.Logging;

namespace VisCPU.Compiler.Parser
{
    public class Tokenizer : VisBase
    {

        protected override LoggerSystems SubSystem => LoggerSystems.Parser;

        private readonly string originalText;
        private int position;

        private Tokenizer(string text)
        {
            originalText = text;
        }

        private char Current => Peek(0);

        public static List<AToken[]> Tokenize(string text)
        {
            Tokenizer reader = new Tokenizer(text);
            List<AToken> tokens = new List<AToken> { new NewLineToken(text, 0, -1) };
            while (reader.Current != '\0')
            {
                AToken current = reader.Advance();
                if (current is EOFToken)
                {
                    break;
                }

                tokens.Add(current);
            }

            List<AToken[]> asmInstructions = new List<AToken[]>();
            List<AToken> asmInstruction = new List<AToken>();
            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i] is NewLineToken)
                {
                    if (asmInstruction.Count != 0)
                    {
                        asmInstructions.Add(asmInstruction.ToArray());
                    }

                    asmInstruction.Clear();
                    continue;
                }

                asmInstruction.Add(tokens[i]);
            }

            if (asmInstruction.Count != 0)
            {
                asmInstructions.Add(asmInstruction.ToArray());
            }

            asmInstruction.Clear();


            return asmInstructions;
        }

        private char Peek(int offset)
        {
            return position + offset < originalText.Length ? originalText[position + offset] : '\0';
        }

        private int ReadUntil(Func<char, bool> func)
        {
            int start = position;
            while (Current != '\0' && !func(Current))
            {
                position++;
            }

            return position - start;
        }

        private AToken Advance()
        {
            if (Current == '\0')
            {
                return new EOFToken(originalText, position, 0);
            }

            ReadUntil(BeginningOfWord);

            if (Current == ';')
            {
                ReadUntil(EndOfSingleLineComment);
                return Advance();
            }

            int start = position;

            if (Current == '"')
            {
                char sepItem = Current;
                position++;
                int len = ReadUntil(x => x == sepItem);
                position++;
                return new StringToken(originalText, start, len + 2);
            }

            if (Current == ':')
            {
                int len = ReadUntil(EndOfWord);
                return new WordToken(originalText, start, len).Resolve();
            }

            if (Current == '\n')
            {
                int size = 1;
                position += size;
                return new NewLineToken(originalText, start, size);
            }

            if (Current == '\r')
            {
                int size = 1;
                if (Peek(1) == '\n')
                {
                    size++;
                }

                position += size;
                return new NewLineToken(originalText, start, size);
            }

            if (Current == '\'')
            {
                char sepItem = Current;
                start++;
                position++;
                int len = ReadUntil(x => x == sepItem);
                position++;
                return new CharToken(originalText, start, len);
            }

            if (char.IsDigit(Current))
            {
                char f = Current;
                char s = Peek(1);
                int len = ReadUntil(EndOfWord);
                if (f == '0' && s == 'x')
                {
                    return new HexToken(originalText, start, len);
                }

                return new DecToken(originalText, start, len);
            }
            else
            {
                int len = ReadUntil(EndOfWord);
                return new WordToken(originalText, start, len).Resolve();
            }
        }

        private bool EndOfWord(char input)
        {
            return IsWhiteSpace(input) || input == ';' || input == '\n' || input == '\r';
        }

        private bool EndOfSingleLineComment(char input)
        {
            return input == '\n' || input == '\r';
        }

        private bool IsWhiteSpace(char input)
        {
            return input == '\t' || input == ' ';
        }

        private bool BeginningOfWord(char input)
        {
            return char.IsLetterOrDigit(input) ||
                   input == '_' ||
                   BeginningOfConstantInstruction(input) ||
                   input == '\n' ||
                   input == '\r' ||
                   input == ':' ||
                   input == ';' ||
                   input == '.' ||
                   input == '"' ||
                   input == '\'';
        }

        private bool BeginningOfConstantInstruction(char input)
        {
            return input == ':';
        }

    }
}