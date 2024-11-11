using System;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private RenderTexture cameraRenderTexture;
    private Texture2D screenTexture;
    private Color[] texturePixels;
    private int textureWidth;
    private int textureHeight;
    private Rect rectReadPicture;
    [SerializeField] private bool calculateLenRadius = true;
    [SerializeField] private RenderTexture computeRenderTexture;
    [SerializeField] private ComputeShader computeShader;
    [SerializeField] private float gamma;
    [SerializeField] private Material eyeVisionMaterial;

    private readonly ThreadManager<float> pupilSizeThread = new();
    private float pupilSize;
    private bool finished = true;

    private float EnvLuminance()
    {
        float sum = 0f;
        for (int x = 0; x < textureWidth; ++x)
        {
            for (int y = 0; y < textureHeight; ++y)
            {
                sum += texturePixels[x + y * textureWidth].g;
            }
        }
        float invSum = 1f / sum;

        float envLum = 0f;
        for (int x = 0; x < textureWidth; ++x)
        {
            for (int y = 0; y < textureHeight; ++y)
            {
                envLum += texturePixels[x + y * textureWidth].b * invSum;
            }
        }

        return envLum;
    }

    private float PupilSize()
    {
        float logL = Mathf.Log10(EnvLuminance());
        return (5.697f - 0.658f * logL + 0.007f * logL * logL) / 10f;
    }

    private void CalculateLensRadius()
    {
        if (!calculateLenRadius) return;
        if (!finished) return;
        finished = false;
        pupilSizeThread.RunNewTask(PupilSize, (float pupilSize) =>
        {
            this.pupilSize = pupilSize;
            finished = true;
        });
    }

    private void Start()
    {
        if (calculateLenRadius)
        {
            computeRenderTexture = new(cameraRenderTexture.width, cameraRenderTexture.height, 24)
            {
                enableRandomWrite = true
            };
            computeRenderTexture.Create();

            computeShader.SetTexture(0, "Input", cameraRenderTexture);
            computeShader.SetTexture(0, "Result", computeRenderTexture);
            computeShader.SetVector("Resolution", new Vector2(cameraRenderTexture.width, cameraRenderTexture.height));
            computeShader.SetFloat("Gamma", gamma);

            screenTexture = new Texture2D(cameraRenderTexture.width, cameraRenderTexture.height);
            rectReadPicture = new(0, 0, screenTexture.width, screenTexture.height);
            texturePixels = screenTexture.GetPixels();
            textureWidth = screenTexture.width;
            textureHeight = screenTexture.height;
            eyeVisionMaterial.SetFloat("_LensRadius", pupilSize);
        }

        if (ValuesManager.instance != null)
        {
            if (ValuesManager.instance.valuesBG.activeSelf)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                eyeVisionMaterial.SetFloat("_Sfera", ValuesManager.SferaValue.SliderValue);
                eyeVisionMaterial.SetFloat("_Cylinder", ValuesManager.CylinderValue.SliderValue);
                eyeVisionMaterial.SetFloat("_Os", ValuesManager.OsValue.SliderValue);
                eyeVisionMaterial.SetFloat("_AngleFactor", ValuesManager.AngleFactorValue.SliderValue);
                eyeVisionMaterial.SetFloat("_DistantFactor", ValuesManager.DistantFactorValue.SliderValue);
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        CalculateLensRadius();
    }

    void Update()
    {
        pupilSizeThread.Update();
        if (finished && calculateLenRadius)
        {
            computeShader.Dispatch(0, computeRenderTexture.width / 8, computeRenderTexture.height / 8, 1);

            RenderTexture old = RenderTexture.active;
            RenderTexture.active = computeRenderTexture;
            screenTexture.ReadPixels(rectReadPicture, 0, 0);
            screenTexture.Apply();
            RenderTexture.active = old;

            texturePixels = screenTexture.GetPixels();
            eyeVisionMaterial.SetFloat("_LensRadius", pupilSize);
        }

        if (ValuesManager.instance != null)
        {
            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                ValuesManager.instance.valuesBG.SetActive(!ValuesManager.instance.valuesBG.activeSelf);

                if (ValuesManager.instance.valuesBG.activeSelf)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }

            if (ValuesManager.instance.valuesBG.activeSelf)
            {
                eyeVisionMaterial.SetFloat("_Sfera", ValuesManager.SferaValue.SliderValue);
                eyeVisionMaterial.SetFloat("_Cylinder", ValuesManager.CylinderValue.SliderValue);
                eyeVisionMaterial.SetFloat("_Os", ValuesManager.OsValue.SliderValue);
                eyeVisionMaterial.SetFloat("_AngleFactor", ValuesManager.AngleFactorValue.SliderValue);
                eyeVisionMaterial.SetFloat("_DistantFactor", ValuesManager.DistantFactorValue.SliderValue);
            }
        }

        CalculateLensRadius();
    }
}
