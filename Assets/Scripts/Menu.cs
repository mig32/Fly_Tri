using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {
	public int menu_height = 300;
	public int menu_width = 200;
	public float soundVolume = 8.0f;
	public float gameSpeed = 1.0f;

	public bool b_isShowMenu = true;
	public bool b_isNewGameButton = true;
	public bool b_isRestartGameButton = false;
	public bool b_isResumeGameButton = false;
	public bool b_isChooseLevelButton = false;
	public bool b_isSoundSlider = true;
	public bool b_isExitButton = true;

	public bool b_isChooseLevelMenu = false;
	public Vector2 scrollPosition = new Vector2(0,0);

	private LoadMap map_class;

	void OnGUI() {
		int menu_top = (Screen.height - menu_height)/2;
		int menu_left = (Screen.width - menu_width)/2;
		if (b_isShowMenu) 
		{
			b_isChooseLevelMenu = false;
			GUILayout.BeginArea (new Rect (menu_left, menu_top, menu_width, menu_height));
			if ((b_isNewGameButton) && (GUILayout.Button ("Новыя игра"))) {
				map_class = gameObject.GetComponent<LoadMap> ();
				map_class.startNewGame ();
				//Debug.Log ("new game");  
				ingameMenu();
				hideMenu();
			}
			if ((b_isRestartGameButton) && (GUILayout.Button ("Начать заного"))) {
				map_class = gameObject.GetComponent<LoadMap> ();
				map_class.restartLevel ();
				//Debug.Log ("new game");  
				hideMenu();
			}
			if (b_isResumeGameButton) {
				GUILayout.FlexibleSpace ();
				if (GUILayout.Button ("Продолжить")) {
					hideMenu();
				}
			}
			if (b_isChooseLevelButton) {
				GUILayout.FlexibleSpace ();
				if (GUILayout.Button ("Выбрать карту")) {
					showMapChooserMenu();
				}
			}
			if (b_isSoundSlider) {
				GUILayout.FlexibleSpace ();
				GUILayout.Label("Громкость звука");
				soundVolume = GUILayout.HorizontalScrollbar(soundVolume, 1.0F, 0.0F, 10.0F);
			}
			
			if (b_isExitButton) {
				GUILayout.FlexibleSpace ();
				if (GUILayout.Button ("Выход")) {
					Application.Quit ();
					//Debug.Log ("exit");  
				}
			}
			GUILayout.EndArea ();
		}

		if (b_isChooseLevelMenu) 
		{
			b_isShowMenu = false;
			GUILayout.BeginArea (new Rect (20, 20, Screen.width - 40, Screen.height - 40));
				scrollPosition = GUILayout.BeginScrollView(scrollPosition);
				map_class = gameObject.GetComponent<LoadMap>();
				GUILayout.BeginHorizontal();
					int i = 0;
					foreach (string map_name in map_class.getMapNames())
					{
						i++;
						if (GUILayout.Button (map_name))
						{
							b_isChooseLevelMenu = false;
							map_class.startNewGame(i);
							ingameMenu();
							hideMenu();
						}
					}
					GUILayout.EndHorizontal();
				GUILayout.EndScrollView();
			GUILayout.EndArea ();
		}
	}

	public void startMenu()
	{
		b_isNewGameButton = true;
		b_isRestartGameButton = false;
		b_isResumeGameButton = false;
	}

	public void ingameMenu()
	{
		b_isNewGameButton = false;
		b_isRestartGameButton = true;
		b_isResumeGameButton = true;
	}

	public void hideMenu()
	{
		b_isShowMenu = false;
		b_isChooseLevelMenu = false;
	}

	public void showMenu()
	{
		b_isShowMenu = true;
		b_isChooseLevelMenu = false;
	}

	public void showMapChooserMenu()
	{
		b_isShowMenu = false;
		b_isChooseLevelMenu = true;
	}

	void onStart () 
	{
		startMenu ();
	}

	void Update () 
	{
		if (Input.GetButtonDown ("Menu"))
		{
			if (b_isShowMenu) 
			{
				hideMenu();
			}
			else
			{
				showMenu();
			}
		}
		if (b_isShowMenu || b_isChooseLevelMenu) 
		{
			Time.timeScale = 0.0f;
		} 
		else 
		{
			Time.timeScale = gameSpeed;
		}
	}
}
