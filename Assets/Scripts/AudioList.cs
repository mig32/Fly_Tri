using UnityEngine;
using System.Collections;

public static class AudioList 
{
	private static Hashtable audioList = new Hashtable();
	public static string currentAudioName;

	public static string getAudioPath(string name)
	{
		if (audioList.ContainsKey(name))
		{
			currentAudioName = name;
			return (string) audioList[name];
		}
		else
		{
			Debug.Log ("Audio with name:" + name + " doesnt exist");
			return null;
		}
	}

	public static int getAudioCount()
	{
		return audioList.Count;
	}

	public static bool addAudio(string name, string audioPath)
	{
		if (audioList.ContainsKey(name))
		{
			Debug.Log ("Audio with name:" + name + " allready exist");
			return false;
		}
		else
		{
			audioList.Add(name, audioPath);
			return true;
		}
	}

	public static void removeAudio(string name)
	{
		audioList.Remove (name);
	}

	public static void clearAudioList()
	{
		audioList.Clear ();
	}
	
	public static bool containsAudio(string name)
	{
		return audioList.ContainsKey(name);
	}

	public static string[] getAudioNamesList()
	{
		string[] result = new string[audioList.Count];
		int i = 0;
		foreach (string key in audioList.Keys)
		{
			result[i] = key;
			i++;
		}
		return result;
	}
}
