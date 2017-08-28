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

        private const float MovementDuration = 0.25f;
        private LerpInformation<Vector3> movementLerpInformation;
        private int playerId;
        private bool canMove;

        public static void Initialize()
        {
            playerControllers = new Dictionary<int, PlayerController>();    
            NetworkController.Instance.Client.Packets[ServerOutgoingPacketType.UpdatePlayerPositions] += HandlePositionalUpdate;
        }

        private void Start()
        {
            playerControllers.Add(playerId, this);
            canMove = true;
        }

        private void Update()
        {
            HandleInput();
            HandleMovement();
        }

        private void HandleInput()
        {
            if (playerId != World.Current.PlayerCharacterManager.ControllableCharacterId || canMove == false) return;
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                SendMovement(MovementDirection.North);
            }

            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                SendMovement(MovementDirection.South);
            }

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                SendMovement(MovementDirection.East);
            }

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                SendMovement(MovementDirection.West);
            }
        }

        private void SendMovement(MovementDirection direction)
        {
            Vector2i playerPosition = (Vector2)transform.position;
            Tile destination = null;

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
            if (destination == null) return;

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
            canMove = false;

            movementLerpInformation.Finished += (sender, args) =>
            {
                movementLerpInformation = null;
                canMove = true;
            };
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

            World.Current.PlayerCharacterManager[id].Tile = tile;
            playerControllers[id].Move((Vector2)tile.Position);
        }
    }
}
