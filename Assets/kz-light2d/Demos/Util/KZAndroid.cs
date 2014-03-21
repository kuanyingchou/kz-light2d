using UnityEngine;
using System.Collections;

//[ android utility methods intended to be used by libraries
//
//Note: the code should not throw any exceptions when things go wrong.

#if UNITY_ANDROID
public class KZAndroid : MonoBehaviour {
    public static AndroidJavaObject GetCurrentActivity() {
        // from http://answers.unity3d.com/questions/59622/accessing-the-activity-context-in-android-plugin.html
        AndroidJavaClass jc = 
                new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        return jo;
    }

    public static AndroidJavaObject GetApplicationContext(
            AndroidJavaObject activity) {
        if(activity == null) {
            return null;
        } else {
            return activity.Call<AndroidJavaObject>("getApplicationContext");
        }
    }
    public static AndroidJavaObject GetBaseContext(
            AndroidJavaObject activity) {
        if(activity == null) return null;
        else return activity.Call<AndroidJavaObject>("getBaseContext");
    }
    public static AndroidJavaObject GetBaseContext() {
        AndroidJavaObject activity = GetCurrentActivity();
        if(activity == null) return null;
        else return activity.Call<AndroidJavaObject>("getBaseContext");
    }

    public static string GetPackageName() {
        AndroidJavaObject activity = GetCurrentActivity();
        if(activity == null) return "";
        else return GetApplicationContext(activity).Call<string>("getPackageName");
    }

    //>>> TODO
    public static string GetStringExtra(string key) {
        AndroidJavaObject activity = GetCurrentActivity();
        AndroidJavaObject intent = activity.Call<AndroidJavaObject>("getIntent"); 
        return intent.Call<string>("getStringExtra", key);
    }

}
#endif
