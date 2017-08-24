using System.Net;
using Lidgren.Network;

namespace GlumOrigins.Common.Networking
{
    public sealed class Packet
    {
        internal NetOutgoingMessage OutgoingMessage { get; }
        internal Packet(NetOutgoingMessage ownerOutgoingMessage) => OutgoingMessage = ownerOutgoingMessage;

        public void Write(byte value) => OutgoingMessage?.Write(value);
        public void Write(sbyte value) => OutgoingMessage?.Write(value);

        public void Write(ushort value) => OutgoingMessage?.Write(value);
        public void Write(short value) => OutgoingMessage?.Write(value);
        public void Write(int value) => OutgoingMessage?.Write(value);

        public void Write(uint value) => OutgoingMessage?.Write(value);
        public void Write(ulong value) => OutgoingMessage?.Write(value);
        public void Write(long value) => OutgoingMessage?.Write(value);

        public void Write(float value) => OutgoingMessage?.Write(value);
        public void Write(double value) => OutgoingMessage?.Write(value);

        public void Write(bool value) => OutgoingMessage?.Write(value);
        public void Write(string value) => OutgoingMessage?.Write(value);

        public void Write(IPEndPoint endPoint) => OutgoingMessage.Write(endPoint);
        public void Write(NetBuffer buffer) => OutgoingMessage.Write(buffer);

        public void WriteTime(bool highPrecision = false) => OutgoingMessage.WriteTime(highPrecision);
        public void WriteTime(double time, bool highPrecision = false) => OutgoingMessage.WriteTime(time, highPrecision);

        public byte ReadByte() => OutgoingMessage.ReadByte();
        public sbyte ReadSByte() => OutgoingMessage.ReadSByte();

        public short ReadShort() => OutgoingMessage.ReadInt16();
        public ushort ReadUShort() => OutgoingMessage.ReadUInt16();
        public int ReadInt() => OutgoingMessage.ReadInt32();
        public uint ReadUInt() => OutgoingMessage.ReadUInt32();
        public long ReadLong() => OutgoingMessage.ReadInt64();
        public ulong ReadULong() => OutgoingMessage.ReadUInt64();

        public float ReadFloat() => OutgoingMessage.ReadFloat();
        public double ReadDouble() => OutgoingMessage.ReadDouble();
        public float ReadSingle() => OutgoingMessage.ReadSingle();

        public bool ReadBoolean() => OutgoingMessage.ReadBoolean();
        public string ReadString() => OutgoingMessage.ReadString();

        public IPEndPoint ReadIPEndPoint() => OutgoingMessage.ReadIPEndPoint();
        public double ReadTime(NetConnection connection, bool highPrecision = false) => OutgoingMessage.ReadTime(connection, highPrecision);
    }
}
