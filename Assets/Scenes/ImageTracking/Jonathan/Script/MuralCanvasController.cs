using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class MuralCanvasController : MonoBehaviour
{
    private MeshRenderer _meshRenderer;
    private MaterialPropertyBlock _propBlock;
    
    private Vector2 _runtimeAspectScale;

    private float _offsetX;
    private float _offsetY;
    private float _rotationZ;
    private float _scaleMultiplier = 1f;
    private float _intensity = 1f;
    
    private float _flipX = 1f;
    private float _flipY = 1f;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _propBlock = new MaterialPropertyBlock();
    }

    public void InitializeCanvas(Texture2D texture, Vector2 aspectScale)
    {
        _meshRenderer.GetPropertyBlock(_propBlock);
        _propBlock.SetTexture("_BaseMap", texture);
        _meshRenderer.SetPropertyBlock(_propBlock);

        _runtimeAspectScale = aspectScale;

        ApplyTransformations();
    }

    public void SetTranslationX(float value) { _offsetX = value; ApplyTransformations(); }
    public void SetTranslationY(float value) { _offsetY = value; ApplyTransformations(); }
    public void SetRotationZ(float value) { _rotationZ = value; ApplyTransformations(); }
    public void SetScaleMultiplier(float value) { _scaleMultiplier = value; ApplyTransformations(); }
    public void SetIntensity(float value) { _intensity = value; ApplyTransformations(); }
    public void ToggleFlipHorizontal(bool flip) { _flipX = flip ? -1f : 1f; ApplyTransformations(); }
    public void ToggleFlipVertical(bool flip) { _flipY = flip ? -1f : 1f; ApplyTransformations(); }

    private void ApplyTransformations()
    {
        transform.localPosition = new Vector3(_offsetX, _offsetY, 0.005f);

        // Ubah rotasi dasar X menjadi 90f agar Quad berdiri sejajar dengan marker fisik.
        // Rotasi Z tetap menerima nilai dari slider UI Anda.
        transform.localRotation = Quaternion.Euler(90f, 0f, _rotationZ);

        float finalScaleX = _runtimeAspectScale.x * _scaleMultiplier * _flipX;
        float finalScaleY = _runtimeAspectScale.y * _scaleMultiplier * _flipY;
        transform.localScale = new Vector3(finalScaleX, finalScaleY, 1f);

        _meshRenderer.GetPropertyBlock(_propBlock);
        _propBlock.SetFloat("_Intensity", _intensity);
        _meshRenderer.SetPropertyBlock(_propBlock);
    }
}