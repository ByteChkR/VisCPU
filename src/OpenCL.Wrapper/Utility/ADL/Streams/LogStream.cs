using System;
using System.IO;
using System.Threading;

namespace Utility.ADL.Streams
{
    /// <summary>
    ///     Log stream class that you can use with virtually any stream(as long as you handle the multithreading on your own if
    ///     used)
    ///     This class wraps around your supplied stream and interacts with the system.
    ///     Because the LogStream implements every override and passes it to the base stream,
    ///     you can use your created base stream itself instead of the LogStream.
    /// </summary>
    public class LogStream : Stream
    {

        /// <summary>
        ///     Base stream
        /// </summary>
        protected readonly Stream BaseStream;


        /// <summary>
        ///     Creates a Log stream based on the parameters supplied.
        /// </summary>
        /// <param name="baseStream"></param>
        /// <param name="mask"></param>
        /// <param name="maskMatchType"></param>
        /// <param name="setTimeStamp"></param>
        public LogStream(Stream baseStream, bool setTimeStamp = false)
        {
            AddTimeStamp = setTimeStamp;
            BaseStream = baseStream;
        }


        /// <summary>
        ///     Is the stream closed?
        /// </summary>
        public bool IsClosed { get; protected set; }

        /// <summary>
        ///     Writes a log to the stream.
        /// </summary>
        /// <param name="log">the log to send</param>
        public virtual void Write(Log log)
        {
            if (IsClosed)
            {
                return;
            }

            if (AddTimeStamp)
            {
                log.Message = Utils.TimeStamp + log.Message;
            }

            byte[] buffer = log.Serialize();
            BaseStream.Write(buffer, 0, buffer.Length);
            Flush();
        }

        #region Properties

        /// <summary>
        ///     If true all the LogChannels/TimeStamp is ignored and only the message will get received.
        /// </summary>
        public bool OverrideChannelTag { get; set; }

        /// <summary>
        ///     Put a timestamp infront of the log.
        /// </summary>
        public bool AddTimeStamp { get; set; }

        public Stream PBaseStream => BaseStream;

        #endregion

        #region Overrides

        #region Methods

        public override void Write(byte[] buffer, int offset, int count)
        {
            byte[] tmp = new byte[count];
            Array.Copy(buffer, 0, tmp, 0, count);
            BaseStream.Write(tmp, 0, count);
            Flush();
        }

        public override void Flush()
        {
            BaseStream.Flush();
        }

        public override IAsyncResult BeginRead(
            byte[] buffer, int offset, int count, AsyncCallback callback,
            object state)
        {
            return BaseStream.BeginRead(buffer, offset, count, callback, state);
        }

        public override IAsyncResult BeginWrite(
            byte[] buffer, int offset, int count, AsyncCallback callback,
            object state)
        {
            return BaseStream.BeginWrite(buffer, offset, count, callback, state);
        }

        public override void Close()
        {
            IsClosed = true;
            BaseStream.Close();
        }


        [Obsolete]
        protected override WaitHandle CreateWaitHandle()
        {
            throw new NotSupportedException();
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            return BaseStream.EndRead(asyncResult);
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            BaseStream.EndWrite(asyncResult);
        }

        public override bool Equals(object obj)
        {
            //Needs to be Checking this instead of _baseStream since the
            //Debug.AddOutputStream is using this to determine if the Stream is already in the system
            // ReSharper disable once BaseObjectEqualsIsObjectEquals
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return BaseStream.GetHashCode();
        }

        public override object InitializeLifetimeService()
        {
            return BaseStream.InitializeLifetimeService();
        }


        public override int ReadByte()
        {
            return BaseStream.ReadByte();
        }


        public override string ToString()
        {
            return BaseStream.ToString();
        }

        public override void WriteByte(byte value)
        {
            BaseStream.WriteByte(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return BaseStream.Read(buffer, offset, count);
        }

        #endregion

        #region Properties

        public override bool CanRead => BaseStream.CanRead;

        public override bool CanSeek => BaseStream.CanSeek;

        public override bool CanWrite => BaseStream.CanWrite;

        public override long Length => BaseStream.Length;

        public override long Position
        {
            get => BaseStream.Position;
            set => BaseStream.Position = value;
        }

        public override bool CanTimeout => BaseStream.CanTimeout;

        public override int ReadTimeout
        {
            get => BaseStream.ReadTimeout;
            set => BaseStream.ReadTimeout = value;
        }


        public override int WriteTimeout
        {
            get => BaseStream.WriteTimeout;
            set => BaseStream.WriteTimeout = value;
        }

        #endregion

        public override long Seek(long offset, SeekOrigin origin)
        {
            return BaseStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            BaseStream.SetLength(value);
        }

        #endregion

    }
}