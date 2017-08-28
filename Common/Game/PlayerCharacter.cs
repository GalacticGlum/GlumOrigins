using System;
using GlumOrigins.Common.Networking;
using Lidgren.Network;

namespace GlumOrigins.Common.Game
{
    public delegate void PlayerCharacterEventHandler(object sender, PlayerCharacterEventArgs args);
    public delegate void PlayerCharacterMovedEventHandler(object sender, PlayerCharacterMovedEventArgs args);

    public class PlayerCharacterEventArgs : EventArgs
    {
        public PlayerCharacter PlayerCharacter { get; }
        public PlayerCharacterEventArgs(PlayerCharacter playerCharacter)
        {
            PlayerCharacter = playerCharacter;
        }
    }

    public class PlayerCharacterMovedEventArgs : EventArgs
    {
        public PlayerCharacter PlayerCharacter { get; }
        public Tile NewTile { get; }
        public Tile OldTile { get; }

        public PlayerCharacterMovedEventArgs(PlayerCharacter playerCharacter, Tile newTile, Tile oldTile)
        {
            PlayerCharacter = playerCharacter;
            NewTile = newTile;
            OldTile = oldTile;
        }
    }

    public class PlayerCharacter : INetworked<PlayerCharacter>
    {
        public int Id { get; private set; }
        public string Name { get; private set; }

        private Tile tile;
        public Tile Tile
        {
            get => tile;
            set
            {
                Tile oldTile = tile;
                tile = value;

                if(tile == oldTile) return;
                OnMoved(oldTile);
            }
        }

        public event PlayerCharacterMovedEventHandler Moved;
        private void OnMoved(Tile oldTile) => Moved?.Invoke(this, new PlayerCharacterMovedEventArgs(this, Tile, oldTile));

        public PlayerCharacter()
        {
            Id = 0;
            Name = string.Empty;
            Tile = default(Tile);
        }

        public PlayerCharacter(int id, string name, Tile tile)
        {
            Id = id;
            Name = name;
            Tile = tile;
        }

        public void Serialize(NetBuffer packet)
        {
            packet.Write(Id);
            packet.Write(Name);
            packet.Write(Tile);
        }

        public void Deserialize(NetBuffer packet)
        {
            Id = packet.ReadInt32();
            Name = packet.ReadString();
            Tile = packet.Read<Tile>();
        }
    }
}
