using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.Azure.DocumentDBStudio
{
    abstract class FeedNode : TreeNode
    {
        protected bool IsFirstTime = true;
        public abstract void ShowContextMenu(TreeView treeview, Point p);

        public abstract void Refresh(bool forceRefresh);
    }
}
