using FancyServer.Nursery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyServer
{
    /// <summary>
    ///    Nursery ←------
    ///       ↓          |
    /// MessageManager   |
    ///       ↓          |
    ///    NoForm -------|
    /// </summary>
    partial class MessageManager
    {
        public static void NurseryAddProcess(string pathName)
        {
            NurseryManager.AddProcess(pathName, "");
        }

        public static void NurseryStartProcess(string pathName)
        {
            NurseryManager.StartProcess(pathName);
        }

        public static void NurseryStopProcess(string pathName)
        {
            NurseryManager.StopProcess(pathName);
        }
    }
}
