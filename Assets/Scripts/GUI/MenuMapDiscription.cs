using UnityEngine;
using System.Collections;

public class MenuMapDiscription : MonoBehaviour {
	private Menu mainMenu;
	private MapInfo m_mapInfo;

	public void initMenu(Menu menu, MapInfo mapInfo)
	{
		m_mapInfo = mapInfo;
		mainMenu = menu;
		WorldControl wc = WorldControl.GetInstance();
		if (wc != null)
		{
			if (mapInfo.m_music)
			{
				wc.PlayMusic(mapInfo.m_music);
			}
			else
			{
				wc.PlayNextMusic();
			}
		}
	}

	public void showMenu()
	{
		WorldControl wc = WorldControl.GetInstance();
		GUILayout.BeginArea (new Rect (20, 20, Screen.width - 40, Screen.height - 40));
		GUILayout.BeginHorizontal();
			GUILayout.BeginVertical();
				GUILayout.Label("-== " + m_mapInfo.m_mapName.ToUpper() + " ==-");
				GUILayout.Space(20);
				GUILayout.BeginHorizontal();
					GUI.color = Color.white;
					GUILayout.Label(m_mapInfo.m_miniImage.texture);
					GUILayout.Space(10);
					GUILayout.BeginVertical();
					GUILayout.Label(m_mapInfo.m_mapIcon.texture);
						GUI.color = Color.green;
						GUILayout.Label("-== " + Localization.T_MAP_PARAMETERS + " ==-");
						GUILayout.Label("  " + Localization.T_GRAVITY.ToUpper() + ": " + m_mapInfo.m_mapGravity);
						GUILayout.Label("  " + Localization.T_DRAG.ToUpper() + ": " + m_mapInfo.m_mapDrag);
						GUILayout.Label("  " + Localization.T_FRICTION.ToUpper() + ": " + m_mapInfo.m_mapFriction);
						GUILayout.Label("  " + Localization.T_BOUNCINESS.ToUpper() + ": " + m_mapInfo.m_mapBounciness);
					GUILayout.EndVertical();
				GUILayout.EndHorizontal();
				GUILayout.Space(10);
				GUILayout.Label("-== " + Localization.T_MAP_DISCRIPTION + " ==-");
				GUILayout.Label(m_mapInfo.m_mapDiscription);
			GUILayout.EndVertical();
			GUILayout.Space(10);
			GUILayout.BeginVertical();
				if (wc != null)
				{
					GameObject rocket = wc.GetRocket();
					RocketControl rocketControl = rocket.GetComponent<RocketControl>();
					GUILayout.Label(rocketControl.rocketName.ToUpper());
					GUILayout.Space(10);
					GUILayout.Label(rocketControl.rocketSprite.texture);
					GUILayout.Label(Localization.T_ROCKET_PARAMETERS + ":");
					GUILayout.Label("  " + Localization.T_MASS.ToUpper() + ": " + rocketControl.m_info.mass);
					GUILayout.Label("  " + Localization.T_ENGINE_POWER_CUT.ToUpper() + ": " + rocketControl.getEngine().GetComponent<EngineControl>().getEnginePower());
					GUILayout.Label("  " + Localization.T_AGILITY_CUT.ToUpper() + ": " + rocketControl.m_info.agility);
					if (GUILayout.Button(Localization.T_ROCKET_EDIT))
					{
						mainMenu.menuRocketGarage.init(mainMenu, rocket);
						mainMenu.showGarageMenu();
					}
				}
			GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		GUILayout.Space(20);
		GUILayout.BeginHorizontal();
			if (GUILayout.Button(Localization.T_START_MAP))
			{
				if (wc != null)
				{
				wc.LoadLevel(0);// m_mapInfo.m_mapName);
				}
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
