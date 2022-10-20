using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReadyPlayerMe
{
    public class RuntimeExampleMultiple : MonoBehaviour
    {
        [SerializeField]
        private string[] avatarUrls =
        {
            "https://api.readyplayer.me/v1/avatars/632d65e99b4c6a4352a9b8db.glb",
            "https://api.readyplayer.me/v1/avatars/632d678974be0f698c0cf4cc.glb",
            "https://api.readyplayer.me/v1/avatars/632d68079b4c6a4352a9bb29.glb",
            "https://api.readyplayer.me/v1/avatars/632d68559b4c6a4352a9bb75.glb"
        };
        private const int RADIUS = 1;
        private List<GameObject> avatarList;

        private void Start()
        {
            ApplicationData.Log();

            avatarList = new List<GameObject>();
            var urlSet = new HashSet<string>(avatarUrls);

            StartCoroutine(LoadAvatars(urlSet));
        }

        private IEnumerator LoadAvatars(HashSet<string> urlSet)
        {
            var loading = false;

            foreach (var url in urlSet)
            {
                loading = true;
                var loader = new AvatarLoader();
                loader.OnCompleted += (sender, args) =>
                {
                    loading = false;
                    AvatarAnimatorHelper.SetupAnimator(args.Metadata.BodyType, args.Avatar);
                    OnAvatarLoaded(args.Avatar);
                };
                loader.LoadAvatar(url);

                yield return new WaitUntil(() => !loading);
            }
        }

        private void OnAvatarLoaded(GameObject avatar)
        {
            if (avatarList != null)
            {
                avatarList.Add(avatar);
                avatar.transform.position = Quaternion.Euler(90, 0, 0) * Random.insideUnitCircle * RADIUS;
            }
            else
            {
                Destroy(avatar);
            }
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
            if (avatarList != null)
            {
                foreach (GameObject avatar in avatarList) Destroy(avatar);
                avatarList.Clear();
                avatarList = null;
            }
        }
    }
}
