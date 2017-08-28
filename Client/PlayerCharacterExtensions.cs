using GlumOrigins.Common.Game;

namespace GlumOrigins.Client
{
    public static class PlayerCharacterExtensions
    {
        public static bool IsClientPlayer(this PlayerCharacter character) => World.Current.PlayerCharacterManager.IsClientPlayer(character);
    }
}
