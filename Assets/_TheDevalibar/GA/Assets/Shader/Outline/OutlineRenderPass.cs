// OutlineRenderPass.cs
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

public class OutlineRenderPass : ScriptableRenderPass
{
    private List<Renderer> _outlineRenderers;
    private OutlineRenderFeature.OutlinePassSettings _settings;
    private Material _outlineMaterial; // 描边材质实例
    private FilteringSettings _filteringSettings;

    public OutlineRenderPass(OutlineRenderFeature.OutlinePassSettings settings)
    {
        _settings = settings;
        renderPassEvent = settings.renderPassEvent;
        _filteringSettings = new FilteringSettings(RenderQueueRange.opaque, settings.outlineLayer);
    }

    public void Setup(List<Renderer> renderers)
    {
        _outlineRenderers = renderers;
    }

    // 在执行渲染命令之前调用
    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        // 确保描边材质在每次渲染前更新
        if (_outlineRenderers != null && _outlineRenderers.Count > 0)
        {
            // 假设所有描边对象使用相同的描边材质属性
            // 实际项目中，您可能需要为每个对象获取其独立的材质
            OutlineController controller = _outlineRenderers[0].GetComponent<OutlineController>();
            if (controller != null)
            {
                _outlineMaterial = controller.GetOutlineMaterial();
            }
        }
    }

    // 执行渲染命令
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (_outlineMaterial == null || _outlineRenderers == null || _outlineRenderers.Count == 0)
        {
            return;
        }

        CommandBuffer cmd = CommandBufferPool.Get("NightSoulOutline");
        using (new ProfilingScope(cmd, new ProfilingSampler("NightSoulOutline")))
        {
            DrawingSettings drawingSettings = CreateDrawingSettings(
                new ShaderTagId("UniversalForward"), // 使用URP的默认着色器标签
                ref renderingData,
                SortingCriteria.RenderQueue);
            drawingSettings.overrideMaterial = _outlineMaterial; // 使用描边材质
            drawingSettings.overrideMaterialPassIndex = 0; // 使用描边着色器的第一个Pass

            // 遍历所有需要描边的渲染器并绘制
            foreach (Renderer renderer in _outlineRenderers)
            {
                if (renderer != null && renderer.gameObject.activeInHierarchy)
                {
                    // 绘制描边
                    cmd.DrawRenderer(renderer, _outlineMaterial, 0); // 0是SubShader的Pass索引
                }
            }
        }

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    // 在渲染通道结束时调用
    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        // 可以在这里进行一些清理工作，例如释放临时渲染目标
    }
}