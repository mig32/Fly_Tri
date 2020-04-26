using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {
	private const int CAMERA_SHOW_MENU = 256;
	private const int CAMERA_SHOW_ALL = -1;
	public int menu_height = 450;
	public int menu_width = 300;
	public MenuChooseMap menuChooseMap;
	public MenuMapDiscription menuMapDiscription;
	public MenuRocketGarage menuRocketGarage;
	public MenuShop menuShop;

	private bool b_isShowMenu = true;
	private bool b_isChooseLevelMenu = false;
	private bool b_isTestChamberMenu = false;
	private bool b_isMapDiscriptionMenu = false;
	private bool b_isGarageMenu = false;
	private bool b_isShopMenu = false;
	private bool b_areYouShureMenu = false;

	private bool b_isStartMenuButtons = true;
	private bool b_isIngameButtons = false;
	private bool b_isTester = false;

	public void setMainMenuStatus(bool isStarMenu)
	{
		b_isStartMenuButtons = isStarMenu;
	}

	public void playMenuMusic()
	{
		WorldControl wc = WorldControl.GetInstance();
		if (wc != null)
		{
			wc.playMusic(Resources.Load(AudioList.getAudioPath(wc.getAudioNames()[0])) as AudioClip, 1.5f);
		}
	}

	void OnGUI() {
		GUI.color = Color.green;
		int menu_top = (Screen.height - menu_height)/2;
		int menu_left = (Screen.width - menu_width)/2;

		if (b_areYouShureMenu)
		{
			GUILayout.Label(Localization.T_DELETE_ALL_MESSAGE);
			GUILayout.BeginHorizontal();
				if (GUILayout.Button (Localization.T_YES)) 
				{
					GameProgress.Clear();
					b_areYouShureMenu = false;
				}
				if (GUILayout.Button (Localization.T_NO)) 
				{
					b_areYouShureMenu = false;
				}
			GUILayout.EndHorizontal();
		}
		else if (b_isShopMenu)
		{
			menuShop.showMenu();
		}

		else if (b_isGarageMenu)
		{
			menuRocketGarage.showMenu();
		}

		else if (b_isMapDiscriptionMenu) 
		{
			menuMapDiscription.showMenu();
		}

		else if (b_isChooseLevelMenu) 
		{
			menuChooseMap.showMenu();
		}

		else if (b_isShowMenu) 
		{
			GUILayout.BeginArea (new Rect (menu_left, menu_top, menu_width, menu_height));
			if (b_isStartMenuButtons)
			{//Или кнопка новая игра
			    if (GUILayout.Button (Localization.T_NEW_GAME)) 
				{
					menuChooseMap.init(this);
					showMapChooserMenu();
					b_isTestChamberMenu = false;
				}
				GUILayout.FlexibleSpace ();
				GUI.color = Color.red;
				if (GUILayout.Button (Localization.T_DELETE_SAVE)) 
				{
					b_areYouShureMenu = true;
				}
				GUI.color = Color.green;
			}
			else if (b_isIngameButtons)
			{//Или кнопки "Начать заново" и "Продолжить"
				
				if (GUILayout.Button (Localization.T_RESUME_GAME)) 
				{
					hideMenu();
				}
				GUILayout.FlexibleSpace ();
				if (GUILayout.Button (Localization.T_RESTART_GAME)) 
				{
					if (WorldControl.GetInstance() != null)
					{
						WorldControl.GetInstance().restartLevel();
					}
					hideMenu();
				}
				GUILayout.FlexibleSpace ();
				if (GUILayout.Button (Localization.T_END_LEVEL))
				{
					if (WorldControl.GetInstance() != null)
					{
						WorldControl.GetInstance().endLevel();
						WorldControl.GetInstance().stopTimer();
					}
					setMainMenuStatus(true);
					showMapDiscriptionMenu();
				}
			}
			else
			{
				if (GUILayout.Button (Localization.T_SELECT_LEVEL)) 
				{
					showMapChooserMenu();
				}
			}
			GUILayout.FlexibleSpace ();
			GUILayout.Label(Localization.T_SOUND_VOLUME);
			if (WorldControl.GetInstance() != null)
			{
				WorldControl.GetInstance().setSoundVolume(GUILayout.HorizontalSlider(WorldControl.GetInstance().getSoundVolume(), WorldControl.MIN_SOUND_VOLUME, WorldControl.MAX_SOUND_VOLUME));
			}
			GUILayout.Space(10);
			GUILayout.Label(Localization.T_FX_VOLUME);
			if (WorldControl.GetInstance() != null)
			{
				WorldControl.GetInstance().setFXVolume(GUILayout.HorizontalSlider(WorldControl.GetInstance().fxVolume, WorldControl.MIN_SOUND_VOLUME, WorldControl.MAX_SOUND_VOLUME));
			}
			GUILayout.FlexibleSpace ();
			if (b_isStartMenuButtons) 
			{//Если мы в клавном меню, то есть кнопки "Тестовая камера" и "Выход"
				if (b_isTester)
				{
					if (GUILayout.Button (Localization.T_TEST_CHAMBER))
					{
						TestChamberControl.loadTestChamber(this);
						hideMenu();
						b_isStartMenuButtons = false;
						b_isTestChamberMenu = true;
					}
					GUILayout.FlexibleSpace ();
				}
				if (GUILayout.Button (Localization.T_SAVE)) 
				{
					GameProgress.Save();
				}
				GUILayout.FlexibleSpace ();
				if (GUILayout.Button (Localization.T_EXIT)) 
				{
					GameProgress.Save();
					Application.Quit ();
				}
			}//Если мы в внутреигровом меню, то вместо них кнопка "Вернуться в меню"
			else if (GUILayout.Button (Localization.T_BACT_TO_MENU)) 
			{
				if (WorldControl.GetInstance() != null)
				{
					WorldControl.GetInstance().endLevel();
				}
				setMainMenuStatus(true);
				b_isTestChamberMenu = false;
				showMenu();
				playMenuMusic();
			}
			GUILayout.EndArea ();
		}

		else if (b_isTestChamberMenu)
		{
			TestChamberControl.showTestChamberMenu();
		}
	}

	public void hideMenu()
	{
		b_isStartMenuButtons = false;
		b_isShowMenu = false;
		b_isChooseLevelMenu = false;
		b_isMapDiscriptionMenu = false;
		b_isGarageMenu = false;
		b_isShopMenu = false;
		b_isIngameButtons = true;
		WorldControl wc = WorldControl.GetInstance();
		if (wc != null)
		{
			wc.getCamera().GetComponent<Camera>().cullingMask = CAMERA_SHOW_ALL;
			wc.unPause();
			wc.showScore();
		}
	}

	public void showMenu()
	{
		b_isShowMenu = true;
		b_isChooseLevelMenu = false;
		b_isMapDiscriptionMenu = false;
		b_isGarageMenu = false;
		b_isShopMenu = false;
		b_isIngameButtons = false;
		WorldControl wc = WorldControl.GetInstance();
		if (wc != null)
		{
			wc.hideScore();
		}
	}

	public void showMapChooserMenu()
	{
		b_isShowMenu = true;
		b_isChooseLevelMenu = true;
		b_isMapDiscriptionMenu = false;
		b_isGarageMenu = false;
		b_isShopMenu = false;
		b_isIngameButtons = false;
		WorldControl wc = WorldControl.GetInstance();
		if (wc != null)
		{
			wc.hideScore();
		}
	}

	public void showMapDiscriptionMenu()
	{
		b_isShowMenu = true;
		b_isChooseLevelMenu = true;
		b_isMapDiscriptionMenu = true;
		b_isGarageMenu = false;
		b_isShopMenu = false;
		b_isIngameButtons = false;
		WorldControl wc = WorldControl.GetInstance();
		if (wc != null)
		{
			wc.getCamera().GetComponent<Camera>().cullingMask = CAMERA_SHOW_MENU;
			wc.showScore();
			wc.pause();
		}
	}

	public void showGarageMenu()
	{
		b_isGarageMenu = true;
		b_isShopMenu = false;
		b_isIngameButtons = false;
	}

	public void showShopMenu()
	{
		b_isShopMenu = true;
		b_isIngameButtons = false;
	}

	public void onMenuButton()
	{
		if (b_areYouShureMenu) 
		{
			b_areYouShureMenu = false;
		} 
		else if (b_isShopMenu) 
		{
			menuShop.exitMenu();
		} 
		else if (b_isGarageMenu) 
		{
			menuRocketGarage.exitMenu();
		} 
		else if (b_isMapDiscriptionMenu) 
		{
			showMapChooserMenu();
		} 
		else if (b_isChooseLevelMenu) 
		{
			showMenu();
		} 
		else if (b_isShowMenu)
		{
			if (b_isIngameButtons) 
			{
				b_isShowMenu = false;
				WorldControl wc = WorldControl.GetInstance();
				if (wc != null)
				{
					wc.getCamera().GetComponent<Camera>().cullingMask = CAMERA_SHOW_ALL;
					wc.unPause();
				}
			}
		}
		else
		{
			b_isShowMenu = true;
			WorldControl wc = WorldControl.GetInstance();
			if (wc != null)
			{
				wc.hideScore();
				wc.getCamera().GetComponent<Camera>().cullingMask = CAMERA_SHOW_MENU;
				wc.pause();
			}
		}
	}

	void Update () 
	{
		//Debug.Log (WorldControl.getCamera ().camera.cullingMask);
		if (Input.GetButtonDown ("Cancel"))
		{
			onMenuButton();
		}
	}
}
