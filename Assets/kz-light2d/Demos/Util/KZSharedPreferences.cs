using UnityEngine;
using System.Collections;

//A wrapper of android.content.SharedPreferences
//2013.11.20  ken  initial version

#if UNITY_ANDROID
public class KZSharedPreferences {
    private static AndroidJavaObject pref = null;
    private static AndroidJavaObject editor = null;

    public static int GetInt(string key) {
        AndroidJavaObject pref = GetSharedPreferences();
        return pref.Call<int>("getInt", key, 0);
    }
    public static long GetLong(string key) {
        AndroidJavaObject pref = GetSharedPreferences();
        return pref.Call<long>("getLong", key, 0L);
    }
    public static float GetFloat(string key) {
        AndroidJavaObject pref = GetSharedPreferences();
        return pref.Call<float>("getFloat", key, 0f);
    }
    public static double GetDouble(string key) {
        AndroidJavaObject pref = GetSharedPreferences();
        return pref.Call<double>("getDouble", key, 0d);
    }
    public static string GetString(string key) {
        AndroidJavaObject pref = GetSharedPreferences();
        return pref.Call<string>("getString", key, "");
    }

    public static void SetBoolean(string key, bool val) {
        AndroidJavaObject editor = GetEditor();
        editor.Call<AndroidJavaObject>("putBoolean", key, val);
        editor.Call<bool>("commit");
    }
    public static void SetInt(string key, int val){
        AndroidJavaObject editor = GetEditor();
        editor.Call<AndroidJavaObject>("putInt", key, val);
        editor.Call<bool>("commit");
    }
    public static void SetLong(string key, long val){
        AndroidJavaObject editor = GetEditor();
        editor.Call<AndroidJavaObject>("putLong", key, val);
        editor.Call<bool>("commit");
    }
    public static void SetFloat(string key, float val){
        AndroidJavaObject editor = GetEditor();
        editor.Call<AndroidJavaObject>("putFloat", key, val);
        editor.Call<bool>("commit");
    }
    public static void SetDouble(string key, double val) {
        AndroidJavaObject editor = GetEditor();
        editor.Call<AndroidJavaObject>("putDouble", key, val);
        editor.Call<bool>("commit");
    }
    public static void SetString(string key, string val) {
        AndroidJavaObject editor = GetEditor();
        editor.Call<AndroidJavaObject>("putString", key, val);
        editor.Call<bool>("commit");
    }
    public static bool HasKey(string key) {
        AndroidJavaObject pref = GetSharedPreferences();
        return pref.Call<bool>("contains");
    }
    public static void DeleteKey(string key) {
        AndroidJavaObject editor = GetEditor();
        editor.Call<AndroidJavaObject>("remove", key);
    }
    public static void Save() {
        AndroidJavaObject editor = GetEditor();
        editor.Call<AndroidJavaObject>("commit");
    }


    private static AndroidJavaObject GetSharedPreferences() {
        if(pref != null) return pref;
        AndroidJavaObject context = KZAndroid.GetBaseContext();
        pref = context.Call<AndroidJavaObject>(
                "getSharedPreferences", KZAndroid.GetPackageName()+"-prefs", 1); 
        return pref;
        //] 1 means MODE_WORLD_READABLE
    }
    private static AndroidJavaObject GetEditor() {
        if(editor != null) return editor;
        AndroidJavaObject pref = GetSharedPreferences();
        editor = pref.Call<AndroidJavaObject>("edit");
        return editor;
    }
}
#endif
