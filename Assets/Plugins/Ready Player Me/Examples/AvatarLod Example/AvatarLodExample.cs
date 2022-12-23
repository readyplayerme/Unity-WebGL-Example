using System.Collections.Generic;
using System.Threading.Tasks;
using ReadyPlayerMe;
using UnityEngine;

public class AvatarLodExample : MonoBehaviour
{
    [SerializeField] private AvatarLodExampleUI lodExampleUI;
    [SerializeField] private string avatarUrl;
    [SerializeField] private AvatarConfig[] lodConfigs;

    private LODGroup lodGroup;
    private bool loading;
    private GameObject mainAvatar;
    private SkinnedMeshRenderer mainMeshRenderer;
    private readonly List<SkinnedMeshRenderer> meshRenderersList = new List<SkinnedMeshRenderer>();

    private void Start()
    {
        ApplicationData.Log();
        if (lodExampleUI) lodExampleUI.Init();
        LoadLodAvatar();
    }

    private async void LoadLodAvatar()
    {
        var bodyType = BodyType.None;

        foreach (var config in lodConfigs)
        {
            var lodLevel = (int) config.MeshLod;

            AvatarLoader loader = new AvatarLoader();
            loader.AvatarConfig = config;
            loader.LoadAvatar(avatarUrl);
            loader.OnCompleted += (sender, args) =>
            {
                if (mainAvatar == null)
                {
                    bodyType = args.Metadata.BodyType;
                    mainAvatar = Instantiate(args.Avatar);
                    mainAvatar.name = args.Avatar.name + "_LOD";

                    var meshTransform = mainAvatar.transform.GetChild(0);
                    meshTransform.name += $"_LOD{lodLevel}";

                    mainMeshRenderer = meshTransform.GetComponent<SkinnedMeshRenderer>();

                    mainMeshRenderer.enabled = false;
                    meshRenderersList.Add(mainMeshRenderer);
                }
                else
                {
                    var lodSkinnedMeshRenderer = args.Avatar.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>();

                    lodSkinnedMeshRenderer.rootBone = mainMeshRenderer.rootBone;
                    lodSkinnedMeshRenderer.bones = mainMeshRenderer.bones;
                    lodSkinnedMeshRenderer.transform.name += $"_LOD{lodLevel}";
                    lodSkinnedMeshRenderer.transform.SetParent(mainAvatar.transform);
                    lodSkinnedMeshRenderer.transform.SetSiblingIndex(lodLevel);
                    lodSkinnedMeshRenderer.enabled = false;
                    meshRenderersList.Add(lodSkinnedMeshRenderer);
                }

                Destroy(args.Avatar);

                loading = false;
            };
            loading = true;

            while (loading)
            {
                await Task.Yield();
            }
        }

        AddLodGroup();
        AvatarAnimatorHelper.SetupAnimator(bodyType, mainAvatar);
        if (lodExampleUI) lodExampleUI.Show();
    }

    private void AddLodGroup()
    {
        lodGroup = mainAvatar.AddComponent<LODGroup>();
        var lods = new LOD[lodConfigs.Length];
        for (var i = 0; i < lodConfigs.Length; i++)
        {
            meshRenderersList[i].enabled = true;
            lods[i] = new LOD(1.05f - ((i + 1f) / lodConfigs.Length), new Renderer[] { meshRenderersList[i] });
        }
        lodGroup.SetLODs(lods);
        lodGroup.RecalculateBounds();

        lodExampleUI.LodGroup = lodGroup;
    }

    private void OnDestroy()
    {
        if (mainAvatar != null)
        {
            Destroy(mainAvatar);
        }
    }
}
