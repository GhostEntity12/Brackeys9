using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
	TextMeshPro damageNumber;
	Rigidbody damageNumberRb;

	float clock = 0;
	bool active = false;

	private void Awake()
	{
		damageNumberRb = GetComponent<Rigidbody>();
		damageNumber = damageNumberRb.GetComponent<TextMeshPro>();
	}

	public void Trigger(int damageAmount)
	{
		ResetDamageNumber();
		damageNumber.text = damageAmount.ToString();
		damageNumberRb.useGravity = true;
		damageNumberRb.AddForceAtPosition(new(0.3f * (Random.value > 0.5f ? 1 : -1), 1f, 0f), damageNumberRb.transform.position, ForceMode.Impulse);
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
			ResetDamageNumber();
		}
	}

	void ResetDamageNumber()
	{
		damageNumber.text = string.Empty;
		damageNumber.transform.localPosition = Vector3.zero;
		damageNumberRb.velocity = Vector3.zero;
		damageNumberRb.useGravity = false;
	}
}
