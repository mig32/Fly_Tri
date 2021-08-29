using UnityEngine;

public class SoundController : MonoBehaviour
{
    [SerializeField] private AudioSource m_musicAudioSource;
    [SerializeField] private AudioSource m_shortSoundAudioSource;
    [SerializeField] private AudioSource m_engineAudioSource;
    [SerializeField] private float m_soundVolume;
    [SerializeField] private float m_musicVolume;

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
        SoundVolume = PlayerPrefs.GetFloat(c_soundKey, m_soundVolume);
        MusicVolume = PlayerPrefs.GetFloat(c_musicKey, m_musicVolume);
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

        PlayerPrefs.SetFloat(c_soundKey, m_soundVolume);
        PlayerPrefs.SetFloat(c_musicKey, m_musicVolume);
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

            m_soundVolume = Mathf.Clamp(value, 0.0f, 1.1f);
            Save();

            var isEnabled = m_soundVolume > 0.05f;
            m_shortSoundAudioSource.enabled = isEnabled;
            m_engineAudioSource.enabled = isEnabled;

            if (isEnabled)
            {
                m_shortSoundAudioSource.volume = m_soundVolume;
                m_engineAudioSource.volume = m_soundVolume;
            }
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

            m_musicVolume = Mathf.Clamp(value, 0.0f, 1.1f);
            Save();

            bool wasEnabled = m_musicAudioSource.enabled;
            m_musicAudioSource.enabled = m_musicVolume > 0.05f;
            if (m_musicAudioSource.enabled)
            {
                m_musicAudioSource.volume = m_musicVolume;
                if (!wasEnabled && m_musicAudioSource.clip != null)
                {
                    m_musicAudioSource.Play();
                }
            }
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        m_musicAudioSource.clip = clip;
        m_musicAudioSource.Play();
    }

    public void PlayShortSound(AudioClip clip)
    {
        m_shortSoundAudioSource.PlayOneShot(clip);
    }

    public void SetEngineAudioClip(AudioClip clip)
    {
        m_engineAudioSource.clip = clip;
    }

    public void PlayEngineSound()
    {
        if (m_engineAudioSource.clip)
        {
            m_engineAudioSource.Play();
        }
    }

    public void StopEngineSound()
    {
        m_engineAudioSource.Stop();
    }

    public bool IsEngineSoundPlaying()
    {
        return m_engineAudioSource.isPlaying;
    }
}
