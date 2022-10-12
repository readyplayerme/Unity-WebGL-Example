using System;

namespace ReadyPlayerMe
{
    public class CustomException : Exception
    {
        public readonly FailureType FailureType;

        public CustomException(FailureType failureType, string message) : base(message)
        {
            FailureType = failureType;
        }
    }
}
