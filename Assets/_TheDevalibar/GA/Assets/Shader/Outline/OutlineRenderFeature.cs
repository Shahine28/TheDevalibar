// OutlineRenderFeature.cs
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

public class OutlineRenderFeature : ScriptableRendererFeature
{
    // 渲染通道的设置
    [System.Serializable]
    public class OutlinePassSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingOpaques; // 渲染时机
        public LayerMask outlineLayer; // 描边层级
    }

    public OutlinePassSettings settings = new OutlinePassSettings();

    private OutlineRenderPass _outlinePass;

    // 在编辑器中创建特性时调用
    public override void Create()
    {
        _outlinePass = new OutlineRenderPass(settings);
    }

    // 将渲染通道添加到渲染器中
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // 查找所有带有 OutlineController 脚本的对象
        List<Renderer> outlineRenderers = new List<Renderer>();
        OutlineController[] controllers = FindObjectsOfType<OutlineController>();
        foreach (OutlineController controller in controllers)
        {
            Renderer r = controller.GetComponent<Renderer>();
            if (r != null && ((1 << r.gameObject.layer) & settings.outlineLayer) != 0) // 检查是否在描边层级
            {
                outlineRenderers.Add(r);
            }
        }

        if (outlineRenderers.Count == 0)
        {
            return; // 没有需要描边的对象，不添加通道
        }

        _outlinePass.Setup(outlineRenderers);
        renderer.EnqueuePass(_outlinePass);
    }
}