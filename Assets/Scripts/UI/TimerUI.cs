using TMPro; 
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    public float timer;
    public bool isRunning;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        timer = 0;
        StartTimer();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isRunning) return;
        timer += Time.deltaTime;
        int minutes      = (int)timer / 60;
        int seconds      = (int)timer % 60;
        int milliseconds = (int)((timer - (int)timer) * 100);

        timerText.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
        
    }
    public void StartTimer()  => isRunning = true;
    public void StopTimer()   => isRunning = false;

}
