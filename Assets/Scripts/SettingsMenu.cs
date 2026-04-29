using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    private GameObject previousScreen;      // tracks which screen opened settings

    void Start()
    {
        gameObject.SetActive(false);
    }

    public void Show(GameObject caller)
    {
        previousScreen = caller;
        previousScreen.SetActive(false);    // hide whichever screen opened settings
        gameObject.SetActive(true);
    }

    public void Back()
    {
        gameObject.SetActive(false);
        if (previousScreen != null)
            previousScreen.SetActive(true); // return to whichever screen opened settings
    }
}