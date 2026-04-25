using UnityEngine;

public abstract class Placeable : MonoBehaviour
{
    private PlacementMode placementMode;

    public void BasePlace()
    {
        Place();
    }

    protected virtual void Place()
    {

    }
}