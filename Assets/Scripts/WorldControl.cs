using UnityEngine;
using System;
using UnityEngine.UI;

public class WorldControl : MonoBehaviour
{
	[SerializeField] private GameObject m_mainGUI;
	[SerializeField] private Transform m_playerTransform;

	public event Action<string> OnMessage;
	public event Action<GameObject> OnRocketCreated;
	public event Action<float> OnHPChanged;

	public bool IsInGame { get; private set; }

	public const float WIN_TIMER = 2.0f;
	private float timer_time;
	private bool m_isTimerStarted = false;
	private bool m_isWinTimer;

	public const string ROCKET_CAMERA_NAME = "rocket_camera";
	public const string PLAYER_OBJECT_NAME = "player";
	public const string MESSAGE_OBJECT_NAME = "message";
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
	public bool IsPaused { get { return m_isPaused; } }
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
	public SoundController soundController { get; private set; }
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
		soundController = GetComponent<SoundController>();
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
		soundController.PlayMusic(Resources.Load< AudioClip>(AudioList.getAudioPath (getAudioNames() [0])));

		SetPause(true);
	}

	private void InstantiateRocket(GameObject rocket)
	{
		m_rocket = GameObject.Instantiate<GameObject>(rocket, m_playerTransform);
		m_rocket.GetComponent<Rigidbody2D>().isKinematic = true;
		m_rocketControl = m_rocket.GetComponent<RocketControl>();
		OnRocketCreated?.Invoke(m_rocket);
	}

	public void chooseRocket(string rocketName)
	{	
		GameObject rocket = RocketList.GetInstance().GetRocketPrefab(rocketName);
		InstantiateRocket(rocket);
	}

	public void chooseRocket(GameObject rocket)
	{
		InstantiateRocket(rocket);
	}

	public void chooseEngine(string engineName)
	{	
		m_rocketControl.setEngine(RocketList.GetInstance().GetEnginePrefab(engineName));
	}
	
	public void chooseEngine(GameObject engine)
	{
		m_rocketControl.setEngine(engine);
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
		GetRocket().transform.position = m_mapInfo.m_startLocation.position; // Поставим ракету на начельную позицию на карте
		GetRocket().transform.rotation = m_mapInfo.m_startLocation.rotation; // Поставим ракету на начельную позицию на карте
		GetRocket().SetActive(true);// .GetComponent<SpriteRenderer>().enabled = true;
		m_rocket.GetComponent<Rigidbody2D>().isKinematic = false;
		m_rocketControl.Reset();
		SetPause(false);
		IsInGame = true;
		CheckAndActiateCheckpoints();
	}

	public void EndLevel()
	{//Уровень закончен - уничтожим карту и ракету
		if (m_map)
		{
			Destroy(m_map.gameObject);
		}

		soundController.StopEngineSound();

		m_rocket.transform.localRotation = Quaternion.identity;
		m_rocket.transform.localPosition = new Vector3(0,0,0);
		m_rocketControl.KillRocketSilent();
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
		soundController.MusicVolume = volume;
	}

	public void SetFXVolume(float volume)
	{
		soundController.SoundVolume = volume;
	}

	public float GetFXVolume()
	{
		return soundController.SoundVolume;
	}

	public float GetMusicVolume()
	{
		return soundController.MusicVolume;
	}

	public GameObject GetRocket()
	{
		return m_rocket;
	}

	public RocketControl GetRocketControl()
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
			OnMessage?.Invoke("You broke you rocket. Try again.");
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

	public void ShowMessage(string message)
	{
		OnMessage?.Invoke(message);
	}

	public void OnCheckpointCollected()
	{
		CheckAndActiateCheckpoints();
	}

	private void CheckAndActiateCheckpoints()
	{
		var checkpoint = m_mapInfo.GetNextCheckpoint();
		if (checkpoint != null)
		{
			checkpoint.Activate();
		}
		else
		{
			GameProgress.SetMap(m_currentMapIdx, true);
			LoadNextLevel();
		}
	}

	private void OnStopTimer(bool isWinTimer)
	{
		if (isWinTimer)
		{
			GameProgress.SetMap(m_currentMapIdx, true);
			LoadNextLevel();
		}
		else
		{
			RestartLevel();
		}
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

	public void CallHpChanged(float value)
	{
		OnHPChanged?.Invoke(value);
	}
}
