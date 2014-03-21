using UnityEngine;

public class KZLauncherUtil {
    public static bool IsGlobalCheatingOn() {
    #if UNITY_ANDROID
        AndroidJavaClass util = new AndroidJavaClass("com.allproducts.kizi.plugins.util.Util"); 
        return util.CallStatic<bool>("isGlobalCheatingOn", KZAndroid.GetBaseContext(KZAndroid.GetCurrentActivity()));
    #else
        return false;
    #endif
    }
    public static void BackToCategory() {
    #if UNITY_ANDROID
        AndroidJavaClass util = new AndroidJavaClass("com.allproducts.kizi.plugins.util.Util"); 
        util.CallStatic("backToCategory", KZAndroid.GetBaseContext(KZAndroid.GetCurrentActivity()));
    #else
        ;
    #endif
    }
}
