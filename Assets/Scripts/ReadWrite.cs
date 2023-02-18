using System.IO;
using UnityEngine;

public static class ReadWrite
{
	public static SaveData Read()
	{
		string saveDataString;
		try
		{
			saveDataString = File.ReadAllText($"{Application.persistentDataPath}{Path.DirectorySeparatorChar}save.dat");
		}
		catch (FileNotFoundException)
		{
			saveDataString = Write(new SaveData());
		}
		return (SaveData)JsonUtility.FromJson(saveDataString, typeof(SaveData));
	}

	public static string Write(SaveData save)
	{
		string json = JsonUtility.ToJson(save);
		File.WriteAllText($"{Application.persistentDataPath}{Path.DirectorySeparatorChar}save.dat", json);
		return json;
	}
}

[System.Serializable]
public class SaveData
{
	public bool endlessModeUnlocked;
	public int bestScore;
}
