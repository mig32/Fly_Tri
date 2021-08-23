using UnityEngine;

public class SoundController : MonoBehaviour
{
    private float m_soundVolume;
    private float m_musicVolume;
    private readonly string c_soundKey = "sound";
    private readonly string c_musicKey = "music";

    private float m_saveTimer = 0.0f;

    private void Save()
    {
        if (m_saveTimer > 0.0f)
        {
            return;
        }

        m_saveTimer = 1.0f;
    }

    private void Awake()
    {
        m_soundVolume = PlayerPrefs.GetFloat(c_soundKey, 0.5f);
        m_musicVolume = PlayerPrefs.GetFloat(c_musicKey, 0.5f);
    }

    private void Update()
    {
        if (m_saveTimer <= 0.0f)
        {
            return;
        }

        m_saveTimer -= Time.unscaledDeltaTime;
        if (m_saveTimer > 0.0f)
        {
            return;
        }

        PlayerPrefs.SetFloat(c_soundKey, SoundVolume);
        PlayerPrefs.Save();
        PlayerPrefs.SetFloat(c_musicKey, MusicVolume);
        PlayerPrefs.Save();
    }

    public float SoundVolume 
    {
        get
        {
            return m_soundVolume;
        }
        set
        {
            if (m_soundVolume < 0.01f)
            {
                m_soundVolume = 0;
            }

            if (m_soundVolume == value)
            {
                return;
            }

            m_soundVolume = value;
            Save();
        }
    }

    public float MusicVolume
    {
        get
        {
            return m_musicVolume;
        }
        set
        {
            if (m_musicVolume < 0.01f)
            {
                m_musicVolume = 0;
            }

            if (m_musicVolume == value)
            {
                return;
            }

            m_musicVolume = value;
            Save();
        }
    }
}
