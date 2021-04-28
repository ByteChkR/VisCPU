using System;
using System.Text;

namespace VPP.Importer
{

    public class VPPTextParser
    {
        private string m_Text;
        private int m_Position;

        public char Current => m_Text[m_Position];
        public char[] Data => m_Text.ToCharArray();
        private static readonly StringBuilder s_Builder = new StringBuilder();

        public override string ToString()
        {
            return m_Text;
        }

        public VPPTextParser(string text)
        {
            m_Text = text.Replace("\r", "");
        }

        public void Set(char c) => Set(m_Position, c);
        public void Set(int i, char c) => m_Text = m_Text.Remove(i, 1).Insert(i, c.ToString());
        public void Insert(string c) => m_Text = m_Text.Insert(m_Position, c);
        public void Replace(string oldS, string newS) => m_Text = m_Text.Replace(oldS, newS);

        public int Eat() => Eat( m_Text[m_Position] );
        public int Eat(char c)
        {
            if (c == m_Text[m_Position])
            {
                m_Position++;
                return m_Position - 1;
            }

            throw new Exception();
        }

        public int EatUntil(char c)
        {
            while (!Is(c)) Eat(m_Text[m_Position]);

            return m_Position;
        }

        public int Eat(string s)
        {
            int r = m_Position;
            foreach (char c in s)
            {
                Eat(c);
            }

            return r;
        }

        public bool IsInRange() => m_Position >= 0 && m_Position < m_Text.Length;
        public bool Is(char c) => m_Text[m_Position] == c;
        public bool IsWhiteSpace() => IsInRange() && char.IsWhiteSpace(m_Text, m_Position);
        public bool IsWordBegin() => char.IsLetter(m_Text, m_Position) || Is('_');
        public bool IsWordMiddle() => char.IsLetterOrDigit(m_Text, m_Position) || Is('_') || Is('.');


        public string EatUntilWhitespace()
        {
            s_Builder.Clear();
            while (!IsWhiteSpace())
            {
                if ( !IsInRange() )
                    break;
                s_Builder.Append(m_Text[m_Position]);
                Eat(m_Text[m_Position]);
            }
            string s = s_Builder.ToString();
            s_Builder.Clear();
            return s;
        }

        public string EatNumber()
        {
            if(!char.IsDigit(m_Text, m_Position))
                throw new Exception();
            s_Builder.Clear();

            s_Builder.Append(m_Text[m_Position]);
            Eat(m_Text[m_Position]);
            while (char.IsDigit(m_Text, m_Position))
            {
                s_Builder.Append(m_Text[m_Position]);
                Eat(m_Text[m_Position]);
            }

            string s = s_Builder.ToString();
            s_Builder.Clear();
            return s;
        }

        public string EatWordOrNumber()
        {
            if (!IsWordBegin())
                return EatNumber();
            return EatWord();
        }

        public string EatWord()
        {
            if (!IsWordBegin())
                throw new Exception();
            s_Builder.Clear();

            s_Builder.Append(m_Text[m_Position]);
            Eat(m_Text[m_Position]);

            while (IsWordMiddle())
            {
                s_Builder.Append(m_Text[m_Position]);
                Eat(m_Text[m_Position]);
            }

            string s = s_Builder.ToString();
            s_Builder.Clear();
            return s;
        }

        public void EatWhiteSpace()
        {
            while (IsWhiteSpace())
                Eat(m_Text[m_Position]);
        }

        public void EatWhiteSpaceUntilNewLine()
        {
            while (IsWhiteSpace())
            {
                if (Is('\n'))
                    break;

                Eat(m_Text[m_Position]);
            }
        }

        public void SetPosition(int p) => m_Position = p;

        public int Seek( string s )
        {
            string t =  m_Position == 0 ? m_Text : m_Text.Remove( 0, m_Position ) ;
            int idx = t.IndexOf(
                                s,
                                StringComparison.InvariantCulture
                               );

            if ( idx == -1 )
                return -1;
            return m_Position = m_Position + idx;
        }
        public bool IsValidPostWordCharacter(int idx) => idx < 0 || idx >= m_Text.Length || !char.IsLetterOrDigit(m_Text[idx]);

        public bool IsValidPreWordCharacter(int idx) => 
            idx < 0 || idx >= m_Text.Length || !char.IsLetterOrDigit(m_Text[idx]);

        public int Seek(char s) => m_Position = m_Text.IndexOf(s, m_Position);

        public void Remove(int length) => m_Text = m_Text.Remove(m_Position, length);

        public void RemoveReverse( int start )
        {
            m_Text = m_Text.Remove(start, m_Position - start);
            m_Position = start;
        }

        public string Get(int length) => m_Text.Substring(m_Position, length);

        public int FindClosing(char open, char close)
        {
            int level = 1;
            while (true)
            {
                if (Is(open)) level++;
                else if (Is(close)) level--;
                if (level == 0)
                {
                    break;
                }

                Eat(m_Text[m_Position]);

            }

            int r = m_Position;
            Eat(m_Text[m_Position]);

            return r;
        }

    }

}