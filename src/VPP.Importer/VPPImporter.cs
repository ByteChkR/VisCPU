using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using VisCPU.HL.TextLoader;

namespace VPP.Importer
{

    public class VPPImporter : TextImporter
    {

        public override string Name => nameof(VPPImporter);

        #region Public

        public override string Import(string text, string rootDir)
        {
            (string ret, List<VPPMakro> makros) = InnerImport(text.Replace("\r", ""), rootDir);

            return ret;
        }

        #endregion

        #region Private

        private static void ResolveMakro(VPPMakro makro, VPPTextParser parser)
        {
            

            parser.SetPosition(0);

            int idx;

            while ((idx = parser.Seek(makro.Name)) != -1)
            {
                parser.Eat(makro.Name);
                if (!parser.IsValidPreWordCharacter(idx - 1) || !parser.IsValidPostWordCharacter(idx+makro.Name.Length))
                {
                    continue;
                }

                parser.EatWhiteSpace();

                if (parser.Is('('))
                {
                    parser.Eat('(');
                    List<string> p = ParseList(parser, x => x.Is(')'));
                    int end = parser.Eat(')');
                    parser.SetPosition(idx);
                    parser.Remove(end+1 - idx);
                    parser.Insert(makro.GenerateValue(p.ToArray()));
                }
                else
                {
                    parser.SetPosition(idx);
                    parser.Remove(makro.Name.Length);
                    parser.Insert(makro.GenerateValue(new string[0]));
                }
            }
        }

        private static void ResolveMakros(VPPTextParser parser, List<VPPMakro> makros)
        {
            foreach (VPPMakro vppMakro in makros)
            {
                ResolveMakro(vppMakro, parser);
            }

        }

        private (string, List<VPPMakro>) InnerImport(string text, string rootDir)
        {
            VPPTextParser parser = new(text);
            string[] includes = ParseIncludes(parser).Concat(ParseInlines(parser)).ToArray();
            List<VPPMakro> makros = ParseDefines(parser);

            ResolveIncludes(includes, makros, rootDir);

            ResolveMakros(parser, makros);

            return (parser.ToString(), makros);
        }

        private string[] ParseKeyedValues(VPPTextParser parser, string key)
        {
            parser.SetPosition( 0 );
            int defIndex;

            List<string> ret = new();

            while ((defIndex = parser.Seek(key)) != -1)
            {
                parser.Eat(key);
                parser.EatWhiteSpace();
                if (parser.Is('\"'))
                    continue;

                int pstart = parser.Eat('<');
                int end = parser.EatUntil('>');
                parser.SetPosition(pstart + 1);

                ret.Add(parser.Get(end - pstart - 1));
                parser.Set(pstart, '\"');
                parser.Set(end, '\"');
            }
            return ret.ToArray();
        }

        private string[] ParseIncludes(VPPTextParser parser)
        {
            return ParseKeyedValues(parser, "#include");
        }
        private string[] ParseInlines(VPPTextParser parser)
        {
            return ParseKeyedValues(parser, "#inline");
        }

        private static List<string> ParseList(VPPTextParser parser, Func<VPPTextParser, bool> isEnd)
        {
            List<string> p = new List<string>();
            while (true)
            {
                if (isEnd(parser))
                    break;
                p.Add(parser.EatWordOrNumber());
                parser.EatWhiteSpace();
                if (isEnd(parser))
                    break;

                parser.Eat(',');
                parser.EatWhiteSpace();
            }

            return p;
        }

        private VPPMakro ParseFuncDefine(int start, string var, VPPTextParser parser)
        {
            parser.Eat('(');
            List<string> p = ParseList(parser, x => x.Is(')'));
            parser.Eat(')');

            parser.EatWhiteSpace();

            int pStart= parser.Eat('{');
            int end = parser.FindClosing('{', '}');
            parser.SetPosition(pStart+1);
            string block = parser.Get(end - pStart - 1).Trim();
            parser.SetPosition( start );
            parser.Remove(end+1 - start);


            return new VPPMakro
                   {
                       Name = var,
                       Parameters = p.Select(x => new VPPMakroParameter { Name = x }).ToList(),
                       Value = block
                   };
        }

        private List<VPPMakro> ParseDefines(VPPTextParser parser)
        {
            int defIndex;

            List<VPPMakro> ret = new();

            while ((defIndex = parser.Seek("#define")) != -1)
            {
                parser.Eat("#define");
                parser.EatWhiteSpace();
                string var = parser.EatWord();
                parser.EatWhiteSpaceUntilNewLine();

                if (parser.Is('('))
                {
                    ret.Add(ParseFuncDefine(defIndex, var, parser));
                    continue;
                }

                string value = "1";
                if (parser.Is('\n'))
                {
                    parser.Eat('\n');
                    parser.RemoveReverse(defIndex);
                }
                else
                {
                    value = parser.EatUntilWhitespace();
                    parser.RemoveReverse(defIndex);
                }

                ret.Add(
                        new VPPMakro
                        {
                            Name = var,
                            Parameters = new List<VPPMakroParameter>(),
                            Value = value
                        }
                       );
            }
            return ret;
        }

        private void ResolveIncludes(string[] includes, List<VPPMakro> result, string rootDir)
        {
            foreach (string include in includes)
            {
                string file = Path.GetFullPath(Path.Combine(rootDir, include));
                string root = Path.GetDirectoryName(file);
                (string s2, List<VPPMakro> fileMakros) = InnerImport(File.ReadAllText(file), root);
                result.AddRange(fileMakros);
            }
        }

        #endregion

    }

}