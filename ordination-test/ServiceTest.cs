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
    public void TestAtKodenSmiderEnException()
    {
        // Herunder skal man så kalde noget kode,
        // der smider en exception.

        // Hvis koden _ikke_ smider en exception,
        // så fejler testen.

        Console.WriteLine("Her kommer der ikke en exception. Testen fejler.");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void GetAnbefaletDosisPerDøgnThrowsExceptionOnWrongPatientId()
    {
        var result = service.GetAnbefaletDosisPerDøgn(123, 1);
    }
    
    [TestMethod]
    public void GetAnbefaletDosisPerDøgnWorks()
    {
        var result = service.GetAnbefaletDosisPerDøgn(1, 1);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void TestOrdinationAntalDage()
    {
        Laegemiddel lm = service.GetLaegemidler().First();
        Ordination ordination = new PN(new DateTime(2025, 5, 1), new DateTime(2025, 5, 1), 123, lm);
        
        // TC 1-4 (PN)
        Assert.AreEqual(1, ordination.antalDage());

        ordination = new PN(new DateTime(2025, 5, 1), new DateTime(2025, 5, 5), 123, lm);
        Assert.AreEqual(5, ordination.antalDage());

        ordination = new PN(new DateTime(2025, 5, 1), new DateTime(2025, 5, 30), 123, lm);
        Assert.AreEqual(30, ordination.antalDage());

        ordination = new PN(new DateTime(2025, 5, 1), new DateTime(2025, 4, 1), 123, lm);
        Assert.ThrowsException<ArgumentException>(() => ordination.antalDage());

        // TC 5-8
        ordination = new DagligFast(new DateTime(2025, 5, 1), new DateTime(2025, 5, 1), lm, 2, 0, 1, 0);
        Assert.AreEqual(1, ordination.antalDage());

        ordination = new DagligFast(new DateTime(2025, 5, 1), new DateTime(2025, 5, 5), lm, 2, 0, 1, 0);
        Assert.AreEqual(5, ordination.antalDage());

        ordination = new DagligFast(new DateTime(2025, 5, 1), new DateTime(2025, 5, 30), lm, 2, 0, 1, 0);
        Assert.AreEqual(30, ordination.antalDage());

        ordination = new DagligFast(new DateTime(2025, 5, 1), new DateTime(2025, 4, 1), lm, 2, 0, 1, 0);
        Assert.ThrowsException<ArgumentException>(() => ordination.antalDage());

        // TC 9-12
        ordination = new DagligSkæv(new DateTime(2025, 5, 1), new DateTime(2025, 5, 1), lm);
        Assert.AreEqual(1, ordination.antalDage());

        ordination = new DagligSkæv(new DateTime(2025, 5, 1), new DateTime(2025, 5, 5), lm);
        Assert.AreEqual(5, ordination.antalDage());

        ordination = new DagligSkæv(new DateTime(2025, 5, 1), new DateTime(2025, 5, 30), lm);
        Assert.AreEqual(30, ordination.antalDage());

        ordination = new DagligSkæv(new DateTime(2025, 5, 1), new DateTime(2025, 4, 1), lm);
        Assert.ThrowsException<ArgumentException>(() => ordination.antalDage());
    }
}