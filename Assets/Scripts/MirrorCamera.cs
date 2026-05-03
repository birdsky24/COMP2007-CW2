using UnityEngine;

public class MirrorCamera : MonoBehaviour
{
    [SerializeField] private Transform mirrorSurface;
    [SerializeField] private float disableDistance = 10f;
    private Camera mainCam;
    private Camera mirrorCam;

    void Start()
    {
        mainCam = Camera.main;
        mirrorCam = GetComponent<Camera>();
    }

    void Update()
    {
        if (mainCam == null || mirrorSurface == null) return;

        float distance = Vector3.Distance(mainCam.transform.position, mirrorSurface.position);

        mirrorCam.enabled = distance <= disableDistance;

        if (!mirrorCam.enabled) return;

        // --- existing mirror logic below ---

        float offset = 0.2f;

        Vector3 planarOffset = Vector3.ProjectOnPlane(
            mainCam.transform.position - mirrorSurface.position,
            mirrorSurface.forward
        );

        transform.position =
            mirrorSurface.position
            - mirrorSurface.forward * offset
            + planarOffset;

        Vector3 reflectedForward = Vector3.Reflect(
            mainCam.transform.forward,
            mirrorSurface.forward
        );

        transform.rotation = Quaternion.LookRotation(reflectedForward, Vector3.up);
    }
}