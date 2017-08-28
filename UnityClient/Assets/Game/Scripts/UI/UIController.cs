using GlumOrigins.Client.Messaging;
using UnityEngine;

namespace GlumOrigins.Client.UI
{
    public class UIController : MonoBehaviour
    {
        private void Start()
        {
            MessageChannel<PlayerNameDisplayCreateMessage>.Subscribe(PlayerNameDisplay.Create);
        }
    }
}
