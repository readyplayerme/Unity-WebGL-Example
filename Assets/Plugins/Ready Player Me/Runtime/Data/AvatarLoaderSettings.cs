using ReadyPlayerMe;
using UnityEngine;

[CreateAssetMenu(fileName = "Avatar Loader Settings", menuName = "Scriptable Objects/Ready Player Me/Avatar Loader Settings", order = 1)]
public class AvatarLoaderSettings : ScriptableObject
{
    public const string RESOURCE_PATH = "Data/Avatar Loader Settings";
    public bool AvatarCachingEnabled;
    public AvatarConfig AvatarConfig;
}
