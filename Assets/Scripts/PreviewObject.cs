using UnityEngine;
public class PreviewObject : MonoBehaviour
{
    [SerializeField] private Renderer previewRenderer;
    [SerializeField] private Material validMaterial;
    [SerializeField] private Material invalidMaterial;
    [SerializeField] private LayerMask ignoreMask;

    public bool isValid { get; private set; } = true;
    private int _overlapCount = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & ignoreMask) != 0) return;
        _overlapCount++;
        isValid = false;
        previewRenderer.material = invalidMaterial;
    }

    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & ignoreMask) != 0) return;
        _overlapCount = Mathf.Max(0, _overlapCount - 1);
        isValid = _overlapCount == 0;
        previewRenderer.material = isValid ? validMaterial : invalidMaterial;
    }
}