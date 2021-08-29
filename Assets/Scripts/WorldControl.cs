using UnityEngine;
using System;
using UnityEngine.UI;

public class WorldControl : MonoBehaviour
{
	[SerializeField] private GameObject m_mainGUI;
	[SerializeField] private Transform m_playerTransform;
	[SerializeField] private AudioSource m_musicAudioSource;
	[SerializeField] private AudioSource m_playerAudioSource;
	[SerializeField] private AudioSource m_soundAudioSource;

	public event Action OnScoreUpdated;
	public event Action<string> OnMessage;
	public event Action<GameObject> OnRocketCreated;

	public bool IsInGame { get; private set; }

	public const float WIN_TIMER = 2.0f;
	private float timer_time;
	private bool m_isTimerStarted = false;
	private bool m_isWinTimer;

	public const string ROCKET_CAMERA_NAME = "rocket_camera";
	public const string PLAYER_OBJECT_NAME = "player";
	public const string MESSAGE_OBJECT_NAME = "message";
	public const string SCORE_OBJECT_NAME = "score_gui";
	public const string START_LOCATION_NAME = "start_location";
	public const string END_LOCATION_NAME = "end_location";
	public const string COLLECTIBLES_TAG = "Collect";
	public const string ROCKET_LANDING_TAG = "rocket_landing";
	public const string ROCKET_TAG = "Rocket";
	public const string ENGINE_TAG = "Engine";
	public const float MAX_GAME_SPEED = 5.0f;
	public const float MIN_GAME_SPEED = 0.0f;
	public const float MAX_SOUND_VOLUME = 1.0f;
	public const float MIN_SOUND_VOLUME = 0.0f;

	private int m_currentMapIdx = 0;
	private Transform thisTransform;
	private bool m_isPaused;
	private float m_gameSpeed;
	private float m_musicVolume = 0.8f;
	private float m_fxVolume = 1.0f;

	private string[] m_rocketsNames;
	private string[] m_enginesNames;
	private string[] m_audioNames;

	private GameObject m_mapPrefab;
	private GameObject m_map;
	private MapInfo m_mapInfo;
	private GameObject m_rocket;
	private RocketControl m_rocketControl;
	public MapList MapList { get; private set; }
	public SoundController SoundController { get; private set; }
	private static WorldControl m_instance;

	private void Awake()
	{
		if (m_instance != null)
		{
			Debug.Assert(m_instance == this, "Second DialogsController insance");
			return;
		}

		m_instance = this;
		MapList = GetComponent<MapList>();
		SoundController = GetComponent<SoundController>();
	}

	private void OnDestroy()
	{
		if (m_instance == this)
		{
			m_instance = null;
		}
	}

	private void Start () 
	{
		SetAllLists();
		GameProgress.Load();
		thisTransform = transform;
		Init();
	}

	public static WorldControl GetInstance()
	{
		return m_instance;
	}

	public void Init()
	{
		Localization.selectLanguage ("RU");
		m_gameSpeed = 1.0f;
		chooseRocket (m_rocketsNames[0]);
		chooseEngine (m_enginesNames[0]);
		PlayMusic (Resources.Load (AudioList.getAudioPath (getAudioNames () [0])) as AudioClip, 1.5f);
		if (!GameProgress.buyedRockets.ContainsKey(m_rocketControl.rocketName))
		{
			GameProgress.setBuyedRocket(m_rocketControl.rocketName, m_rocketControl.m_info);
		}
		if (!GameProgress.buyedEngines.ContainsKey(m_rocketControl.getEngineControl().engineName))
		{
			GameProgress.setBuyedEngine(m_rocketControl.getEngineControl().engineName, m_rocketControl.getEngineControl().m_info);
		}

		SetPause(true);
	}

	private void InstantiateRocket(GameObject rocket)
	{
		m_rocket = GameObject.Instantiate<GameObject>(rocket, m_playerTransform);
		m_rocketControl = m_rocket.GetComponent<RocketControl> ();
		OnRocketCreated?.Invoke(m_rocket);

		LoadRocketParameners();
	}

	public void chooseRocket(string rocketName)
	{	
		GameObject rocket = RocketList.GetInstance().GetRocketPrefab(rocketName);
		InstantiateRocket (rocket);
		m_rocketControl.rocketName = rocketName;
	}

	public void chooseRocket(GameObject rocket)
	{
		InstantiateRocket(rocket);
	}

	public void chooseEngine(string engineName)
	{	
		m_rocketControl.setEngine(RocketList.GetInstance().GetEnginePrefab(engineName));
		m_rocketControl.getEngineControl ().engineName = engineName;
	}
	
	public void chooseEngine(GameObject engine)
	{
		m_rocketControl.setEngine(engine);
	}

	public void LoadRocketParameners()
	{
		if (GameProgress.buyedRockets.ContainsKey(m_rocketControl.rocketName))
		{
			m_rocketControl.m_info = GameProgress.buyedRockets[m_rocketControl.rocketName];
		}
		GetRocket().GetComponent<Rigidbody2D>().mass = m_rocketControl.m_info.mass;
	}

	public void SetMapPhysicParametres()
	{
		if (m_mapInfo)
		{
			GetRocket().GetComponent<Rigidbody2D>().angularDrag = GetRocket().GetComponent<Rigidbody2D>().drag = m_mapInfo.m_mapDrag;
			GetRocket().GetComponent<Rigidbody2D>().gravityScale = m_mapInfo.m_mapGravity;

			var material = new PhysicsMaterial2D();
			material.bounciness = m_mapInfo.m_mapBounciness;
			material.friction = m_mapInfo.m_mapFriction;
			Collider2D rocketLanding = GetRocket().GetComponent<RocketControl>().rocketLanding;
			rocketLanding.sharedMaterial = material;
			GetRocket().GetComponent<Collider2D>().sharedMaterial = material;
		}
	}

//========================== Level loaders ===============================================
	public void LoadCustomLevel(GameObject map)
	{
		if (m_map != null)
		{
			EndLevel();
		}

		m_mapPrefab = map;
		m_currentMapIdx = -1;
		LoadSelectedLevel();
	}

	private void LoadSelectedLevel()
	{
		m_map = GameObject.Instantiate (m_mapPrefab) as GameObject; //Загрузим карту
		m_mapInfo = m_map.GetComponent<MapInfo> ();
		m_map.transform.parent = thisTransform; //Удочерим ее
		SetMapPhysicParametres();
		GetRocket().transform.position = GameObject.Find(START_LOCATION_NAME).transform.position; // Поставим ракету на начельную позицию на карте
		GetRocket().SetActive(true);// .GetComponent<SpriteRenderer>().enabled = true;
		m_rocket.GetComponent<Rigidbody2D>().isKinematic = false;
		m_rocketControl.Reset();
		SetPause(false);
		IsInGame = true;
	}

	public void EndLevel()
	{//Уровень закончен - уничтожим карту и ракету
		ClearTempScore();
		if (m_map)
		{
			Destroy(m_map.gameObject);
		}
		m_rocket.transform.localRotation = Quaternion.identity;
		m_rocket.transform.localPosition = new Vector3(0,0,0);
		m_rocketControl.b_isAlive = false;
		m_rocket.GetComponent<Rigidbody2D>().isKinematic = true;
		m_rocket.SetActive(false);// .GetComponent<SpriteRenderer>().enabled = false;
	}

	public void LoadLevel(int levelIdx)
	{
		var mapPaths = MapList.GetMapPaths(levelIdx);
		if (mapPaths == null)
		{
			return;
		}

		if (m_map != null)
		{
			EndLevel();
		}

		m_currentMapIdx = levelIdx;
		m_mapPrefab = mapPaths.GetMapPrefab();
		LoadSelectedLevel();
	}

	public void LoadNextLevel()
	{
		EndLevel();

		if (m_currentMapIdx < MapList.Count - 1)
		{
			++m_currentMapIdx;
		}
		else
		{
			DialogsController.GetInstance().ShowDialog(DialogType.MapSelectorMenu);
			return;
		}

		var mapPahs = MapList.GetMapPaths(m_currentMapIdx);
		if (mapPahs == null)
		{
			return;
		}

		MapInfo map_info = (mapPahs.GetMapPrefab()).GetComponent<MapInfo>();
		map_info.m_mapName = mapPahs.Name;
		map_info.m_mapIcon = mapPahs.GetMapIcon();
		GameProgress.SetMap(m_currentMapIdx, false);

		DialogsController.GetInstance().ShowMapDescriptionMenu(m_currentMapIdx);
	}

	public void RestartLevel()
	{
		EndLevel();
		LoadSelectedLevel();
	}

//=================================================================================== Map loaders

	public string[] getRocketsNames()
	{
		return m_rocketsNames;
	}

	public string[] getEnginesNames()
	{
		return m_enginesNames;
	}

	public string[] getAudioNames()
	{
		return m_audioNames;
	}

	public void SetPause(bool isPause)
	{
		m_isPaused = isPause;
		Time.timeScale = isPause? 0.0f : m_gameSpeed;

		if (IsInGame)
		{
			if (isPause)
			{
				DialogsController.GetInstance().ShowDialog(DialogType.PauseMenu);
			}
			else
			{
				DialogsController.GetInstance().CloseAllDialogs();
			}
		}
	}

	public bool IsPaused()
	{
		return m_isPaused;
	}

	public void setGameSpeed(float speed)
	{
		Mathf.Clamp (speed, MIN_GAME_SPEED, MAX_GAME_SPEED);
		m_gameSpeed = speed;
		Time.timeScale = m_gameSpeed;
	}

	public float getGameSpeed()
	{
		return m_gameSpeed;
	}

	public void SetMusicVolume(float volume)
	{
		SoundController.MusicVolume = volume;
		m_musicVolume = Mathf.Clamp(volume, MIN_SOUND_VOLUME, MAX_SOUND_VOLUME);
		if (m_musicVolume < 0.05f)
		{
			m_musicAudioSource.enabled = false;
		}
		else
		{
			m_musicAudioSource.volume = m_musicVolume;
			if (!m_musicAudioSource.enabled)
			{
				m_musicAudioSource.enabled = true;
				if (m_musicAudioSource.clip != null)
				{
					m_musicAudioSource.Play();
				}
			}
		}
	}

	public void SetFXVolume(float volume)
	{
		SoundController.SoundVolume = volume;
		m_fxVolume = Mathf.Clamp(volume, MIN_SOUND_VOLUME, MAX_SOUND_VOLUME);
		if (m_fxVolume < 0.05f)
		{
			m_playerAudioSource.enabled = false;
			m_soundAudioSource.enabled = false;
		}
		else
		{
			m_playerAudioSource.volume = m_fxVolume;
			m_soundAudioSource.volume = m_fxVolume;
			m_playerAudioSource.enabled = true;
			m_soundAudioSource.enabled = true;
		}
	}

	public float GetFXVolume()
	{
		return SoundController.SoundVolume;
	}

	public float GetMusicVolume()
	{
		return SoundController.MusicVolume;
	}

	public GameObject GetRocket()
	{
		return m_rocket;
	}

	public RocketControl getRocketControl()
	{
		return m_rocketControl;
	}

	public GameObject getMap()
	{
		return m_map;
	}

	public int GetCurrentLevelIdx()
	{
		return m_currentMapIdx;
	}

	public void SetCurrentLevelIdx(int mapIdx)
	{
		m_currentMapIdx = mapIdx;
	}

	public MapInfo GetMapInfo()
	{
		return m_mapInfo;
	}

	public void StartTimer(float secons, bool isWinTimer)
	{
		m_isTimerStarted = true;
		timer_time = secons;
		m_isWinTimer = isWinTimer;
		OnStartTimer(m_isWinTimer);
	}

	public void StopTimer()
	{
		m_isTimerStarted = false;
	}

	private void OnStartTimer(bool isWinTimer)
	{
		if (isWinTimer)
		{
			OnMessage?.Invoke(Localization.T_WIN_MESSAGE + " (" + string.Format("{0:0.00}", Mathf.Round (timer_time*100.0f)/100.0f) + ")");
		}
		else
		{
			OnMessage?.Invoke(Localization.T_LOSE_MESSAGE + ((int)((m_rocketControl.cost+m_rocketControl.getEngineControl().cost)/2)).ToString());
		}
	}

	private void OnTimerTick(bool isWinTimer)
	{
		if (isWinTimer)
		{
			string timeLeft = string.Format("{0:0.00}", Mathf.Round (timer_time*100.0f)/100.0f);
			OnMessage?.Invoke(Localization.T_WIN_MESSAGE + " (" + timeLeft + ")");
		}
	}

	private void OnStopTimer(bool isWinTimer)
	{
		if (isWinTimer)
		{
			SaveScore();
			m_soundAudioSource.Stop();
			GameProgress.SetMap(m_currentMapIdx, true);
			LoadNextLevel();
		}
		else
		{
			RestartLevel();
		}
	}

	public void PlayMusic(AudioClip music, float pitch = 1.0f)
	{
		m_musicAudioSource.clip = music;
		m_musicAudioSource.pitch = pitch;
		m_musicAudioSource.Play();
	}

	public void PlayNextMusic(float pitch = 1.0f)
	{
		bool isNext = false;
		foreach (string name in m_audioNames)
		{
			if (isNext) 
			{
				PlayMusic(Resources.Load (AudioList.getAudioPath (name)) as AudioClip, pitch);
				return;
			}
			if (name == AudioList.currentAudioName) isNext = true;
		}
		PlayMusic(Resources.Load (AudioList.getAudioPath (getAudioNames () [0])) as AudioClip, pitch);
	}

	public void PlayOneShotFX(AudioClip audioFX)
	{
		m_playerAudioSource.PlayOneShot(audioFX, GetFXVolume());
	}

	public void PlayConstFX(AudioClip audioFX)
	{
		m_soundAudioSource.clip = audioFX;
		m_soundAudioSource.Play();
	}

	public void StopConstFX()
	{
		m_soundAudioSource.Stop();
	}

	public bool IsFXSoundPlaying()
	{
		return m_soundAudioSource.isPlaying || m_playerAudioSource.isPlaying;
	}

	void Update()
	{
		if (m_isTimerStarted)
		{
			timer_time -= Time.deltaTime;
			if (timer_time <= 0)
			{
				StopTimer();
				OnStopTimer (m_isWinTimer);
			}
			OnTimerTick(m_isWinTimer);
		}

		if (IsInGame && Input.GetKeyDown(KeyCode.Escape))
		{
			SetPause(!m_isPaused); 
		}
	}

	public void HideScore()
	{
		m_mainGUI.SetActive(false);
	}
	
	public void ShowScore()
	{
		m_mainGUI.SetActive(true);
	}
	
	public void AddScore(int score)
	{
		GameProgress.tempScore += score;
		OnScoreUpdated?.Invoke();
	}
	
	public void SaveScore()
	{
		GameProgress.score += GameProgress.tempScore;
		GameProgress.tempScore = 0;
		OnScoreUpdated?.Invoke();
	}

	public void ClearTempScore()
	{
		GameProgress.tempScore = 0;
		OnScoreUpdated?.Invoke();
	}
	

	void SetAllLists()
	{
		//Rockets
		m_rocketsNames = RocketList.GetInstance().GetRocketNamesList();
		m_enginesNames = RocketList.GetInstance().GetEngineNamesList();

		//Audio
		m_audioNames = new string[5] {"DontWorry", "DerWienerWalzer", "HevenAndHell", "BeeFly", "ValkyriFly"};
		AudioList.addAudio(m_audioNames[0], "Music/Bobby McFerrin - Don't Worry, Be Happy");
		AudioList.addAudio(m_audioNames[1], "Music/Johann Strauss - Der Wiener Walzer");
		AudioList.addAudio(m_audioNames[2], "Music/Вильгельм Рихард Вагнер - Воплощение ада и рая");
		AudioList.addAudio(m_audioNames[3], "Music/Римский-Корсаков - Полет шмеля");
		AudioList.addAudio(m_audioNames[4], "Music/Рихард Вагнер - Полет валькирий");
	}
}
