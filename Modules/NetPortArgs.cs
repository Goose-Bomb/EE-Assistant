using System.Runtime.Serialization;

namespace EEAssistant.Modules
{
    [DataContract]
    class NetPortArgs : IPortArgs
    {
        public static string[] ProtcolSelections { get; } = { "TCP Server", "TCP Client", "UDP" };

        public bool IsOpen { get; set; }

        public RxHandler RxHandler { get; set; }

        public TxHandler TxHandler { get; set; }

        [DataMember]
        public string Protcol { get; set; }

        [DataMember]
        public string LocalAddress { get; set; }

        [DataMember]
        public int LocalPort { get; set; }

        [DataMember]
        public string RemoteAddress { get; set; }

    }
}
