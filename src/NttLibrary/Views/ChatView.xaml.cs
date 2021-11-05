using System.Windows.Controls;

namespace NttLibrary.Views
{
    /// <summary>
    /// Represent Chat control.
    /// Contai chat head, message feed, message field with send button.
    /// </summary>
    public partial class ChatView : Grid
    {
        public ChatView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Property for custom contorol in right side of chat head.
        /// </summary>
        public Control TopRightCustomControl { get; set; }
    }
}
