#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class PartnerPostProcessBuild : IPreprocessBuildWithReport
{
    #region Constants
    private const string PARTNER_SO_PATH = "Partner";

    private const string BUILD_ERROR = "Build Error";
    private const string SUBDOMAIN_EMPTY_ERROR =
        "Partner subdomain can not be empty. Go to 'Ready Player Me > WebView Partner Editor' menu and the fill the input field with your partner subdomain or 'demo' keyword.";
    private const string USE_DEMO_SUBDOMAIN = "Use 'demo' subdomain";
    private const string CANCEL_BUILD = "Cancel Build";
    private const string DEMO_SUBDOMAIN = "demo";
    private const string SUBDOMAIN_NOT_SET = "Build cancelled: Partner subdomain is not set.";
    private const string BUILD_WARNING = "Build Warning";
    private const string SUBDOMAIN_WARNING =
        "You are using 'demo' subdomain. If you already have a partner subdomain please go to 'Ready Player Me > WebView Partner Editor' menu and the fill the input field with your partner subdomain";
    private const string USING_DEMO_SUBDOMAIN = "Build cancelled: Using demo subdomain.";
    private const string CONTINUE_WITH_DEMO = "Continue with demo";
    #endregion

    public int callbackOrder => 0;
    
    public void OnPreprocessBuild(BuildReport report)
    {
        PartnerSO partnerSO = Resources.Load<PartnerSO>(PARTNER_SO_PATH);
        string url = partnerSO.GetSubdomain();

        if (string.IsNullOrEmpty(url))
        {
            bool result = true;
            
            if (!Application.isBatchMode)
            {
                result = EditorUtility.DisplayDialog(BUILD_ERROR, SUBDOMAIN_EMPTY_ERROR, USE_DEMO_SUBDOMAIN, CANCEL_BUILD);
            }

            if (result)
            {
                partnerSO.Subdomain = DEMO_SUBDOMAIN;
                EditorUtility.SetDirty(partnerSO);
                AssetDatabase.SaveAssets();
            }
            else
            {
                throw new BuildFailedException(SUBDOMAIN_NOT_SET);
            }
        }
        else if(url == DEMO_SUBDOMAIN)
        {
            if (!Application.isBatchMode)
            {
                bool result = EditorUtility.DisplayDialog(BUILD_WARNING,
                    SUBDOMAIN_WARNING,
                    CONTINUE_WITH_DEMO, CANCEL_BUILD);

                if (!result)
                {
                    throw new BuildFailedException(USING_DEMO_SUBDOMAIN);
                }
            }
        }
    }
}
#endif
