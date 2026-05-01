using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    private float elapsedTime = 0f;
    private bool isRunning = true;

    public float ElapsedTime { get => elapsedTime; }
    public bool IsRunning { get => isRunning; }

    void Update()
    {
        if (!isRunning) return;

        elapsedTime += Time.deltaTime;
        UpdateDisplay();
    }

    public void Pause()
    {
        isRunning = false;
    }

    public void Resume()
    {
        isRunning = true;
    }

    public void Stop()
    {
        isRunning = false;
    }

    public string GetFormattedTime()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void UpdateDisplay()
    {
        timerText.text = GetFormattedTime();
    }
}