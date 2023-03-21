using UnityEngine;
using System.Collections;
using TMPro;

public class TimerDisplay : MonoBehaviour {

    public TMP_Text timerText;
    public int timer;
    public int timerTemp;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (timer != timerTemp) {
            timerTemp = timer;
            refreshTimer();
        }

        //On cache le timer si timer <= 0
        if (timer < 0)
            gameObject.SetActive(false);
    }

    public void refreshTimer() {
        timerText.text = timer.ToString();
    }

    public IEnumerator startTimerAt(int timeInSecond) {
        gameObject.SetActive(true);
        timer = timeInSecond;
        while (timer > 0) {
            timer--;
            yield return new WaitForSeconds(1);
        }
    }
}
