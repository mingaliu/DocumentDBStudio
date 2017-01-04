using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.Azure.DocumentDBStudio
{
    abstract class FeedNode : TreeNode
    {
        protected bool IsFirstTime = true;
        public abstract void ShowContextMenu(TreeView treeview, Point p);

        public abstract void Refresh(bool forceRefresh);

        public abstract void HandleNodeKeyDown(object sender, KeyEventArgs keyEventArgs);

        public abstract void HandleNodeKeyPress(object sender, KeyPressEventArgs keyPressEventArgs);
    }
}
