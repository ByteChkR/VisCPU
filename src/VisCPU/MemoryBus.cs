using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using VisCPU.Events;
using VisCPU.Peripherals;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;
using VisCPU.Utility.IO.Settings;
using VisCPU.Utility.Logging;
using VisCPU.Utility.SharedBase;

namespace VisCPU
{

    internal class MemoryBusPeripheralList
    {

        private struct ListItem
        {

            public int AccessCount;
            public Peripheral Peripheral;

        }

        public MemoryBusPeripheralList( IEnumerable < Peripheral > peripherals )
        {
            m_InternalList = peripherals.Select(
                                                x => new ListItem
                                                     {
                                                         AccessCount = 0,
                                                         Peripheral = x
                                                     }
                                               ).
                                         ToList();
        }

            private readonly List < ListItem > m_InternalList;

        public int Count => m_InternalList.Count;

        public void Add( Peripheral p ) =>
            m_InternalList.Add(
                               new ListItem
                               {
                                   AccessCount = 0,
                                   Peripheral = p
                               }
                              );

        public Peripheral this[ int i ] => m_InternalList[i].Peripheral;

        public void ForEach( Action < Peripheral > ac ) => m_InternalList.ForEach( x => ac( x.Peripheral ) );

        public bool All( Func < Peripheral, bool > f ) => m_InternalList.All( x=> f(x.Peripheral) );

        public IEnumerable < T > Get < T >() where T : Peripheral => m_InternalList.Select(x=>x.Peripheral).Where(x=>x is T).Cast < T >();

        public Peripheral Find( uint address )
        {
            Peripheral ret = null;
            for ( int i = 0; i < m_InternalList.Count; i++ )
            {
                ListItem listItem = m_InternalList[i];
                Peripheral peripheral = listItem.Peripheral;

                if ( peripheral.AddressRangeEnd < address ||
                     peripheral.AddressRangeStart > address )
                {
                    continue;
                }

                listItem.AccessCount++;
                m_InternalList[i] = listItem;

                ret = peripheral;
                BubbleUp( i );
                break;
            }



            return ret;
        }

        private void BubbleUp( int itemIndex )
        {
            int ac = m_InternalList[itemIndex].AccessCount;
            for ( int i = itemIndex - 1; i >= 0; i-- )
            {
                if ( ac <= m_InternalList[i].AccessCount )
                {
                    if ( i == itemIndex - 1 )
                        return;

                    ListItem item = m_InternalList[itemIndex];
                    m_InternalList.RemoveAt( itemIndex );
                    m_InternalList.Insert( i + 1, item );
                    return;
                }
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("Peripheral Usages: \n");

            foreach ( ListItem listItem in m_InternalList )
            {
                sb.AppendFormat( "\t{0} : {1}\n", listItem.AccessCount, listItem.Peripheral.PeripheralName );
            }

            return sb.ToString();
        }

    }

    public class MemoryBus : VisBase
    {

        private readonly MemoryBusPeripheralList m_Peripherals;
        private readonly CpuSettings m_Settings;

        public int PeripheralCount => m_Peripherals.Count;

        protected override LoggerSystems SubSystem => LoggerSystems.MemoryBus;

        #region Unity Event Functions

        public void Reset()
        {
            m_Peripherals.ForEach( x => x.Reset() );
        }

        #endregion

        #region Public

        public MemoryBus() : this( new List < Peripheral >() { new MemoryBusDriver() } )
        {
        }

        public MemoryBus( IEnumerable < Peripheral > peripherals )
        {
            m_Peripherals = new MemoryBusPeripheralList(peripherals);

            if ( m_Peripherals.All( x => x.PeripheralType != PeripheralType.MemoryBusDriver ) )
            {
                m_Peripherals.Add( new MemoryBusDriver() );
            }

            m_Settings=  SettingsManager.GetSettings < CpuSettings >();
        }

        public MemoryBus( params Peripheral[] peripherals ): this((IEnumerable <Peripheral>)peripherals)
        {
            
        }

        public void Add( Peripheral p )
        {
            m_Peripherals.Add( p );
        }

        public void Dump()
        {
            for ( int i = 0; i < m_Peripherals.Count; i++ )
            {
                Peripheral peripheral = m_Peripherals[i];
                FileStream fs = File.Create( ".\\crash.per_" + i + ".dump" );
                peripheral.Dump( fs );
                fs.Close();
            }
        }

        public Peripheral GetPeripheralAt( int index )
        {
            return m_Peripherals[index];
        }

        public IEnumerable < T > GetPeripherals < T >() where T : Peripheral
        {
            return m_Peripherals.Get < T >();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public uint Read( uint address )
        {
            uint data = 0;

            Peripheral receiver = m_Peripherals.Find( address );

            if ( receiver != null )
                data = receiver.ReadData( address );
            else if(m_Settings.WarnOnUnmappedAccess)
                EventManager<WarningEvent>.SendEvent(new ReadFromUnmappedAddressEvent(address));
            
            return data;
        }

        public void Shutdown()
        {
            Log( m_Peripherals.ToString() );
            m_Peripherals.ForEach( x => x.Shutdown() );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public void Write( uint address, uint data )
        {
            Peripheral receiver = m_Peripherals.Find( address );
            if (receiver != null)
                 receiver.WriteData(address, data);
            else if(m_Settings.WarnOnUnmappedAccess)
                EventManager<WarningEvent>.SendEvent(new WriteToUnmappedAddressEvent(address, data));
        }

        internal void SetCpu( Cpu cpu )
        {
            m_Peripherals.ForEach( x => x.SetCpu( cpu ) );
        }

        #endregion

    }

}
