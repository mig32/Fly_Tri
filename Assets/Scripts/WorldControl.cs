﻿using UnityEngine;
using System;
using UnityEngine.UI;

public class WorldControl : MonoBehaviour
{
	[SerializeField] private GameObject m_mainGUI;
	[SerializeField] private Transform m_playerTransform;

	public event Action<string> OnMessage;
	public event Action<RocketControl> OnRocketCreated;
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
	private int m_completedTargetZonesCount = 0;
	private int m_savedCheckpoint = 0;
	public int CurrentCheckpoint => m_savedCheckpoint;
	private string[] m_audioNames;

	private GameObject m_mapPrefab;
	private GameObject m_map;
	private MapInfo m_mapInfo;
	private RocketControl m_rocketControl;
	public MapList MapList { get; private set; }
	public RocketList RocketList { get; private set; }
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
		RocketList = GetComponent<RocketList>();
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

		var rocketPrefab = RocketList.GetRocketPrefab(0);
		var enginePrefab = RocketList.GetEnginePrefab(0);
		m_rocketControl = RocketControl.Create(rocketPrefab, enginePrefab, m_playerTransform);
		OnRocketCreated?.Invoke(m_rocketControl);

		soundController.PlayMusic(Resources.Load< AudioClip>(AudioList.getAudioPath (getAudioNames() [0])));

		SetPause(true);
	}

	public void SetMapPhysicParametres()
	{
		if (m_mapInfo)
		{
			m_rocketControl.GetComponent<Rigidbody2D>().angularDrag = m_mapInfo.m_mapDrag;
			m_rocketControl.GetComponent<Rigidbody2D>().drag = m_mapInfo.m_mapDrag;
			m_rocketControl.GetComponent<Rigidbody2D>().gravityScale = m_mapInfo.m_mapGravity;

			var material = new PhysicsMaterial2D();
			material.bounciness = m_mapInfo.m_mapBounciness;
			material.friction = m_mapInfo.m_mapFriction;
			Collider2D rocketLanding = m_rocketControl.rocketLanding;
			rocketLanding.sharedMaterial = material;
			m_rocketControl.GetComponent<Collider2D>().sharedMaterial = material;
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
		ResetTempMapData();
		LoadSelectedLevel();
	}

	private void LoadSelectedLevel()
	{
		m_map = Instantiate(m_mapPrefab, thisTransform); //Загрузим карту
		m_mapInfo = m_map.GetComponent<MapInfo>();
		m_mapInfo.MarkCheckedTargetZones(m_completedTargetZonesCount);
		SetMapPhysicParametres();
		var checkpoint = m_mapInfo.GetCheckpointPosition(m_savedCheckpoint);
		m_rocketControl.transform.position = checkpoint.position; // Поставим ракету на начельную позицию на карте
		m_rocketControl.transform.rotation = checkpoint.rotation; // Поставим ракету на начельную позицию на карте
		m_rocketControl.gameObject.SetActive(true);
		m_rocketControl.GetComponent<Rigidbody2D>().isKinematic = false;
		m_rocketControl.Reset();
		SetPause(false);
		IsInGame = true;
		CheckAndActiateTargetZone();
	}

	public void EndLevel()
	{//Уровень закончен - уничтожим карту и ракету
		if (m_map)
		{
			Destroy(m_map.gameObject);
		}

		soundController.StopEngineSound();

		m_rocketControl.transform.localRotation = Quaternion.identity;
		m_rocketControl.transform.localPosition = new Vector3(0,0,0);
		m_rocketControl.KillRocketSilent();
		m_rocketControl.GetComponent<Rigidbody2D>().isKinematic = true;
		m_rocketControl.gameObject.SetActive(false);
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
		ResetTempMapData();
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

		ResetTempMapData();
		MapInfo map_info = (mapPahs.GetMapPrefab()).GetComponent<MapInfo>();
		map_info.m_mapName = mapPahs.Name;
		map_info.m_mapIcon = mapPahs.GetMapIcon();
		GameProgress.SetMap(m_currentMapIdx, false);

		DialogsController.GetInstance().ShowMapDescriptionMenu(m_currentMapIdx);
	}

	public void RestartFromCheckpoint()
	{
		EndLevel();
		LoadSelectedLevel();
	}

	public void RestartLevel()
	{
		ResetTempMapData();
		RestartFromCheckpoint();
	}

//=================================================================================== Map loaders

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

	public void ShowMessage(string message)
	{
		OnMessage?.Invoke(message);
	}

	private void ResetTempMapData()
	{
		m_savedCheckpoint = 0;
		m_completedTargetZonesCount = 0;
	}

	public void OnCheckpointSaved(int idx)
	{
		m_savedCheckpoint = idx;
	}

	public void OnTargetZoneChecked()
	{
		++m_completedTargetZonesCount;
		CheckAndActiateTargetZone();
	}

	private void CheckAndActiateTargetZone()
	{
		var checkpoint = m_mapInfo.GetNextTargetZone();
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

	void Update()
	{
		if (IsInGame && Input.GetKeyDown(KeyCode.Escape))
		{
			SetPause(!m_isPaused); 
		}
	}

	void SetAllLists()
	{
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
