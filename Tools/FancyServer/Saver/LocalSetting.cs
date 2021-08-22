using System;
using System.IO;
using System.Collections.Generic;

using Newtonsoft.Json;

using FancyUtil;

using FancyServer.Logging;


namespace FancyServer.Saver {

    internal class LocalSetting {
        public enum Client {
            Nursery = 1,
            Image = 2,
        }

        private const string Json = "data.json";
        internal static LocalSetting Singleton { get; private set; }

        public event DataLoadedEventHandler DataLoaded;

        public delegate void DataLoadedEventHandler(object sender, DataLoadedEventArgs e);


        public LocalSetting() { Singleton = this; }

        public class DataLoadedEventArgs : EventArgs {
            public readonly Dictionary<NurseryAsset, object> nursery;

            public DataLoadedEventArgs(IReadOnlyDictionary<Client, object> clients) {
                nursery = clients[Client.Nursery] as Dictionary<NurseryAsset, object>;
            }
        }

        protected virtual void OnDataLoaded(DataLoadedEventArgs e) { DataLoaded?.Invoke(this, e); }

        public static void Write() {
            var clients = new Dictionary<Client, object> {
                [Client.Nursery] = NurseryCollector.Collect()
            };

            using (StreamWriter writer = new StreamWriter(Json)) {
                writer.WriteLine(JsonConvert.SerializeObject(clients));
            }
        }

        public void Read() {
            using (StreamReader reader = new StreamReader(Json)) {
                bool success = JsonUtil.ParseStruct(reader.ReadLine(), out Dictionary<Client, object> clients);

                if (success) {
                    OnDataLoaded(new DataLoadedEventArgs(clients));
                } else {
                    LogClerk.Fatal("Load settings failed");
                }
            }
        }
    }

}