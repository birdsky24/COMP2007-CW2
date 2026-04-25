using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : Interactable
{
    private BarrelCounter counter;

    void Start()
    {
        counter = FindObjectOfType<BarrelCounter>();
    }

    protected override void Interact()
    {
        Debug.Log("Interacted with " + gameObject.name);
        if (counter != null)
            counter.Increment();
        Destroy(gameObject);
    }
}