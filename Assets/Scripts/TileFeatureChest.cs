using UnityEngine;

public class TileFeatureChest : TileFeature
{
	[SerializeField]
	AudioSource audioSource;
	[SerializeField]
	AudioClip clip;
	public override bool Trigger(Unit u)
	{
		if (!base.Trigger(u) || u is not UnitPlayer p) return false;
		if (Random.value < 0.5f)
		{
			p.IncreaseOffence(1);
			p.TriggerGetItem(PlayerItemGet.Item.Sword);
		}
		else
		{
			p.IncreaseDefence(1);
			p.TriggerGetItem(PlayerItemGet.Item.Shield);
		}
		audioSource.PlayOneShot(clip);
		return true;
	}
}
