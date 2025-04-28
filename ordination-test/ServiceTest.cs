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
}