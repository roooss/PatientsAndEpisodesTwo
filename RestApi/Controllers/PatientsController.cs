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
        public PatientResult Get(string patientId)
        {
            PatientResult result = new PatientResult();
            result.IsSuccessful = false;
            result.ResultStatus = ResultStatusEnum.NotFound;

            int patientIdInt = 0;

            if (!int.TryParse(patientId.Trim(), out patientIdInt))
            {
                result.Message = "PatientId: " + patientId + " - is not a valid number";
                result.ResultStatus = ResultStatusEnum.NotANumber;
                return result;
            }

            if (patientIdInt <= 0)
            {
                result.Message = "PatientId: " + patientId + " - is not greater than zero";
                result.ResultStatus = ResultStatusEnum.ZeroOrLess;

                return result;
            }

            var patientsAndEpisodes =
                from p in _databaseContext.Patients
                join e in _databaseContext.Episodes on p.PatientId equals e.PatientId into patientEpisodes
                from e in patientEpisodes.DefaultIfEmpty()
                where p.PatientId == patientIdInt
                select new {p, e};

            if (patientsAndEpisodes.Any())
            {
                var first = patientsAndEpisodes.First().p;
                first.Episodes = patientsAndEpisodes.Select(x => x.e).ToArray();

                result.IsSuccessful = true;
                result.Message = "Patient " + patientId + " found.";
                result.ResultStatus = ResultStatusEnum.Successful;

                result.Patient = first;
            }
            else
            {
                result.Message = "Patient " + patientId + " not found in our records.";
            }

            return result;
        }
    }
}