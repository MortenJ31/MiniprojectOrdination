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
        // TODO: Implement!
        return false;
    }

    public override double doegnDosis() {
        //Hvis listen er tom, returerne vi 0, da der ikke er nogen doser givet
        if (dates.Count == 0)
        {
            return 0;
        }

        DateTime firstDate = dates.Min(d => d.dato);
        DateTime lastDate = dates.Max(d => d.dato);

        // Antal dage mellem første og sidste dato. +1 for at inkludere begge dage
        int totalAmountOfDays = (lastDate - firstDate).Days + 1;

        //Døgndosis
        return (getAntalGangeGivet() * antalEnheder) / totalAmountOfDays;
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
