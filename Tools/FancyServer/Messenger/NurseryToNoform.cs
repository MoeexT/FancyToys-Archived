using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FancyServer.NotifyForm;

namespace FancyServer.Messenger
{
    /// <summary>
    ///     Nursery
    ///       ↓          
    /// NurseryToNoform   
    ///       ↓          
    ///     NoForm
    /// </summary>
    class NurseryToNoform
    {
        public static bool AddNurseryItem(string pathName)
        {
            return NoForm.GetTheForm().AddNurseryItem(pathName);
        }

        public static bool SetNurseryItemCheckState(string pathName, CheckState checkState)
        {
            return NoForm.GetTheForm().SetNurseryItemCheckState(pathName, checkState);
        }

        public static bool UpdateNurseryItem(string pathName, string processName)
        {
            return UpdateNurseryItem(pathName, processName);
        }

        public static bool RemoveNurseryItem(string pathName)
        {
            return NoForm.GetTheForm().RemoveNurseryItem(pathName);
        }
    }
}
