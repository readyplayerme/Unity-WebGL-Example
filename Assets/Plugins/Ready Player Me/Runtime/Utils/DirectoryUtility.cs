using System.IO;
using UnityEngine;

namespace ReadyPlayerMe
{
    public static class DirectoryUtility
    {
        /// The directory where avatar files will be downloaded.
        public static string DefaultAvatarFolder { get; set; } = "Avatars";

        public static void ValidateAvatarSaveDirectory(string guid, bool saveInProjectFolder = false)
        {
            var path = GetAvatarSaveDirectory(guid, saveInProjectFolder);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static string GetAvatarSaveDirectory(string guid, bool saveInProjectFolder = false)
        {
            var directory = saveInProjectFolder ? Application.dataPath : Application.persistentDataPath;
            return $"{directory}/{DefaultAvatarFolder}/{guid}";
        }

        public static string GetRelativeProjectPath(string guid) => $"Assets/{DefaultAvatarFolder}/{guid}";
    }
}
