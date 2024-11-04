using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private RenderTexture renderTexture;
    private Texture2D screenTexture;
    private Rect rectReadPicture;
    [SerializeField] private float gamma;
    [SerializeField] private Material eyeVisionMaterial;

    private float Luminance(Color pixel)
    {
        return 0.3f * pixel.r + 0.59f * pixel.g + 0.11f * pixel.b;
    }

    private float EnvLuminance()
    {
        RenderTexture old = RenderTexture.active;
        RenderTexture.active = renderTexture;
        screenTexture.ReadPixels(rectReadPicture, 0, 0);
        screenTexture.Apply();
        RenderTexture.active = old;

        Color[] pixels = screenTexture.GetPixels();
        float[] es = new float[pixels.Length];

        float sum = 0f;
        float invHalfTexelWidthM1 = 2f / (screenTexture.width - 1f);
        float invHalfTexelHeightM1 = 2f / (screenTexture.height - 1f);
        float inv2GammaSqr = 1f / (2f * gamma * gamma);
        for (int x = 0; x < screenTexture.width; ++x)
        {
            float a = x * invHalfTexelWidthM1;
            for (int y = 0; y < screenTexture.height; ++y)
            {
                float b = y * invHalfTexelHeightM1;
                float e = es[x + y * screenTexture.width] = Mathf.Exp(-(a * a + b * b) * inv2GammaSqr);
                sum += e;
            }
        }
        float invSum = 1f / sum;

        float envLum = 0f;
        for (int x = 0; x < screenTexture.width; ++x)
        {
            for (int y = 0; y < screenTexture.height; ++y)
            {
                envLum += (es[x + y * screenTexture.width] * Luminance(pixels[x + y * screenTexture.width])) * invSum;
            }
        }

        return envLum;
    }

    private void Start()
    {
        screenTexture = new Texture2D(renderTexture.width, renderTexture.height);
        rectReadPicture = new(0, 0, screenTexture.width, screenTexture.height);
        if (ValuesManager.instance != null)
        {
            if (ValuesManager.instance.valuesBG.activeSelf)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                eyeVisionMaterial.SetFloat("_Sfera", ValuesManager.SferaValue.SliderValue);
                eyeVisionMaterial.SetFloat("_Cylinder", ValuesManager.CylinderValue.SliderValue);
                eyeVisionMaterial.SetFloat("_Os", ValuesManager.OsValue.SliderValue);
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            //eyeVisionMaterial.SetFloat("DistantFactor", EnvLuminance());
        }
    }

    void Update()
    {
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
            }
        }

        //eyeVisionMaterial.SetFloat("DistantFactor", EnvLuminance());
    }
}
