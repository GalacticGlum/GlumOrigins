using System.Collections.Generic;
using GlumOrigins.Common.Game;
using UnityEngine;

namespace GlumOrigins.Client.Controllers.Graphic
{
    public class TileGraphicController 
    {
        private readonly Transform tileParent;
        private readonly Dictionary<Tile, GameObject> tileGameObjects;

        public TileGraphicController()
        {
            WorldController.Instance.World.TileCreated += OnTileCreated;
            tileParent = new GameObject("Tiles").transform;
            tileGameObjects = new Dictionary<Tile, GameObject>();
        }

        private void OnTileCreated(object sender, TileEventArgs args)
        {
            GameObject tileGameObject = new GameObject($"Tile_{args.Tile.Position}");
            tileGameObject.transform.position = (Vector2)args.Tile.Position;
            tileGameObject.transform.SetParent(tileParent);

            SpriteRenderer spriteRenderer = tileGameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = Resources.Load<Sprite>("Tiles/_Default");
            spriteRenderer.sortingLayerName = "Tiles";

            tileGameObjects.Add(args.Tile, tileGameObject);
        }
    }
}
