using System.Reflection;
using System.Windows.Forms;

namespace Microsoft.Azure.DocumentDBStudio.Helpers
{
    public static class MenuItemHelper
    {
        /// <summary>
        ///     This method use reflection to set a custom shortcut key for MenuItem objects.
        ///     This is to allow setting short cut keys not available in the normal Shortcut enumeration.
        /// </summary>
        /// <param name="menuItem"></param>
        /// <param name="keys"></param>
        public static void SetCustomShortcut(MenuItem menuItem, Keys keys)
        {
            var customShortCut = (Shortcut)keys;
            var dataField = typeof(MenuItem).GetField("data", BindingFlags.NonPublic | BindingFlags.Instance);
            if (dataField == null) return;

            var updateMenuItemMethod = typeof(MenuItem).GetMethod("UpdateMenuItem", BindingFlags.NonPublic | BindingFlags.Instance);
            var menuItemDataShortcutField = typeof(MenuItem).GetNestedType("MenuItemData", BindingFlags.NonPublic).GetField("shortcut", BindingFlags.NonPublic | BindingFlags.Instance);
            if (menuItemDataShortcutField == null) return;

            var fieldData = dataField.GetValue(menuItem);
            menuItemDataShortcutField.SetValue(fieldData, customShortCut);
            updateMenuItemMethod.Invoke(menuItem, new object[] { true });
        }
    }
}
