using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

public static class CIScript
{
    private class Platform
    {
        public PlatformInfo Android { get; set; }
        public PlatformInfo Ios { get; set; }

        public class PlatformInfo
        {
            public string Version { get; set; }
            public int BundleNumber { get; set; }
        }
    }

    public const string PLIST_FILE = "Info.plist";
    public const string CURRENT_PROJECT_VERSION = "1.0.1";
    public const string APPLE_GENERIC_VALUE = "apple-generic";
    public const string ENABLE_BITCODE = "YES";
    public const string CODE_SIGN_STYLE = "Manual";

    public const string PROVISIONING_PROFILE_KEY = "PROVISIONING_PROFILE";

    private const string BuildPath = "./Builds/Android/";
    private const string BuildPathIOS = "./Builds/iOS/";

    private const string projectJsonFilePath = @".ci/project-info.json";

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

        SetProjectVertsion(BuildTarget.Android);

        BuildPipeline.BuildPlayer(FindEnabledEditorScenes(), BuildPath, BuildTarget.Android, BuildOptions.AcceptExternalModificationsToPlayer);
    }

    public static void GenerateIOSBuild()
    {
        SetProjectVertsion(BuildTarget.iOS);

        BuildPipeline.BuildPlayer(FindEnabledEditorScenes(), BuildPathIOS, BuildTarget.iOS, BuildOptions.None);
    }

    public static void GenerateIOSTestBuild()
    {
        SetProjectVertsion(BuildTarget.iOS);

        PlayerSettings.iOS.sdkVersion = iOSSdkVersion.SimulatorSDK;

        BuildPipeline.BuildPlayer(FindEnabledEditorScenes(), BuildPathIOS, BuildTarget.iOS, BuildOptions.None);
    }

    private static void SetProjectVertsion(BuildTarget buildTarget)
    {
        Platform platform = JsonConvert.DeserializeObject<Platform>(File.ReadAllText(projectJsonFilePath));

        switch (buildTarget)
        {
            case BuildTarget.Android:
                PlayerSettings.Android.bundleVersionCode = platform.Android.BundleNumber;
                PlayerSettings.bundleVersion = platform.Android.Version;

                break;
            case BuildTarget.iOS:
                PlayerSettings.iOS.buildNumber = platform.Ios.BundleNumber.ToString();
                PlayerSettings.bundleVersion = platform.Ios.Version;

                break;
            default:
                throw new ArgumentException("Choise android or ios build target!");
        }
    }
}
