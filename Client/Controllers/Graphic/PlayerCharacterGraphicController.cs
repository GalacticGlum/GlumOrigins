using System.Collections.Generic;
using GlumOrigins.Client.Messaging;
using GlumOrigins.Client.UI;
using GlumOrigins.Common.Game;
using UnityEngine;

namespace GlumOrigins.Client.Controllers.Graphic
{
    public class PlayerCharacterGraphicController 
    {
        public GameObject ClientPlayerGameObject { get; private set; }

        private readonly Transform playerCharactersParent;
        private readonly Dictionary<int, GameObject> playerCharacterGameObjects;
 
        public PlayerCharacterGraphicController()
        {
            WorldController.Instance.World.PlayerCharacterManager.Created += OnPlayerCreated;
            WorldController.Instance.World.PlayerCharacterManager.Removed += OnPlayerRemoved;

            playerCharactersParent = new GameObject("Player Characters").transform;
            playerCharacterGameObjects = new Dictionary<int, GameObject>();
        }

        private void OnPlayerRemoved(object sender, PlayerCharacterEventArgs args)
        {
            if (!playerCharacterGameObjects.ContainsKey(args.PlayerCharacter.Id)) return;

            Object.Destroy(playerCharacterGameObjects[args.PlayerCharacter.Id]);
            playerCharacterGameObjects.Remove(args.PlayerCharacter.Id);
        }

        private void OnPlayerCreated(object sender, PlayerCharacterEventArgs args)
        {
            GameObject playerCharacterGameObject = new GameObject($"PlayerCharacter_{args.PlayerCharacter.Id}_{args.PlayerCharacter.Name}");
            playerCharacterGameObject.transform.position = (Vector2)args.PlayerCharacter.Tile.Position;
            playerCharacterGameObject.transform.SetParent(playerCharactersParent);

            PlayerController.Attach(args.PlayerCharacter.Id, playerCharacterGameObject);

            MessageChannel<PlayerNameDisplayCreateMessage>.Broadcast(new PlayerNameDisplayCreateMessage(playerCharacterGameObject, args.PlayerCharacter));

            SpriteRenderer spriteRenderer = playerCharacterGameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = Resources.Load<Sprite>("Characters/_DefaultPlayer");
            spriteRenderer.sortingLayerName = "Characters";

            if (args.PlayerCharacter.IsClientPlayer())
            {
                ClientPlayerGameObject = playerCharacterGameObject;
                spriteRenderer.color = Color.red;
            }

            playerCharacterGameObjects.Add(args.PlayerCharacter.Id, playerCharacterGameObject);
        }
    }
}
