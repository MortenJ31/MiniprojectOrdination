namespace ordination_test;

using Data;
using Microsoft.EntityFrameworkCore;
using Service;
using shared.Model;

[TestClass]
public class ServiceTest
{
    private DataService service;

    [TestInitialize]
    public void SetupBeforeEachTest()
    {
        var optionsBuilder = new DbContextOptionsBuilder<OrdinationContext>();
        optionsBuilder.UseInMemoryDatabase(databaseName: "test-database");
        var context = new OrdinationContext(optionsBuilder.Options);
        service = new DataService(context);
        service.SeedData();
    }

    [TestMethod]
    public void PatientsExist()
    {
        Assert.IsNotNull(service.GetPatienter());
    }

    [TestMethod]
    public void OpretDagligFast()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        Assert.AreEqual(1, service.GetDagligFaste().Count());

        service.OpretDagligFast(patient.PatientId, lm.LaegemiddelId,
            2, 2, 1, 0, DateTime.Now, DateTime.Now.AddDays(3));

        Assert.AreEqual(2, service.GetDagligFaste().Count());
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void OpretDagligFastNullExceptionTest()
    {
        Patient patient = service.GetPatienter().First();
        service.OpretDagligFast(patient.PatientId, 0,
 2, 2, 1, 0, DateTime.Now, DateTime.Now.AddDays(3));
    }

    [TestMethod]
    public void DoegnDosisTest()
    {
        Laegemiddel laegemiddel = service.GetLaegemidler().First();

        // TC1: Negative dose - throws exception
        DagligFast tc1 = new DagligFast(DateTime.Now, DateTime.Now.AddDays(3), laegemiddel, 0, 0, -1, 0);
        Assert.ThrowsException<ArgumentException>(() => tc1.doegnDosis(), "TC1: Negative dose throw ArgumentException");

        // TC2: All zeros - throws exception
        DagligFast tc2 = new DagligFast(DateTime.Now, DateTime.Now.AddDays(3), laegemiddel, 0, 0, 0, 0);
        Assert.ThrowsException<ArgumentException>(() => tc2.doegnDosis(), "TC2: Alle parametre som nul throw ArgumentException");

        // TC3: Morning dose only (1, 0, 0, 0)
        DagligFast tc3 = new DagligFast(DateTime.Now, DateTime.Now.AddDays(3), laegemiddel, 1, 0, 0, 0);
        Assert.AreEqual(1, tc3.doegnDosis(), "TC3: Morgen dose only");

        // TC4: Noon dose only (0, 1, 0, 0)
        DagligFast tc4 = new DagligFast(DateTime.Now, DateTime.Now.AddDays(3), laegemiddel, 0, 1, 0, 0);
        Assert.AreEqual(1, tc4.doegnDosis(), "TC4: middag dose only");

        // TC5: Evening dose only (0, 0, 1, 0)
        DagligFast tc5 = new DagligFast(DateTime.Now, DateTime.Now.AddDays(3), laegemiddel, 0, 0, 1, 0);
        Assert.AreEqual(1, tc5.doegnDosis(), "TC5: Aften dose only");

        // TC6: Night dose only (0, 0, 0, 1)
        DagligFast tc6 = new DagligFast(DateTime.Now, DateTime.Now.AddDays(3), laegemiddel, 0, 0, 0, 1);
        Assert.AreEqual(1, tc6.doegnDosis(), "TC6: Nat dose only");

        // TC7: Multiple doses (2, 1, 4, 2)
        DagligFast tc7 = new DagligFast(DateTime.Now, DateTime.Now.AddDays(3), laegemiddel, 2, 1, 4, 2);
        Assert.AreEqual(9, tc7.doegnDosis(), "TC7: Multiple doses sum should be 9");

        // TC8: Large morning dose (14, 0, 0, 0)
        DagligFast tc8 = new DagligFast(DateTime.Now, DateTime.Now.AddDays(3), laegemiddel, 14, 0, 0, 0);
        Assert.AreEqual(14, tc8.doegnDosis(), "TC8: Stor morning dose");

        // TC9: Decimal doses (2.5, 4, 2, 2)
        DagligFast tc9 = new DagligFast(DateTime.Now, DateTime.Now.AddDays(3), laegemiddel, 2.5, 4, 2, 2);
        Assert.AreEqual(10.5, tc9.doegnDosis(), "TC9: Decimal dose sum 10.5");
    }

    // Test af constructor i DagligFast.cs
    [TestMethod]
    public void DagligFastStartDenStørreEndSlutDen()
    {
        Laegemiddel laegemiddel = service.GetLaegemidler().First();

        // TC2: Startdato er efter slutdato
        DateTime startDato = DateTime.Now.AddDays(5);
        DateTime slutDato = DateTime.Now.AddDays(2); 

        try
        {
            DagligFast dagligFast2 = new DagligFast(startDato, slutDato, laegemiddel, 1, 1, 1, 1);
            Assert.Fail("Exception når startdato er efter slutdato");
        }
        catch (Exception)
        {
            // Forventet exception blev kastet
        }
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void GetAnbefaletDosisPerDøgnThrowsExceptionOnWrongPatientId()
    {
        var result = service.GetAnbefaletDosisPerDøgn(123, 1);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void GetAnbefaletDosisPerDøgnThrowsExceptionOnWrongLaegemiddelId()
    {
        var result = service.GetAnbefaletDosisPerDøgn(1, 123);
    }
    
    [TestMethod]
    public void GetAnbefaletDosisPerDøgnWorks()
    {
        var result = service.GetAnbefaletDosisPerDøgn(1, 1);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void SamletDosis_TC1_EnDagMedFireDoser()
    {
        //Arrange
        var StartDato = DateTime.Now;
        var SlutDato = DateTime.Now;
        var laegemiddel = service.GetLaegemidler().First();
        var dagligFast = new DagligFast(StartDato, SlutDato, laegemiddel, 1, 1, 1, 1);

        //Act
        var samletDosis = dagligFast.samletDosis();

        //Assert
        Assert.AreEqual(4, samletDosis);
    }

    [TestMethod]
    public void SamletDosis_TC2_TreDageMedToMorgenDoser()
    {
        //Arrange
        var StartDato = new DateTime(2025, 5, 1);
        var SlutDato = new DateTime(2025, 5, 3);
        var laegemiddel = service.GetLaegemidler().First();
        var dagligFast = new DagligFast(StartDato, SlutDato, laegemiddel, 2, 0, 0, 0);

        //Act
        var samletDosis = dagligFast.samletDosis();

        //Assert
        Assert.AreEqual(6, samletDosis);
    }

    [TestMethod]
    public void SamletDosis_TC3_FemDageMedNulDoser()
    {
        //Arrange
        var StartDato = new DateTime(2025, 5, 1);
        var SlutDato = new DateTime(2025, 5, 5);
        var laegemiddel = service.GetLaegemidler().First();
        var dagligFast = new DagligFast(StartDato, SlutDato, laegemiddel, 0, 0, 0, 0);

        //Assert
        Assert.ThrowsException<ArgumentException>(() => dagligFast.samletDosis());
    }

    [TestMethod] 
    public void SamletDosis_TC4_TiDageMedNoejeDoser()
    {
        //Arrange
        var StartDato = new DateTime(2025, 5, 1);
        var SlutDato = new DateTime(2025, 5, 10);
        var laegemiddel = service.GetLaegemidler().First();
        var dagligFast = new DagligFast(StartDato, SlutDato, laegemiddel, 3, 2, 1, 4);

        //Act
        var samletDosis = dagligFast.samletDosis();

        //Assert
        Assert.AreEqual(100, samletDosis);
    }

    [TestMethod]
    public void SamletDosis_TC5_EnDagMedNulDoser()
    {
        //Arrange
        var StartDato = new DateTime(2025, 5, 1);
        var SlutDato = new DateTime(2025, 5, 1);
        var laegemiddel = service.GetLaegemidler().First();
        var dagligFast = new DagligFast(StartDato, SlutDato, laegemiddel, 0, 0, 0, 0);

        //Assert
        Assert.ThrowsException<ArgumentException>(() => dagligFast.samletDosis());
    }

    [TestMethod]
    public void SamletDosis_TC6_31DageMedFireDoser()
    {
        //Arrange
        var StartDato = new DateTime(2025, 5, 1);
        var SlutDato = new DateTime(2025, 5, 31);
        var laegemiddel = service.GetLaegemidler().First();
        var dagligFast = new DagligFast(StartDato, SlutDato, laegemiddel, 1, 1, 1, 1);

        //Act
        var samletDosis = dagligFast.samletDosis();

        //Assert
        Assert.AreEqual(124, samletDosis);
    }
}