using UnityEngine;
using System.Collections;

public class TestKZTimer : MonoBehaviour {

    KZTimer timer;

    void Start () {
    }
	
    void Update () {
        if(timer == null) {
            timer=new KZTimer(3);
            timer.Start();
            //timer.Pause();
            //timer.Cancel();
            //timer.Stop();
        }

        timer.Update();
        if(timer.IsTimeUp()) {
            Debug.Log("Times up");

        } else {
            Debug.Log("counting...");
        }   

    }
}
