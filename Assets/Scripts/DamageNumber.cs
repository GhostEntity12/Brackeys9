using TMPro;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
	TextMeshPro text;
	Rigidbody rb;

	float clock = 0;
	bool active = false;

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
		text = GetComponent<TextMeshPro>();
	}

	public void Trigger(int damageAmount)
	{
		ResetValues();
		text.text = damageAmount.ToString();
		rb.useGravity = true;
		rb.AddForceAtPosition(new(0.3f * (Random.value > 0.5f ? 1 : -1), 1f, 0f), rb.transform.position, ForceMode.Impulse);
		active = true;
	}

	private void Update()
	{
		if (!active) return;

		clock += Time.deltaTime;
		if (clock > 0.5f)
		{
			clock = 0;
			active = false;
			ResetValues();
		}
	}

	void ResetValues()
	{
		text.text = string.Empty;
		text.transform.localPosition = Vector3.zero;
		rb.velocity = Vector3.zero;
		rb.useGravity = false;
	}
}
