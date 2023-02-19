using UnityEngine;

public class TileFeatureChest : TileFeature
{
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
		return true;
	}
}
