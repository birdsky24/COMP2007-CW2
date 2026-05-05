using UnityEngine;

public class PreviewObject : MonoBehaviour
{
    [SerializeField] private Renderer previewRenderer;
    [SerializeField] private Material validMaterial;
    [SerializeField] private Material invalidMaterial;
    [SerializeField] private LayerMask ignoreMask;

    public bool isValid { get; private set; } = true;
    private Collider previewCollider;

    void Start()
    {
        previewCollider = GetComponent<Collider>();
    }

    void Update()
    {
        if (previewCollider == null) return;

        // check actual live colliders every frame
        Collider[] overlaps = Physics.OverlapBox(
            previewCollider.bounds.center,
            previewCollider.bounds.extents,
            transform.rotation,
            ~ignoreMask);

        // filter out self
        int count = 0;
        foreach (Collider col in overlaps)
        {
            if (col.gameObject != gameObject)
                count++;
        }

        isValid = count == 0;
        previewRenderer.material = isValid ? validMaterial : invalidMaterial;
    }
}