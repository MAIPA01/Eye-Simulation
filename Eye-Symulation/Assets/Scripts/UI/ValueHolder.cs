using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using UnityEditor;

public class ValueHolder : MonoBehaviour
{
    [Header("Data:")]
    public string valueName = "%Name%";
    public string unitName = "";
    public string valueFormat = "0.00";
    public string minValueFormat = "0.00";
    public string maxValueFormat = "0.00";
    public float minValue = 0f;
    public float maxValue = 1f;
    public float startValue = 0f;

    [Header("UI:")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI valueText;
    public TextMeshProUGUI minText;
    public TextMeshProUGUI maxText;
    public Slider valueSlider;

    public float SliderValue
    {
        get
        {
            if (valueSlider != null)
            {
                return valueSlider.value;
            }
            return .0f;
        }
        set
        {
            if (SliderValue != value)
            {
                if (valueSlider != null)
                {
                    valueSlider.value = value;
                }

                if (valueText != null)
                {
                    valueText.text = unitName != "" ? new StringBuilder(SliderValue.ToString()).Append(' ').Append(unitName).ToString() : SliderValue.ToString();
                }
            }
        }
    }
    public float SliderMin
    {
        get
        {
            if (valueSlider != null)
            {
                return valueSlider.minValue;
            }
            return .0f;
        }
        set
        {
            if (SliderMin != value)
            {
                if (valueSlider != null)
                {
                    valueSlider.minValue = value;
                }
            }
        }
    }
    public float SliderMax
    {
        get
        {
            if (valueSlider != null)
            {
                return valueSlider.maxValue;
            }
            return .0f;
        }
        set
        {
            if (SliderMin != value)
            {
                if (valueSlider != null)
                {
                    valueSlider.maxValue = value;
                }
            }
        }
    }

    void OnValidate()
    {
        if (nameText != null)
        {
            nameText.text = valueName;
        }

        if (minText != null)
        {
            minText.text = unitName != "" ? new StringBuilder(minValue.ToString(minValueFormat)).Append(' ').Append(unitName).ToString() : minValue.ToString();
        }
        SliderMin = minValue;

        if (maxText != null)
        {
            maxText.text = unitName != "" ? new StringBuilder(maxValue.ToString(maxValueFormat)).Append(' ').Append(unitName).ToString() : maxValue.ToString();
        }
        SliderMax = maxValue;

        if (!EditorApplication.isPlaying)
        {
            SliderValue = startValue;
        }

        if (valueText != null)
        {
            valueText.text = unitName != "" ? new StringBuilder(SliderValue.ToString(valueFormat)).Append(' ').Append(unitName).ToString() : SliderValue.ToString();
        }
    }

    void Awake()
    {
        startValue = Mathf.Clamp(startValue, minValue, maxValue);
        if (valueSlider != null)
        {
            SliderValue = startValue;
        }
    }

    void Update()
    {
        if (valueText != null)
        {
            valueText.text = unitName != "" ? new StringBuilder(SliderValue.ToString(valueFormat)).Append(' ').Append(unitName).ToString() : SliderValue.ToString();
        }
    }
}
