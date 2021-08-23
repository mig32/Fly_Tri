using UnityEngine;
using System.Collections;

public class MenuRocketGarage : MonoBehaviour {
	private Menu mainMenu;
	private GameObject m_rocket;
	private GameObject m_engine;
	private RocketControl m_rocketControl;
	private EngineControl m_engineControl;

	private float rMass;
	private float MAX_ROCKET_MASS = 10.0f;
	private float MIN_ROCKET_MASS = 0.1f;
	
	private float rEnginePower;
	private float MAX_ROCKET_ENGINE_POWER = 100000.0f;
	private float MIN_ROCKET_ENGINE_POWER = 10.0f;
	
	private float rAgility;
	private float MAX_ROCKET_AGILITY = 1000.0f;
	private float MIN_ROCKET_AGILITY = 10.0f;

	/*private string[] m_rocketsNames;
	private int[] m_rocketsCosts;
	private Texture2D[] m_rocketsImages;
	private string[] m_enginesNames;
	private int[] m_enginesCosts;
	private Texture2D[] m_enginesImages;
	private int selectedItem;*/

	public void init(Menu menu, GameObject rocket)
	{
		m_rocket = GameObject.Instantiate (rocket) as GameObject;
		WorldControl wc = WorldControl.GetInstance();
		if (wc != null)
		{
			wc.GetRocket().SetActive(false);
		}
		m_rocketControl = m_rocket.GetComponent<RocketControl>();
		m_engine = GameObject.FindWithTag(WorldControl.ENGINE_TAG);
		m_engine.transform.parent = transform.parent;
		m_engineControl = m_engine.GetComponent<EngineControl> ();
		mainMenu = menu;
		if (GameProgress.buyedRockets.ContainsKey(m_rocketControl.rocketName))
		{
			m_rocketControl.m_info = GameProgress.buyedRockets[m_rocketControl.rocketName];
		}
		if (GameProgress.buyedEngines.ContainsKey(m_engineControl.engineName))
		{
			m_engineControl.m_info = GameProgress.buyedEngines[m_engineControl.engineName];
		}
		setMass ();
		setEnginePower ();
		setAgility ();
	}

	public void setRocket (GameObject rocket)
	{
		GameObject temp_rocket = GameObject.Instantiate (rocket) as GameObject;
		if (m_rocket) Destroy(m_rocket);
		m_rocket = temp_rocket;
		m_rocketControl = m_rocket.GetComponent<RocketControl>();
		if (GameProgress.buyedRockets.ContainsKey(m_rocketControl.rocketName))
		{
			m_rocketControl.m_info = GameProgress.buyedRockets[m_rocketControl.rocketName];
		}
		setMass ();
		setAgility ();
	}

	public GameObject getRocket()
	{
		return m_rocket;
	}

	public RocketControl getRocketControl()
	{
		return m_rocketControl;
	}

	public void setEngine (GameObject engine)
	{
		GameObject temp_engine = GameObject.Instantiate (engine) as GameObject;
		if (m_engine) Destroy(m_engine);
		m_engine = temp_engine;
		m_engineControl = m_engine.GetComponent<EngineControl> ();
		if (GameProgress.buyedEngines.ContainsKey(m_engineControl.engineName))
		{
			m_engineControl.m_info = GameProgress.buyedEngines[m_engineControl.engineName];
		}
		setEnginePower ();
	}

	public GameObject getEngine()
	{
		return m_engine;
	}
	
	public EngineControl getEngineControl()
	{
		return m_engineControl;
	}

	public void setMass()
	{
		rMass = m_rocketControl.getMass ();
		MAX_ROCKET_MASS = m_rocketControl.m_info.current_max_mass;
		MIN_ROCKET_MASS = m_rocketControl.m_info.current_min_mass;
	}

	public void setEnginePower()
	{
		rEnginePower = m_engineControl.getEnginePower();
		MAX_ROCKET_ENGINE_POWER = m_engineControl.m_info.current_max_power;
		MIN_ROCKET_ENGINE_POWER = m_engineControl.m_info.current_min_power;
	}

	public void setAgility()
	{
		rAgility = m_rocketControl.m_info.agility;
		MAX_ROCKET_AGILITY = m_rocketControl.m_info.current_max_agility;
		MIN_ROCKET_AGILITY = m_rocketControl.m_info.current_min_agility;
	}

	/*public void loadRocketPrefabs(string[] rocketNames, string[] engineNames)
	{
		m_rocketsNames = rocketNames;
		m_enginesNames = engineNames;
		int col = rocketNames.Length;
		m_rocketsCosts = new int[col];
		m_enginesCosts = new int[col];
		m_rocketsImages = new Texture2D[col];
		m_enginesImages = new Texture2D[col];
		GameObject goods;
		for (int i=0; i<col; i++)
		{
			goods = Resources.Load(RocketList.getRocketPath(rocketNames[i])) as GameObject;
			m_rocketsCosts[i] = goods.GetComponent<RocketControl>().cost;
			m_rocketsImages[i] = goods.GetComponent<SpriteRenderer>().sprite.texture;
		}

		col = engineNames.Length;
		for (int i=0;i<col;i++)
		{
			goods = Resources.Load(RocketList.getEnginePath(engineNames[i])) as GameObject;
			m_enginesCosts[i] = goods.GetComponent<EngineControl>().cost;
			m_enginesImages[i] = goods.GetComponent<EngineControl>().image;
		}
	}*/

	public void showMenu()
	{
		WorldControl wc = WorldControl.GetInstance();
		GUILayout.BeginArea (new Rect (20, 20, Screen.width - 40, Screen.height - 40));
		GUILayout.BeginHorizontal();
			GUILayout.BeginVertical();
				if (GUILayout.Button(Localization.T_SELECT_HULL))
				{
					if (!GameProgress.buyedRockets.ContainsKey(m_rocketControl.rocketName))
					{
						if (wc != null)
						{
							wc.AddScore(m_rocketControl.cost);
						}
					}
					mainMenu.menuShop.init (mainMenu, false);
					mainMenu.showShopMenu();
				}
				GUILayout.Space(20);
				if (GUILayout.Button(Localization.T_SELECT_ENGINE))
				{
					if (!GameProgress.buyedEngines.ContainsKey(m_engineControl.engineName))
					{
						if (wc != null)
						{
							wc.AddScore(m_engineControl.cost);
						}
					}
					mainMenu.menuShop.init (mainMenu, true);
					mainMenu.showShopMenu();
				}
			GUILayout.EndVertical();
			GUILayout.Space(10);
			GUILayout.BeginVertical();
				GUILayout.Label("-== " + m_rocketControl.rocketName.ToUpper() + " ==-");
				GUILayout.Space(10);
				GUI.color = Color.white;
				GUILayout.Label(m_rocket.GetComponent<RocketControl>().rocketSprite.texture);
				GUI.color = Color.green;
			GUILayout.EndVertical();
			GUILayout.Space(10);
			GUILayout.BeginVertical();
				GUILayout.Label(Localization.T_ROCKET_PARAMETERS + ":");
				GUILayout.BeginHorizontal();
					GUILayout.BeginVertical();
						GUILayout.Label(" - " + Localization.T_MASS + " (" + rMass + ")");
						rMass = GUILayout.HorizontalSlider(rMass, MIN_ROCKET_MASS, MAX_ROCKET_MASS);
						rMass = Mathf.Round (rMass*100.0f)/100.0f;
					GUILayout.EndVertical();
					if (MIN_ROCKET_MASS>(m_rocketControl.min_mass))
					{
						if (m_rocketControl.mass_upgrade_cost > (GameProgress.score + GameProgress.tempScore))
						{
							GUILayout.Label(Localization.T_NEED_MORE_GOLD + " ( " + m_rocketControl.mass_upgrade_cost + " )");
						}
						else
						{
							if (GUILayout.Button("( " + Localization.T_UPGRADE + " = $" + m_rocketControl.mass_upgrade_cost + " )"))
							{
								m_rocketControl.upgradeHullMass();
								setMass ();
							}
						}
					}
				GUILayout.EndHorizontal();
				GUILayout.Space(10);
				GUILayout.BeginHorizontal();
					GUILayout.BeginVertical();
						GUILayout.Label(" - " + Localization.T_AGILITY + " (" + rAgility + ")");
						rAgility = GUILayout.HorizontalSlider(rAgility, MIN_ROCKET_AGILITY, MAX_ROCKET_AGILITY);
						rAgility = Mathf.Round (rAgility);
					GUILayout.EndVertical();
					if (m_rocketControl.agility_upgrade_cost > (GameProgress.score + GameProgress.tempScore))
					{
						GUILayout.Label(Localization.T_NEED_MORE_GOLD + " ( " + m_rocketControl.agility_upgrade_cost + " )");
					}
					else
					{
						if (MAX_ROCKET_AGILITY<m_rocketControl.max_agility || MIN_ROCKET_AGILITY>m_rocketControl.min_agility)
						{
							if (GUILayout.Button("( " + Localization.T_UPGRADE + " = $" + m_rocketControl.agility_upgrade_cost + " )"))
							{
								m_rocketControl.upgradeHullAgility();
								setAgility ();
							}
						}
					}
				GUILayout.EndHorizontal();
				GUILayout.Space(10);

				GUILayout.BeginHorizontal();
					GUILayout.BeginVertical();
						float log_maxEP;
						if (MAX_ROCKET_ENGINE_POWER==0)
						{
							log_maxEP = 0;
						}
						else
						{
							log_maxEP = Mathf.Log (MAX_ROCKET_ENGINE_POWER);
						}
						float log_minEP;
						if (MIN_ROCKET_ENGINE_POWER==0)
						{
							log_minEP = 0;
						}
						else
						{
							log_minEP = Mathf.Log (MIN_ROCKET_ENGINE_POWER);
						}
						float log_EP;
						if (rEnginePower==0)
						{
							log_EP = 0;
						}
						else
						{
							log_EP = Mathf.Log (rEnginePower);
						}
						GUILayout.Label(" - " + Localization.T_ENGINE_POWER + " (" + rEnginePower + ")");
						log_EP = GUILayout.HorizontalSlider(log_EP, log_minEP, log_maxEP);
						if (log_EP==0)
						{
							rEnginePower = 0;
						}
						else
						{
							rEnginePower = Mathf.Round (Mathf.Exp(log_EP));
						}
					GUILayout.EndVertical();
					if (m_engineControl.power_upgrade_cost > (GameProgress.score + GameProgress.tempScore))
					{
						GUILayout.Label(Localization.T_NEED_MORE_GOLD + " ( " + m_engineControl.power_upgrade_cost + " )");
					}
					else
					{
						if (MAX_ROCKET_ENGINE_POWER<m_engineControl.max_engine_power || MIN_ROCKET_ENGINE_POWER>m_engineControl.min_engine_power)
						{
							if (GUILayout.Button("( " + Localization.T_UPGRADE + " = $" + m_engineControl.power_upgrade_cost + " )"))
							{
								m_engineControl.upgradeEngine();
								setEnginePower ();
							}
						}
					}
				GUILayout.EndHorizontal();
			GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
			if (GUILayout.Button(Localization.T_REJECT))
			{
				exitMenu();
			}
			GUILayout.Space(20);
			if (GUILayout.Button(Localization.T_ACCEPT))
			{
				if (wc != null)
				{
					wc.GetRocket().SetActive(true);
					saveParameters();
					wc.chooseRocket(m_rocket);
					wc.chooseEngine(m_engine);
					if (m_rocket) Destroy(m_rocket);
					if (m_engine) Destroy(m_engine);
					wc.SaveScore();
					GameProgress.setBuyedEngine(m_engineControl.engineName, m_engineControl.m_info);
					GameProgress.setBuyedRocket(m_rocketControl.rocketName, m_rocketControl.m_info);
					mainMenu.showMapDiscriptionMenu();
				}
			}
		GUILayout.EndHorizontal();
		GUILayout.EndArea ();
	}

	private void saveParameters()
	{
		m_rocketControl.setMass (rMass);
		m_rocketControl.m_info.current_max_mass = MAX_ROCKET_MASS;
		m_rocketControl.m_info.current_min_mass = MIN_ROCKET_MASS;
		m_rocketControl.m_info.agility = rAgility;
		m_rocketControl.m_info.current_max_agility = MAX_ROCKET_AGILITY;
		m_rocketControl.m_info.current_min_agility = MIN_ROCKET_AGILITY;
		m_engineControl.setEnginePower (rEnginePower);
		m_engineControl.m_info.current_max_power = MAX_ROCKET_ENGINE_POWER;
		m_engineControl.m_info.current_min_power = MIN_ROCKET_ENGINE_POWER;
	}

	public void exitMenu()
	{
		WorldControl wc = WorldControl.GetInstance();
		if (wc != null)
		{
			wc.ClearTempScore();
			wc.GetRocket().SetActive(true);
		}
		if (m_rocket) Destroy(m_rocket);
		if (m_engine) Destroy(m_engine);
		mainMenu.showMapDiscriptionMenu();
	}
}
