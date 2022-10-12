using UnityEngine;

namespace ReadyPlayerMe
{
    public class RuntimeExample : MonoBehaviour
    {
        [SerializeField]
        private string avatarUrl = "https://api.readyplayer.me/v1/avatars/632d65e99b4c6a4352a9b8db.glb";

        private GameObject avatar;

        private void Start()
        {
            ApplicationData.Log();
            var avatarLoader = new AvatarLoader();
            avatarLoader.OnCompleted += (_, args) =>
            {
                avatar = args.Avatar;
                AvatarAnimatorHelper.SetupAnimator(args.Metadata.BodyType, avatar);
            };
            avatarLoader.LoadAvatar(avatarUrl);
        }

        private void OnDestroy()
        {
            if (avatar != null) Destroy(avatar);
        }
    }
}
