using NttLibrary.Utils;
using System;

namespace NttLibrary.Models
{
    public abstract class ChatMessage : Notifier
    {
        public ChatMessage(string text)
        {
            Text = text;
            Time = DateTime.Now;
        }

        public string Text { get; init; }

        /// <summary>
        /// Time of receiving for RecivedMessege and time of sending for SendedMessage.
        /// </summary>
        public DateTime Time { get; init; }
    }
}
