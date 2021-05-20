using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using VisCPU.HL.TextLoader;

using VPP.Importer.Directives;

namespace VPP.Importer
{

    public class VPPImporter : TextImporter
    {

        public override string Name => nameof( VPPImporter );

        private static readonly List <VPPDirective> s_Directives = new List < VPPDirective >()
                                                                   {
                                                                       new ForKeyDirective(),
                                                                       new ForLineDirective(),
                                                                       new IfDirective()
                                                                   };

        #region Public

        public override string Import( string text, string rootDir )
        {
            ( string ret, List < VPPMakro > makros ) =
                InnerImport( text.Replace( "\r", "" ), rootDir, CreateFromArgs() );

            return ret;
        }

        #endregion

        #region Private

        private static List < VPPMakro > CreateFromArgs()
        {
            List < VPPMakro > m = new List < VPPMakro >{new VPPUniqueMakro(), new VPPGetValueMakro()};

            foreach ( (string, string) importerArg in ImporterArgs )
            {
                m.Add(
                      new VPPTextMakro(importerArg.Item1, importerArg.Item2, new List < VPPMakroParameter >())
                     );
            }

            return m;
        }

        private static List < string > ParseList( VPPTextParser parser, Func < VPPTextParser, bool > isEnd )
        {
            List < string > p = new List < string >();

            while ( true )
            {
                if ( isEnd( parser ) )
                {
                    break;
                }

                p.Add( parser.EatAnyUntil(isEnd , x => x.Is(',')) );

                if ( isEnd( parser ) )
                {
                    break;
                }

                parser.Eat( ',' );
                parser.EatWhiteSpace();
            }

            return p;
        }

        private static bool ResolveDirective(VPPDirective directive, List <VPPMakro> makros, VPPTextParser parser )
        {
            parser.SetPosition(0);

            int idx;

            bool resolved = false;

            while ((idx = parser.Seek(directive.DirectiveName)) != -1)
            {
                parser.Eat(directive.DirectiveName);

                if (!parser.IsValidPreWordCharacter(idx - 1) ||
                    !parser.IsValidPostWordCharacter(idx + directive.DirectiveName.Length))
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
                    parser.Remove(end + 1 - idx);

                    if ( directive.ResolveParameters )
                    {
                        for ( int i = 0; i < p.Count; i++ )
                        {
                            VPPTextParser arg = new VPPTextParser( p[i] );

                            while ( ResolveMakros(arg , makros ) )
                            {

                            }

                            p[i] = arg.ToString();
                        }
                    }

                    string resolvedString = directive.Resolve( makros, parser, p.ToArray() );
                    parser.Insert(resolvedString);
                }
                else
                {
                    parser.SetPosition(idx);
                    parser.Remove(directive.DirectiveName.Length);
                    parser.Insert(directive.Resolve(makros, parser, new string[0]));
                }
                resolved = true;
            }

            return resolved;
        }

        private static bool ResolveMakro( VPPMakro makro, VPPTextParser parser )
        {
            parser.SetPosition( 0 );

            int idx;

            bool resolved = false;

            while ( ( idx = parser.Seek( makro.Name ) ) != -1 )
            {
                parser.Eat( makro.Name );

                if ( !parser.IsValidPreWordCharacter( idx - 1 ) ||
                     !parser.IsValidPostWordCharacter( idx + makro.Name.Length ) )
                {
                    continue;
                }

                parser.EatWhiteSpace();

                if ( parser.Is( '(' ) )
                {
                    parser.Eat( '(' );
                    List < string > p = ParseList( parser, x => x.Is( ')' ) );
                    int end = parser.Eat( ')' );
                    parser.SetPosition( idx );
                    parser.Remove( end + 1 - idx );
                    parser.Insert( makro.GenerateValue( p.ToArray() ) );
                }
                else
                {
                    parser.SetPosition( idx );
                    parser.Remove( makro.Name.Length );
                    parser.Insert( makro.GenerateValue( new string[0] ) );
                }

                resolved = true;
            }

            return resolved;
        }



        private static bool ResolveMakros(VPPTextParser parser, List<VPPMakro> makros)
        {
            bool resolved = false;

            foreach (VPPMakro vppMakro in makros)
            {
                resolved |= ResolveMakro(vppMakro, parser);
            }

            return resolved;
        }

        private static bool ResolveDirectives(VPPTextParser parser, List <VPPDirective> directives, List<VPPMakro> makros, bool makroResolved)
        {
            bool resolved = false;

            foreach (VPPDirective directive in directives)
            {
                if(directive.ResolveParameters == makroResolved)
                    resolved |= ResolveDirective(directive, makros, parser);
            }

            return resolved;
        }

        private (string, List < VPPMakro >) InnerImport( string text, string rootDir, List < VPPMakro > makros = null )
        {
            VPPTextParser parser = new( text );
            makros ??= new List < VPPMakro >();

            List < string > incs = new List < string >();

            bool recurse = true;

            while ( recurse )
            {
                List < VPPMakro > curM = ParseDefines( parser );
                List < string > curI = ParseIncludes( parser ).Concat( ParseInlines( parser ) ).ToList();
                makros.AddRange( curM );
                incs.AddRange( curI );
                recurse = ResolveIncludes( curI, makros, rootDir );
                recurse |= ResolveDirectives(parser, s_Directives, makros, false);
                recurse |= ResolveMakros(parser, makros);
                recurse |= ResolveDirectives(parser, s_Directives, makros, true);
            }

            return ( parser.ToString(), makros );
        }


        private List < VPPMakro > ParseDefines( VPPTextParser parser )
        {
            int defIndex;

            List < VPPMakro > ret = new();

            while ( ( defIndex = parser.Seek( "#define" ) ) != -1 )
            {
                parser.Eat( "#define" );
                parser.EatWhiteSpace();
                string var = parser.EatWord();
                parser.EatWhiteSpaceUntilNewLine();

                if ( parser.Is( '(' ) )
                {
                    ret.Add( ParseFuncDefine( defIndex, var, parser ) );

                    continue;
                }

                string value = "1";

                if ( parser.Is( '\n' ) )
                {
                    parser.Eat( '\n' );
                    parser.RemoveReverse( defIndex );
                }
                else
                {
                    value = parser.EatUntilWhitespace();
                    parser.RemoveReverse( defIndex );
                }

                ret.Add(
                        new VPPTextMakro(var, value, new List < VPPMakroParameter >())
                       );
            }

            return ret;
        }

        

        private VPPMakro ParseFuncDefine( int start, string var, VPPTextParser parser )
        {
            parser.Eat( '(' );
            List < string > p = ParseList( parser, x => x.Is( ')' ) );
            parser.Eat( ')' );

            parser.EatWhiteSpace();

            int pStart = parser.Eat( '{' );
            int end = parser.FindClosing( '{', '}' );
            parser.SetPosition( pStart + 1 );
            string block = parser.Get( end - pStart - 1 ).Trim();
            parser.SetPosition( start );
            parser.Remove( end + 1 - start );

            return new VPPTextMakro( var, block, p.Select( x => new VPPMakroParameter { Name = x } ).ToList() );
        }

        private string[] ParseIncludes( VPPTextParser parser )
        {
            return ParseKeyedValues( parser, "#include" );
        }

        private string[] ParseInlines( VPPTextParser parser )
        {
            return ParseKeyedValues( parser, "#inline" );
        }

        private string[] ParseKeyedValues( VPPTextParser parser, string key )
        {
            parser.SetPosition( 0 );
            int defIndex;

            List < string > ret = new();

            while ( ( defIndex = parser.Seek( key ) ) != -1 )
            {
                parser.Eat( key );
                parser.EatWhiteSpace();

                if ( parser.Is( '\"' ) )
                {
                    continue;
                }

                int pstart = parser.Eat( '<' );
                int end = parser.EatUntil( '>' );
                parser.SetPosition( pstart + 1 );

                ret.Add( parser.Get( end - pstart - 1 ) );
                parser.Set( pstart, '\"' );
                parser.Set( end, '\"' );
            }

            return ret.ToArray();
        }

        private bool ResolveIncludes( List < string > includes, List < VPPMakro > result, string rootDir )
        {
            bool resolved = false;

            foreach ( string include in includes )
            {
                resolved = true;
                string file = Path.GetFullPath( Path.Combine( rootDir, include ) );
                string root = Path.GetDirectoryName( file );
                ( string s2, List < VPPMakro > fileMakros ) = InnerImport( File.ReadAllText( file ), root );
                result.AddRange( fileMakros );
            }

            return resolved;
        }

        #endregion

    }

}
