using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using VisCPU.Utility;
using VisCPU.Utility.IO.Settings;

namespace VisCPU.Peripherals.Drive
{
    public abstract class Drive : Peripheral
    {
        protected DrivePinSettings m_Settings;


        public override PeripheralType PeripheralType => PeripheralType.Drive;

        public override uint PresentPin => m_Settings.PresentAddress;


        public abstract uint Read(uint address);
        public abstract void ReadBuffer(uint address, uint destination, uint start, uint length);
        public abstract void Write(uint address, uint data);
        public abstract void WriteBuffer(uint address, uint destination, uint start, uint length);
        public abstract uint GetSize();

        private uint m_WriteDataStep = 0;
        private uint m_WriteDataAddr = 0;

        private uint m_ReadDataStep = 0;
        private uint m_ReadDataAddr = 0;


        private uint m_WriteBufferStep = 0;
        private uint m_WriteBufferDstAddr = 0;
        private uint m_WriteBufferSrcAddr = 0;
        private uint m_WriteBufferSrcStart = 0;
        private uint m_WriteBufferSrcLen = 0;

        private uint m_ReadBufferStep = 0;
        private uint m_ReadBufferDstAddr = 0;
        private uint m_ReadBufferSrcAddr = 0;
        private uint m_ReadBufferSrcStart = 0;
        private uint m_ReadBufferSrcLen = 0;

        #region Unity Event Functions

        #endregion

        #region Public

        protected Drive(DrivePinSettings settings)
        {
            m_Settings = settings;

            
        }

        public override bool CanRead(uint address)
        {
            return m_Settings.GetSizeAddress == address ||
                   m_Settings.PresentAddress == address ||
                   m_Settings.ReadAddress == address;
        }

        public override bool CanWrite(uint address)
        {
            return m_Settings.GetSizeAddress == address ||
                   m_Settings.ReadAddress == address ||
                   m_Settings.ReadBufferAddress == address ||
                   m_Settings.WriteAddress == address ||
                   m_Settings.WriteBufferAddress == address;
        }

        public override uint ReadData(uint address)
        {
            if (m_Settings.PresentAddress == address)
                return 1;
            if (m_Settings.ReadAddress == address && m_ReadDataStep == 1)
            {
                m_ReadDataStep = 0;

                return Read( m_ReadDataAddr );
            }
            if (m_Settings.GetSizeAddress == address)
            {
                return GetSize();
            }

            return 0;
        }

        public override void WriteData(uint address, uint data)
        {
            if (address == m_Settings.WriteAddress)
            {
                if (m_WriteDataStep == 0)
                {
                    m_WriteDataStep++;
                    m_WriteDataAddr = data;
                }
                else
                {
                    m_WriteDataStep = 0;
                    Write( m_WriteDataAddr, data );
                }
            }

            if (address == m_Settings.ReadAddress)
            {
                if (m_ReadDataStep == 0)
                {
                    m_ReadDataStep++;
                    m_ReadDataAddr = data;
                }
            }

            if (address == m_Settings.WriteBufferAddress)
            {
                if (m_WriteBufferStep == 0)
                {
                    m_WriteBufferStep++;
                    m_WriteBufferDstAddr = data;
                }
                else if (m_WriteBufferStep == 1)
                {
                    m_WriteBufferStep++;
                    m_WriteBufferSrcAddr = data;
                }
                else if (m_WriteBufferStep == 2)
                {
                    m_WriteBufferStep++;
                    m_WriteBufferSrcStart = data;
                }
                else
                {
                    m_WriteBufferSrcLen = data;
                    m_WriteBufferStep = 0;

                    WriteBuffer(
                                m_WriteBufferDstAddr,
                                m_WriteBufferSrcAddr,
                                m_WriteBufferSrcStart,
                                m_WriteBufferSrcLen
                               );
                }


            }
            if (address == m_Settings.ReadBufferAddress)
            {
                if (m_ReadBufferStep == 0)
                {
                    m_ReadBufferStep++;
                    m_ReadBufferDstAddr = data;
                }
                else if (m_ReadBufferStep == 1)
                {
                    m_ReadBufferStep++;
                    m_ReadBufferSrcAddr = data;
                }
                else if (m_ReadBufferStep == 2)
                {
                    m_ReadBufferStep++;
                    m_ReadBufferSrcStart = data;
                }
                else
                {
                    m_ReadBufferSrcLen = data;
                    m_ReadBufferStep = 0;

                    ReadBuffer(m_ReadBufferDstAddr, m_ReadBufferSrcAddr , m_ReadBufferSrcStart, m_ReadBufferSrcLen );
                }
            }
        }

        #endregion


    }
    public class FileDrive : Drive
    {
        public override string PeripheralName => "File Drive";

        private FileStream m_FileStream;

        public FileDrive() : this(SettingsManager.GetSettings<FileDriveSettings>()) { }

        public FileDrive( FileDriveSettings settings ) : base( settings )
        {
            m_FileStream = File.Open(settings.FileDrive, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            m_FileStream.SetLength(settings.FileLength);
            Log("File Drive is Present");
        }

        public override void Reset()
        {
            m_FileStream.Close();
            FileDriveSettings ds = ( FileDriveSettings ) m_Settings ;
            m_FileStream = File.Open(ds.FileDrive, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        }

        public override void Shutdown()
        {
            base.Shutdown();
            m_FileStream.Close();
        }

        public override uint Read( uint address )
        {
            m_FileStream.Position = address * sizeof(uint);
            byte[] d = new byte[sizeof(uint)];
            m_FileStream.Read(d, 0, sizeof(uint));

            return BitConverter.ToUInt32(d, 0);
        }

        public override void ReadBuffer( uint address, uint destination, uint start, uint length )
        {
            IEnumerable<Memory.Memory> n = AttachedCpu.MemoryBus.GetPeripherals<Memory.Memory>();

            foreach (Memory.Memory memory in n)
            {
                if (memory.StartAddress <= destination &&
                    memory.EndAddress >= destination + destination)
                {
                    m_FileStream.Position = address * sizeof(uint);

                    byte[] buffer = new byte[length * sizeof(uint)];

                    m_FileStream.Read(buffer, 0, buffer.Length);

                    Array.Copy(
                               buffer.ToUInt(),
                               0,
                               memory.GetInternalBuffer(),
                               (int)(destination - memory.StartAddress + start),
                               length
                              );
                }
            }
        }

        public override void Write( uint address , uint data)
        {
            m_FileStream.Position = address * sizeof(uint);
            m_FileStream.Write(BitConverter.GetBytes(data), 0, sizeof(uint));
        }

        public override void WriteBuffer( uint address, uint destination, uint start, uint length )
        {
            IEnumerable<Memory.Memory> n = AttachedCpu.MemoryBus.GetPeripherals<Memory.Memory>();

            foreach (Memory.Memory memory in n)
            {
                if (memory.StartAddress <= destination &&
                    memory.EndAddress >= destination + start + length)
                {
                    m_FileStream.Position = address * sizeof(uint);

                    byte[] buffer = memory.GetInternalBuffer().
                                           Skip((int)(destination - memory.StartAddress + start)).
                                           Take((int)length).
                                           SelectMany(BitConverter.GetBytes).
                                           ToArray();

                    m_FileStream.Write(buffer, 0, buffer.Length);
                }
            }
        }

        public override uint GetSize()
        {
            return (uint)m_FileStream.Length / sizeof(uint);
        }

    }

}
