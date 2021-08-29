using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HpController : MonoBehaviour
{
    [SerializeField] private Image m_imageHPBarBg;
    [SerializeField] private Image m_imageHPBar;
    [SerializeField] private TMP_Text m_textHPValue;
    [SerializeField] private Color m_colorFull;
    [SerializeField] private Color m_colorEmpty;

    private float m_maxValue;

    private void Start()
    {
        WorldControl.GetInstance().OnHPChanged += OnValueChanged;
        WorldControl.GetInstance().OnRocketCreated += OnRocketCreated;

        var rocket = WorldControl.GetInstance().GetRocket();
        if (rocket)
        {
            OnRocketCreated(rocket);
        }
    }

    public void OnRocketCreated(GameObject rocket)
    {
        var rocketControl = rocket.GetComponent<RocketControl>();
        m_maxValue = rocketControl.HealthMax;
        OnValueChanged(rocketControl.HealthCurrent);
    }

    private int ValueForGui(float value)
    {
        return Mathf.RoundToInt(value * 1000);
    }

    private void OnValueChanged(float value)
    {
        m_textHPValue.text = $"{ValueForGui(Mathf.Clamp(value, 0.0f, m_maxValue))}/{ValueForGui(m_maxValue)}";

        if (m_maxValue == 0)
        {
            SetProgress(1.0f);
        }
        else
        {
            SetProgress(Mathf.Clamp(value/m_maxValue, 0.0f, 1.0f));
        }
    }

    private void SetProgress(float percent)
    {
        m_imageHPBar.color = Color.Lerp(m_colorEmpty, m_colorFull, percent);

        var size = m_imageHPBar.rectTransform.sizeDelta;
        size.x = m_imageHPBarBg.rectTransform.sizeDelta.x * percent;
        m_imageHPBar.rectTransform.sizeDelta = size;
    }
}
