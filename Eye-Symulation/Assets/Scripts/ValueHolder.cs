using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class ValueHolder : MonoBehaviour
{
    [Header("Data:")]
    public string valueName;
    public string unitName;
    public float minValue;
    public float maxValue;
    public float startValue;

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
            minText.text = unitName != "" ? new StringBuilder(minValue.ToString()).Append(' ').Append(unitName).ToString() : minValue.ToString();
        }
        SliderMin = minValue;

        if (maxText != null)
        {
            maxText.text = unitName != "" ? new StringBuilder(maxValue.ToString()).Append(' ').Append(unitName).ToString() : maxValue.ToString();
        }
        SliderMax = maxValue;

        startValue = Mathf.Clamp(startValue, minValue, maxValue);
        SliderValue = startValue;

        if (valueText != null)
        {
            valueText.text = unitName != "" ? new StringBuilder(SliderValue.ToString()).Append(' ').Append(unitName).ToString() : SliderValue.ToString();
        }
    }

    void Awake()
    {
        if (valueSlider != null)
        {
            SliderValue = startValue;
        }
    }

    void Update()
    {
        if (valueText != null)
        {
            valueText.text = unitName != "" ? new StringBuilder(SliderValue.ToString()).Append(' ').Append(unitName).ToString() : SliderValue.ToString();
        }
    }
}
