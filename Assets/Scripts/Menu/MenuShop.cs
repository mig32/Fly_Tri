using UnityEngine;
using System.Collections;

public class MenuShop : MonoBehaviour {
	private Menu mainMenu;
	private string[] m_names;
	private EngineControl m_engineControl;
	private RocketControl m_rocketControl;
	private Vector2 scrollPosition = new Vector2 (0, 0);
	private bool b_isEngineShop;

	public void init(Menu menu, bool isEngine)
	{
		mainMenu = menu;
		b_isEngineShop = isEngine;
		if (b_isEngineShop)
		{
			m_names = WorldControl.getEnginesNames();
		}
		else
		{
			m_names = WorldControl.getRocketsNames();
		}
	}

	public void showMenu()
	{
		GUILayout.BeginArea (new Rect (20, 20, Screen.width - 40, 450));
			scrollPosition=GUILayout.BeginScrollView(scrollPosition);
				GUILayout.BeginHorizontal();
					int i = 0;
					foreach (string name in m_names)
					{
					if (b_isEngineShop)
					{
						m_engineControl = (Resources.Load(RocketList.getEnginePath(name)) as GameObject).GetComponent<EngineControl>();
					}
					else
					{
						m_rocketControl = (Resources.Load(RocketList.getRocketPath(name)) as GameObject).GetComponent<RocketControl>();
					}
						GUILayout.BeginVertical();
							GUILayout.Label(name);
							GUILayout.BeginHorizontal();
								GUI.color = Color.white;
								if (b_isEngineShop)
								{
									GUILayout.Label(m_engineControl.image);
								}
								else
								{
									GUILayout.Label(m_rocketControl.rocketSprite.texture);
								}
								GUI.color = Color.green;
								GUILayout.Space(10);
								GUILayout.BeginVertical();
									if (b_isEngineShop)
									{
										GUILayout.Label(Localization.T_ENGINE_POWER_CUT.ToUpper(), GUILayout.Width(60));
										GUILayout.Label("MIN: " + m_engineControl.min_engine_power);
										GUILayout.Label("MAX: " + m_engineControl.max_engine_power);
									}
									else 
									{
										GUILayout.Label(Localization.T_MASS.ToUpper(), GUILayout.Width(60));
										GUILayout.Label("MIN: " + m_rocketControl.min_mass);
										GUILayout.Label("MAX: " + m_rocketControl.max_mass);
										GUILayout.Space(10);
										GUILayout.Label(Localization.T_AGILITY_CUT.ToUpper());
										GUILayout.Label("MIN: " + m_rocketControl.min_agility);
										GUILayout.Label("MAX: " + m_rocketControl.max_agility);
									}
								GUILayout.EndVertical();
							GUILayout.EndHorizontal();
							if (b_isEngineShop)
							{
								if (GameProgress.buyedEngines.ContainsKey(name))
								{
									if (GUILayout.Button(Localization.T_SELECT))
									{
										GameObject newEngine = Resources.Load(RocketList.getEnginePath(name)) as GameObject;
										mainMenu.menuRocketGarage.setEngine(newEngine);
										mainMenu.showGarageMenu();
									}
								}
								else if (m_engineControl.cost <= GameProgress.score + GameProgress.tempScore)
								{
									if (GUILayout.Button(Localization.T_BUY + " ( " + m_engineControl.cost + " )"))
									{
										WorldControl.addScore(-m_engineControl.cost);
										GameObject newEngine = Resources.Load(RocketList.getEnginePath(name)) as GameObject;
										mainMenu.menuRocketGarage.setEngine(newEngine);
										mainMenu.showGarageMenu();
									}
								}
								else
								{
									GUILayout.Label(Localization.T_NEED_MORE_GOLD + " ( " + m_engineControl.cost + " )");
								}
							}
							else
							{
								if (GameProgress.buyedRockets.ContainsKey(name))
								{
									if (GUILayout.Button(Localization.T_SELECT))
									{
										GameObject newRocket = Resources.Load(RocketList.getRocketPath(name)) as GameObject;
										mainMenu.menuRocketGarage.setRocket(newRocket);
										mainMenu.showGarageMenu();
									}
								}
								else if (m_rocketControl.cost <= GameProgress.score + GameProgress.tempScore)
								{
									if (GUILayout.Button(Localization.T_BUY + " ( " + m_rocketControl.cost + " )"))
									{
										WorldControl.addScore(-m_rocketControl.cost);
										GameObject newRocket = Resources.Load(RocketList.getRocketPath(name)) as GameObject;
										mainMenu.menuRocketGarage.setRocket(newRocket);
										mainMenu.showGarageMenu();
									}
								}
								else
								{
									GUILayout.Label(Localization.T_NEED_MORE_GOLD + " ( " + m_rocketControl.cost + " )");
								}
							}
						GUILayout.EndVertical();
						i++;
					}
				GUILayout.EndHorizontal();
			GUILayout.EndScrollView();
			GUILayout.Space (20);
			if (GUILayout.Button(Localization.T_BACK))
			{
				exitMenu();
			}
		GUILayout.EndArea ();
	}

	public void exitMenu()
	{
		if (b_isEngineShop)
		{
			if (!GameProgress.buyedEngines.ContainsKey(mainMenu.menuRocketGarage.getEngineControl().engineName))
			{
				WorldControl.addScore(-mainMenu.menuRocketGarage.getEngineControl().cost);
			}
		}
		else
		{
			if (!GameProgress.buyedRockets.ContainsKey(mainMenu.menuRocketGarage.getRocketControl().rocketName))
			{
				WorldControl.addScore(-mainMenu.menuRocketGarage.getRocketControl().cost);
			}
		}
		mainMenu.showGarageMenu();
	}
}
