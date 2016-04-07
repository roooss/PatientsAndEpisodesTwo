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

        private Patient Patient3
        {
            get
            {
                return new Patient
                {
                    DateOfBirth = new DateTime(1972, 10, 27),
                    FirstName = "Millicent",
                    PatientId = 3,
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

        [TestCase("1", false, TestName = "If the endpoint is called with the ID (1) of a patient that exists, status code 200 is returned along with the successful patient result.")]
        [TestCase("2", true, TestName = "If the endpoint is called with the ID (2) of a patient that does not exist, status code 200 is returned alon with an unsuccessful patient result (Not Found).")]
        [TestCase("a", true, TestName = "If the endpoint is called with the ID (a) of a patient that does not exist, status code 200 is returned alon with an unsuccessful patient result (Not A Number).")]
        [TestCase("-1", true, TestName = "If the endpoint is called with the ID (-1) of a patient that does not exist, status code 200 is returned alon with an unsuccessful patient result (Zero Or Less).")]
        [TestCase("0", true, TestName = "If the endpoint is called with the ID (0) of a patient that does not exist, status code 200 is returned alon with an unsuccessful patient result (Zero Or Less).")]
        [TestCase("3", false, TestName = "If the endpoint is called with the ID (3) of a patient that exists, status code 200 is returned along with the successful patient result")]
        public void IfThePatientExistsItIsReturned(string patientId, bool shouldError)
        {
            var patient1 = Patient1;
            var patient3 = Patient3;
            var patient1Episode = Patient1Episode;

            using (var scope = DiContainerScope)
            {
                var databaseContext = scope.Resolve<IDatabaseContext>();
                databaseContext.Patients.Add(patient1);
                databaseContext.Patients.Add(patient3);
                databaseContext.Episodes.Add(patient1Episode);
                var controller = scope.Resolve<PatientsController>();

                PatientResult returnedPatient = null;

                try
                {
                    returnedPatient = controller.Get(patientId);
                }
                catch (HttpResponseException)
                {
                    Assert.Fail("An HttpResponseException was thrown when it should not have been.");

                }

                if (shouldError)
                {
                    Assert.IsFalse(returnedPatient.IsSuccessful);

                    switch (patientId)
                    {
                        case "2":
                            Assert.AreEqual((int)ResultStatusEnum.NotFound, (int)returnedPatient.ResultStatus, "Expexted not found status (1).");
                            break;
                        case "a":
                            Assert.AreEqual((int)ResultStatusEnum.NotANumber, (int)returnedPatient.ResultStatus, "Expexted not a number status (2).");
                            break;
                        case "0":
                            Assert.AreEqual((int)ResultStatusEnum.ZeroOrLess, (int)returnedPatient.ResultStatus, "Expexted zero or less status (3).");
                            break;
                        case "-1":
                            Assert.AreEqual((int)ResultStatusEnum.ZeroOrLess, (int)returnedPatient.ResultStatus, "Expexted zero or less status (3).");
                            break;
                        default:
                            Assert.Fail("Expected a negative status, one was not returned.");
                            break;
                    }

                    return;
                }

                var patientToAssertWith = patient1;

                if (patientId.Equals("3"))
                {
                    patientToAssertWith = patient3;
                }

                Assert.IsTrue(returnedPatient.IsSuccessful);
                Assert.AreEqual((int)ResultStatusEnum.Successful, (int)returnedPatient.ResultStatus, "Expexted successful status (0).");
                Assert.AreEqual(patientToAssertWith, returnedPatient.Patient, "The patient returned by the API was not the correct one.");
            }
        }
    }
}
