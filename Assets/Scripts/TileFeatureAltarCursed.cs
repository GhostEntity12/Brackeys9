public class TileFeatureAltarCursed : TileFeature
{
	public override bool Trigger(Unit u)
	{
		if (!base.Trigger(u) || u is not UnitPlayer p || p.Offence == p.Defence) return false;
		else if (p.Offence > p.Defence)
		{
			p.IncreaseOffence(1);
			p.IncreaseDefence(-1);
		}
		else
		{
			p.IncreaseOffence(-1);
			p.IncreaseDefence(1);
		}

		return true;
	}
}
