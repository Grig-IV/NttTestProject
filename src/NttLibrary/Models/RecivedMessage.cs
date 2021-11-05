namespace NttLibrary.Models
{
    public class RecivedMessage : ChatMessage
    {
        private bool _isReaded;

        public RecivedMessage(string text) : base(text)
        {
            IsReaded = false;
        }

        public bool IsReaded
        {
            get { return _isReaded; }
            set
            {
                _isReaded = value;
                NotifyPropertyChanged();
            }
        }
    }
}
