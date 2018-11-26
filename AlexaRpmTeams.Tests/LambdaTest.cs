using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace AlexaRpmTeams.Tests
{
    [TestClass]
    public class LambdaTest
    {
        private Function func;

        [TestInitialize]
        public void Initialize()
        {
            this.func = new Function();
        }

        [TestMethod]
        public void TestCustomexiIntent()
        {
            var request = new SkillRequest
            {
                Request = new IntentRequest
                {
                    Intent = new Intent
                    {
                        Name = "IntentPerson",
                        Slots = new Dictionary<string, Slot>
                        {
                            { "Role", new Slot{ Name = "Role", Value = "Project Manager" } },
                            { "Team", new Slot{ Name = "Team", Value = "Simulation" } },
                        }
                    },
                    Locale = "en-AU"
                }
            };

            var response = this.func.FunctionHandler(request, null);
            var speech = response.Response.OutputSpeech as PlainTextOutputSpeech;
            Assert.IsNotNull(speech);
            Assert.IsTrue(speech.Text.Contains("Andre Corea de Sa"));
        }
    }
}
