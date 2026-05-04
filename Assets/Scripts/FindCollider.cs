using UnityEngine;

public class FindCollider : MonoBehaviour
{
    void Start()
    {
        Collider[] colliders = Physics.OverlapSphere(new Vector3(-29, 7f, 66), 5f);
        foreach (Collider col in colliders)
        {
            Debug.Log("Found: " + col.gameObject.name +
                      " pos: " + col.transform.position +
                      " type: " + col.GetType().Name);
        }
    }
}