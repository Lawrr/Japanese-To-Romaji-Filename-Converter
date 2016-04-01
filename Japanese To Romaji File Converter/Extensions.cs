using System;
using System.Windows.Forms;

namespace Japanese_To_Romaji_File_Converter {
    public static class Extensions {

        /// <summary>
        /// Invoke thread-safe calls to windows forms controls.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="action"></param>
        public static void InvokeSafe(this Control control, Action action) {
            if (control != null && !control.IsDisposed) {
                if (control.InvokeRequired) {
                    control.Invoke(action);
                } else {
                    action();
                }
            }
        }

    }
}
