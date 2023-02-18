using UnityEngine;

public class Billboard : MonoBehaviour
{
	private Camera cam;
	private Quaternion cache;

	[SerializeField]
	private bool doBillboard = true;

	private void Awake()
	{
		cam = Camera.main;
		cache = transform.rotation;
	}

	// Update is called once per frame
	void Update()
	{
		transform.rotation = doBillboard ? cam.transform.rotation : cache;
	}

	public void UpdateCache(Quaternion newCache) => cache = newCache;
}
