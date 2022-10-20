using System.Collections.Generic;
using UnityEngine;

namespace ReadyPlayerMe
{
    [CreateAssetMenu(fileName = "Avatar Configuration", menuName = "Scriptable Objects/Ready Player Me/Avatar Configuration", order = 2)]
    [HelpURL("https://docs.readyplayer.me/ready-player-me/integration-guides/unity-sdk/how-to-use#load-avatars-at-runtime")]
    public class AvatarConfig : ScriptableObject
    {
        public MeshLod MeshLod;
        public Pose Pose;
        public TextureAtlas TextureAtlas;
        [Range(256, 1024)]
        public int TextureSizeLimit = 1024;
        public bool UseHands;
        [HideInInspector]
        public List<string> MorphTargets = new List<string>();
    }
}
