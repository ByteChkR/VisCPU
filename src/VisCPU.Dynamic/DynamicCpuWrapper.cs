using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

using VisCPU.Utility;
using VisCPU.Utility.SharedBase;

namespace VisCPU.Dynamic
{

    public class DynamicCpuWrapper : DynamicObject
    {

        private readonly Cpu m_Instance;

        public IEnumerable < string > Labels =>
            m_Instance.SymbolServer.LoadedSymbols.SelectMany( x => x.Labels.Keys ).Distinct();

        public IEnumerable < string > Constants =>
            m_Instance.SymbolServer.LoadedSymbols.SelectMany( x => x.Constants.Keys ).Distinct();

        public IEnumerable < string > Data =>
            m_Instance.SymbolServer.LoadedSymbols.
                       SelectMany( x => x.DataSectionHeader.Keys ).
                       Distinct();

        #region Public

        public DynamicCpuWrapper( Cpu instance )
        {
            m_Instance = instance;
        }

        public AddressItem GetConstant( string name )
        {
            return m_Instance.SymbolServer.LoadedSymbols.
                              First( x => x.Constants.ContainsKey( name ) ).
                              Constants[name];
        }

        public uint GetConstantValue( string name )
        {
            if ( !Constants.Contains( name ) )
            {
                return 0;
            }

            return GetConstant( name ).Address;
        }

        public AddressItem GetData( string name )
        {
            return m_Instance.SymbolServer.LoadedSymbols.
                              First( x => x.DataSectionHeader.ContainsKey( name ) ).
                              DataSectionHeader[name];
        }

        public uint GetDataValue( string name )
        {
            if ( !Data.Contains( name ) )
            {
                return 0;
            }

            return m_Instance.MemoryBus.Read( GetData( name ).Address );
        }

        public override IEnumerable < string > GetDynamicMemberNames()
        {
            return Labels.Concat( Constants ).Concat( Data );
        }

        public AddressItem GetLabel( string label )
        {
            return m_Instance.SymbolServer.LoadedSymbols.
                              First( x => x.Labels.ContainsKey( label ) ).
                              Labels[label];
        }

        public uint InvokeLabel( string name, object[] args )
        {
            if ( name == "Entry" )
            {
                return Run( 0 );
            }

            if ( !Labels.Contains( name ) )
            {
                return 0;
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

            return Run( GetLabel( name ).Address );
        }

        public void SetDataValue( string name, uint value )
        {
            if ( !Data.Contains( name ) )
            {
                return;
            }

            m_Instance.MemoryBus.Write( GetData( name ).Address, value );
        }

        public override bool TryGetMember( GetMemberBinder binder, out object result )
        {
            if ( Labels.Contains( binder.Name ) )
            {
                result = GetLabel( binder.Name ).Address;

                return true;
            }

            if ( Constants.Contains( binder.Name ) )
            {
                result = GetConstantValue( binder.Name );

                return true;
            }

            if ( Data.Contains( binder.Name ) )
            {
                result = GetDataValue( binder.Name );

                return true;
            }

            result = null;

            return false;
        }

        public override bool TryInvoke( InvokeBinder binder, object[] args, out object result )
        {
            result = Run( 0 );

            return true;
        }

        public override bool TryInvokeMember( InvokeMemberBinder binder, object[] args, out object result )
        {
            if ( Labels.Contains( binder.Name ) )
            {
                result = Run( GetLabel( binder.Name ).Address );

                return true;
            }

            result = null;

            return false;
        }

        public override bool TrySetMember( SetMemberBinder binder, object value )
        {
            if ( Data.Contains( binder.Name ) )
            {
                if ( value is float f )
                {
                    SetDataValue( binder.Name, f.RawConvert() );
                }
                else
                {
                    SetDataValue( binder.Name, ( uint ) Convert.ChangeType( value, TypeCode.UInt32 ) );
                }

                return true;
            }

            return false;
        }

        #endregion

        #region Private

        private uint Run( uint address )
        {
            m_Instance.PushState( address );

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
