namespace shared.Model;

public class PN : Ordination {
	public double antalEnheder { get; set; }
    public List<Dato> dates { get; set; } = new List<Dato>();

    public PN (DateTime startDen, DateTime slutDen, double antalEnheder, Laegemiddel laegemiddel) : base(laegemiddel, startDen, slutDen) {
		this.antalEnheder = antalEnheder;
	}

    public PN() : base(null!, new DateTime(), new DateTime()) {
    }

    /// <summary>
    /// Registrerer at der er givet en dosis på dagen givesDen
    /// Returnerer true hvis givesDen er inden for ordinationens gyldighedsperiode og datoen huskes
    /// Returner false ellers og datoen givesDen ignoreres
    /// </summary>
    public bool givDosis(Dato givesDen) {
        //Først skal vi tjekke om datoen er indenfor perioden
        if (givesDen.dato >= startDen && givesDen.dato <= slutDen)
        {
            //så skal datoen tilføjes til listen, hvis den ikke allerede er tilføjet
            if (!dates.Any(d => d.dato == givesDen.dato))
            {
                dates.Add(givesDen);
            }
            return true;
        }
        //Returnerer false, hvis datoen er udenfor perioden
        return false;
    }

    public override double doegnDosis() {
    	// TODO: Implement!

        
        return -1;
    }


    public override double samletDosis() {
        return dates.Count() * antalEnheder;
    }

    public int getAntalGangeGivet() {
        return dates.Count();
    }

	public override String getType() {
		return "PN";
	}
}
