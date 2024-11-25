using UnityEngine;

public class PassThroughModifier : MonoBehaviour
{
    [SerializeField] private OVRPassthroughLayer _passthroughLayer;

    private void Start()
    {
        if (_passthroughLayer == null)
        {
            Debug.LogError("PassthroughLayer is not assigned!");
        }
    }

    public void DecreaseBrightnessTo50Percent()
    {
        if (_passthroughLayer == null)
        {
            Debug.LogError("PassthroughLayer is not assigned!");
            return;
        }

        _passthroughLayer.SetBrightnessContrastSaturation(-0.5f, -0.5f, -0.5f);

        Debug.Log($"Brightness decreased to 50% - Contrast: {_passthroughLayer.colorMapEditorContrast}, Saturation: {_passthroughLayer.colorMapEditorPosterize}");
    }
}
