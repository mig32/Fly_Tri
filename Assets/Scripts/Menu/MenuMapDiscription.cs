using UnityEngine;
using System.Collections;

public class MenuMapDiscription : MonoBehaviour {
	private Menu mainMenu;
	private MapInfo m_mapInfo;

	public void initMenu(Menu menu, MapInfo mapInfo)
	{
		m_mapInfo = mapInfo;
		mainMenu = menu;
		if (mapInfo.music)
		{
			WorldControl.playMusic(mapInfo.music);
		}
		else
		{
			WorldControl.playNextMusic();
		}
	}

	public void showMenu()
	{
		GUILayout.BeginArea (new Rect (20, 20, Screen.width - 40, Screen.height - 40));
		GUILayout.BeginHorizontal();
			GUILayout.BeginVertical();
				GUILayout.Label("-== " + m_mapInfo.mapName.ToUpper() + " ==-");
				GUILayout.Space(20);
				GUILayout.BeginHorizontal();
					GUI.color = Color.white;
					GUILayout.Label(m_mapInfo.miniImage);
					GUILayout.Space(10);
					GUILayout.BeginVertical();
					GUILayout.Label(m_mapInfo.mapLogo);
						GUI.color = Color.green;
						GUILayout.Label("-== " + Localization.T_MAP_PARAMETERS + " ==-");
						GUILayout.Label("  " + Localization.T_GRAVITY.ToUpper() + ": " + m_mapInfo.mapGravity);
						GUILayout.Label("  " + Localization.T_DRAG.ToUpper() + ": " + m_mapInfo.mapDrag);
						GUILayout.Label("  " + Localization.T_FRICTION.ToUpper() + ": " + m_mapInfo.mapFriction);
						GUILayout.Label("  " + Localization.T_BOUNCINESS.ToUpper() + ": " + m_mapInfo.mapBounciness);
					GUILayout.EndVertical();
				GUILayout.EndHorizontal();
				GUILayout.Space(10);
				GUILayout.Label("-== " + Localization.T_MAP_DISCRIPTION + " ==-");
				GUILayout.Label(m_mapInfo.mapDiscription);
			GUILayout.EndVertical();
			GUILayout.Space(10);
			GUILayout.BeginVertical();
				GameObject rocket = WorldControl.getRocket ();
				RocketControl rocketControl = rocket.GetComponent<RocketControl>();
				GUILayout.Label(rocketControl.rocketName.ToUpper());
				GUILayout.Space(10);
				GUILayout.Label(rocket.GetComponent<SpriteRenderer>().sprite.texture);
				GUILayout.Label(Localization.T_ROCKET_PARAMETERS + ":");
				GUILayout.Label("  " + Localization.T_MASS.ToUpper() + ": " + rocketControl.m_info.mass);
				GUILayout.Label("  " + Localization.T_ENGINE_POWER_CUT.ToUpper() + ": " + rocketControl.getEngine().GetComponent<EngineControl>().getEnginePower());
				GUILayout.Label("  " + Localization.T_AGILITY_CUT.ToUpper() + ": " + rocketControl.m_info.agility);
				if (GUILayout.Button(Localization.T_ROCKET_EDIT))
				{
					mainMenu.menuRocketGarage.init(mainMenu, rocket);
					mainMenu.showGarageMenu();
				}
			GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		GUILayout.Space(20);
		GUILayout.BeginHorizontal();
			if (GUILayout.Button(Localization.T_START_MAP))
			{
				WorldControl.loadLevel(m_mapInfo.mapName);
				mainMenu.setMainMenuStatus(false);
				mainMenu.hideMenu();
			}
			GUILayout.Space(20);
			if (GUILayout.Button(Localization.T_BACK))
			{
				mainMenu.showMapChooserMenu();
			}
		GUILayout.EndHorizontal();
		GUILayout.EndArea ();
	}
}
