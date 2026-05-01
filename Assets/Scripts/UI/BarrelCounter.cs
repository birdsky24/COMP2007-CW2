using UnityEngine;
using TMPro;

public class BarrelCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI counterText;
    [SerializeField] private TextMeshProUGUI durabilityText;
    [SerializeField] private GameObject normalCounterUI;
    [SerializeField] private GameObject placementCounterUI;
    [SerializeField] private TextMeshProUGUI placementCounterText;


    public int Count { get; private set; } = 5;
    private BarrelHitbox barrelHitbox;
    private PlacementMode placementMode;

    void Start()
    {
        barrelHitbox = FindObjectOfType<BarrelHitbox>();
        placementMode = FindObjectOfType<PlacementMode>();
        placementCounterUI.SetActive(false);
        SyncAll();
    }

    void Update()
    {
        if (placementMode == null || Count <= 0) return;

        if (placementMode.isActive)
        {
            normalCounterUI.SetActive(false); // hide normal counter
            placementCounterUI.SetActive(true); // show placement counter
            placementCounterText.text = "Barrels: " + Count; // keep count in sync
        }
        else
        {
            normalCounterUI.SetActive(true); // show normal counter
            placementCounterUI.SetActive(false); // hide placement counter
        }
    }

    private void SyncAll()
    {
        if (barrelHitbox == null)
            barrelHitbox = FindObjectOfType<BarrelHitbox>();

        if (Count <= 0)
        {
            // no barrels left — hide everything
            if (barrelHitbox != null)
                barrelHitbox.HideBarrel();
            if (durabilityText != null)
                durabilityText.gameObject.SetActive(false);
            normalCounterUI.SetActive(false);
            placementCounterUI.SetActive(false);
        }
        else
        {
            // barrels available — show and reset
            if (barrelHitbox != null)
            {
                barrelHitbox.ShowBarrel();
                barrelHitbox.ResetDurability();                 // ADD THIS: sync durability with counter
            }
            if (durabilityText != null)
                durabilityText.gameObject.SetActive(true);
        }

        UpdateDisplay();
    }

    public void Increment()
    {
        Count++;
        SyncAll();
    }

    public void Decrement()
    {
        Count = Mathf.Max(0, Count - 1);
        SyncAll();
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
        {
            barrelHitbox.HideBarrel();
            if (durabilityText != null)
                durabilityText.gameObject.SetActive(false);
            placementCounterUI.SetActive(false);
            normalCounterUI.SetActive(false);
        }
        else
        {
            barrelHitbox.ShowBarrel();
            if (durabilityText != null)
                durabilityText.gameObject.SetActive(true);
        }
    }

    private void UpdateDisplay()
    {
        counterText.text = "Barrels: " + Count;
    }
}