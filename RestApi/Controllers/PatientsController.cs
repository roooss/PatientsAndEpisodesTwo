using System.Linq;
using System.Net;
using System.Web.Http;
using RestApi.Interfaces;
using RestApi.Models;

namespace RestApi.Controllers
{
    public class PatientsController : ApiController
    {
        private readonly IDatabaseContext _databaseContext;

        public PatientsController(IDatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        [HttpGet]
        public Patient Get(int patientId)
        {
            var patientsAndEpisodes =
                from p in _databaseContext.Patients
                join e in _databaseContext.Episodes on p.PatientId equals e.PatientId
                where p.PatientId == patientId
                select new {p, e};

            if (patientsAndEpisodes.Any())
            {
                var first = patientsAndEpisodes.First().p;
                first.Episodes = patientsAndEpisodes.Select(x => x.e).ToArray();
                return first;
            }

            throw new HttpResponseException(HttpStatusCode.NotFound);
        }
    }
}