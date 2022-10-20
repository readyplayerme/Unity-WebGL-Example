using System.Collections.Generic;

namespace ReadyPlayerMe
{
    public static class AvatarConfigProcessor
    {
        private const string TAG = nameof(AvatarConfigProcessor);

        public static string ProcessAvatarConfiguration(AvatarConfig avatarConfig)
        {
            SDKLogger.Log(TAG, "Processing Avatar Configuration");

            return $"?pose={AvatarConfigMap.Pose[avatarConfig.Pose]}" +
                   $"&meshLod={(int) avatarConfig.MeshLod}" +
                   $"&textureAtlas={AvatarConfigMap.TextureAtlas[avatarConfig.TextureAtlas]}" +
                   $"&textureSizeLimit={ProcessTextureSizeLimit(avatarConfig.TextureSizeLimit)}" +
                   $"{ProcessMorphTargets(avatarConfig.MorphTargets)}" +
                   $"&useHands={(avatarConfig.UseHands ? "true" : "false")}";
        }

        private static int ProcessTextureSizeLimit(int textureSize)
        {
            return (textureSize % 2 == 0) ? textureSize : textureSize + 1;
        }

        private static string ProcessMorphTargets(List<string> targets)
        {
            if (targets.Count == 0)
            {
                return string.Empty;
            }
            return $"&morphTargets={string.Join(",", targets)}";
        }
    }
}
