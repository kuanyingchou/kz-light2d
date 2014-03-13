using UnityEngine;
using System.Collections;

// A simple wrapper around KZNoticeCenter so that users can use:
//   KZNC.Send(this, "Go");
// instead of:
//   KZNoticeCenter.Instance.PostNotice(this, "Go");

public class KZNC {

    public static void Send(Object sender, string name) {
        KZNoticeCenter.Instance.PostNotice(sender, name);
    }
    public static void Send(
            Object sender, string name, System.Object obj) {
        KZNoticeCenter.Instance.PostNotice(sender, name, obj);
    }

    public static void Receive(Object observer, string name) {
        KZNoticeCenter.Instance.AddObserver(observer, name);
    }
}
