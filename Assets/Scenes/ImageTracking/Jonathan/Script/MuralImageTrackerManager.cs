using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImageManager))]
public class MuralImageTrackerManager : MonoBehaviour
{
    public static MuralImageTrackerManager Instance { get; private set; }

    private ARTrackedImageManager _trackedImageManager;
    private GameObject _currentSpawnedMural;
    private Texture2D _activeTexture;
    private bool _isLocked;

    [SerializeField] private GameObject genericCanvasPrefab;
    [SerializeField] private float maxWorldSizeMeters = 0.8f;

    public event Action<MuralCanvasController> OnMuralReady;
    public event Action<bool> OnTrackingStatusChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        _trackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    private void OnEnable()
    {
        _trackedImageManager.trackablesChanged.AddListener(OnTrackedImagesChanged);
    }

    private void OnDisable()
    {
        _trackedImageManager.trackablesChanged.RemoveListener(OnTrackedImagesChanged);
    }

    public void SetActiveTexture(Texture2D texture)
    {
        _activeTexture = texture;
        if (_currentSpawnedMural != null && !_isLocked)
        {
            MuralCanvasController canvasController = _currentSpawnedMural.GetComponent<MuralCanvasController>();
            if (canvasController != null)
            {
                Vector2 targetScale = CalculateAspectScale(_activeTexture.width, _activeTexture.height);
                canvasController.InitializeCanvas(_activeTexture, targetScale);
            }
        }
    }

    public void LockCurrentMural()
    {
        if (_currentSpawnedMural != null && !_isLocked)
        {
            _currentSpawnedMural.transform.SetParent(null, true);
            _isLocked = true;
        }
    }

    private void OnTrackedImagesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> args)
    {
        bool isAnyTracking = false;
        foreach (var trackedImage in _trackedImageManager.trackables)
        {
            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                isAnyTracking = true;
                break;
            }
        }
        
        OnTrackingStatusChanged?.Invoke(isAnyTracking);

        if (_isLocked) return;

        foreach (var trackedImage in args.added)
        {
            if (_currentSpawnedMural == null)
            {
                SpawnMuralOnMarker(trackedImage);
            }
            else
            {
                _currentSpawnedMural.transform.SetParent(trackedImage.transform, false);
                _currentSpawnedMural.transform.localPosition = new Vector3(0f, 0f, 0.005f);
                _currentSpawnedMural.transform.localRotation = Quaternion.identity;
            }
        }
    }

    private void SpawnMuralOnMarker(ARTrackedImage trackedImage)
    {
        _currentSpawnedMural = Instantiate(genericCanvasPrefab, trackedImage.transform);
        _currentSpawnedMural.transform.localPosition = new Vector3(0f, 0f, 0.005f);
        _currentSpawnedMural.transform.localRotation = Quaternion.identity;

        if (_activeTexture != null)
        {
            MuralCanvasController canvasController = _currentSpawnedMural.GetComponent<MuralCanvasController>();
            if (canvasController != null)
            {
                Vector2 targetScale = CalculateAspectScale(_activeTexture.width, _activeTexture.height);
                canvasController.InitializeCanvas(_activeTexture, targetScale);
                OnMuralReady?.Invoke(canvasController);
            }
        }
    }

    private Vector2 CalculateAspectScale(float width, float height)
    {
        float aspectRatio = width / height;
        float scaleX = maxWorldSizeMeters;
        float scaleY = maxWorldSizeMeters;

        if (aspectRatio > 1f)
        {
            scaleY = maxWorldSizeMeters / aspectRatio;
        }
        else
        {
            scaleX = maxWorldSizeMeters * aspectRatio;
        }

        return new Vector2(scaleX, scaleY);
    }
}