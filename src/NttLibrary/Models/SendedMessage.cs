using System;

namespace NttLibrary.Models
{
    public class SendedMessage : ChatMessage
    {
        public SendedMessage(string text, Exception capturedEx = null) : base(text)
        {
            SendingException = capturedEx;
        }

        public bool IsSenden => SendingException == null;

        public Exception SendingException { get; init; }
    }
}
