using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
 
#if UNITY_ANDROID
public class SetKeystore : IPreprocessBuildWithReport
{
    public int callbackOrder {  get { return 0; } }

    void IPreprocessBuildWithReport.OnPreprocessBuild(BuildReport report)
    {
         PlayerSettings.Android.keystoreName = System.Environment.ExpandEnvironmentVariables("ANDROID_KEYSTORE_PATH");
         PlayerSettings.Android.keystorePass = System.Environment.ExpandEnvironmentVariables("ANDROID_KEYSTORE_STORE_PASS");
         PlayerSettings.Android.keyaliasName = System.Environment.ExpandEnvironmentVariables("ANDROID_KEYSTORE_ALIAS");
         PlayerSettings.Android.keyaliasPass = System.Environment.ExpandEnvironmentVariables("ANDROID_KEYSTORE_PASS");
    }
}
#endif