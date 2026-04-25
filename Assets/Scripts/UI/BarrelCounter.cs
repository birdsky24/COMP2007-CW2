using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BarrelCounter : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI counterText;
    public int Count { get; private set; } = 5;

    void Start()
    {
        UpdateDisplay();
    }

    public void Increment()
    {
        Count++;
        UpdateDisplay();
    }

    public void Decrement()
    {
        Count = Mathf.Max(0, Count - 1);
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        counterText.text = "Barrels: " + Count;
    }
}