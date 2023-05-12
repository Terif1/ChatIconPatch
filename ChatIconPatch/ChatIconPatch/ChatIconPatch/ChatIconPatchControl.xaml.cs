using System.Windows;
using System.Windows.Controls;

namespace ChatIconPatch
{
    public partial class ChatIconPatchControl : UserControl
    {

        private ChatIconPatch Plugin { get; }

        private ChatIconPatchControl()
        {
            InitializeComponent();
        }

        public ChatIconPatchControl(ChatIconPatch plugin) : this()
        {
            Plugin = plugin;
            DataContext = plugin.Config;
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            Plugin.Save();
        }
    }
}
