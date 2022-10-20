using System.IO;
using System.Linq;
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

        public static string GetAvatarSaveDirectory(string guid, bool saveInProjectFolder = false, string paramsHash = null)
        {
            return saveInProjectFolder ? $"{GetAvatarsDirectoryPath(true)}/{guid}" : $"{GetAvatarsDirectoryPath()}/{guid}/{paramsHash}";
        }

        public static string GetRelativeProjectPath(string guid) => $"Assets/{DefaultAvatarFolder}/{guid}";

        public static long GetDirectorySize(DirectoryInfo directoryInfo)
        {
            // Add file sizes.
            var fileInfos = directoryInfo.GetFiles();
            var size = fileInfos.Sum(fi => fi.Length);

            // Add subdirectory sizes.
            var directoryInfos = directoryInfo.GetDirectories();
            size += directoryInfos.Sum(GetDirectorySize);
            return size;
        }

        public static string GetAvatarsDirectoryPath(bool saveInProjectFolder = false)
        {
            var directory = saveInProjectFolder ? Application.dataPath : Application.persistentDataPath;
            return $"{directory}/{DefaultAvatarFolder}";
        }
    }
}
