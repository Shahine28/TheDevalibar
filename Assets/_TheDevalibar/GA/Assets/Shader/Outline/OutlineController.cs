// OutlineController.cs
using UnityEngine;

[ExecuteAlways] // 允许在编辑模式下运行，方便调试
public class OutlineController : MonoBehaviour
{
    [Header("Outline Settings")]
    [SerializeField] private Color _outlineColor = Color.white; // 描边颜色
    [SerializeField, Range(0.0f, 0.1f)] private float _outlineThickness = 0.01f; // 描边厚度

    [Header("Noise Settings")]
    [SerializeField] private Texture2D _noiseTex; // 噪声纹理
    [SerializeField, Range(0.0f, 10.0f)] private float _noiseSpeed = 1.0f; // 噪声动画速度
    [SerializeField, Range(0.0f, 1.0f)] private float _noiseIntensity = 0.5f; // 噪声强度

    private Material _outlineMaterial; // 描边材质

    void OnEnable()
    {
        // 获取或创建描边材质
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            // 查找名为 "Custom/NightSoulOutline" 的着色器
            Shader outlineShader = Shader.Find("Custom/NightSoulOutline");
            if (outlineShader != null)
            {
                // 如果已经有描边材质，则使用它，否则创建一个新的
                // 确保描边材质是独立的实例，而不是共享的
                if (_outlineMaterial == null || _outlineMaterial.shader != outlineShader)
                {
                    _outlineMaterial = new Material(outlineShader);
                }
                // 将描边材质添加到渲染器的额外材质列表中，确保它在原始材质之后渲染
                // 或者，在URP Custom Render Feature中直接使用此材质进行渲染
                // 为了本例的简单性，我们假设它会被Custom Render Feature使用
            }
            else
            {
                Debug.LogError("Shader 'Custom/NightSoulOutline' not found! Make sure the shader file exists and is correctly named.");
            }
        }
        else
        {
            Debug.LogError("No Renderer found on this GameObject. OutlineController requires a Renderer component.");
        }
    }

    void OnDisable()
    {
        // 在禁用时清理材质，防止内存泄漏
        if (_outlineMaterial != null)
        {
            DestroyImmediate(_outlineMaterial); // 在编辑器模式下使用 DestroyImmediate
            _outlineMaterial = null;
        }
    }

    void Update()
    {
        // 实时更新材质属性
        if (_outlineMaterial != null)
        {
            _outlineMaterial.SetColor("_OutlineColor", _outlineColor);
            _outlineMaterial.SetFloat("_OutlineThickness", _outlineThickness);
            _outlineMaterial.SetTexture("_NoiseTex", _noiseTex);
            _outlineMaterial.SetFloat("_NoiseSpeed", _noiseSpeed);
            _outlineMaterial.SetFloat("_NoiseIntensity", _noiseIntensity);
        }
    }

    // 提供一个公共方法来获取描边材质，供 Custom Render Feature 使用
    public Material GetOutlineMaterial()
    {
        return _outlineMaterial;
    }
}