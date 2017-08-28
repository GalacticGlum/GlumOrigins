using GlumOrigins.Client.Messaging;
using GlumOrigins.Common.Game;
using UnityEngine;

namespace GlumOrigins.Client.UI
{
    public class PlayerNameDisplayCreateMessage : IMessage
    {
        public GameObject PlayerGameObject { get; }
        public PlayerCharacter PlayerCharacter { get; }

        public PlayerNameDisplayCreateMessage(GameObject playerGameObject, PlayerCharacter playerCharacter)
        {
            PlayerGameObject = playerGameObject;
            PlayerCharacter = playerCharacter;
        }
    }
}
