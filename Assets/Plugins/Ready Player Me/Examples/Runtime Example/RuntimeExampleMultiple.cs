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
            "https://d1a370nemizbjq.cloudfront.net/9bcc6840-8b8b-420d-a9d8-bc9c275fce8f.glb",
            "https://d1a370nemizbjq.cloudfront.net/6b0d5152-586e-4b9d-ac00-1a85ad2ef4e4.glb",
            "https://d1a370nemizbjq.cloudfront.net/fabdf402-cd3a-438a-a34b-3e3ca4ab4314.glb",
            "https://d1a370nemizbjq.cloudfront.net/81b6fadb-6c81-4632-ab0c-7eaf15835e40.glb",
            "https://d1a370nemizbjq.cloudfront.net/567a05c0-6ac8-4c78-8cc4-e160a6fe81b2.glb",
            "https://d1a370nemizbjq.cloudfront.net/b3962d36-5dec-4778-a483-185b9303b8d5.glb",
            "https://d1a370nemizbjq.cloudfront.net/188ab5f4-c786-457c-a961-15fa5cd1f0d7.glb",
            "https://d1a370nemizbjq.cloudfront.net/e5eff169-4d3e-4e80-a83d-98bd9949dec0.glb",
            "https://d1a370nemizbjq.cloudfront.net/61c0a063-1c9a-4772-bd8c-eb6622ccc8e2.glb",
            "https://d1a370nemizbjq.cloudfront.net/40e05c6c-3015-47ef-9145-fb593bc69bf1.glb"
        };
        private const int RADIUS = 2;
        private List<GameObject> avatarList;

        private void Start()
        {
            ApplicationData.Log();

            avatarList = new List<GameObject>();
            var urlSet = new HashSet<string>(avatarUrls);

            // LoadAtOnce(urlSet);
            StartCoroutine(LoadOneByOne(urlSet));
        }

        private void LoadAtOnce(HashSet<string> urlSet)
        {
            foreach (var url in urlSet)
            {
                var loader = new AvatarLoader();
                loader.OnCompleted += (sender, args) =>
                {
                    OnAvatarLoaded(args.Avatar);
                };
                loader.LoadAvatar(url);
            }
        }

        private IEnumerator LoadOneByOne(HashSet<string> urlSet)
        {
            var loading = false;

            foreach (var url in urlSet)
            {
                loading = true;
                var loader = new AvatarLoader();
                loader.OnCompleted += (sender, args) =>
                {
                    loading = false;
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
