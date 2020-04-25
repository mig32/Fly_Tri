using UnityEngine;
using System.Collections;

public static class Localization {
	public static string T_NEW_GAME = "T_NEW_GAME?";
	public static string T_RESUME_GAME = "T_RESUME_GAME?";
	public static string T_RESTART_GAME = "T_RESTART_GAME?";
	public static string T_SOUND_VOLUME = "T_SOUND_VOLUME?";
	public static string T_FX_VOLUME = "T_FX_VOLUME?";
	public static string T_END_LEVEL = "T_END_LEVEL?";
	public static string T_SELECT_LEVEL = "T_SELECT_LEVEL?";
	public static string T_TEST_CHAMBER = "T_TEST_CHAMBER?";
	public static string T_SAVE = "T_SAVE?";
	public static string T_EXIT = "T_EXIT?";
	public static string T_BACT_TO_MENU = "T_BACT_TO_MENU?";
	public static string T_DELETE_SAVE = "T_DELETE_SAVE?";
	public static string T_YES = "T_YES?";
	public static string T_NO = "T_NO?";
	public static string T_DELETE_ALL_MESSAGE = "T_DELETE_ALL_MESSAGE?";

	public static string T_MAP_PARAMETERS = "T_MAP_PARAMETERS?";
	public static string T_GRAVITY = "T_GRAVITY?";
	public static string T_DRAG = "T_DRAG?";
	public static string T_FRICTION = "T_FRICTION?";
	public static string T_BOUNCINESS = "T_BOUNCINESS?";

	public static string T_ROCKET_PARAMETERS = "T_ROCKET_PARAMETERS?";
	public static string T_MASS = "T_MASS?";
	public static string T_AGILITY = "T_AGILITY?";
	public static string T_AGILITY_CUT = "T_AGILITY_CUT?";
	public static string T_ENGINE_POWER = "T_ENGINE_POWER?";
	public static string T_ENGINE_POWER_CUT = "T_ENGINE_POWER_CUT?";

	public static string T_MAP_DISCRIPTION = "T_MAP_DISCRIPTION?";
	public static string T_START_MAP = "T_START_MAP?";
	public static string T_BACK = "T_BACK?";
	public static string T_CLOSED = "T_CLOSED?";
	public static string T_COMPLITED = "T_COMPLITED?";

	public static string T_LOSE_MESSAGE = "T_LOSE_MESSAGE?";
	public static string T_WIN_MESSAGE = "T_WIN_MESSAGE?";

	public static string T_ROCKET_EDIT = "T_ROCKET_EDIT?";
	public static string T_SELECT_HULL = "T_SELECT_HULL?";
	public static string T_SELECT_ENGINE = "T_SELECT_ENGINE?";
	public static string T_ACCEPT = "T_ACCEPT?";
	public static string T_REJECT = "T_REJECT?";
	public static string T_UPGRADE = "T_UPGRADE?";
	public static string T_BUY = "T_BUY?";
	public static string T_SELECT = "T_SELECT?";
	public static string T_NEED_MORE_GOLD = "T_NEED_MORE_GOLD?";

	public static void selectLanguage(string lang)
	{
		if (lang == "RU" || lang == "ru")
		{
			T_NEW_GAME = "Начать игру";
			T_RESUME_GAME = "Продолжить";
			T_RESTART_GAME = "Начать заново";
			T_SOUND_VOLUME = "Громкость звука";
			T_FX_VOLUME = "Громкость спецэффектов";
			T_SELECT_LEVEL = "Выбрать карту";
			T_END_LEVEL = "Завершить раунд";
			T_TEST_CHAMBER = "Тестовая камера";
			T_SAVE = "Сохранить";
			T_EXIT = "Выход";
			T_BACT_TO_MENU = "Вернуться в меню";
			T_DELETE_SAVE = "Обнулить прогресс";
			T_YES = "Да, я понимаю, что я делаю";
			T_NO = "Нет, убрать эту менюшку";
			T_DELETE_ALL_MESSAGE = "Будет удален весь прогресс.\n - обнулены очки\n - закрыты карты\n - закрыты все ракеты и двигатели\nВ общем жми ДА и не сомневайся.";
			
			T_MAP_PARAMETERS = "Параметры карты:";
			T_GRAVITY = "гравитация";
			T_DRAG = "вязкость атмосферы";
			T_FRICTION = "сила трения";
			T_BOUNCINESS = "упругость поверхностей";
			
			T_ROCKET_PARAMETERS = "Параметры ракеты";
			T_MASS = "масса";
			T_AGILITY = "маневренность";
			T_AGILITY_CUT = "манев.";
			T_ENGINE_POWER = "мощность двигателей";
			T_ENGINE_POWER_CUT = "мощн.";

			T_MAP_DISCRIPTION = "Описание";
			T_START_MAP = "Запустить";
			T_BACK = "Назад";
			T_CLOSED = "ЗАКРЫТО";
			T_COMPLITED = "ПРОЙДЕНА";

			T_LOSE_MESSAGE = "О нет, ты разбил ракету!\nРемонт будет стоить ";
			T_WIN_MESSAGE = "Тебя ждет победа";

			T_ROCKET_EDIT = "Мастерская";
			T_SELECT_HULL = "Выбери корпус";
			T_SELECT_ENGINE = "Выбери двигатель";
			T_ACCEPT = "Принять";
			T_REJECT = "Отменить";
			T_UPGRADE = "Up";
			T_BUY = "Купить";
			T_SELECT = "Выбрать";
			T_NEED_MORE_GOLD = "Х";
		}
	}
}
