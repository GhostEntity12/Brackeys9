using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
	Camera c;
	private Quaternion cache;

	[SerializeField]
	private bool doBillboard = true;

	private void Awake()
	{
		c = Camera.main;
		cache = transform.rotation;
	}

	// Update is called once per frame
	void Update()
	{
		transform.rotation = doBillboard ? c.transform.rotation : cache;
	}

	public void UpdateCache(Quaternion newCache) => cache = newCache;
}
