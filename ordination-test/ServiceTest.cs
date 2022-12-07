namespace ordination_test;

using Microsoft.EntityFrameworkCore;
using static shared.Util;

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
    public void OpretDagligSkaev()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel laegemiddel = service.GetLaegemidler().First();

        Assert.AreEqual(1, service.GetDagligSkæve().Count);

        service.OpretDagligSkaev(patient.PatientId, laegemiddel.LaegemiddelId, new Dosis[] {
                new Dosis(CreateTimeOnly(12, 0, 0), 0.5),
                new Dosis(CreateTimeOnly(12, 40, 0), 1),
                new Dosis(CreateTimeOnly(16, 0, 0), 2.5),
                new Dosis(CreateTimeOnly(18, 45, 0), 3)
            }, DateTime.Now, DateTime.Now.AddDays(3));

        Assert.AreEqual(2, service.GetDagligSkæve().Count());
    }


    [TestMethod]
    public void OpretPN()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel laegemiddel = service.GetLaegemidler().First();

        Assert.AreEqual(4, service.GetPNs().Count);

        service.OpretPN(patient.PatientId, laegemiddel.LaegemiddelId, 10, DateTime.Now, DateTime.Now.AddDays(3));

        Assert.AreEqual(5, service.GetPNs().Count());
    }


    [TestMethod]
    public void DagligFastMetoder()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        Assert.AreEqual(2, service.GetDagligFaste().Count());

        DagligFast dagligFast = service.OpretDagligFast(patient.PatientId, lm.LaegemiddelId,
            2, 2, 1, 0, DateTime.Now, DateTime.Now.AddDays(3));

        Assert.AreEqual(20, dagligFast.samletDosis());
        Assert.AreEqual(5, dagligFast.doegnDosis());
    }


    [TestMethod]
    public void AnbefaletEnheder()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel laegemiddel = service.GetLaegemidler().First();

        Assert.AreEqual(9.51, service.GetAnbefaletDosisPerDøgn(patient.PatientId, laegemiddel.LaegemiddelId));
    }

    
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void TestAtKodenSmiderEnException()
    {
        Patient patient = new Patient();
        Laegemiddel laegemiddel = service.GetLaegemidler().First();

        service.OpretPN(-1, laegemiddel.LaegemiddelId, 1, DateTime.Now, DateTime.Now.AddDays(3));

        Assert.AreEqual(10, service.GetPNs().Count());

        Console.WriteLine("Her kommer der ikke en exception. Testen fejler.");
    }
}