using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace NttLibrary.Controls
{
    public partial class KebabButton : Button
    {
        public KebabButton()
        {
            InitializeComponent();

            LostFocus += (_, _) =>
            {
                if (PopupMenu != null)
                {
                    PopupMenu.IsOpen = false;
                }
            };
        }

        public Popup PopupMenu { get; set; }

        private void KebabButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (PopupMenu != null)
            {
                PopupMenu.IsOpen = !PopupMenu.IsOpen;
            }
        }
    }
}
