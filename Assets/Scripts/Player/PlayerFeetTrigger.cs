using UnityEngine;

public class PlayerFeetTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        StompTarget stomp = other.GetComponent<StompTarget>();
        if (stomp != null)
            stomp.SendMessage("OnTriggerEnter", GetComponent<Collider>());
    }
}