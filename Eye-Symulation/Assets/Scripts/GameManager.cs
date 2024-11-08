using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private RenderTexture cameraRenderTexture;
    private Texture2D screenTexture;
    private Rect rectReadPicture;
    [SerializeField] private RenderTexture computeRenderTexture;
    [SerializeField] private ComputeShader computeShader;
    [SerializeField] private float gamma;
    [SerializeField] private Material eyeVisionMaterial;

    private readonly ThreadManager<float> pupilSizeThread = new();

    private float EnvLuminance()
    {
        Color[] pixels;
        int width, height;
        lock (screenTexture)
        {
            pixels = screenTexture.GetPixels();
            width = screenTexture.width;
            height = screenTexture.height;
        }

        float sum = 0f;
        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                sum += pixels[x + y * width].g;
            }
        }
        float invSum = 1f / sum;

        float envLum = 0f;
        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                envLum += pixels[x + y * width].b * invSum;
            }
        }

        return envLum;
    }

    private float PupilSize()
    {
        float logL = Mathf.Log10(EnvLuminance());
        return 5.697f - 0.658f * logL + 0.007f * logL * logL;
    }

    private void CalculateDistantFactor()
    {
        pupilSizeThread.RunNewTask(PupilSize, (float pupilSize) =>
        {
            eyeVisionMaterial.SetFloat("DistantFactor", pupilSize);
        });
    }

    private void Start()
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

            //eyeVisionMaterial.SetFloat("DistantFactor", PupilSize());
            CalculateDistantFactor();
        }
    }

    void Update()
    {
        computeShader.Dispatch(0, computeRenderTexture.width / 8, computeRenderTexture.height / 8, 1);

        RenderTexture old = RenderTexture.active;
        RenderTexture.active = computeRenderTexture;
        screenTexture.ReadPixels(rectReadPicture, 0, 0);
        screenTexture.Apply();
        RenderTexture.active = old;

        pupilSizeThread.Update();
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

        //eyeVisionMaterial.SetFloat("DistantFactor", PupilSize());
        CalculateDistantFactor();
    }
}
