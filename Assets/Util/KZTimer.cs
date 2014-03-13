using UnityEngine;

public class KZTimer {
    private bool running=false;
    private float elapsedTime=0;
    private float limit;

    public KZTimer() {}
    public KZTimer(float t) {
        limit=t;
    }

    public void SetTime(float t) {
        limit=t;
    }

    public void Start() {
        running=true;
    }
    public void Pause() {
        running=false;
    }
    public void Resume() {
        Start();
    }
    public void Reset() {
        elapsedTime=0;
    }
    public void Cancel() {
        Pause();
        Reset();
    }
    public void Stop() {
        Cancel();
    }

    public void Update() {
        Update(Time.deltaTime * Time.timeScale);
    }
    public void Update(float increment) {
        if(running) {
            elapsedTime += increment;
        }
    }

    public bool IsTimeUp() {
        if(elapsedTime >= limit) {
            return true;
        } else {
            return false;
        }
    }
}


