using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Linq;
using System.Reflection;

public class KZUtil {
    //======== Game ==========
    public static void SetFrameRate(int fps) {
        Application.targetFrameRate = fps; 
    }

    //======== GameObject =========
    public static T Instantiate<T>(T target) where T: Object {
        return Object.Instantiate(
                target, Vector3.zero, Quaternion.identity) as T;
    }

    private static Dictionary<string, Object> loadedResources=
        new Dictionary<string, Object>();

    public static T LoadResource<T>(string name) where T:Object {
        if(loadedResources.ContainsKey(name)) {
            return (T)loadedResources[name];
        } else {
            T t=Resources.Load(name) as T;
            loadedResources[name]=t;
            return t;
        }
    }

    //blink(disable renderer) a gameobject <repeat> times 
    //for <duration> seconds, e.g.
    //  0101 or 1010 if repeat == 2
    public static IEnumerator Blink(
            GameObject obj, float duration, int repeat) {
        for(int i=0; i<repeat*2; i++) {
            obj.renderer.enabled = ! obj.renderer.enabled;
            yield return new WaitForSeconds(duration / (repeat*2));
        }
    }
    //place GameObjects in a diamond, used in the farm project 
    public static void PlaceDiamond(
            GameObject[] targets, int numCol, int numRow,
            Vector3 start, Vector3 colDiff, Vector3 rowDiff) {
        if(numCol<0) numCol = 0;
        if(numRow<0) numRow = 0;
        Vector3 rowStart=start;
        for(int row=0; row<numRow; row++) {
            Vector3 rowRunner=rowStart;
            for(int col=0; col<numCol; col++) {
                targets[row * numCol + col].transform.position=rowRunner;
                rowRunner += colDiff;
            }
            rowStart += rowDiff;
        }
    }

    //[ auto shrink/expand game objects to fill the grid
    private static List<GameObject> diamondElements = new List<GameObject>(); 
    //] >>> how about two diamonds?

    public static void PlaceDiamond(GameObject prefab, int numCol, int numRow,
            Vector3 start, Vector3 colDiff, Vector3 rowDiff) {
        if(numCol<0) numCol = 0;
        if(numRow<0) numRow = 0;
        int size = numCol * numRow;
        while(size > diamondElements.Count) {
            diamondElements.Add(Instantiate(prefab));
        }
        while(size < diamondElements.Count) {
            int last = diamondElements.Count - 1;
            GameObject o = diamondElements[last];
            diamondElements.RemoveAt(last);
            GameObject.Destroy(o);
        }
        GameObject[] targets = diamondElements.ToArray();
        PlaceDiamond(targets, numCol, numRow, start, colDiff, rowDiff);
    }


    public static IEnumerable<GameObject> SortByY(
            IList<GameObject> targets) {
        return targets.OrderBy(x => -x.transform.position.y);
    }

    public static void AdjustZOrder(IList<GameObject> targets, float zGap) {
        IEnumerable<GameObject> sortedList=SortByY(targets);
        float z=0;
        foreach(GameObject o in sortedList) {
            o.transform.Translate(Vector3.forward * z);
            z -= zGap;
        }
    }

    //======== string =========
    //e.g. ToTitleCase("father") > "Father"
    public static string ToTitleCase(string input) {
        if(input.Length==0) return input;
        return char.ToUpper(input[0])+input.Substring(1);
    }

    //======== List or array =========
    //e.g. GetString(["a", "b", "c"]) > "a, b, c"
    public static string GetString<T>(IList<T> arr) {
        return Join(arr);
    }
    public static string Join<T>(IList<T> arr) {
        return Join(arr, ", ");
    }

    //e.g. Join(["a", "b", "c"], ";") > "a;b;c"
    public static string Join<T>(IList<T> arr, string del) {
        return Join(arr, o => (o==null?"null":o.ToString()) , del);
    }
    public static string Join<T>(
            IList<T> arr, 
            System.Func<T, string> fun, 
            string del) {
        //return string.Join(del, arr); 
        //] .net framework 4+, not yet supported in unity
        if(arr == null) return "null";
        if(arr.Count == 0) return "";
        StringBuilder sb=new StringBuilder(fun(arr[0]));
        for(int i=1; i<arr.Count; i++) {
            sb.Append(del).Append(fun(arr[i]));
        }
        return sb.ToString();
    }
    public static object[] Array(params object[] t) {
        return t;
    }

    public static void Shuffle<T>(IList<T> arr) {
        for(int i=0; i<arr.Count; i++) {
            T t=arr[i];
            int j=Random.Range(i, arr.Count);
            arr[i]=arr[j];
            arr[j]=t;
        }
    }

    public static void Reload() {
        Application.LoadLevel(Application.loadedLevel);
    }

    public static bool Assert(bool exp) {
        if(!exp) PrintAssertError(null);
        else PrintAssertOK(null);
        return exp;
    }
    public static bool AssertEquals(System.Object a, System.Object b) {
        bool res = a.Equals(b);
        if(!res) PrintAssertError("\""+a + "\" is not \"" + b+"\"");
        else PrintAssertOK(null);
        return res;
    }
    private static void PrintAssertError(string msg) {
        Debug.LogError("Wrong Assertion! "+msg);
    }
    private static void PrintAssertOK(string msg) {
        Debug.LogWarning("Right Assertion! "+msg);
    }

    //Unlike SendMessage(), Call is not limited to GameObjects, and it returns value.
    //and it doesn't produce warnings when no receiver is found.
    //Like SendMessage(), Call only allows one argument.
    public static System.Object Call(System.Object target, string method) {
        return Call(target, method, null);
    }
    public static System.Object Call(
            System.Object target, 
            string method, 
            System.Object arg) {
        //Debug.Log("Calling " + method + "("+arg+") on "+target);
        if(target == null) return null;
        if(string.IsNullOrEmpty(method)) return null;
        System.Type targetType = target.GetType();
        MethodInfo m = null; 
        try {
            m = targetType.GetMethod(method);
        } catch(AmbiguousMatchException) {
            Debug.LogError(target + " has multiple \""+method+"()\"!");
            return null;
        }
        if(m == null) {
            Debug.LogWarning("method not found: "+
                    targetType+"."+method+"("+(arg==null?"":arg)+")");
            return null;
        }

        ParameterInfo[] pars = m.GetParameters();
        System.Type returnType = m.ReturnType;

        //[ determine if is coroutine
        bool isCoroutine = false;
        if(returnType.Equals(typeof(IEnumerator))) {
            isCoroutine = true;
        }
        //Debug.Log("type: "+returnType + " vs. "+typeof(IEnumerator)+", is coroutine: "+isCoroutine);

        //[ prepare parameters
        System.Object[] p;
        if(pars.Length == 0) {
            p = new System.Object[0];
        } else {
            p = new System.Object[] { arg };
        }
        //Debug.Log("processing "+m.Name);
        if(isCoroutine && target is MonoBehaviour) {
            //Debug.Log("calling coroutine");
            return ((MonoBehaviour)target).StartCoroutine(
                (IEnumerator)(m.Invoke(target, p)));
        } else {
            //Debug.Log("calling normal routine");
            return m.Invoke(target, p);
        }
    }

    //modified from http://forum.unity3d.com/threads/31351-GUIText-width-and-height
    public static Rect AutoWordWrap(
            GUIText guiText, float width) {
        string[] words = guiText.text.Split(); 
        //Debug.Log(KZUtil.Join(words, "', '"));
        string result = "";
        Rect textArea = new Rect();

        for(int i = 0; i < words.Length; i++) {
            if(words[i].Trim() == "") continue;
            // set the gui text to the current string including new word
            guiText.text = (result + words[i] + " ");
            // measure it
            textArea = guiText.GetScreenRect();
            // if it didn't fit, put word onto next line, otherwise keep it
            if(textArea.width > width) {
                result += ("\n" + words[i] + " ");
            } else {
                result = guiText.text;
            }
        }
        return textArea;
    }

    public static Rect AutoCharWrap(
            GUIText guiText, float width) {
        //Debug.Log(KZUtil.Join(words, "', '"));
        string words = guiText.text;
        string result = "";
        Rect textArea = new Rect();

        for(int i = 0; i < words.Length; i++) {
            if(words[i] == '\n') continue;
            // set the gui text to the current string including new word
            guiText.text = (result + words[i]);
            // measure it
            textArea = guiText.GetScreenRect();
            // if it didn't fit, put word onto next line, otherwise keep it
            if(textArea.width > width) {
                result += ("\n" + words[i]);
            } else {
                result = guiText.text;
            }
        }
        return textArea;
    }
}
