using UnityEngine;
using System.Collections;

public class MenuChooseMap : MonoBehaviour {
	private Menu mainMenu;
	private Texture2D[] m_icons;
	private Vector2 scrollPosition = new Vector2(0,0);

	public void init(Menu menu)
	{
		mainMenu = menu;
	}

	public void loadMapIcons(string[] mapNames)
	{
		int col = mapNames.Length;
		m_icons = new Texture2D[col];
		for (int i=0;i<col;i++)
		{
			m_icons[i] = Resources.Load(MapList.getMapLogoPath(mapNames[i])) as Texture2D;
		}
	}

	public void showMenu()
	{
		GUILayout.BeginArea (new Rect (20, 20, Screen.width - 40, 400));
			scrollPosition=GUILayout.BeginScrollView(scrollPosition);
			GUILayout.BeginHorizontal();
			int i = 0;
			foreach (string map_name in WorldControl.getMapsNames())
			{
				GUILayout.BeginVertical();
					if (GameProgress.maps.ContainsKey(map_name))
					{
						if (GameProgress.maps[map_name])
						{
							GUILayout.Label(Localization.T_COMPLITED);
						}
						else
						{
							GUILayout.Label(" ");
						}
						GUI.color = Color.white;
						if (GUILayout.Button(m_icons[i]))
						{
							MapInfo map_info = (Resources.Load(MapList.getMapPath(map_name)) as GameObject).GetComponent<MapInfo>();
							map_info.mapName = map_name;
							map_info.mapLogo = m_icons[i];
							mainMenu.menuMapDiscription.initMenu(mainMenu, map_info);
							mainMenu.showMapDiscriptionMenu();
						}
					}
					else
					{
						GUILayout.Label(Localization.T_CLOSED);
						GUILayout.Label(m_icons[i]);
					}
					
					GUI.color = Color.green;
					GUILayout.Label(map_name);
				GUILayout.EndVertical();
				i++;
			}
			GUILayout.EndHorizontal();
			GUILayout.EndScrollView();
			GUILayout.Space(30);
			GUILayout.BeginHorizontal ();
				if (GUILayout.Button(Localization.T_BACK))
				{
					mainMenu.showMenu();
				}
			GUILayout.EndHorizontal();
			GUILayout.EndArea ();
	}
}
