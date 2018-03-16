namespace EEAssistant.Modules
{
    interface IPortArgs
    {
        bool IsOpen { get; set; }

        RxHandler RxHandler { get; set; }
        TxHandler TxHandler { get; set; }
    }
}
