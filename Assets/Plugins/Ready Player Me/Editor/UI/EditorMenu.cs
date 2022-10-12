using UnityEditor;

namespace ReadyPlayerMe
{
    public static class EditorMenu
    {
        [MenuItem("Ready Player Me/Avatar Loader", priority = 0)]
        public static void OpenAvatarLoaderWindow()
        {
            AvatarLoaderEditorWindow.ShowWindowMenu();
        }

        [MenuItem("Ready Player Me/Settings", priority = 1)]
        public static void OpenSettingsWindow()
        {
            SettingsEditorWindow.ShowWindowMenu();
        }
    }
}
