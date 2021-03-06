﻿using System;
using System.Collections.Generic;
using System.Net;
using GlumOrigins.Common.Logging;
using Lidgren.Network;

namespace GlumOrigins.Common.Networking
{
    public delegate void ConnectionEventHandler(object sender, ConnectionEventArgs args);
    public class ConnectionEventArgs : EventArgs
    {
        public NetConnection Connection { get; }
        public NetBuffer Buffer { get; }

        public ConnectionEventArgs(NetConnection connection, NetBuffer buffer)
        {
            Connection = connection;
            Buffer = buffer;
        }
    }

    public delegate void PacketRecievedEventHandler(object sender, PacketRecievedEventArgs args);
    public class PacketRecievedEventArgs : EventArgs
    {
        public NetConnection SenderConnection { get; }
        public NetBuffer Buffer { get; }
        public PacketRecievedEventArgs(NetConnection senderConnection, NetBuffer buffer)
        {
            SenderConnection = senderConnection;
            Buffer = buffer;
        }
    }

    public abstract class GamePeer<T> where T : NetPeer
    {
        protected delegate void IncomingMessageEventHandler(object sender, IncomingMessageEventArgs args);
        protected class IncomingMessageEventArgs : EventArgs
        {
            public NetIncomingMessage Message { get; }
            public IncomingMessageEventArgs(NetIncomingMessage message)
            {
                Message = message;
            }
        }

        public event ConnectionEventHandler PeerConnected;
        protected void OnConnected(ConnectionEventArgs args)
        {
            PeerConnected?.Invoke(this, args);
        }

        public event ConnectionEventHandler PeerDisconnected;
        private void OnDisconnected(ConnectionEventArgs args)
        {
            PeerDisconnected?.Invoke(this, args);
        }

        public abstract NetPeerConfiguration NetConfiguration { get; }
        public PacketCollection Packets { get; }

        public IPAddress LocalIpAddress => NetPeer.Configuration.LocalAddress;
        public int? Port => NetPeer.Configuration.Port;

        protected T NetPeer { get; private set; }

        private readonly Dictionary<NetIncomingMessageType, IncomingMessageEventHandler> incomingMessageHandlers;

        protected GamePeer()
        {
            incomingMessageHandlers = new Dictionary<NetIncomingMessageType, IncomingMessageEventHandler>();
            Packets = new PacketCollection();
        }

        public void Listen()
        {
            Validate();
            NetIncomingMessage message = NetPeer.ReadMessage();
            if (message == null) return;
            switch (message.MessageType)
            {
                case NetIncomingMessageType.Data:
                    int packetHeader = message.ReadInt32();
                    if (Packets.Contains(packetHeader))
                    {
                        Packets[packetHeader]?.Invoke(this, new PacketRecievedEventArgs(message.SenderConnection, message));
                    }

                    break;
                case NetIncomingMessageType.StatusChanged:
                    switch (message.SenderConnection.Status)
                    {
                        case NetConnectionStatus.Connected:
                            OnConnected(new ConnectionEventArgs(message.SenderConnection, message));
                            break;
                        case NetConnectionStatus.Disconnected:
                            OnDisconnected(new ConnectionEventArgs(message.SenderConnection, message));
                            break;
                    }
                    break;
                default:
                    if (incomingMessageHandlers.ContainsKey(message.MessageType))
                    {
                        incomingMessageHandlers[message.MessageType](this, new IncomingMessageEventArgs(message));
                    }
                    break;
            }

            NetPeer.Recycle(message);
        }

        public Packet CreatePacket(ClientOutgoingPacketType packetType) => CreatePacket((int)packetType);
        public Packet CreatePacket(ServerOutgoingPacketType packetType) => CreatePacket((int)packetType);
        public Packet CreatePacket(int packetType) => new Packet(CreateMessageWithHeader(packetType));
        protected NetOutgoingMessage GetOutgoingMessageFromPacket(Packet packet) => packet.OutgoingMessage;

        protected NetOutgoingMessage CreateMessageWithHeader(int packetType)
        {
            NetOutgoingMessage message = NetPeer.CreateMessage();
            message.Write(packetType);

            return message;
        }


        protected abstract T ConstructPeer();
        protected void HandleMessageType(NetIncomingMessageType type, IncomingMessageEventHandler handler)
        {
            // NetIncomingMessageType.Data is reserved
            if (type == NetIncomingMessageType.Data)
            {
                Logger.Log("GamePeer::HandleMessageType: \"NetIncomingMessageType.Data\" is a reserved message type!");
                return;
            }

            if (!incomingMessageHandlers.ContainsKey(type))
            {
                incomingMessageHandlers[type] = null;
            }

            incomingMessageHandlers[type] += handler;
        }

        /// <summary>
        /// Makes sure that the NetPeer is constructed.
        /// </summary>
        protected void Validate()
        {
            if (NetPeer != null) return;
            NetPeer = ConstructPeer();
        }
    }
}
