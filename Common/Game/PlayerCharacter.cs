using System;

namespace GlumOrigins.Common.Game
{
    public delegate void PlayerCharacterEventHandler(object sender, PlayerCharacterEventArgs args);
    public class PlayerCharacterEventArgs : EventArgs
    {
        public PlayerCharacter Character { get; }
        public PlayerCharacterEventArgs(PlayerCharacter character)
        {
            Character = character;
        }
    }

    public class PlayerCharacter 
    {
        public int Id { get; }
        public string Name { get; }
        public Tile Tile { get; set; }

        public PlayerCharacter(int id, string name, Tile tile)
        {
            Id = id;
            Name = name;
            Tile = tile;
        }
    }
}
