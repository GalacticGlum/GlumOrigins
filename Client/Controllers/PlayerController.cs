using System.Collections.Generic;
using GlumOrigins.Client.Controllers;
using GlumOrigins.Common.Game;
using GlumOrigins.Common.Networking;
using Lidgren.Network;
using UnityEngine;
using UnityUtilities.Math;
using Logger = GlumOrigins.Common.Logging.Logger;

namespace GlumOrigins.Client
{
    public class PlayerController : MonoBehaviour
    {
        private static Dictionary<int, PlayerController> playerControllers;

        private const float MovementDuration = 1;
        private LerpInformation<Vector3> movementLerpInformation;
        private int playerId;

        public static void Initialize()
        {
            playerControllers = new Dictionary<int, PlayerController>();    
            NetworkController.Instance.Client.Packets[ServerOutgoingPacketType.UpdatePlayerPositions] += HandlePositionalUpdate;
        }

        private void Start()
        {
            playerControllers.Add(playerId, this);
        }

        private void Update()
        {
            if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && playerId == World.Current.PlayerCharacterManager.ControllableCharacterId)
            {
                SendMovement(MovementDirection.North);       
            }

            HandleMovement();
        }

        private void SendMovement(MovementDirection direction)
        {
            Vector2i playerPosition = (Vector2)transform.position;
            Tile destination = new Tile(playerPosition);

            switch (direction)
            {
                case MovementDirection.North:
                    destination = World.Current.GetTileAt(playerPosition.X, playerPosition.Y + 1);
                    break;
                case MovementDirection.South:
                    destination = World.Current.GetTileAt(playerPosition.X, playerPosition.Y - 1);
                    break;
                case MovementDirection.East:
                    destination = World.Current.GetTileAt(playerPosition.X + 1, playerPosition.Y);
                    break;
                case MovementDirection.West:
                    destination = World.Current.GetTileAt(playerPosition.X - 1, playerPosition.Y);
                    break;
            }
            
            // TODO: "Predict" validation.
            if (destination.Position == playerPosition) return;

            World.Current.PlayerCharacterManager.ClientPlayerCharacter.Tile = destination;
            Packet packet = NetworkController.Instance.Client.CreatePacket(ClientOutgoingPacketType.SendMovement);
            packet.Write(World.Current.PlayerCharacterManager.ControllableCharacterId);
            packet.Write(destination);
            NetworkController.Instance.Client.Send(packet, NetDeliveryMethod.ReliableUnordered);

            Move((Vector2)destination.Position);
        }

        private void Move(Vector3 target)
        {
            movementLerpInformation = new LerpInformation<Vector3>(transform.position, target, MovementDuration, Vector3.Lerp);
            movementLerpInformation.Finished += (sender, args) => movementLerpInformation = null;
        }

        private void HandleMovement()
        {
            if (movementLerpInformation == null) return;
            transform.position = movementLerpInformation.Step(Time.deltaTime);
        }

        public static void Attach(int playerId, GameObject gameObject)
        {
            PlayerController controller = gameObject.AddComponent<PlayerController>();
            controller.playerId = playerId;
        }

        private static void HandlePositionalUpdate(object sender, PacketRecievedEventArgs args)
        {
            int id = args.Buffer.ReadInt32();
            if (!playerControllers.ContainsKey(id)) return;

            Tile tile = args.Buffer.Read<Tile>();
            Logger.Log(tile.Position);

            World.Current.PlayerCharacterManager[id].Tile = tile;
            playerControllers[id].Move((Vector2)tile.Position);
        }
    }
}
