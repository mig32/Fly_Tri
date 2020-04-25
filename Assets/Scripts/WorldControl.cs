using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public static class GameProgress
{
	public static int score = 5000;
	public static int tempScore = 0;
	public static Dictionary<string, bool> maps = new Dictionary<string, bool>();
	public static Dictionary<string, RocketInfo> buyedRockets = new Dictionary<string, RocketInfo>();
	public static Dictionary<string, EngineInfo>  buyedEngines = new Dictionary<string, EngineInfo>();

	public static void Save()
	{
		PlayerPrefs.SetInt ("score", score);
		PlayerPrefs.SetInt ("tempScore", tempScore);

		string temp = "";
		foreach (string map in maps.Keys)
		{
			temp += map + "\n" + (maps[map] ? "1" : "0") + "\n";
		}
		PlayerPrefs.SetString ("maps", temp);

		temp = "";
		RocketInfo rInfo;
		foreach (string rocket in buyedRockets.Keys)
		{
			rInfo = buyedRockets[rocket];
			temp += rocket + "\n" +  rInfo.current_max_agility + "\n" +  rInfo.agility + "\n" +  rInfo.current_min_agility + "\n";
			temp += rInfo.current_max_mass + "\n" +  rInfo.mass + "\n" +  rInfo.current_min_mass + "\n";
		}
		PlayerPrefs.SetString ("buyedRockets", temp);

		temp = "";
		EngineInfo eInfo;
		foreach (string engine in buyedEngines.Keys)
		{
			eInfo = buyedEngines[engine];
			temp += engine + "\n" +  eInfo.current_max_power + "\n" +  eInfo.enginePower + "\n" +  eInfo.current_min_power + "\n";
		}
		PlayerPrefs.SetString ("buyedEngines", temp);

		PlayerPrefs.Save ();
	}

	public static void Load()
	{
		init ();
		if (PlayerPrefs.HasKey("score"))
		{
			score = PlayerPrefs.GetInt ("score");
		}
		if (PlayerPrefs.HasKey("tempScore"))
		{
			tempScore = PlayerPrefs.GetInt ("tempScore");
		}
		if (PlayerPrefs.HasKey("maps"))
		{
			string[] temp = PlayerPrefs.GetString("maps").Split('\n');
			for (int i = 0; i < temp.Length-1; i += 2)
			{
				//Debug.Log(temp[i] + temp[i+1]);
				maps.Add(temp[i], (temp[i+1] == "1"));
			}
		}

		if (PlayerPrefs.HasKey("buyedRockets"))
		{
			string[] temp = PlayerPrefs.GetString("buyedRockets").Split('\n');
			for (int i = 0; i < temp.Length-1; i += 7)
			{
				RocketInfo rInfo = new RocketInfo();
				rInfo.current_max_agility = float.Parse(temp[i+1]);
				rInfo.agility = float.Parse(temp[i+2]);
				rInfo.current_min_agility = float.Parse(temp[i+3]);
				rInfo.current_max_mass = float.Parse(temp[i+4]);
				rInfo.mass = float.Parse(temp[i+5]);
				rInfo.current_min_mass = float.Parse(temp[i+6]);
				buyedRockets.Add(temp[i], rInfo);
			}
		}

		if (PlayerPrefs.HasKey("buyedEngines"))
		{
			string[] temp = PlayerPrefs.GetString("buyedEngines").Split('\n');
			for (int i = 0; i < temp.Length-1; i += 4)
			{
				EngineInfo eInfo = new EngineInfo();
				eInfo.current_max_power = float.Parse(temp[i+1]);
				eInfo.enginePower = float.Parse(temp[i+2]);
				eInfo.current_min_power = float.Parse(temp[i+3]);
				buyedEngines.Add(temp[i], eInfo);
			}
		}
	}

	public static void Clear()
	{
		PlayerPrefs.DeleteAll();
		init ();
		WorldControl.init ();
	}

	private static void init()
	{
		score = 5000;
		tempScore = 0;
		maps.Clear ();
		buyedRockets.Clear ();
		buyedEngines.Clear ();
	}

	public static void setBuyedRocket(string rocketName, RocketInfo rocketInfo)
	{
		if (buyedRockets.ContainsKey(rocketName))
		{
			buyedRockets[rocketName] = rocketInfo;
		}
		else
		{
			buyedRockets.Add(rocketName, rocketInfo);
		}
	}

	public static void setBuyedEngine(string engineName, EngineInfo engineInfo)
	{
		if (buyedEngines.ContainsKey(engineName))
		{
			buyedEngines[engineName] = engineInfo;
		}
		else
		{
			buyedEngines.Add(engineName, engineInfo);
		}
	}

	public static void setMap(string mapName, bool isCompleted)
	{
		if (maps.ContainsKey(mapName))
		{
			if (!maps[mapName])
				maps[mapName] = isCompleted;
		}
		else
		{
			maps.Add(mapName, isCompleted);
		}
	}
}

public class WorldControl : MonoBehaviour {
	public const float WIN_TIMER = 2.0f;
	private static float timer_time;
	private static bool b_isTimerStarted = false;
	private static bool b_isWinTimer;

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

	public static Menu m_mainMenu;

	private static string currentLevel = "";
	private static Transform thisTransform;
	private static bool b_isPauseOn;
	private static float gameSpeed;
	private static float soundVolume = 0.8f;
	public static float fxVolume = 1.0f;

	private static string[] m_mapsNames;
	private static string[] m_rocketsNames;
	private static string[] m_enginesNames;
	private static string[] m_audioNames;

	private static GameObject m_mapPrefab;
	private static GameObject m_map;
	private static MapInfo m_mapInfo;
	private static GameObject m_rocket;
	private static GameObject m_player;
	private static RocketControl m_rocketControl;
	private static GameObject m_enginePrefab;
	private static GameObject m_camera;
	private static GameObject m_guiText;
	private static GameObject m_ScoreGUIText;


	// Use this for initialization
	void Start () 
	{
		setAllLists ();
		GameProgress.Load ();
		thisTransform = transform;
		m_mainMenu = gameObject.GetComponent<Menu> ();
		init ();
	}

	public static void init()
	{
		Localization.selectLanguage ("RU");
		gameSpeed = 1.0f;
		m_player = GameObject.Find (PLAYER_OBJECT_NAME).gameObject;
		m_camera = GameObject.Find (ROCKET_CAMERA_NAME).gameObject; //Камера, которая будет следить за ракетой
		m_guiText = GameObject.Find (MESSAGE_OBJECT_NAME).gameObject; //guiText для отображения сообщений
		m_ScoreGUIText = GameObject.Find (SCORE_OBJECT_NAME).gameObject;
		chooseRocket (m_rocketsNames[0]);
		chooseEngine (m_enginesNames[0]);
		playMusic (Resources.Load (AudioList.getAudioPath (getAudioNames () [0])) as AudioClip, 1.5f);
		if (!GameProgress.buyedRockets.ContainsKey(m_rocketControl.rocketName))
		{
			GameProgress.setBuyedRocket(m_rocketControl.rocketName, m_rocketControl.m_info);
		}
		if (!GameProgress.buyedEngines.ContainsKey(m_rocketControl.getEngineControl().engineName))
		{
			GameProgress.setBuyedEngine(m_rocketControl.getEngineControl().engineName, m_rocketControl.getEngineControl().m_info);
		}
		foreach (string mapName in m_mapsNames)
		{
			if (!GameProgress.maps.ContainsKey(mapName))
			{
				GameProgress.setMap(mapName, false);
			}
		}
		pause();
	}

	private static void instantiateRocket(GameObject rocket)
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

	public static void chooseRocket(string rocketName)
	{	
		GameObject rocket = Resources.Load(RocketList.getRocketPath(rocketName)) as GameObject;
		instantiateRocket (rocket);
		m_rocketControl.rocketName = rocketName;
	}
	public static void chooseRocket(GameObject rocket)
	{
		instantiateRocket (rocket);
	}

	public static void chooseEngine(string engineName)
	{	
		m_rocketControl.setEngine(Resources.Load(RocketList.getEnginePath(engineName)) as GameObject);
		m_rocketControl.getEngineControl ().engineName = engineName;
	}
	
	public static void chooseEngine(GameObject engine)
	{
		m_rocketControl.setEngine(engine);
	}

	public static void LoadRocketParameners()
	{
		if (GameProgress.buyedRockets.ContainsKey(m_rocketControl.rocketName))
		{
			m_rocketControl.m_info = GameProgress.buyedRockets[m_rocketControl.rocketName];
		}
		getRocket().GetComponent<Rigidbody2D>().mass = m_rocketControl.m_info.mass;
	}

	public static void setMapPhysicParametres()
	{
		if (m_mapInfo)
		{
			getRocket().GetComponent<Rigidbody2D>().angularDrag = getRocket().GetComponent<Rigidbody2D>().drag = m_mapInfo.mapDrag;
			getRocket().GetComponent<Rigidbody2D>().gravityScale = m_mapInfo.mapGravity;
			PhysicsMaterial2D mapMaterial = new PhysicsMaterial2D();
			mapMaterial.bounciness = m_mapInfo.mapBounciness;
			mapMaterial.friction = m_mapInfo.mapFriction;
			GameObject tempLanding = GameObject.Instantiate (GameObject.FindWithTag(ROCKET_LANDING_TAG)) as GameObject;
			tempLanding.GetComponent<Collider2D>().sharedMaterial = mapMaterial;

			GameObject.Destroy (GameObject.FindWithTag(WorldControl.ROCKET_LANDING_TAG));
			
			tempLanding.transform.parent = m_rocket.transform;
			tempLanding.transform.localPosition = new Vector3(0,0,0);
			tempLanding.transform.localRotation = Quaternion.identity;

		}
	}

//========================== Level loaders ===============================================
	public static void loadCustomLevel(GameObject map)
	{
		m_mapPrefab = map;
		currentLevel = "CutomLevel";
		if (m_map) endLevel ();
		loadSelectedLevel ();
	}

	private static void loadSelectedLevel()
	{
		m_map = GameObject.Instantiate (m_mapPrefab) as GameObject; //Загрузим карту
		m_mapInfo = m_map.GetComponent<MapInfo> ();
		m_map.transform.parent = thisTransform; //Удочерим ее
		setMapPhysicParametres ();
		getRocket ().transform.position = m_map.transform.Find (START_LOCATION_NAME).position; // Поставим ракету на начельную позицию на карте
		getRocket ().GetComponent<SpriteRenderer>().enabled = true;
		m_rocket.GetComponent<Rigidbody2D>().isKinematic = false;
		m_rocketControl.Reset();
		unPause();
	}

	public static void endLevel()
	{//Уровень закончен - уничтожим карту и ракету
		clearTempScore ();
		if (m_map) GameObject.Destroy(m_map.gameObject);
		m_rocket.transform.localRotation = Quaternion.identity;
		m_rocket.transform.localPosition = new Vector3(0,0,0);
		m_rocket.GetComponent<SpriteRenderer> ().sprite = m_rocketControl.rocketSprite;
		m_rocketControl.b_isAlive = false;
		m_rocket.GetComponent<Rigidbody2D>().isKinematic = true;
		m_rocket.GetComponent<SpriteRenderer>().enabled = false;
	}

	public static void loadLevel(string levelName)
	{//Загрузка карты
		if (MapList.contains(levelName)) 
		{//Если карта с таким индексом существует
			currentLevel = levelName;
			m_mapPrefab = Resources.Load(MapList.getMapPath(levelName)) as GameObject;
			if (m_map) endLevel ();
			loadSelectedLevel();
		}
	}

	public static void loadNextLevel()
	{
		endLevel ();
		bool isNext = false;
		foreach (string name in m_mapsNames)
		{
			if (isNext) 
			{
				currentLevel = name;
				break;
			}
			if (name == currentLevel) 
			{
				isNext = true;
				currentLevel = m_mapsNames[0];
			}
		}
		
		MapInfo map_info = (Resources.Load(MapList.getMapPath(currentLevel)) as GameObject).GetComponent<MapInfo>();
		map_info.mapName = currentLevel;
		map_info.mapLogo = Resources.Load(MapList.getMapLogoPath(currentLevel)) as Texture2D;
		GameProgress.setMap (currentLevel, false);
		m_mainMenu.menuMapDiscription.initMenu(m_mainMenu, map_info);
		m_mainMenu.showMapDiscriptionMenu();
	}

	public static void restartLevel()
	{
		endLevel();
		loadSelectedLevel();
	}

//=================================================================================== Map loaders
	public static void showMessage(string text, Color color)
	{//Большие буквы по центру экрана
		m_guiText.GetComponent<GUIText>().enabled = true;
		m_guiText.GetComponent<GUIText>().text = text;
		m_guiText.GetComponent<GUIText>().color = color;

	}

	public static void hideMessage()
	{
		m_guiText.GetComponent<GUIText>().enabled = false;
	}

	public static string[] getMapsNames()
	{
		return m_mapsNames;
	}

	public static string[] getRocketsNames()
	{
		return m_rocketsNames;
	}

	public static string[] getEnginesNames()
	{
		return m_enginesNames;
	}

	public static string[] getAudioNames()
	{
		return m_audioNames;
	}

	public static void pause()
	{
		b_isPauseOn = true;
		Time.timeScale = 0.0f;
	}
	
	public static void unPause()
	{
		b_isPauseOn = false;
		Time.timeScale = gameSpeed;
	}

	public static bool isPaused()
	{
		return b_isPauseOn;
	}

	public static void setGameSpeed(float speed)
	{
		Mathf.Clamp (speed, MIN_GAME_SPEED, MAX_GAME_SPEED);
		gameSpeed = speed;
		Time.timeScale = gameSpeed;
	}

	public static float getGameSpeed()
	{
		return gameSpeed;
	}

	public static void setSoundVolume(float volume)
	{
		soundVolume = volume;
		getCamera().GetComponent<AudioSource>().volume = Mathf.Clamp (soundVolume, MIN_SOUND_VOLUME, MAX_SOUND_VOLUME);
		m_player.GetComponent<AudioSource>().volume = Mathf.Clamp (fxVolume*soundVolume, MIN_SOUND_VOLUME, MAX_SOUND_VOLUME);
		m_guiText.GetComponent<AudioSource>().volume = Mathf.Clamp (fxVolume*soundVolume, MIN_SOUND_VOLUME, MAX_SOUND_VOLUME);
		//setFXVolume (volume);
	}

	public static void setFXVolume(float volume)
	{
		fxVolume = volume;
		m_player.GetComponent<AudioSource>().volume = Mathf.Clamp (fxVolume*soundVolume, MIN_SOUND_VOLUME, MAX_SOUND_VOLUME);
		m_guiText.GetComponent<AudioSource>().volume = Mathf.Clamp (fxVolume*soundVolume, MIN_SOUND_VOLUME, MAX_SOUND_VOLUME);
	}

	public static float getFXVolume()
	{
		return fxVolume*soundVolume;
	}

	public static float getSoundVolume()
	{
		return soundVolume;
	}

	public static GameObject getRocket()
	{
		return m_rocket;
	}

	public static RocketControl getRocketControl()
	{
		return m_rocketControl;
	}

	public static GameObject getMap()
	{
		return m_map;
	}

	public static GameObject getCamera()
	{
		return m_camera;
	}

	public static string getCurrentLevelNumber()
	{
		return currentLevel;
	}

	public static void setCurrentLevelNumber(string levelName)
	{
		currentLevel = levelName;
	}

	public static MapInfo getMapInfo()
	{
		return m_mapInfo;
	}

	public static void startTimer(float secons, bool isWinTimer)
	{
		b_isTimerStarted = true;
		timer_time = secons;
		b_isWinTimer = isWinTimer;
		onStartTimer (b_isWinTimer);
	}

	public static void stopTimer()
	{
		b_isTimerStarted = false;
		hideMessage();
	}

	private static void onStartTimer (bool isWinTimer)
	{
		if (isWinTimer)
		{
			showMessage (Localization.T_WIN_MESSAGE + " (" + string.Format("{0:0.00}", Mathf.Round (timer_time*100.0f)/100.0f) + ")", Color.green);
		}
		else
		{
			showMessage (Localization.T_LOSE_MESSAGE + ((int)((m_rocketControl.cost+m_rocketControl.getEngineControl().cost)/2)).ToString(), Color.red);
		}
	}

	private static void onTimerTick (bool isWinTimer)
	{
		if (isWinTimer)
		{
			string timeLeft = string.Format("{0:0.00}", Mathf.Round (timer_time*100.0f)/100.0f);
			m_guiText.GetComponent<GUIText>().text = Localization.T_WIN_MESSAGE + " (" + timeLeft + ")";
		}
	}

	private static void onStopTimer(bool isWinTimer)
	{
		if (isWinTimer)
		{
			saveScore();
			m_guiText.GetComponent<AudioSource>().Stop();
			GameProgress.setMap(m_mapInfo.mapName, true);
			loadNextLevel();
		}
		else
		{
			restartLevel();
		}
	}

	public static void playMusic(AudioClip music, float pitch = 1.0f)
	{
		getCamera ().GetComponent<AudioSource>().clip = music;
		getCamera ().GetComponent<AudioSource>().pitch = pitch;
		getCamera ().GetComponent<AudioSource>().Play ();
	}

	public static void playNextMusic(float pitch = 1.0f)
	{
		bool isNext = false;
		foreach (string name in m_audioNames)
		{
			if (isNext) 
			{
				playMusic (Resources.Load (AudioList.getAudioPath (name)) as AudioClip, pitch);
				return;
			}
			if (name == AudioList.currentAudioName) isNext = true;
		}
		playMusic (Resources.Load (AudioList.getAudioPath (getAudioNames () [0])) as AudioClip, pitch);
	}

	public static void playOneShotFX(AudioClip audioFX)
	{
		m_player.GetComponent<AudioSource>().PlayOneShot(audioFX, getFXVolume());
	}

	public static void playConstFX(AudioClip audioFX)
	{
		m_guiText.GetComponent<AudioSource>().clip = audioFX;
		m_guiText.GetComponent<AudioSource>().Play();
	}

	public static void stopConstFX()
	{
		m_guiText.GetComponent<AudioSource>().Stop ();
	}

	public static bool isFXSoundPlaying()
	{
		return m_guiText.GetComponent<AudioSource>().isPlaying || m_player.GetComponent<AudioSource>().isPlaying;
	}

	void Update()
	{
		if (b_isTimerStarted)
		{
			timer_time -= Time.deltaTime;
			if (timer_time <= 0)
			{
				stopTimer();
				onStopTimer (b_isWinTimer);
			}
			onTimerTick(b_isWinTimer);
		}
	}

	public static void hideScore()
	{
		m_ScoreGUIText.GetComponent<GUIText>().enabled = false;
	}
	
	public static void showScore()
	{
		string temp = "";
		if (GameProgress.tempScore > 0) temp = "(+" + GameProgress.tempScore  + ")";
		else if (GameProgress.tempScore < 0) temp = "(" + GameProgress.tempScore  + ")";
		m_ScoreGUIText.GetComponent<GUIText>().text = (GameProgress.score + temp);
		m_ScoreGUIText.GetComponent<GUIText>().enabled = true;
	}
	
	public static void addScore(int score)
	{
		GameProgress.tempScore += score;
		showScore ();
	}
	
	public static void saveScore()
	{
		/*if (GameProgress.tempScore < 0)
		{
			if (GameProgress.score < GameProgress.tempScore)
			{
				return false;
			}
		}*/
		GameProgress.score += GameProgress.tempScore;
		GameProgress.tempScore = 0;
		//return true;
	}

	public static void clearTempScore()
	{
		GameProgress.tempScore = 0;
		m_ScoreGUIText.GetComponent<GUIText>().text = GameProgress.score.ToString();
	}
	

	void setAllLists()
	{
		//Maps
		m_mapsNames = new string[6] {"map1","map2", "map3", "So_what","StarStage_1", "Карта имени 8-ого марта"};
		MapList.clearMapList ();
		MapList.addMap (m_mapsNames[0], "Images/maps/map1_logo", "Prefabs/mapPrefabs/map1");
		MapList.addMap (m_mapsNames[1], "Images/maps/map2_logo", "Prefabs/mapPrefabs/map2");
		MapList.addMap(m_mapsNames[2], "Images/maps/map3_logo", "Prefabs/mapPrefabs/map3");
		MapList.addMap (m_mapsNames[3], "Images/maps/feniks_map1_logo", "Prefabs/mapPrefabs/So_what");
		MapList.addMap (m_mapsNames[4], "Images/maps/feniks_map2_logo", "Prefabs/mapPrefabs/StarStage_1");
		MapList.addMap (m_mapsNames[5], "Images/maps/logo_1", "Prefabs/mapPrefabs/8march");
		gameObject.GetComponent<MenuChooseMap> ().loadMapIcons (m_mapsNames);

		//Rockets
		m_rocketsNames = new string[4] {"Ракета","Карандаш","Бумажная ракета","Треугольник"};
		RocketList.clearRocketList();
		RocketList.addRocket (m_rocketsNames[0], "Prefabs/rocketPrefabs/rocket");
		RocketList.addRocket (m_rocketsNames[1], "Prefabs/rocketPrefabs/PenRocket");
		RocketList.addRocket (m_rocketsNames[2], "Prefabs/rocketPrefabs/PaperRocket");
		RocketList.addRocket (m_rocketsNames[3], "Prefabs/rocketPrefabs/TriangleRocket");
		foreach (string rName in m_rocketsNames)
		{
			(Resources.Load(RocketList.getRocketPath(rName)) as GameObject).GetComponent<RocketControl>().rocketName = rName;
		}

		//Engines
		m_enginesNames = new string[4] {"Двигатель","Импульсник","Дымок","Генератор треугольного поля"};
		RocketList.addEngine (m_enginesNames[0], "Prefabs/enginePrefabs/engine");
		RocketList.addEngine (m_enginesNames[1], "Prefabs/enginePrefabs/ImpulseEngine");
		RocketList.addEngine (m_enginesNames[2], "Prefabs/enginePrefabs/WhiteFogEngine");
		RocketList.addEngine (m_enginesNames[3], "Prefabs/enginePrefabs/TriangleEngine");
		foreach (string eName in m_enginesNames)
		{
			(Resources.Load(RocketList.getEnginePath(eName)) as GameObject).GetComponent<EngineControl>().engineName = eName;
		}

		//Audio
		m_audioNames = new string[5] {"DontWorry", "DerWienerWalzer", "HevenAndHell", "BeeFly", "ValkyriFly"};
		AudioList.addAudio (m_audioNames[0], "Music/Bobby McFerrin - Don't Worry, Be Happy");
		AudioList.addAudio (m_audioNames[1], "Music/Johann Strauss - Der Wiener Walzer");
		AudioList.addAudio (m_audioNames[2], "Music/Вильгельм Рихард Вагнер - Воплощение ада и рая");
		AudioList.addAudio (m_audioNames[3], "Music/Римский-Корсаков - Полет шмеля");
		AudioList.addAudio (m_audioNames[4], "Music/Рихард Вагнер - Полет валькирий");
	}
}
