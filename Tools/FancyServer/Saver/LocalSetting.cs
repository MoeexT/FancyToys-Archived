using FancyServer.Log;
using FancyServer.Messenger;
using FancyServer.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyServer.Saver
{
    class LocalSetting
    {
        public enum Client
        {
            Nursery = 1,
            Image = 2,
        }

        public delegate void DataLoadedEventHandler(object sender, DataLoadedEventArgs e);
        public event DataLoadedEventHandler DataLoaded;
        private static LocalSetting theOne;
        public static string json = "data.json";

        internal static LocalSetting TheOne { get => theOne;}

        public LocalSetting()
        {
            theOne = this;
        }

        public class DataLoadedEventArgs: EventArgs
        {
            public readonly Dictionary<NurseryAsset, object> Nursery;
            public DataLoadedEventArgs(Dictionary<Client, object> clients)
            {
                Nursery = clients[Client.Nursery] as Dictionary<NurseryAsset, object>;
            }
        }

        protected virtual void OnDataLoaded(DataLoadedEventArgs e)
        {
            DataLoaded?.Invoke(this, e);
        }

        public static void Write()
        {
            var clients = new Dictionary<Client, object>
            {
                [Client.Nursery] = NurseryCollector.Collector.Collect()
            };
            using (StreamWriter writer = new StreamWriter(json))
            {
                writer.WriteLine(JsonConvert.SerializeObject(clients));
            }
        }

        public void Read()
        {
            using (StreamReader reader = new StreamReader(json))
            {
                bool success = JsonUtil.ParseStruct<Dictionary<Client, object>>(reader.ReadLine(), out Dictionary<Client, object> clients);
                if (success)
                {
                    OnDataLoaded(new DataLoadedEventArgs(clients));
                }
                else
                {
                    LogClerk.Fatal("Load settings failed");
                }
            }
        }
    }
}
