using System.Collections.Generic;

namespace ReadyPlayerMe
{
    public static class AvatarConfigMap
    {
        public static readonly Dictionary<Pose, string> Pose = new Dictionary<Pose, string>
        {
            { ReadyPlayerMe.Pose.APose, "A" },
            { ReadyPlayerMe.Pose.TPose, "T" }
        };

        public static readonly Dictionary<TextureAtlas, string> TextureAtlas = new Dictionary<TextureAtlas, string>
        {
            { ReadyPlayerMe.TextureAtlas.None, "none" },
            { ReadyPlayerMe.TextureAtlas.High, "1024" },
            { ReadyPlayerMe.TextureAtlas.Medium, "512" },
            { ReadyPlayerMe.TextureAtlas.Low, "256" }
        };
    }
}
