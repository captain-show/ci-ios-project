using UnityEditor.Callbacks;
using UnityEditor;
using System.IO;
using UnityEngine;
using System.Linq;

#if UNITY_IOS
using UnityEditor.iOS.Xcode;

public static class BuildPostProcess
{
    private const string EXIST_ON_SUSPEND_KEY = "UIApplicationExitsOnSuspend";

    private const string VERSIONING_SYSTEM_KEY = "VERSIONING_SYSTEM";
    private const string CURRENT_PROJECT_VERSION_KEY = "CURRENT_PROJECT_VERSION";

    private const string ENABLE_BITCODE_KEY = "ENABLE_BITCODE";

    private const string CODE_SIGN_STYLE_KEY = "CODE_SIGN_STYLE";

    private const string PROVISIONING_PROFILE_SPECIFIER_KEY = "PROVISIONING_PROFILE_SPECIFIER";

    [PostProcessBuild(1)]
    public static void IOSBuildPostProcess(BuildTarget target, string pathToBuiltProject)
    {
        AddPListValues(pathToBuiltProject);
        RemoveDeprecatedInfoPListKeys(pathToBuiltProject);

        string projectPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
        var pbxProject = new PBXProject();
        pbxProject.ReadFromFile(projectPath);

        string guidProject = pbxProject.TargetGuidByName(pbxProject.GetUnityMainTargetGuid());

        Debug.Log("Setting Versioning system to Apple Generic...");
        pbxProject.SetBuildProperty(guidProject, VERSIONING_SYSTEM_KEY, CIScript.APPLE_GENERIC_VALUE);
        pbxProject.SetBuildProperty(guidProject, CURRENT_PROJECT_VERSION_KEY, CIScript.CURRENT_PROJECT_VERSION);

        Debug.Log("Disabling bitcode...");
        pbxProject.SetBuildProperty(guidProject, ENABLE_BITCODE_KEY, CIScript.ENABLE_BITCODE);

        Debug.Log("Setting Code sign style to manual and setup provisioning profile specifier...");
        pbxProject.SetBuildProperty(guidProject, CODE_SIGN_STYLE_KEY, CIScript.CODE_SIGN_STYLE);
        pbxProject.SetBuildProperty(guidProject, PROVISIONING_PROFILE_SPECIFIER_KEY, CIScript.PROVISIONING_PROFILE_KEY);

        pbxProject.WriteToFile(projectPath);
    }

    private static void RemoveDeprecatedInfoPListKeys(string pathToBuiltProject)
    {
        string plistPath = Path.Combine(pathToBuiltProject, CIScript.PLIST_FILE);
        PlistDocument plist = new PlistDocument();
        plist.ReadFromString(File.ReadAllText(plistPath));

        PlistElementDict rootDict = plist.root;

        if (rootDict.values.ContainsKey(EXIST_ON_SUSPEND_KEY))
        {
            Debug.LogFormat("Removing deprecated key \"{0}\" on \"{1}\" file", EXIST_ON_SUSPEND_KEY, CIScript.PLIST_FILE);
            rootDict.values.Remove(EXIST_ON_SUSPEND_KEY);
        }

        File.WriteAllText(plistPath, plist.WriteToString());
    }

    private static void AddPListValues(string pathToBuiltProject)
    {
        string plistPath = Path.Combine(pathToBuiltProject, CIScript.PLIST_FILE);
        string mopubTextPath = "./.ci/sk_adnetworks.txt";

        if (!File.Exists(mopubTextPath))
        {
            return;
        }

        string mopubText = File.ReadAllText(mopubTextPath);

        if (mopubText.Length == 0)
        { 
            return;
        }

        string plistText = File.ReadAllText(plistPath);

        var lines = plistText.Split('\n').ToList();
        lines.Insert(lines.Count - 3, mopubText);

        File.WriteAllText(plistPath, string.Join("\n", lines));
    }
}
#endif