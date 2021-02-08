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
    class NoformToOperation
    {
        public static void AddProcess(string pathName)
        {
            OperationClerk.AddProcess(pathName);
        }

        public static void StartProcess(string pathName)
        {
            OperationClerk.StartProcess(pathName, "");
        }

        public static void StopProcess(string pathName)
        {
            OperationClerk.StopProcess(pathName);
        }
    }
}
