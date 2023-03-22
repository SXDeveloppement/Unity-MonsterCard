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
        //On cache le timer si timer <= 0
        if (timer < 0 && gameObject.activeSelf) {
            gameObject.SetActive(false);
        }
            

        if (timer != timerTemp) {
            timerTemp = timer;
            refreshTimer();
        }
    }

    public void refreshTimer() {
        timerText.text = timer.ToString();
    }

    public IEnumerator startTimerAt(int timeInSecond) {
        timer = timeInSecond;
        gameObject.SetActive(true);
        while (timer >= 0) {
            timer--;
            yield return new WaitForSeconds(1);
        }
    }
}
