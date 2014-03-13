using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

//An HUD console for debugging
//2013.3.6  ken  initial version
public class KZConsole : MonoBehaviour {
    public int MAX_LINE = 52;
    public Rect bounds = new Rect(400, 0, 400, 300);

    public bool collapse;
    private int repeatCount = 1;
    
    private List<string> lines=null;    

    public void Awake() {
        KZNC.Receive(this, "Log");
        lines=new List<string>();
        scrollPosition = Vector2.zero; //new Vector2(x, y);
    }
    public void Start() { }

    public void Log(KZNotice notice) {
        WriteLine(notice.data);
    }
    public void WriteLine(System.Object o) {
        WriteLine(o.ToString());
    }
    public void WriteLine(string msg) {
        if(collapse && lines.Count > 0 && lines[lines.Count-1] == msg) {
            ++repeatCount;
        } else {
            if(lines.Count == MAX_LINE) {
                lines.RemoveAt(0);
            }
            repeatCount = 0;
            lines.Add(msg);
        }
    }

    public void Clear() {
        lines.Clear();
    }
    public string GetMessages() {
        StringBuilder sb=new StringBuilder();
        foreach(string line in lines) {
            sb.Append(line);
            sb.Append("\n");
        }
        if(collapse && repeatCount > 1) {
            sb.Append("repeat ").Append(repeatCount).Append(" times");
        }
        return sb.ToString();
    }
    
    private Vector2 scrollPosition;

    public void OnGUI() {
        GUILayout.BeginArea(bounds);
        scrollPosition = GUILayout.BeginScrollView (
                scrollPosition, 
                false, true,
                GUILayout.Width(bounds.width), 
                GUILayout.Height(bounds.height));

        GUILayout.Label(GetMessages());
        
        GUILayout.EndScrollView ();
        GUILayout.EndArea();
        
    }
}
