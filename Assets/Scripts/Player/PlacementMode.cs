using UnityEngine;

public class PlacementMode : MonoBehaviour
{
    private Camera cam;
    private InputManager inputManager;

    [SerializeField] private GameObject[] placeableObjectPrefabs;
    [SerializeField] private GameObject previewObjectPrefab;
    [SerializeField] private LayerMask placementSurfaceMask;
    [SerializeField] private float distance = 3f;

    private GameObject _previewObject = null;
    private Vector3 _currentPlacementPosition = Vector3.zero;
    private BarrelCounter counter;
    private PreviewObject _previewObjectComponent = null;
    private BarrelHitbox barrelHitbox;

    public bool isActive { get; private set; }

    void Start()
    {
        cam = GetComponent<PlayerLook>().cam;
        inputManager = GetComponent<InputManager>();
        counter = FindObjectOfType<BarrelCounter>();
        barrelHitbox = FindObjectOfType<BarrelHitbox>();
    }

    void Update()
    {
        if (!isActive || _previewObject == null) return;

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * distance);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, distance, placementSurfaceMask))
        {
            _currentPlacementPosition = hitInfo.point;

            Quaternion rotation = Quaternion.Euler(
                0f, cam.transform.rotation.eulerAngles.y, 0f);

            _previewObject.transform.position = _currentPlacementPosition;
            _previewObject.transform.rotation = rotation;

            if (inputManager.onFoot.Place.triggered && counter.Count > 0 && _previewObjectComponent.isValid)
            {
                Place(rotation);
                counter.Decrement();
            }
        }
    }

    private void Place(Quaternion rotation)
    {
        Instantiate(placeableObjectPrefabs[Random.Range(0, placeableObjectPrefabs.Length)],
            _currentPlacementPosition, rotation);

        // reset durability since a new barrel is now held
        BarrelHitbox barrelHitbox = FindObjectOfType<BarrelHitbox>();
        if (barrelHitbox != null)
            barrelHitbox.ResetDurability();
    }

    public void TogglePlacementMode()
    {
        if (!isActive && barrelHitbox != null && barrelHitbox.IsSwinging)
            return;

        isActive = !isActive;

        if (isActive)
        {
            Debug.Log("Entering placement mode");
            Quaternion rotation = Quaternion.Euler(0f, cam.transform.rotation.eulerAngles.y, 0f);
            _previewObject = Instantiate(previewObjectPrefab, _currentPlacementPosition, rotation);
            _previewObjectComponent = _previewObject.GetComponent<PreviewObject>();
        }
        else
        {
            Debug.Log("Exiting placement mode");
            Destroy(_previewObject); //Delete ghost
            _previewObject = null;
        }
    }
}