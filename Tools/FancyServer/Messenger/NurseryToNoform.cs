using System.Windows.Forms;

using FancyServer.NotifyForm;


namespace FancyServer.Messenger {

    /// <summary>
    ///     Nursery
    ///       ↓          
    /// NurseryToNoform   
    ///       ↓          
    ///     NoForm
    /// </summary>
    internal static class NurseryToNoform {
        public static bool AddNurseryItem(string pathName) { return NoForm.Form.AddNurseryItem(pathName); }

        public static bool SetNurseryItemCheckState(string pathName, CheckState checkState) {
            return NoForm.Form.SetNurseryItemCheckState(pathName, checkState);
        }

        public static bool UpdateNurseryItem(string pathName, string processName) {
            return NoForm.Form.UpdateNurseryItem(pathName, processName);
        }

        public static bool RemoveNurseryItem(string pathName) { return NoForm.Form.RemoveNurseryItem(pathName); }
    }

}