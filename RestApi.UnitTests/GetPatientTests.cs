using System;
using System.Net;
using System.Web.Http;
using Autofac;
using NUnit.Framework;
using RestApi.Controllers;
using RestApi.DependencyInjection;
using RestApi.Interfaces;
using RestApi.Models;

namespace RestApi.UnitTests
{
    [TestFixture]
    public class GetPatientTests
    {
        private ILifetimeScope DiContainerScope
        {
            get
            {
                var applicationDependencyContainer = new ApplicationDependencyContainer(typeof(PatientsController).Assembly);
                applicationDependencyContainer.OverrideRegistration<IDatabaseContext, InMemoryPatientContext>();
                applicationDependencyContainer.Build();
                return applicationDependencyContainer.BeginLifetimeScope();
            }
        }

        private Patient Patient1
        {
            get
            {
                return new Patient
                    {
                        DateOfBirth = new DateTime(1972, 10, 27),
                        FirstName = "Millicent",
                        PatientId = 1,
                        LastName = "Hammond",
                        NhsNumber = "1111111111"
                    };
            }
        }

        private Episode Patient1Episode
        {
            get
            {
                return new Episode
                {
                    AdmissionDate = new DateTime(2014, 11, 12),
                    Diagnosis = "Irritation of inner ear",
                    DischargeDate = new DateTime(2014, 11, 27),
                    EpisodeId = 1,
                    PatientId = 1
                };
            }
        }

        [TestCase(1, false, TestName="If the endpoint is called with the ID of a patient that exists, status code 200 is returned along with the patient.")]
        [TestCase(2, true, TestName="If the endpoint is called with the ID of a patient that does not exist, status code 404 is returned.")]
        public void IfThePatientExistsItIsReturned(int patientId, bool shouldError)
        {
            var patient1 = Patient1;
            var patient1Episode = Patient1Episode;

            using (var scope = DiContainerScope)
            {
                var databaseContext = scope.Resolve<IDatabaseContext>();
                databaseContext.Patients.Add(patient1);
                databaseContext.Episodes.Add(patient1Episode);
                var controller = scope.Resolve<PatientsController>();

                Patient returnedPatient = null;

                try
                {
                    returnedPatient = controller.Get(patientId);
                }
                catch (HttpResponseException httpResponseException)
                {
                    if (shouldError)
                    {
                        Assert.AreEqual(HttpStatusCode.NotFound, httpResponseException.Response.StatusCode,
                                        "An incorrect status code was returned.");
                        return;
                    }
                    else
                    {
                        Assert.Fail("An HttpResponseException was thrown when it should not have been.");    
                    }
                }

                if (shouldError)
                {
                    Assert.Fail("An HttpResponseException was expected but not thrown.");
                }

                Assert.AreEqual(patient1, returnedPatient, "The patient returned by the API was not the correct one.");
            }
        }
    }
}
