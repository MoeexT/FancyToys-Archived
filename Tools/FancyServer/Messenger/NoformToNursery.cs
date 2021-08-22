
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
    internal static class NoformToOperation
    {
        public static void AddProcess(string pathName)
        {
            OperationClerk.AddProcess(pathName);
        }

        public static void StartProcess(string pathName)
        {
            OperationClerk.StartProcess(pathName);
        }

        public static void StopProcess(string pathName)
        {
            OperationClerk.StopProcess(pathName);
        }
    }
}
