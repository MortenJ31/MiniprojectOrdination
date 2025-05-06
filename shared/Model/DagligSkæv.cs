namespace shared.Model;

public class DagligSkæv : Ordination {
    public List<Dosis> doser { get; set; } = new List<Dosis>();

    public DagligSkæv(DateTime startDen, DateTime slutDen, Laegemiddel laegemiddel) : base(laegemiddel, startDen, slutDen) {
	}

    public DagligSkæv(DateTime startDen, DateTime slutDen, Laegemiddel laegemiddel, Dosis[] doser) : base(laegemiddel, startDen, slutDen) {
        this.doser = doser.ToList();
    }    

    public DagligSkæv() : base(null!, new DateTime(), new DateTime()) {
    }

	public void opretDosis(DateTime tid, double antal) {
        doser.Add(new Dosis(tid, antal));
    }

	public override double samletDosis() {
		return base.antalDage() * doegnDosis();
	}

	public override double doegnDosis()
	{
		var doegnDosis = doser.Sum(d => d.antal);
		
		if (doegnDosis <= 0)
		{
			throw new ArgumentException("Samlet dosis kan ikke være <= 0");
		}

		if (doser.Any(d => d.antal < 0))
		{
			throw new ArgumentException("Dosis kan ikke være negativ");
		}
		
		return doegnDosis;
	}

	public override String getType() {
		return "DagligSkæv";
	}
}