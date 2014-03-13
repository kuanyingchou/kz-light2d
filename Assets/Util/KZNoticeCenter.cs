using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
 
//2013.3.29  ken  modified from 
//                http://wiki.unity3d.com/index.php/NotificationCenter3_5

public class KZNoticeCenter {
 
    private static KZNoticeCenter instance;

    public static bool DEBUG=false;
 
    public static KZNoticeCenter Instance {
        get {
            if (instance == null) {
                instance = new KZNoticeCenter();
            }
            return instance;
        }
    }

    private Dictionary<string, List<Object>> registrations = 
       new Dictionary<string, List<Object>>();
 
 
    // since the == operator on System.Object doesn't handle Unity's 
    // Destroy(), we use UnityEngine.Object as observer type.
    public void AddObserver(Object observer, string name) {
        if(name == null || name == "") {
            Debug.LogError ("Please specify a name for this observer!");
            return;
        }
        if(registrations.ContainsKey(name) == false) {
            registrations[name] = new List<Object>();
        }
 
        List<Object> notifyList = registrations[name];
 
        if(notifyList.Contains(observer)) {
            Debug.LogWarning("Observer had been added before!");
        } else {
            notifyList.Add(observer);
        }
 
    }
 
    public void RemoveObserver(Object observer, string name) {
        List<Object> notifyList = registrations[name];
 
        if(notifyList != null) {
            if(notifyList.Contains(observer)) {
                notifyList.Remove(observer);
            }
            if(notifyList.Count == 0) {
                registrations.Remove(name);
            }
        }
    }
 
    public void PostNotice (Object aSender, string aName) {
        PostNotice(aSender, aName, null);
    }
 
    public void PostNotice(Object aSender, string aName, Hashtable aData)
    {
        PostNotice(new KZNotice(aSender, aName, aData));
    }
    public void PostNotice(Object aSender, string aName, System.Object aData)
    {
        PostNotice(new KZNotice(aSender, aName, aData));
    }
 
    private void PostNotice(KZNotice aNotice)
    {
        if(aNotice.name == null || aNotice.name == "")
        {
            Debug.Log("Null name sent to PostNotice.");
            return;
        }

        List<Object> notifyList = null;
        if(registrations.ContainsKey(aNotice.name)) {
            notifyList = registrations[aNotice.name];
        }
 
        if(notifyList == null)
        {
            Debug.Log("No observer found for notice \""+aNotice.name+"\"");
            return;
        }
 
        List<Object> observersToRemove = new List<Object>();
        List<Object> receiver = new List<Object>();

        for(int i=0; i<notifyList.Count; i++)
        {
            Object observer=notifyList[i];
            if(observer == null) { 
                observersToRemove.Add(observer);
                //since the observer may be destroyed after subscription
            } else {
                if(DEBUG) receiver.Add(observer);
                KZUtil.Call(observer, aNotice.name, aNotice);
                //Since the target type of SendMessage() is GameObject,
                //when multiple scripts attached to the same GameObject
                //listen to the same event, the event would be sent 
                //to the same GameObject several times and produce 
                //undesirable results. For fixing the problem, we bypass
                //GameObject, and use reflection to send the event directly
                //to the observing Component.
            }
        }
        if(DEBUG) {
            string list = KZUtil.Join(receiver, 
                    o => o.GetType() + 
                    ((o.GetType() == typeof(MonoBehaviour))
                    ?
                    "(" + ((MonoBehaviour)o).GetInstanceID() +
                    ") of \"" + ((MonoBehaviour)o).gameObject.name + "\""
                    :
                    "")
                    , 
                    ", ");
            string msg="Sent \""+aNotice.name+"\" to ";
            Debug.Log(msg + list);
        }
 
        foreach(Object observer in observersToRemove) {
            notifyList.Remove(observer);
        }
    }

}
 
public class KZNotice {
 
    public Object sender;
    public string name;
    public System.Object data;
 
    public KZNotice(Object aSender, string aName ) { 
        sender = aSender; 
        name = aName; 
    }
 
    public KZNotice(Object aSender, string aName, Hashtable aData) { 
        sender = aSender; 
        name = aName; 
        data = aData; 
    }

    public KZNotice(Object aSender, string aName, System.Object aData) { 
        sender = aSender; 
        name = aName; 
        data = aData; 
    }
}
