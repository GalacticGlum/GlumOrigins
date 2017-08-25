using System.Collections.Generic;
using GlumOrigins.Common.Game;
using UnityEngine;

namespace GlumOrigins.Client.Controllers.Graphic
{
    public class PlayerCharacterGraphicController 
    {
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
            if (!playerCharacterGameObjects.ContainsKey(args.Character.Id)) return;
            playerCharacterGameObjects.Remove(args.Character.Id);
            Object.Destroy(playerCharacterGameObjects[args.Character.Id]);
        }

        private void OnPlayerCreated(object sender, PlayerCharacterEventArgs args)
        {
            GameObject playerCharacterGameObject = new GameObject($"PlayerCharacter_{args.Character.Id}_{args.Character.Name}");
            playerCharacterGameObject.transform.position = (Vector2)args.Character.Tile.Position;
            playerCharacterGameObject.transform.SetParent(playerCharactersParent);

            SpriteRenderer spriteRenderer = playerCharacterGameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = Resources.Load<Sprite>("Characters/_DefaultPlayer");
            spriteRenderer.sortingLayerName = "Characters";

            playerCharacterGameObjects.Add(args.Character.Id, playerCharacterGameObject);
        }
    }
}
