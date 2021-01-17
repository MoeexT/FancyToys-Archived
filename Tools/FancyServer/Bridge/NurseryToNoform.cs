using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FancyServer.NotifyForm;

namespace FancyServer.Bridge
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
        public static void AddNurseryItem(string pathName)
        {
            NoForm.GetTheForm().AddItemToMenu(pathName);
        }

        public static void SetNurseryItemCheckState(string pathName, CheckState checkState)
        {
            NoForm.GetTheForm().SetNurseryItemCheckState(pathName, checkState);
        }
        public static void RemoveNurseryItem(string pathName)
        {
            NoForm.GetTheForm().RemoveNurseryItem(pathName);
        }
    }
}
