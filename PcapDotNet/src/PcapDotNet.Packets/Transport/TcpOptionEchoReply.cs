using System;

namespace PcapDotNet.Packets.Transport
{
    /// <summary>
    /// TCP Echo Reply Option:
    /// +--------+--------+--------+--------+--------+--------+
    /// | Kind=7 | Length |    4 bytes of echoed info         |
    /// +--------+--------+--------+--------+--------+--------+
    /// 
    /// A TCP that receives a TCP Echo option containing four information bytes will return these same bytes in a TCP Echo Reply option.
    /// 
    /// This TCP Echo Reply option must be returned in the next segment (e.g., an ACK segment) that is sent.
    /// If more than one Echo option is received before a reply segment is sent, the TCP must choose only one of the options to echo, 
    /// ignoring the others; specifically, it must choose the newest segment with the oldest sequence number.
    /// 
    /// To use the TCP Echo and Echo Reply options, a TCP must send a TCP Echo option in its own SYN segment 
    /// and receive a TCP Echo option in a SYN segment from the other TCP.  
    /// A TCP that does not implement the TCP Echo or Echo Reply options must simply ignore any TCP Echo options it receives.  
    /// However, a TCP should not receive one of these options in a non-SYN segment unless it included a TCP Echo option in its own SYN segment.
    /// </summary>
    [OptionTypeRegistration(typeof(TcpOptionType), TcpOptionType.EchoReply)]
    public class TcpOptionEchoReply : TcpOptionComplex, IOptionComplexFactory, IEquatable<TcpOptionEchoReply>
    {
        /// <summary>
        /// The number of bytes this option take.
        /// </summary>
        public const int OptionLength = 6;

        public const int OptionValueLength = OptionLength - OptionHeaderLength;

        public TcpOptionEchoReply(uint info)
            : base(TcpOptionType.EchoReply)
        {
            Info = info;
        }

        public TcpOptionEchoReply()
            : this(0)
        {
        }

        public uint Info { get; private set; }

        /// <summary>
        /// The number of bytes this option will take.
        /// </summary>
        public override int Length
        {
            get { return OptionLength; }
        }

        /// <summary>
        /// True iff this option may appear at most once in a datagram.
        /// </summary>
        public override bool IsAppearsAtMostOnce
        {
            get { return true; }
        }

        public bool Equals(TcpOptionEchoReply other)
        {
            if (other == null)
                return false;

            return Info == other.Info;
        }

        public override bool Equals(TcpOption other)
        {
            return Equals(other as TcpOptionEchoReply);
        }

        /// <summary>
        /// Tries to read the option from a buffer starting from the option value (after the type and length).
        /// </summary>
        /// <param name="buffer">The buffer to read the option from.</param>
        /// <param name="offset">The offset to the first byte to read the buffer. Will be incremented by the number of bytes read.</param>
        /// <param name="valueLength">The number of bytes the option value should take according to the length field that was already read.</param>
        /// <returns>On success - the complex option read. On failure - null.</returns>
        public Option CreateInstance(byte[] buffer, ref int offset, byte valueLength)
        {
            if (valueLength != OptionValueLength)
                return null;

            uint info = buffer.ReadUInt(ref offset, Endianity.Big);
            return new TcpOptionEchoReply(info);
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, Info, Endianity.Big);
        }
    }
}