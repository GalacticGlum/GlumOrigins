using GlumOrigins.Common.Game;
using TMPro;
using UnityEngine;

namespace GlumOrigins.Client.UI
{
    [RequireComponent(typeof(TextMeshPro))]
    public class PlayerNameDisplay : MonoBehaviour
    {
        private PlayerCharacter playerCharacter;
        private TextMeshPro textComponent;

        private void Initialize()
        {
            textComponent = GetComponent<TextMeshPro>();
            textComponent.text = playerCharacter.Name;
        }

        public static void Create(PlayerNameDisplayCreateMessage message)
        {
            GameObject displayGameObject = Instantiate(Resources.Load<GameObject>("Prefabs/UI/PlayerNameDisplay"), message.PlayerGameObject.transform, false);
            PlayerNameDisplay dispaly = displayGameObject.AddComponent<PlayerNameDisplay>();

            dispaly.playerCharacter = message.PlayerCharacter;
            dispaly.Initialize();
        }
    }
}
