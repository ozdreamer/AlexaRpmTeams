using System;
using System.IO;
using System.Linq;
using System.Reflection;

using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Amazon.Lambda.Core;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace AlexaRpmTeams
{
    public class Function
    {
        private LambdaResponse lambdaResponse;

        public Function()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream("AlexaRpmTeams.Scripts.SkillResponses.json"))
            {
                using (var reader = new StreamReader(stream))
                {
                    this.lambdaResponse = JsonConvert.DeserializeObject<LambdaResponse>(reader.ReadToEnd());
                }
            }
        }

        /// <summary>
        /// Alexa skill handler function.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="context">The context.</param>
        /// <returns>Skill response.</returns>
        public SkillResponse FunctionHandler(SkillRequest request, ILambdaContext context)
        {
            switch (request.Request)
            {
                case LaunchRequest launchRequest:
                    return CreateResponse(this.lambdaResponse.LaunchResponse);

                case IntentRequest intentRequest:
                    return GetIntentResponse(intentRequest);

                case SessionEndedRequest sessionEndedRequest:
                    return CreateResponse(this.lambdaResponse.SessionEndedResponse, true);

                default:
                    return CreateResponse(this.lambdaResponse.RepromptResponse);
            }
        }

        private SkillResponse GetIntentResponse(IntentRequest request)
        {
            var intentResponse = this.lambdaResponse.IntentResponses.FirstOrDefault(x => x.Intent == request.Intent.Name);
            var shouldEndSession = request.Intent.Name == this.lambdaResponse.StopIntent || request.Intent.Name == this.lambdaResponse.CancelIntent;
            if (intentResponse != null)
            {
                switch (request.Intent.Name)
                {
                    case "IntentPerson":
                        if (request.Intent.Slots.TryGetValue("Team", out Slot team) && request.Intent.Slots.TryGetValue("Role", out Slot role))
                        {
                            var response = intentResponse.CustomResponses.FirstOrDefault(x => string.Equals(x.Team, team.Value, StringComparison.OrdinalIgnoreCase) && string.Equals(x.Role, role.Value, StringComparison.OrdinalIgnoreCase));
                            if (response != null)
                            {
                                var outputSpeech = $"{response.Role} of {response.Team} is {response.Person}.";
                                return CreateResponse(outputSpeech);
                            }
                        }
                        break;

                    default:
                        return CreateResponse(intentResponse.Response, shouldEndSession);
                }
            }

            return CreateResponse(this.lambdaResponse.InvalidResponse, shouldEndSession);
        }

        private SkillResponse CreateResponse(string outputSpeech, bool shouldEndSession = false, bool shouldReprompt = true)
        {
            var response = new ResponseBody
            {
                OutputSpeech = new PlainTextOutputSpeech { Text = outputSpeech },
                ShouldEndSession = shouldEndSession
            };

            if (shouldReprompt)
            {
                response.Reprompt = new Reprompt
                {
                    OutputSpeech = new PlainTextOutputSpeech { Text = this.lambdaResponse.RepromptResponse }
                };
            }

            return new SkillResponse
            {
                Response = response,
                Version = "1.0"
            };
        }
    }
}