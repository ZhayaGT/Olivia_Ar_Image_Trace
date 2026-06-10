using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MuralUIManager : MonoBehaviour
{
    [SerializeField] private GameObject window1Scan;
    [SerializeField] private GameObject window2Select;
    [SerializeField] private GameObject window3Edit;
    [SerializeField] private GameObject window4Trace;

    [SerializeField] private Button window1NextButton;
    [SerializeField] private Button window2NextButton;
    [SerializeField] private Button window3NextButton;

    [SerializeField] private Texture2D[] defaultTextures;
    
    [SerializeField] private RectTransform scalePanel;
    [SerializeField] private RectTransform offsetPanel;
    [SerializeField] private RectTransform opacityPanel;
    [SerializeField] private RectTransform rotationPanel;
    
    [SerializeField] private float animationDuration = 0.3f;
    [SerializeField] private float panelHiddenY = -300f;
    [SerializeField] private float panelVisibleY = 0f;

    [SerializeField] private Slider xSlider;
    [SerializeField] private Slider ySlider;
    [SerializeField] private Slider rotateSlider;
    [SerializeField] private Slider scaleSlider;
    [SerializeField] private Slider intensitySlider;

    private MuralCanvasController _activeController;
    private RectTransform _currentActivePanel;
    private Coroutine _animationCoroutine;
    private Texture2D _customTexture;

    private void Start()
    {
        SwitchToWindow(1);
        window1NextButton.interactable = false;
        window2NextButton.interactable = false;

        InitializePanels();

        if (MuralImageTrackerManager.Instance != null)
        {
            MuralImageTrackerManager.Instance.OnTrackingStatusChanged += HandleTrackingStatus;
            MuralImageTrackerManager.Instance.OnMuralReady += HandleNewMuralSpawned;
        }
    }

    private void OnDestroy()
    {
        if (MuralImageTrackerManager.Instance != null)
        {
            MuralImageTrackerManager.Instance.OnTrackingStatusChanged -= HandleTrackingStatus;
            MuralImageTrackerManager.Instance.OnMuralReady -= HandleNewMuralSpawned;
        }
        
        if (_customTexture != null)
        {
            Destroy(_customTexture);
        }
    }

    public void SwitchToWindow(int windowIndex)
    {
        window1Scan.SetActive(windowIndex == 1);
        window2Select.SetActive(windowIndex == 2);
        window3Edit.SetActive(windowIndex == 3);
        window4Trace.SetActive(windowIndex == 4);

        if (windowIndex == 4)
        {
            MuralImageTrackerManager.Instance.LockCurrentMural();
        }
    }

    private void HandleTrackingStatus(bool isTracking)
    {
        if (window1Scan.activeSelf)
        {
            window1NextButton.interactable = isTracking;
        }
    }

    public void SelectBuiltInImage(int index)
    {
        if (index >= 0 && index < defaultTextures.Length)
        {
            MuralImageTrackerManager.Instance.SetActiveTexture(defaultTextures[index]);
            window2NextButton.interactable = true;
        }
    }

    public void OpenNativeGallery()
    {
        NativeGallery.GetImageFromGallery((path) =>
        {
            if (path != null)
            {
                if (_customTexture != null)
                {
                    Destroy(_customTexture);
                }

                _customTexture = NativeGallery.LoadImageAtPath(path, -1, false);
                if (_customTexture != null)
                {
                    MuralImageTrackerManager.Instance.SetActiveTexture(_customTexture);
                    window2NextButton.interactable = true;
                }
            }
        });
    }

    private void InitializePanels()
    {
        SetPanelY(scalePanel, panelHiddenY);
        SetPanelY(offsetPanel, panelHiddenY);
        SetPanelY(opacityPanel, panelHiddenY);
        SetPanelY(rotationPanel, panelHiddenY);
    }

    private void SetPanelY(RectTransform panel, float y)
    {
        if (panel != null)
        {
            Vector2 pos = panel.anchoredPosition;
            pos.y = y;
            panel.anchoredPosition = pos;
        }
    }

    public void TogglePanel(RectTransform targetPanel)
    {
        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
        }
        _animationCoroutine = StartCoroutine(AnimatePanels(targetPanel));
    }

    private IEnumerator AnimatePanels(RectTransform targetPanel)
    {
        float time = 0f;
        Vector2 currentPanelStartPos = Vector2.zero;
        Vector2 targetPanelStartPos = Vector2.zero;

        if (_currentActivePanel != null && _currentActivePanel != targetPanel)
        {
            currentPanelStartPos = _currentActivePanel.anchoredPosition;
        }

        if (targetPanel != null)
        {
            targetPanelStartPos = targetPanel.anchoredPosition;
            
            if (_currentActivePanel == targetPanel)
            {
                while (time < animationDuration)
                {
                    time += Time.deltaTime;
                    float t = time / animationDuration;
                    float newY = Mathf.Lerp(targetPanelStartPos.y, panelHiddenY, t);
                    SetPanelY(targetPanel, newY);
                    yield return null;
                }
                _currentActivePanel = null;
                yield break;
            }
        }

        while (time < animationDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, time / animationDuration);

            if (_currentActivePanel != null && _currentActivePanel != targetPanel)
            {
                float hideY = Mathf.Lerp(currentPanelStartPos.y, panelHiddenY, t);
                SetPanelY(_currentActivePanel, hideY);
            }

            if (targetPanel != null)
            {
                float showY = Mathf.Lerp(targetPanelStartPos.y, panelVisibleY, t);
                SetPanelY(targetPanel, showY);
            }

            yield return null;
        }

        _currentActivePanel = targetPanel;
    }

    private void HandleNewMuralSpawned(MuralCanvasController canvasController)
    {
        _activeController = canvasController;
        _activeController.ResetTransformations();

        if (xSlider != null) xSlider.SetValueWithoutNotify(0f);
        if (ySlider != null) ySlider.SetValueWithoutNotify(0f);
        if (rotateSlider != null) rotateSlider.SetValueWithoutNotify(0f);
        if (scaleSlider != null) scaleSlider.SetValueWithoutNotify(1f);
        if (intensitySlider != null) intensitySlider.SetValueWithoutNotify(1f);
    }

    public void OnXSliderChanged(float value) { if (_activeController != null) _activeController.SetTranslationX(value); }
    public void OnYSliderChanged(float value) { if (_activeController != null) _activeController.SetTranslationY(value); }
    public void OnRotateSliderChanged(float value) { if (_activeController != null) _activeController.SetRotationZ(value); }
    public void OnScaleSliderChanged(float value) { if (_activeController != null) _activeController.SetScaleMultiplier(value); }
    public void OnIntensitySliderChanged(float value) { if (_activeController != null) _activeController.SetIntensity(value); }
    
    public void OnFlipXButtonClicked() { if (_activeController != null) _activeController.ToggleFlipHorizontal(); }
    public void OnFlipYButtonClicked() { if (_activeController != null) _activeController.ToggleFlipVertical(); }
}