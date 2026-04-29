using UnityEngine;
using TMPro;

public class BarrelCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI counterText;
    [SerializeField] private TextMeshProUGUI durabilityText;
    [SerializeField] private GameObject normalCounterUI;        // ADD THIS: drag normal counter panel
    [SerializeField] private GameObject placementCounterUI;     // ADD THIS: drag placement counter panel
    [SerializeField] private TextMeshProUGUI placementCounterText;


    public int Count { get; private set; } = 5;
    private BarrelHitbox barrelHitbox;
    private PlacementMode placementMode;

    void Start()
    {
        barrelHitbox = FindObjectOfType<BarrelHitbox>();
        placementMode = FindObjectOfType<PlacementMode>();
        placementCounterUI.SetActive(false);
        UpdateDisplay();
    }

    void Update()
    {
        if (placementMode == null) return;

        if (placementMode.isActive)
        {
            normalCounterUI.SetActive(false);                   // hide normal counter
            placementCounterUI.SetActive(true);                 // show placement counter
            placementCounterText.text = "Barrels: " + Count;   // keep count in sync
        }
        else
        {
            normalCounterUI.SetActive(true);                    // show normal counter
            placementCounterUI.SetActive(false);                // hide placement counter
        }
    }

    public void Increment()
    {
        Count++;
        UpdateDisplay();
        CheckBarrelVisibility();
    }

    public void Decrement()
    {
        Count = Mathf.Max(0, Count - 1);
        UpdateDisplay();
        CheckBarrelVisibility();
    }

    public void UpdateDurabilityDisplay(int durability)
    {
        if (durabilityText != null)
            durabilityText.text = "Durability: " + durability;
    }

    private void CheckBarrelVisibility()
    {
        if (barrelHitbox == null)
            barrelHitbox = FindObjectOfType<BarrelHitbox>();

        if (barrelHitbox == null) return;

        if (Count <= 0)
            barrelHitbox.HideBarrel();
        else
            barrelHitbox.ShowBarrel();
    }

    private void UpdateDisplay()
    {
        counterText.text = "Barrels: " + Count;
    }
}