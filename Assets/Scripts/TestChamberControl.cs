using UnityEngine;
using System.Collections;

public class TestChamberControl : MonoBehaviour {
	//public string testChamberPath = "Prefabs/TestChamber";
	public static string m_testChamberName;

	public static float mapFriction;
	public const float MAX_MAP_FRICTION = 10.0f;
	public const float MIN_MAP_FRICTION = 0.0f;
	
	public static float mapBounciness;
	public const float MAX_MAP_BOUNCINESS = 1.0f;
	public const float MIN_MAP_BOUNCINESS = 0.0f;

	public static float mapGravity;
	public const float MAX_MAP_GRAVITY = 10.0f;
	public const float MIN_MAP_GRAVITY = 0.0f;

	public static float mapDrag;
	public const float MAX_MAP_DRAG = 20.0f;
	public const float MIN_MAP_DRAG = 0.0f;

	public static float rMass;
	public const float MAX_ROCKET_MASS = 10.0f;
	public const float MIN_ROCKET_MASS = 0.1f;

	public static float rEnginePower;
	public const float MAX_ROCKET_ENGINE_POWER = 100000.0f;
	public const float MIN_ROCKET_ENGINE_POWER = 10.0f;

	public static float rAgility;
	public const float MAX_ROCKET_AGILITY = 1000.0f;
	public const float MIN_ROCKET_AGILITY = 10.0f;

	private static GameObject m_map;
	private static GameObject m_rocket;
	private static GameObject m_rocketLanding;
	private static RocketControl m_rocket_script;
	//private static EngineInfo m_engineInfo;
	private static string m_currentMapName;
	private static Menu mainMenu;

	void Start()
	{

		m_testChamberName = "TestChamber";
		m_currentMapName = "";
	}

	//============================================================================================
	public static void loadTestChamber(Menu menu)
	{
		WorldControl wc = WorldControl.GetInstance();
		if (wc != null)
		{
			wc.loadCustomLevel(Resources.Load("Prefabs/TestChamber") as GameObject);
		}
		mainMenu = menu;
		initObjects ();
	}

	private static void initObjects()
	{
		WorldControl wc = WorldControl.GetInstance();
		if (wc != null)
		{
			m_map = wc.getMap();
			m_rocket = wc.getRocket();
		}
		m_rocket_script = m_rocket.GetComponent<RocketControl> ();
		if (m_currentMapName != m_map.name)
		{
			initPhyicParameters ();
			m_currentMapName = m_map.name;
		}
	}

	private static void initPhyicParameters()
	{
		MapInfo map_info = m_map.GetComponent<MapInfo> ();
		mainMenu.menuMapDiscription.initMenu(mainMenu, map_info);
		mapFriction = map_info.mapFriction;
		mapBounciness = map_info.mapBounciness;
		mapGravity = map_info.mapGravity;
		mapDrag = map_info.mapDrag;
		rMass = m_rocket.GetComponent<Rigidbody2D>().mass;
		rEnginePower = m_rocket_script.getEngine().GetComponent<EngineControl>().getEnginePower();
		rAgility = m_rocket_script.m_info.agility;
	}

	public static void setRocketParameters()
	{
		if (m_rocket)
		{
			m_rocket.GetComponent<Rigidbody2D>().gravityScale = mapGravity;
			m_rocket.GetComponent<Rigidbody2D>().drag = mapDrag;
			m_rocket.GetComponent<Rigidbody2D>().angularDrag = mapDrag;
			m_rocket.GetComponent<Rigidbody2D>().mass = rMass;

			m_rocket_script.getEngine().GetComponent<EngineControl>().setEnginePower(rEnginePower);
			m_rocket_script.m_info.agility = rAgility;
		}
		if (m_rocketLanding)
		{
			if (m_rocketLanding.GetComponent<Collider2D>().sharedMaterial.bounciness != mapBounciness || (m_rocketLanding.GetComponent<Collider2D>().sharedMaterial.friction != mapFriction))
			{
				PhysicsMaterial2D mapMaterial = new PhysicsMaterial2D();
				mapMaterial.bounciness = mapBounciness;
				mapMaterial.friction = mapFriction;
				m_rocketLanding.GetComponent<Collider2D>().sharedMaterial = mapMaterial;
				GameObject tempLanding = GameObject.Instantiate (m_rocketLanding) as GameObject;
				Destroy(m_rocketLanding);
				tempLanding.transform.parent = m_rocket.transform;
				tempLanding.transform.localPosition = new Vector3(0,0,0);
				tempLanding.transform.localRotation = Quaternion.identity;
				m_rocketLanding = tempLanding;
			}
		}
	}
	//============================================================================================

	public static void showTestChamberMenu()
	{
		GUILayout.BeginArea (new Rect (10, 10, 200, 500));
		GUI.color = Color.green;
		GUILayout.Label(Localization.T_MAP_PARAMETERS + ":");
		GUILayout.Label(" - " + Localization.T_GRAVITY + " (" + mapGravity + ")");
		mapGravity = GUILayout.HorizontalSlider(mapGravity, MIN_MAP_GRAVITY, MAX_MAP_GRAVITY);
		mapGravity = Mathf.Round (mapGravity*100.0f)/100.0f;
		GUILayout.Space(10);
		GUILayout.Label(" - " + Localization.T_DRAG + " (" + mapDrag + ")");
		mapDrag = GUILayout.HorizontalSlider(mapDrag, MIN_MAP_DRAG, MAX_MAP_DRAG);
		mapDrag = Mathf.Round (mapDrag*100.0f)/100.0f;
		GUILayout.Space(10);
		GUILayout.Label(" - " + Localization.T_FRICTION + " (" + mapFriction + ")");
		mapFriction = GUILayout.HorizontalSlider(mapFriction, MIN_MAP_FRICTION, MAX_MAP_FRICTION);
		mapFriction = Mathf.Round (mapFriction*100.0f)/100.0f;
		GUILayout.Space(10);
		GUILayout.Label(" - " + Localization.T_BOUNCINESS + " (" + mapBounciness + ")");
		mapBounciness = GUILayout.HorizontalSlider(mapBounciness, MIN_MAP_BOUNCINESS, MAX_MAP_BOUNCINESS);
		mapBounciness = Mathf.Round (mapBounciness*100.0f)/100.0f;
		GUILayout.EndArea ();

		GUI.color = Color.blue;
		GUILayout.BeginArea (new Rect (Screen.width - 210, 10, 200, 500));
		GUILayout.Label(Localization.T_ROCKET_PARAMETERS + ":");
		GUILayout.Label(" - " + Localization.T_MASS + " (" + rMass + ")");
		rMass = GUILayout.HorizontalSlider(rMass, MIN_ROCKET_MASS, MAX_ROCKET_MASS);
		rMass = Mathf.Round (rMass*100.0f)/100.0f;
		GUILayout.Space(10);
		GUILayout.Label(" - " + Localization.T_AGILITY + " (" + rAgility + ")");
		rAgility = GUILayout.HorizontalSlider(rAgility, MIN_ROCKET_AGILITY, MAX_ROCKET_AGILITY);
		rAgility = Mathf.Round (rAgility);
		GUILayout.Space(10);
		float log_maxEP = Mathf.Log (MAX_ROCKET_ENGINE_POWER);
		float log_minEP = Mathf.Log (MIN_ROCKET_ENGINE_POWER);
		float log_EP = Mathf.Log (rEnginePower);
		GUILayout.Label(" - " + Localization.T_ENGINE_POWER + " (" + rEnginePower + ")");
		log_EP = GUILayout.HorizontalSlider(log_EP, log_minEP, log_maxEP);
		rEnginePower = Mathf.Round (Mathf.Exp(log_EP));
		GUILayout.EndArea ();

		Mathf.Clamp (mapFriction, MIN_MAP_FRICTION, MAX_MAP_FRICTION);
		Mathf.Clamp (mapBounciness, MIN_MAP_BOUNCINESS, MAX_MAP_BOUNCINESS);
		Mathf.Clamp (mapGravity, MIN_MAP_GRAVITY, MAX_MAP_GRAVITY);
		Mathf.Clamp (mapDrag, MIN_MAP_DRAG, MAX_MAP_DRAG);
		Mathf.Clamp (rMass, MIN_ROCKET_MASS, MAX_ROCKET_MASS);
		Mathf.Clamp (rEnginePower, MIN_ROCKET_ENGINE_POWER, MAX_ROCKET_ENGINE_POWER);
		Mathf.Clamp (rAgility, MIN_ROCKET_AGILITY, MAX_ROCKET_AGILITY);

		if (!m_map || !m_rocket || !m_rocket_script)
		{
			initObjects();
		}
		if (!m_rocketLanding)
		{
			m_rocketLanding = GameObject.FindWithTag(WorldControl.ROCKET_LANDING_TAG);
		}
		setRocketParameters ();
	}
}
