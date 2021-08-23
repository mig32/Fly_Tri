using UnityEngine;
using System;
using UnityEngine.UI;

public class WorldControl : MonoBehaviour
{
	[SerializeField] private GameObject m_mainGUI;
	public event Action OnScoreUpdated;

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

	public Menu m_mainMenu;

	private int m_currentMapIdx = 0;
	private Transform thisTransform;
	private bool b_isPauseOn;
	private float gameSpeed;
	private float m_musicVolume = 0.8f;
	public float m_fxVolume = 1.0f;

	private string[] m_rocketsNames;
	private string[] m_enginesNames;
	private string[] m_audioNames;

	private GameObject m_mapPrefab;
	private GameObject m_map;
	private MapInfo m_mapInfo;
	private GameObject m_rocket;
	private GameObject m_player;
	private RocketControl m_rocketControl;
	private GameObject m_camera;
	private GameObject m_guiText;
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
		m_player = GameObject.Find(PLAYER_OBJECT_NAME).gameObject;
		m_camera = GameObject.Find(ROCKET_CAMERA_NAME).gameObject; //Камера, которая будет следить за ракетой
		m_guiText = GameObject.Find(MESSAGE_OBJECT_NAME).gameObject; //guiText для отображения сообщений
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
		init();
		m_mainMenu = gameObject.GetComponent<Menu>();
	}

	public static WorldControl GetInstance()
	{
		return m_instance;
	}

	public void init()
	{
		Localization.selectLanguage ("RU");
		gameSpeed = 1.0f;
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

		Pause();
	}

	private void instantiateRocket(GameObject rocket)
	{
		GameObject temp_rocket = GameObject.Instantiate (rocket) as GameObject;
		if (m_rocket) GameObject.Destroy(m_rocket);
		m_rocket = temp_rocket; //Загрузим ракету игрока
		m_rocket.transform.parent = GameObject.Find(PLAYER_OBJECT_NAME).transform; //Сделаем ее ребенком "player"
		m_rocketControl = m_rocket.GetComponent<RocketControl> ();

		CameraControl cameraScript = m_camera.GetComponent<CameraControl> ();
		cameraScript.setTargetTransform (m_rocket.transform); //Укажем скрипту камеры за каким объектом следить
		
		LoadRocketParameners ();
	}

	public void chooseRocket(string rocketName)
	{	
		GameObject rocket = RocketList.GetInstance().GetRocketPrefab(rocketName);
		instantiateRocket (rocket);
		m_rocketControl.rocketName = rocketName;
	}
	public void chooseRocket(GameObject rocket)
	{
		instantiateRocket (rocket);
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
			Collider2D rocketLanding = GetRocket().GetComponent<RocketControl>().rocketLanding;
			if (!rocketLanding.sharedMaterial)
			{
				rocketLanding.sharedMaterial = new PhysicsMaterial2D();
			}
			rocketLanding.sharedMaterial.bounciness = m_mapInfo.m_mapBounciness;
			rocketLanding.sharedMaterial.friction = m_mapInfo.m_mapFriction;
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
		SetMapPhysicParametres ();
		GetRocket().transform.position = GameObject.Find(START_LOCATION_NAME).transform.position; // Поставим ракету на начельную позицию на карте
		GetRocket().SetActive(true);// .GetComponent<SpriteRenderer>().enabled = true;
		m_rocket.GetComponent<Rigidbody2D>().isKinematic = false;
		m_rocketControl.Reset();
		UnPause();
	}

	public void EndLevel()
	{//Уровень закончен - уничтожим карту и ракету
		ClearTempScore();
		if (m_map) GameObject.Destroy(m_map.gameObject);
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

		bool isNext = m_currentMapIdx < MapList.Count;
		if (isNext)
		{
			++m_currentMapIdx;
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
		m_mainMenu.menuMapDiscription.initMenu(m_mainMenu, map_info);
		m_mainMenu.showMapDiscriptionMenu();
	}

	public void RestartLevel()
	{
		EndLevel();
		LoadSelectedLevel();
	}

//=================================================================================== Map loaders
	public void ShowMessage(string text, Color color)
	{//Большие буквы по центру экрана
		m_guiText.GetComponent<Text>().enabled = true;
		m_guiText.GetComponent<Text>().text = text;
		m_guiText.GetComponent<Text>().color = color;

	}

	public void HideMessage()
	{
		m_guiText.GetComponent<Text>().enabled = false;
	}

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

	public void Pause()
	{
		b_isPauseOn = true;
		Time.timeScale = 0.0f;
	}
	
	public void UnPause()
	{
		b_isPauseOn = false;
		Time.timeScale = gameSpeed;
	}

	public bool isPaused()
	{
		return b_isPauseOn;
	}

	public void setGameSpeed(float speed)
	{
		Mathf.Clamp (speed, MIN_GAME_SPEED, MAX_GAME_SPEED);
		gameSpeed = speed;
		Time.timeScale = gameSpeed;
	}

	public float getGameSpeed()
	{
		return gameSpeed;
	}

	public void SetMusicVolume(float volume)
	{
		SoundController.MusicVolume = volume;
		var audioSource = m_camera.GetComponent<AudioSource>();
		m_musicVolume = Mathf.Clamp(volume, MIN_SOUND_VOLUME, MAX_SOUND_VOLUME);
		if (m_musicVolume < 0.05f)
		{
			audioSource.enabled = false;
		}
		else
		{
			audioSource.volume = m_musicVolume;
			if (!audioSource.enabled)
			{
				audioSource.enabled = true;
				if (audioSource.clip != null)
				{
					audioSource.Play();
				}
			}
		}
	}

	public void SetFXVolume(float volume)
	{
		SoundController.SoundVolume = volume;
		var audioSourcePlayer = m_player.GetComponent<AudioSource>();
		var audioSourceMap = m_guiText.GetComponent<AudioSource>();
		m_fxVolume = Mathf.Clamp(volume, MIN_SOUND_VOLUME, MAX_SOUND_VOLUME);
		if (m_fxVolume < 0.05f)
		{
			audioSourcePlayer.enabled = false;
			audioSourceMap.enabled = false;
		}
		else
		{
			audioSourcePlayer.volume = m_fxVolume;
			audioSourceMap.volume = m_fxVolume;
			audioSourcePlayer.enabled = true;
			audioSourceMap.enabled = true;
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

	public GameObject GetCamera()
	{
		return m_camera;
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
		HideMessage();
	}

	private void OnStartTimer(bool isWinTimer)
	{
		if (isWinTimer)
		{
			ShowMessage(Localization.T_WIN_MESSAGE + " (" + string.Format("{0:0.00}", Mathf.Round (timer_time*100.0f)/100.0f) + ")", Color.green);
		}
		else
		{
			ShowMessage(Localization.T_LOSE_MESSAGE + ((int)((m_rocketControl.cost+m_rocketControl.getEngineControl().cost)/2)).ToString(), Color.red);
		}
	}

	private void OnTimerTick(bool isWinTimer)
	{
		if (isWinTimer)
		{
			string timeLeft = string.Format("{0:0.00}", Mathf.Round (timer_time*100.0f)/100.0f);
			m_guiText.GetComponent<Text>().text = Localization.T_WIN_MESSAGE + " (" + timeLeft + ")";
		}
	}

	private void OnStopTimer(bool isWinTimer)
	{
		if (isWinTimer)
		{
			SaveScore();
			m_guiText.GetComponent<AudioSource>().Stop();
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
		GetCamera ().GetComponent<AudioSource>().clip = music;
		GetCamera ().GetComponent<AudioSource>().pitch = pitch;
		GetCamera ().GetComponent<AudioSource>().Play ();
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
		m_player.GetComponent<AudioSource>().PlayOneShot(audioFX, GetFXVolume());
	}

	public void PlayConstFX(AudioClip audioFX)
	{
		m_guiText.GetComponent<AudioSource>().clip = audioFX;
		m_guiText.GetComponent<AudioSource>().Play();
	}

	public void StopConstFX()
	{
		m_guiText.GetComponent<AudioSource>().Stop ();
	}

	public bool IsFXSoundPlaying()
	{
		return m_guiText.GetComponent<AudioSource>().isPlaying || m_player.GetComponent<AudioSource>().isPlaying;
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
