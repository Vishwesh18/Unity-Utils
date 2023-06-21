using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class MyNativeBindings
{
#if UNITY_IPHONE
    [DllImport("__Internal")]
    public static extern string GetSettingsURL();

    [DllImport("__Internal")]
    public static extern void OpenSettings();
#endif

#if UNITY_ANDROID

    public static void OpenSettings()
    {
        Debug.Log("Open Settings Clicked");
        try
        {
            using (var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject currentActivityObject = unityClass.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                string packageName = currentActivityObject.Call<string>("getPackageName");

                using (var uriClass = new AndroidJavaClass("android.net.Uri"))
                using (AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("fromParts", "package", packageName, null))
                using (var intentObject = new AndroidJavaObject("android.content.Intent", "android.settings.APPLICATION_DETAILS_SETTINGS"))
                {
                    intentObject.Call<AndroidJavaObject>("setData", uriObject);
                    currentActivityObject.Call("startActivity", intentObject);
                }
            }
        }

        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
#endif
}