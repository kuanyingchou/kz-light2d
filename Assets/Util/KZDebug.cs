using UnityEngine;
using System.Collections;

// 2013.8.12  ken  initial version
public class KZDebug {
    public static int LEVEL_NO_DEBUG = 0;
    public static int LEVEL_ERROR = 1;
    public static int LEVEL_WARNING = 2;
    public static int LEVEL_INFO = 3;
    // the higher the level, the more the info

    private static int level = LEVEL_INFO;
    private static bool enableBroadcast = false;

    public static void SetLevel(int lev) {
        level = lev;
    }

    public static void SetBroadcast(bool enable) {
        enableBroadcast = enable;
    }

    public static int GetLevel() { return level; }
    
    public static void LogError(System.Object msg) {
        if(level >= LEVEL_ERROR) {
            Debug.LogError(msg.ToString());
            SendBroadcast(msg);
        }
    }
    public static void LogWarning(System.Object msg) {
        if(level >= LEVEL_WARNING) {
            Debug.LogWarning(msg.ToString());
            SendBroadcast(msg);
        }
    }
    public static void Log(System.Object msg) {
        if(level >=  LEVEL_INFO) {
            Debug.Log(msg.ToString());
            SendBroadcast(msg);
        }
    }
    private static void SendBroadcast(System.Object msg) {
        if(enableBroadcast) KZNC.Send(null, "Log", msg);
    }
}
