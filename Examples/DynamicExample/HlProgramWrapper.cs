using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using VisCPU;
using VisCPU.Utility;
using VisCPU.Utility.SharedBase;

namespace DynamicExample
{

    //Dynamic Object implementation for Vis CPU Binaries.
    //Requires Linker Symbols
    public class HlProgramWrapper : DynamicObject
    {
        //The CPU Instance
        private readonly Cpu m_Instance;

        #region Public

        public HlProgramWrapper( Cpu instance )
        {
            m_Instance = instance;

            //Invoke Entry Point(Global Level Expressions)
            Invoke( "Entry" );
        }


        //All Labels that can be jumped to.
        public override IEnumerable < string > GetDynamicMemberNames()
        {
            return m_Instance.SymbolServer.LoadedSymbols.SelectMany( x => x.Labels.Keys ).Distinct();
        }

        //Invoke Implementation.
        public uint Invoke( string name, params object[] args )
        {
            m_Instance.ClearStackAndStates();

            if ( name == "Entry" )
            {
                return RunPc( 0 );
            }



            foreach ( object arg in args )
            {
                if ( arg is uint u )
                {
                    m_Instance.Push( u );
                }
                else if ( arg is int i )
                {
                    m_Instance.Push( ( uint ) i );
                }
                else if ( arg is float f )
                {
                    m_Instance.Push( f.RawConvert() );
                }
            }

            return RunPc(FindLabel(name).Address );

        }

        private bool ContainsLabel(string name)
        {
            return m_Instance.SymbolServer.LoadedSymbols.Any(x => x.Labels.ContainsKey(name));
        }

        private bool ContainsConstant(string name)
        {
            return m_Instance.SymbolServer.LoadedSymbols.Any(x => x.Constants.ContainsKey(name));
        }

        private bool ContainsVariable(string name)
        {
            return m_Instance.SymbolServer.LoadedSymbols.Any(x => x.DataSectionHeader.ContainsKey(name));
        }

        private AddressItem FindLabel(string name)
        {
            return m_Instance.SymbolServer.LoadedSymbols
                             .Where(x => x.Labels.ContainsKey(name))
                             .Select(x => x.Labels[name])
                             .First();
        }
        private AddressItem FindConstant(string name)
        {
            return m_Instance.SymbolServer.LoadedSymbols
                             .Where(x => x.Constants.ContainsKey(name))
                             .Select(x => x.Constants[name])
                             .First();
        }

        private AddressItem FindVariable(string name)
        {
            return m_Instance.SymbolServer.LoadedSymbols
                             .Where(x => x.DataSectionHeader.ContainsKey(name))
                             .Select(x => x.DataSectionHeader[name])
                             .First();
        }

        //Get Value Implementation
        public uint GetValue(string name)
        {
            if (ContainsLabel(name))
            {
                //Return Address of Label(Function Start in Memory)
                return FindLabel(name).Address;
            }
            else if (ContainsConstant(name))
            {
                //Return Actual Constant Value(entirely from Linker Info, constant values are resolved at compiletime)
                return FindConstant(name).Address;
            }
            else if (ContainsVariable("__" + name))
            {
                //Return Value of the variable (Only works with Global Variables)
                return m_Instance.MemoryBus.Read(FindVariable("__" + name).Address);
            }

            return 0;

        }

        //Set Variable Implementation
        public void SetValue(string name, object value)
        {
            if (ContainsVariable("__" + name))
            {
                m_Instance.MemoryBus.Write(FindVariable("__" + name).Address, (uint)value);
            }
        }

        //Dynamic Override
        //Enables code like: uint v = dynamicObject.MyVariable
        public override bool TryGetMember( GetMemberBinder binder, out object result )
        {
            if ( ContainsLabel( binder.Name ) )
            {
                result =FindLabel(binder.Name).Address;

                return true;
            }
            else if ( ContainsConstant( binder.Name ) )
            {
                result = FindConstant(binder.Name).Address;

                return true;
            }
            else if (ContainsVariable( "__" + binder.Name ) )
            {
                AddressItem item = FindVariable( "__" + binder.Name );
                result = m_Instance.MemoryBus.Read(item.Address );

                return true;
            }

            result = null;

            return false;
        }

        //Dynamic Override
        //Enables code like: uint v = dynamicObject.MyFunction()
        public override bool TryInvokeMember( InvokeMemberBinder binder, object[] args, out object result )
        {
            if ( binder.Name == "Entry" )
            {
                result = RunPc( 0 );

                return true;
            }

            if (ContainsLabel( binder.Name ) )
            {
                result = Invoke( binder.Name, args );

                return true;
            }

            result = null;

            return false;

        }

        //Dynamic Override
        //Enables code like: dynamicObject.MyVariable = 123
        public override bool TrySetMember( SetMemberBinder binder, object value )
        {
            if ( ContainsVariable( "__" + binder.Name ) )
            {
                SetValue( binder.Name, value );
                return true;
            }

            return false;
        }

        #endregion

        #region Private

        //Jumps to a position in Memory and Starts executing until the CPU is halted or returns from the position we started at.
        private uint RunPc( uint pc )
        {
            m_Instance.PushState( pc );

            while ( m_Instance.StackDepth != 0 )
            {
                m_Instance.Cycle();

                if ( m_Instance.HasSet( Cpu.Flags.Halt ) )
                {
                    m_Instance.UnSet( Cpu.Flags.Halt );
                    return 0;
                }
            }

            return m_Instance.Pop();
        }

        #endregion
    }

}
