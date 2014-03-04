using UnityEngine;
using System.Collections;

public class LoadMap : MonoBehaviour {
	public GameObject[] mapPrefabs;
	public GameObject rocketPrefab;

	private SpriteRenderer thisSpriteRenderer;
	private PolygonCollider2D thisPolygonCollider2D;

	private static int currentLevel = 0;

	private GameObject m_map;
	private GameObject m_player;
	private GameObject m_rocket;
	private GameObject m_camera;
	private GameObject m_guiText;

	// Use this for initialization
	void Start () 
	{
		m_player = m_map.transform.Find ("player").gameObject; //Игрок (должен состоять из ракеты и камеры)
		m_camera = m_player.transform.Find ("rocket_camera").gameObject; //Камера, которая будет следить за ракетой
		m_guiText = m_map.transform.Find ("message").gameObject; //guiText для отображения сообщений

	}

	bool static loadLevel(ref int levelNumber)
	{//Загрузка карты
		int col = mapPrefabs.Length; //Кличество карт
		if ((levelNumber <= col) && (levelNumber > 0)) 
		{//Если карта с таким индексом существует
			m_map = GameObject.Instantiate (mapPrefabs [levelNumber-1]) as GameObject; //Загрузим карту
			m_map.transform.parent = transform; //Удочерим ее

			m_rocket = GameObject.Instantiate (rocketPrefab) as GameObject; //Загрузим ракету игрока
			m_rocket.transform.parent = m_player.transform; //Сделаем ее ребенком "player"

			m_camera.GetComponent<CameraControl> ().setTargetTransform (m_rocket.transform); //Укажем скрипту камеры за каким объектом следить

			m_rocket.transform.position = m_map.transform.Find ("start_location").position; // Поставим ракету на начельную позицию на карте

			Time.timeScale = 1.0f;
			return true;
		}
		else
		{
			return false;
		}
	}

	void static endLevel()
	{//Уровень закончен - уничтожим карту и ракету
		if (m_map) Destroy (m_map.gameObject);
		if (m_rocket) Destroy (m_rocket.gameObject);
	}

	public static bool startNewGame(ref int levelIndex = 1)
	{//Стартуем игру с указанного уровня (вернет false если загрузить карту не удалось)
		currentLevel = levelIndex;
		endLevel(); //Почистим мир
		return loadLevel (levelIndex); //Загрузим карту
	}

	public static bool loadNextLevel()
	{
		currentLevel++;
		return restartLevel();
	}

	public static bool restartLevel()
	{
		endLevel();
		return loadLevel (currentLevel);
	}

	public void showMessage(string text, Color color)
	{//Большие буквы по центру экрана
		m_guiText.guiText.enabled = true;
		m_guiText.guiText.text = text;
		m_guiText.guiText.color = color;

	}

	public void hideMessage()
	{
		m_guiText.guiText.enabled = false;
	}

	public string[] getMapNames()
	{
		int col = mapPrefabs.Length;
		string[] map_names = new string[col];
		for (int i = 0; i < col; i++)
			map_names[i] = mapPrefabs[i].gameObject.name;
		return map_names;
	}
}
