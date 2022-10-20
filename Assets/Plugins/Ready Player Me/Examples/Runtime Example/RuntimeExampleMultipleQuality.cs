using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ReadyPlayerMe
{
    public class RuntimeExampleMultipleQuality : MonoBehaviour
    {
        [Serializable]
        private struct AvatarConfigData
        {
            public AvatarConfig Config;
            public float PosX;

            // TODO Find a fix for ignoring warning
            // Had to add this constructor because of "Field is never assigned" warning
            public AvatarConfigData(AvatarConfig config, float posX)
            {
                Config = config;
                PosX = posX;
            }
        }

        [SerializeField]
        private string avatarUrl = "https://api.readyplayer.me/v1/avatars/632d65e99b4c6a4352a9b8db.glb";
        [SerializeField]
        private Transform qualityContainerPrefab;
        [SerializeField]
        private AvatarConfigData[] avatarConfigs;

        private List<GameObject> avatarList;

        private void Start()
        {
            ApplicationData.Log();
            avatarList = new List<GameObject>();
            StartCoroutine(LoadAvatars());
        }

        private IEnumerator LoadAvatars()
        {
            var loading = false;

            foreach (var config in avatarConfigs)
            {
                loading = true;
                var loader = new AvatarLoader();
                loader.OnCompleted += (sender, args) =>
                {
                    loading = false;
                    AvatarAnimatorHelper.SetupAnimator(args.Metadata.BodyType, args.Avatar);
                    OnAvatarLoaded(args.Avatar, config);
                };
                loader.AvatarConfig = config.Config;
                loader.LoadAvatar(avatarUrl);

                yield return new WaitUntil(() => !loading);
            }
        }

        private void OnAvatarLoaded(GameObject avatar, AvatarConfigData data)
        {
            if (avatarList != null)
            {
                var quality = data.Config.name.Substring("Avatar Configuration".Length);
                var container = Instantiate(qualityContainerPrefab);
                container.name = quality;
                container.position = new Vector3(data.PosX, 0, 0);
                container.GetComponentInChildren<Text>().text = "<b>" + quality + "</b>\n" +
                                                                "MeshLoad: " + data.Config.MeshLod + "\n" +
                                                                "Texture: " + data.Config.TextureAtlas;
                avatar.name = "Avatar";
                avatar.transform.SetParent(container, false);
                avatarList.Add(container.gameObject);
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
                foreach (var avatar in avatarList) Destroy(avatar);
                avatarList.Clear();
                avatarList = null;
            }
        }
    }
}
