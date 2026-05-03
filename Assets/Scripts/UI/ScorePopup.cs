using UnityEngine;
using TMPro;
using System.Collections;

public class ScorePopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI popupText;
    [SerializeField] private float fadeSpeed = 2f;
    [SerializeField] private float moveSpeed = 150f;
    [SerializeField] private float stillTime = 0.2f;

    public static ScorePopup Instance;
    private Vector3 startPosition;

    void Awake()
    {
        Instance = this;
        startPosition = transform.localPosition;
        if (popupText != null)
            popupText.color = new Color(1f, 1f, 1f, 0f);
    }

    public void ShowPopup(string message, Color color)
    {
        StopAllCoroutines();
        transform.localPosition = startPosition; // reset position each popup
        StartCoroutine(DisplayPopup(message, color));
    }

    private IEnumerator DisplayPopup(string message, Color color)
    {
        popupText.text = message;
        popupText.color = color;

        // stay still for 0.2 seconds
        yield return new WaitForSeconds(stillTime);

        // move up and fade simultaneously
        float alpha = 1f;
        while (alpha > 0f)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            popupText.color = new Color(color.r, color.g, color.b, alpha);
            transform.localPosition += Vector3.up * moveSpeed * Time.deltaTime;
            yield return null;
        }

        transform.localPosition = startPosition; // reset for next popup
    }
}