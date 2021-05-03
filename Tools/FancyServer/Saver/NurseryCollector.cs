using System.Collections.Generic;

using FancyServer.Log;
using FancyServer.Nursery;
using static FancyServer.Saver.LocalSetting;

namespace FancyServer.Saver
{
    enum NurseryAsset
    {
        Processes = 1,
        FPName
    }
    class NurseryCollector
    {
        private static NurseryCollector collector;
        internal static NurseryCollector Collector { get => collector; }

        public NurseryCollector()
        {
            collector = this; 
            LocalSetting.TheOne.DataLoaded += Release;
        }


        public Dictionary<NurseryAsset, object> Collect()
        {
            return new Dictionary<NurseryAsset, object>
            {
                { NurseryAsset.Processes, ProcessManager.Processes },
                { NurseryAsset.FPName, ProcessManager.FPName }
            };
        }

        public void Release(object sender, DataLoadedEventArgs e)
        {
            foreach (KeyValuePair<NurseryAsset, object> pair in e.Nursery)
            {
                switch (pair.Key)
                {
                    case NurseryAsset.Processes:
                        ProcessManager.Processes = pair.Value as Dictionary<string, ProcessStruct>;
                        LogClerk.Info($"Loaded {NurseryAsset.Processes} successfully");
                        break;
                    case NurseryAsset.FPName:
                        ProcessManager.FPName = pair.Value as Dictionary<string, string>;
                        LogClerk.Info($"Loaded {NurseryAsset.FPName} successfully");
                        break;
                }
            }
        }
    }
}
