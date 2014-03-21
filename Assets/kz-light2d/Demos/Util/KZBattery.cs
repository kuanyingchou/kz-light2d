using UnityEngine;
using System.Collections;

public class KZBattery : MonoBehaviour {
    public static float GetBatteryLevel() {
        #if UNITY_ANDROID
        AndroidJavaObject activity = KZAndroid.GetCurrentActivity();
        if(activity == null) return 0;
        AndroidJavaObject context = KZAndroid.GetBaseContext(activity);
        if(context == null) return 0;
        AndroidJavaClass util = new AndroidJavaClass(
                "com.allproducts.kizi.plugins.util.Util");
        if(util == null) return 0;
        return util.CallStatic<float>("getBatteryLevel", context);
        #else
        return 0f;
        #endif
    }

}
