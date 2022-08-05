using UnityEngine;

namespace ReadyPlayerMe
{
    public class RuntimeExample : MonoBehaviour
    {
        [SerializeField]
        private string avatarUrl = "https://d1a370nemizbjq.cloudfront.net/9bcc6840-8b8b-420d-a9d8-bc9c275fce8f.glb";

        private GameObject avatar;

        private void Start()
        {
            ApplicationData.Log();

            var avatarLoader = new AvatarLoader();
            avatarLoader.OnCompleted += (_, args) => { avatar = args.Avatar; };
            avatarLoader.LoadAvatar(avatarUrl);
        }

        private void OnDestroy()
        {
            if (avatar != null) Destroy(avatar);
        }
    }
}
