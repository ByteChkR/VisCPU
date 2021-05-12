using System;
using System.Collections.Generic;
using System.Linq;

using VisCPU.HL.DataTypes;
using VisCPU.HL.Importer;
using VisCPU.Integration;
using VisCPU.Peripherals;
using VisCPU.Peripherals.Memory;
using VisCPU.Utility.SharedBase;

namespace VisCPU.SIMD
{

    public class SIMDIntrinsics : PeripheralImporter
    {

        private uint[] m_Data;
        private ApiImporter m_Importer;
        private uint m_Start;

        #region Public

        public SIMDIntrinsics()
        {
            m_Importer = new ApiImporter();
            ImporterSystem.Add( m_Importer );
            m_Start = 0xFFFF9000;
        }

        public void Add( uint a, uint b, uint r, uint l )
        {
            for ( uint i = 0; i < l; i++ )
            {
                m_Data[r + i] = m_Data[a + i] + m_Data[b + i];
            }
        }

        public void AddConstant( uint a, uint c, uint r, uint l )
        {
            for ( uint i = 0; i < l; i++ )
            {
                m_Data[r + i] = m_Data[a + i] + c;
            }
        }

        public void Copy( uint src, uint dst, uint l )
        {
            for ( uint i = 0; i < l; i++ )
            {
                m_Data[dst + i] = m_Data[src + i];
            }
        }

        public void Div( uint a, uint b, uint r, uint l )
        {
            for ( uint i = 0; i < l; i++ )
            {
                m_Data[r + i] = m_Data[a + i] / m_Data[b + i];
            }
        }

        public void DivConstant( uint a, uint c, uint r, uint l )
        {
            for ( uint i = 0; i < l; i++ )
            {
                m_Data[r + i] = m_Data[a + i] / c;
            }
        }

        public override List < Peripheral > GetPeripherals( IEnumerable < Peripheral > p )
        {
            List < Peripheral > r = new List < Peripheral >();
            m_Data = ( p.FirstOrDefault( x => x is Memory ) as Memory )?.GetInternalBuffer();
            r.Add( ExposeApi( AddConstant, "SIMD_HW_AddConstant", 4 ) );
            r.Add( ExposeApi( Add, "SIMD_HW_Add", 4 ) );
            r.Add( ExposeApi( SubConstant, "SIMD_HW_SubConstant", 4 ) );
            r.Add( ExposeApi( Sub, "SIMD_HW_Sub", 4 ) );
            r.Add( ExposeApi( MulConstant, "SIMD_HW_MulConstant", 4 ) );
            r.Add( ExposeApi( Mul, "SIMD_HW_Mul", 4 ) );
            r.Add( ExposeApi( DivConstant, "SIMD_HW_DivConstant", 4 ) );
            r.Add( ExposeApi( Div, "SIMD_HW_Div", 4 ) );
            r.Add( ExposeApi( ModConstant, "SIMD_HW_ModConstant", 4 ) );
            r.Add( ExposeApi( Mod, "SIMD_HW_Mod", 4 ) );
            r.Add( ExposeApi( Set, "SIMD_HW_Set", 3 ) );
            r.Add( ExposeApi( Copy, "SIMD_HW_Copy", 3 ) );
            r.Add( ExposeApi( Move, "SIMD_HW_Move", 3 ) );
            r.Add( ExposeApi( Swap, "SIMD_HW_Swap", 3 ) );

            return r;
        }

        public void Mod( uint a, uint b, uint r, uint l )
        {
            for ( uint i = 0; i < l; i++ )
            {
                m_Data[r + i] = m_Data[a + i] % m_Data[b + i];
            }
        }

        public void ModConstant( uint a, uint c, uint r, uint l )
        {
            for ( uint i = 0; i < l; i++ )
            {
                m_Data[r + i] = m_Data[a + i] % c;
            }
        }

        public void Move( uint src, uint dst, uint l )
        {
            for ( uint i = 0; i < l; i++ )
            {
                m_Data[dst + i] = m_Data[src + i];
                m_Data[src + i] = 0;
            }
        }

        public void Mul( uint a, uint b, uint r, uint l )
        {
            for ( uint i = 0; i < l; i++ )
            {
                m_Data[r + i] = m_Data[a + i] * m_Data[b + i];
            }
        }

        public void MulConstant( uint a, uint c, uint r, uint l )
        {
            for ( uint i = 0; i < l; i++ )
            {
                m_Data[r + i] = m_Data[a + i] * c;
            }
        }

        public void Set( uint a, uint c, uint l )
        {
            uint e = a + l;

            for ( uint i = a; i < e; i++ )
            {
                m_Data[i] = c;
            }
        }

        public void Sub( uint a, uint b, uint r, uint l )
        {
            for ( uint i = 0; i < l; i++ )
            {
                m_Data[r + i] = m_Data[a + i] - m_Data[b + i];
            }
        }

        public void SubConstant( uint a, uint c, uint r, uint l )
        {
            uint e = a + l;

            for ( uint i = a; i < e; i++ )
            {
                m_Data[r + i] = m_Data[a + i] - c;
            }
        }

        public void Swap( uint src, uint dst, uint l )
        {
            for ( uint i = 0; i < l; i++ )
            {
                m_Data[dst + i] ^= m_Data[src + i];
                m_Data[src + i] ^= m_Data[dst + i];
                m_Data[dst + i] ^= m_Data[src + i];
            }
        }

        #endregion

        #region Private

        private uint Add( Cpu i )
        {
            uint l = i.Pop();
            uint r = i.Pop();
            uint b = i.Pop();
            uint a = i.Pop();
            Add( a, b, r, l );

            return 0;
        }

        private uint AddConstant( Cpu i )
        {
            uint l = i.Pop();
            uint r = i.Pop();
            uint c = i.Pop();
            uint a = i.Pop();
            AddConstant( a, c, r, l );

            return 0;
        }

        private uint Copy( Cpu i )
        {
            uint l = i.Pop();
            uint dst = i.Pop();
            uint src = i.Pop();
            Copy( src, dst, l );

            return 0;
        }

        private uint Div( Cpu i )
        {
            uint l = i.Pop();
            uint r = i.Pop();
            uint b = i.Pop();
            uint a = i.Pop();
            Div( a, b, r, l );

            return 0;
        }

        private uint DivConstant( Cpu i )
        {
            uint l = i.Pop();
            uint r = i.Pop();
            uint c = i.Pop();
            uint a = i.Pop();
            DivConstant( a, c, r, l );

            return 0;
        }

        private Peripheral ExposeApi( Func < Cpu, uint > func, string name, int argC )
        {
            m_Importer.AddApi(
                              m_Start,
                              new FunctionData( name, true, true, null, argC, HLBaseTypeNames.s_UintTypeName )
                             );

            ApiImporterDevice d = new ApiImporterDevice( m_Start, func );
            m_Start++;

            return d;
        }

        private uint Mod( Cpu i )
        {
            uint l = i.Pop();
            uint r = i.Pop();
            uint b = i.Pop();
            uint a = i.Pop();
            Mod( a, b, r, l );

            return 0;
        }

        private uint ModConstant( Cpu i )
        {
            uint l = i.Pop();
            uint r = i.Pop();
            uint c = i.Pop();
            uint a = i.Pop();
            ModConstant( a, c, r, l );

            return 0;
        }

        private uint Move( Cpu i )
        {
            uint l = i.Pop();
            uint dst = i.Pop();
            uint src = i.Pop();
            Move( src, dst, l );

            return 0;
        }

        private uint Mul( Cpu i )
        {
            uint l = i.Pop();
            uint r = i.Pop();
            uint b = i.Pop();
            uint a = i.Pop();
            Mul( a, b, r, l );

            return 0;
        }

        private uint MulConstant( Cpu i )
        {
            uint l = i.Pop();
            uint r = i.Pop();
            uint c = i.Pop();
            uint a = i.Pop();
            MulConstant( a, c, r, l );

            return 0;
        }

        private uint Set( Cpu i )
        {
            uint l = i.Pop();
            uint c = i.Pop();
            uint a = i.Pop();
            Set( a, c, l );

            return 0;
        }

        private uint Sub( Cpu i )
        {
            uint l = i.Pop();
            uint r = i.Pop();
            uint b = i.Pop();
            uint a = i.Pop();
            Sub( a, b, r, l );

            return 0;
        }

        private uint SubConstant( Cpu i )
        {
            uint l = i.Pop();
            uint r = i.Pop();
            uint c = i.Pop();
            uint a = i.Pop();
            SubConstant( a, c, r, l );

            return 0;
        }

        private uint Swap( Cpu i )
        {
            uint l = i.Pop();
            uint dst = i.Pop();
            uint src = i.Pop();
            Swap( src, dst, l );

            return 0;
        }

        #endregion

    }

}
