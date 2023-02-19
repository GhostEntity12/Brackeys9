using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoadType : MonoBehaviour
{
	private void Awake()
	{
		SceneLoadTypeData.Create();
	}

	public void SetLoadType(int i)
	{
		SceneLoadTypeData.GetInstance().loadType = (SceneLoadTypeData.LoadType)i;
	}
}

public class SceneLoadTypeData
{
	public enum LoadType { Normal, Endless }
	
	private static SceneLoadTypeData instance = null;

	public LoadType loadType;

	private SceneLoadTypeData() { }

	public static void Create()
	{
		instance ??= new SceneLoadTypeData();
	}
	public static SceneLoadTypeData GetInstance()
	{
		return instance;
	}

}
