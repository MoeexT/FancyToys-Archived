using System.Collections.Generic;

using FancyServer.Logging;
using FancyServer.Nursery;

using static FancyServer.Saver.LocalSetting;


namespace FancyServer.Saver {

    internal enum NurseryAsset {
        Processes = 1,
        FPName = 2,
    }

    internal class NurseryCollector {
        internal static NurseryCollector Collector { get; private set; }

        public NurseryCollector() {
            Collector = this;
            Singleton.DataLoaded += Release;
        }


        public static Dictionary<NurseryAsset, object> Collect() {
            return new Dictionary<NurseryAsset, object> {
                {NurseryAsset.Processes, ProcessManager.Processes}, {NurseryAsset.FPName, ProcessManager.FPName}
            };
        }

        private static void Release(object sender, DataLoadedEventArgs e) {
            foreach (KeyValuePair<NurseryAsset, object> pair in e.nursery) {
                switch (pair.Key) {
                    case NurseryAsset.Processes:
                        ProcessManager.Processes = pair.Value as Dictionary<string, ProcessStruct>;
                        LogClerk.Info($"Loaded {NurseryAsset.Processes} successfully");
                        break;
                    case NurseryAsset.FPName:
                        ProcessManager.FPName = pair.Value as Dictionary<string, string>;
                        LogClerk.Info($"Loaded {NurseryAsset.FPName} successfully");
                        break;
                    default:
                        LogClerk.Warn($"No such name: {pair.Key}");
                        break;
                }
            }
        }
    }

}