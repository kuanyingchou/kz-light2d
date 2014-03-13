using UnityEngine;
using System.Collections;
using System;

//Attach it to any gameobject in scene to print revision number

//2013.3.15  ken  initial version
public class KZRevisionPrinter : MonoBehaviour {
    private KZRevision revision;
    //private Rect labelRect=new Rect(780, 770, 500, 30);
    private static Rect labelRect=new Rect(100, 10, 800, 60);
    private static Rect shadowRect=new Rect(
            labelRect.x + 1, labelRect.y + 1, 
            labelRect.width, labelRect.height);

    public void Awake() {
        revision=KZRevision.Load();
    }

    public void OnGUI() {
        #if UNITY_ANDROID
        if(revision == null) {
            Write("Version N/A. Kizi Lab Inc.");
        } else {
            Write("Commit "+revision.revision + 
                    ". Built at "+revision.buildTime +
                    ". Kizi Lab Inc.");
        }
        #endif
    }

    private void Write(string text) {
        GUI.contentColor = Color.black;
        GUI.Label(shadowRect, text);
        GUI.contentColor = Color.white;
        GUI.Label(labelRect, text);
    }
}
