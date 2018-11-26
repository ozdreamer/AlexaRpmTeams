using System;
using System.Collections.Generic;

namespace AlexaRpmTeams
{
    public class LambdaResponse
    {
        public LambdaResponse()
        {
            this.IntentResponses = new List<IntentResponse>();
        }

        public List<IntentResponse> IntentResponses { get; set; }
        public string LaunchResponse { get; set; }
        public string SessionEndedResponse { get; set; }
        public string RepromptResponse { get; set; }
        public string InvalidResponse { get; set; }
        public string StopIntent { get; set; }
        public string CancelIntent { get; set; }

        [Serializable]
        public class IntentResponse
        {
            public string Intent { get; set; }
            public string Response { get; set; }
            public List<CustomResponse> CustomResponses { get; set; }
        }

        [Serializable]
        public class CustomResponse
        {
            public string Team { get; set; }
            public string Role { get; set; }
            public string Person { get; set; }
        }
    }
}