namespace ordination_test;

using Microsoft.EntityFrameworkCore;

using Service;
using Data;
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
    [ExpectedException(typeof(ArgumentException))]
    public void doegnDosisIsLessThanEqualZero()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel laegemiddel = service.GetLaegemidler().First();
        DagligFast dagligFast = new DagligFast(DateTime.Now, DateTime.Now.AddDays(3), laegemiddel, 0, 0, -1, 0);
        dagligFast.doegnDosis();
        dagligFast.AftenDosis.antal = 0;
        dagligFast.doegnDosis();
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
        var StartDato = new DateTime(2025, 5, 1);
        var SlutDato = new DateTime(2025, 5, 1);
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

        //Act
        var samletDosis = dagligFast.samletDosis();

        //Assert
        Assert.AreEqual(0, samletDosis);
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

        //Act
        var samletDosis = dagligFast.samletDosis();

        //Assert
        Assert.AreEqual(0, samletDosis);
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