public class TileFeatureTown : TileFeature
{
	public override bool Trigger(Unit u)
	{
		if (!base.Trigger(u)) return false;
		u.HealDamage(3);
		return true;
	}
}
