using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EyePass : ScriptableRenderPass
{
    private EyeFeature.PassSettings settings;

    private RTHandle colorBuffer, pixelBuffer;
    private int pixelBufferID = Shader.PropertyToID("_PixelBuffer");

    private Material material;

    public EyePass(EyeFeature.PassSettings settings)
    {
        this.settings = settings;
        if (material == null)
        {
            if (settings.eyeShader == null)
                material = CoreUtils.CreateEngineMaterial("Shader Graphs/CameraTest");
            else
                material = CoreUtils.CreateEngineMaterial(settings.eyeShader);
        }
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get();
        using (new ProfilingScope(cmd, new ProfilingSampler("Eye Pass")))
        {
            Blit(cmd, colorBuffer, pixelBuffer, material);
            Blit(cmd, pixelBuffer, colorBuffer);
        }

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        colorBuffer = renderingData.cameraData.renderer.cameraColorTargetHandle;
        RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;

        //pixelScreenHeight = settings.screenHeight;
        //pixelScreenWidth = (int)(pixelScreenHeight * renderingData.cameraData.camera.aspect + 0.5f);

        //material.SetVector("_BlockCount", new Vector2(pixelScreenWidth, pixelScreenHeight));
        //material.SetVector("_BlockSize", new Vector2(1.0f / pixelScreenWidth, 1.0f / pixelScreenHeight));
        //material.SetVector("_HalfBlockSize", new Vector2(0.5f / pixelScreenWidth, 0.5f / pixelScreenHeight));

        //descriptor.height = pixelScreenHeight;
        //descriptor.width = pixelScreenWidth;

        cmd.GetTemporaryRT(pixelBufferID, descriptor, FilterMode.Point);
        pixelBuffer = RTHandles.Alloc(new RenderTargetIdentifier(pixelBufferID));
    }

    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        if (cmd == null) throw new System.ArgumentNullException("cmd");
        cmd.ReleaseTemporaryRT(pixelBufferID);
    }
}
