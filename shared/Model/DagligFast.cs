namespace shared.Model;
using static shared.Util;

public class DagligFast : Ordination {
	
	public Dosis MorgenDosis { get; set; } = new Dosis();
	public Dosis MiddagDosis { get; set; } = new Dosis();
	public Dosis AftenDosis { get; set; } = new Dosis();
	public Dosis NatDosis { get; set; } = new Dosis();

	public DagligFast(DateTime startDen, DateTime slutDen, Laegemiddel laegemiddel, double morgenAntal, double middagAntal, double aftenAntal, double natAntal) : base(laegemiddel, startDen, slutDen) {
		if (startDen > slutDen)
		{
			throw new ArgumentException("StartDen større end slutDen.");
		}
		if (morgenAntal + middagAntal + aftenAntal + natAntal == 0)
		{
			throw new ArgumentException("Alle parametre for DagligFast lig nul");
		}
		if (morgenAntal < 0 || middagAntal < 0 || aftenAntal < 0 || natAntal < 0)
		{
			throw new ArgumentException("Et eller flere parametre for DagligFast er under nul");  
		}
		MorgenDosis = new Dosis(CreateTimeOnly(6, 0, 0), morgenAntal);
		MiddagDosis = new Dosis(CreateTimeOnly(12, 0, 0), middagAntal);
		AftenDosis = new Dosis(CreateTimeOnly(18, 0, 0), aftenAntal);
		NatDosis = new Dosis(CreateTimeOnly(23, 59, 0), natAntal);
	}

	public DagligFast() : base(null!, new DateTime(), new DateTime()) {
	}

	public override double samletDosis() {
		
		return base.antalDage() * doegnDosis();
	}

	public override double doegnDosis()
	{
		var DosisInput = MorgenDosis.antal + MiddagDosis.antal + AftenDosis.antal + NatDosis.antal;
		if (DosisInput <= 0)
		{
			throw new ArgumentException("Samlet dose er 0 eller mindre.");
		}
		return DosisInput;
	}


	public Dosis[] getDoser() {
		Dosis[] doser = {MorgenDosis, MiddagDosis, AftenDosis, NatDosis};
		return doser;
	}

	public override String getType() {
		return "DagligFast";
	}
}
