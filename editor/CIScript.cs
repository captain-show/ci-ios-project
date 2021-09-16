using System.Collections.Generic;
using UnityEditor;

public static class CIScript
{
    public const string PLIST_FILE = "Info.plist";
    public const string CURRENT_PROJECT_VERSION = "1.0.1";
    public const string APPLE_GENERIC_VALUE = "apple-generic";
    public const string ENABLE_BITCODE = "YES";
    public const string CODE_SIGN_STYLE = "Manual";

    public const string PROVISIONING_PROFILE_KEY = "PROVISIONING_PROFILE";

    private static string BuildPath = "./Builds/Android/";
    private static string BuildPathIOS = "./Builds/iOS/";

    private const string IOSBuildNumber = "4";

    private static string[] FindEnabledEditorScenes()
    {
        List<string> sceneNames = new List<string>();

        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                sceneNames.Add(scene.path);
            }
        }

        return sceneNames.ToArray();
    }

    public static void GenerateBuild()
    {
    #if !UNITY_5_6_OR_NEWER
    			// https://issuetracker.unity3d.com/issues/buildoptions-dot-acceptexternalmodificationstoplayer-causes-unityexception-unknown-project-type-0
    			// Fixed in Unity 5.6.0
    			// side effect to fix android build system:
    	EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Internal;
    #endif
        EditorUserBuildSettings.exportAsGoogleAndroidProject = true;

        BuildPipeline.BuildPlayer(FindEnabledEditorScenes(), BuildPath, BuildTarget.Android, BuildOptions.AcceptExternalModificationsToPlayer);
    }

    public static void GenerateIOSBuild()
    {
        PlayerSettings.iOS.buildNumber = IOSBuildNumber;

        BuildPipeline.BuildPlayer(FindEnabledEditorScenes(), BuildPathIOS, BuildTarget.iOS, BuildOptions.None);
    }
}
