public class TileFeatureFarm : TileFeature
{
	public override bool Trigger(Unit u)
	{
		if (!base.Trigger(u)) return false;
		u.HealDamage(1);
		return true;
	}
}
