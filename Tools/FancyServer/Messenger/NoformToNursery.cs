using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FancyServer.Nursery;

namespace FancyServer.Messenger
{
    /// <summary>
    ///    Nursery
    ///       ↑         
    /// NoformToNursery   
    ///       ↑          
    ///    NoForm
    /// </summary>
    class NoformToNursery
    {
        public static void AddProcess(string pathName)
        {
            NurseryManager.AddProcess(pathName);
        }

        public static void StartProcess(string pathName)
        {
            NurseryManager.StartProcess(pathName, "");
        }

        public static void StopProcess(string pathName)
        {
            NurseryManager.StopProcess(pathName);
        }
    }
}
