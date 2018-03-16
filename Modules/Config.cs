using System.IO;
using System.IO.Ports;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace EEAssistant.Modules
{
    [DataContract]
    class ConfigArgs
    {
        public static string[] EncodingSelections { get; } = { "GBK", "UTF-8", "Unicode", };

        [DataMember]
        public string EncodingName { get; set; }

        public Encoding Encoding
        {
            get
            {
                switch(EncodingName)
                {
                    case "GBK": return Encoding.Default;
                    case "UTF-8": return Encoding.UTF8;
                    case "Unicode": return Encoding.Unicode;
                    default: return Encoding.Default;
                }
            }
            set
            {
                if (value == Encoding.Default) EncodingName = "GBK";
                else if (value == Encoding.UTF8) EncodingName = "UTF-8";
                else if (value == Encoding.Unicode) EncodingName = "Unicode";
            }
        }

        [DataMember]
        public SerialPortArgs SerialPortArgs { get; set; }

        //[DataMember]
        //public NetPortArgs NetPortArgs { get; set; }
    }

    static class Config
    {
        public static ConfigArgs Args { get; set; }
        private static DataContractJsonSerializer jsonSerializer;

        public static void Load()
        {
            jsonSerializer = new DataContractJsonSerializer(typeof(ConfigArgs));

            try
            {
                var fs = new FileStream("config.json", FileMode.Open, FileAccess.Read);
                Args = jsonSerializer.ReadObject(fs) as ConfigArgs;
                fs.Close();
            }
            catch
            {
                Args = new ConfigArgs
                {
                    Encoding = Encoding.Default,

                    SerialPortArgs = new SerialPortArgs
                    {
                        BaudRate = 115200,
                        DataBits = 8,
                        Parity = Parity.None,
                        StopBits = StopBits.One,

                        TxHandler = new TxHandler
                        {
                            IsAutoTiming = false,
                            HasAddtionalLineBreak = false,
                        },

                        RxHandler = new RxHandler
                        {
                            IsHexDisplay = false,
                        }
                    },

                    /*
                    NetPortArgs = new NetPortArgs
                    {
                        LocalAddress = "192.168.1.1",
                        LocalPort = 8081,
                    },
                    */
                };
            }
        }

        public static void Save()
        {
            var fs = new FileStream("config.json", FileMode.Create, FileAccess.Write);
            jsonSerializer.WriteObject(fs, Args);
            fs.Close();
        }
    }
}
