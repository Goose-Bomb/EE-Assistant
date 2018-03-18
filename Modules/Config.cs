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
        public static string[] EncodingSelections { get; } = { "ASCII", "GBK", "UTF-8", "Unicode", };

        public Encoding Encoding { get => Encoding.GetEncoding(CodePage); }

        [DataMember]
        public int CodePage { get; set; }

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
                    CodePage = 936,

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
