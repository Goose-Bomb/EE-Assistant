using EEAssistant.Modules;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace EEAssistant.Helpers
{
    static class USBPortWatcher
    {
        private const int WM_DEVICE_CHANGE = 0x0219;
        private const int DBT_DEVICEARRIVAL = 0x8000;
        private const int DBT_DEVICE_REMOVE_COMPLETE = 0x8004;
        private const int DBT_DEVTYP_PORT = 0x0003;

        [StructLayout(LayoutKind.Sequential)]
        private struct DEV_BROADCAST_HDR
        {
            public int dbch_size;
            public int dbch_devicetype;
            public int dbch_reserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DEV_BROADCAST_PORT_Fixed
        {
            public int dbcp_size;
            public int dbcp_devicetype;
            public int dbcp_reserved;
        }

        public static void Init(Window window)
        {
            HwndSource hwndSource = PresentationSource.FromVisual(window) as HwndSource;
            hwndSource.AddHook(new HwndSourceHook(OnDeveiceChanged));

            foreach (var portName in System.IO.Ports.SerialPort.GetPortNames())
            {
                SerialPortArgs.AvailablePorts.Add(portName);
            }

            if (SerialPortArgs.AvailablePorts.Any())
            {
                Config.Args.SerialPortArgs.SelectedPortIndex = 0;
            }
        }

        public static IntPtr OnDeveiceChanged(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_DEVICE_CHANGE)        // 捕获USB设备的拔出消息WM_DEVICECHANGE
            {
                switch (wParam.ToInt32())
                {
                    case DBT_DEVICE_REMOVE_COMPLETE:    // USB拔出          
                        DEV_BROADCAST_HDR dbhdr = (DEV_BROADCAST_HDR)Marshal.PtrToStructure(lParam, typeof(DEV_BROADCAST_HDR));
                        if (dbhdr.dbch_devicetype == DBT_DEVTYP_PORT)
                        {
                            var portName = Marshal.PtrToStringUni((IntPtr)(lParam.ToInt32() + Marshal.SizeOf(typeof(DEV_BROADCAST_PORT_Fixed))));

                            SerialPortArgs.AvailablePorts.Remove(portName);
                            if (SerialPortArgs.AvailablePorts.Any())
                            {
                                Config.Args.SerialPortArgs.SelectedPortIndex = 0;
                            }

                            if (Config.Args.SerialPortArgs.IsOpen && SerialPortArgs.Instance.PortName == portName)
                            {
                                Config.Args.SerialPortArgs.IsOpen = false;
                            }
                        }
                        break;

                    case DBT_DEVICEARRIVAL:             // USB插入获取对应串口名称
                        dbhdr = (DEV_BROADCAST_HDR)Marshal.PtrToStructure(lParam, typeof(DEV_BROADCAST_HDR));
                        if (dbhdr.dbch_devicetype == DBT_DEVTYP_PORT)
                        {
                            var portName = Marshal.PtrToStringUni((IntPtr)(lParam.ToInt32() + Marshal.SizeOf(typeof(DEV_BROADCAST_PORT_Fixed))));

                            SerialPortArgs.AvailablePorts.Add(portName);

                            if (!Config.Args.SerialPortArgs.IsOpen)
                            {
                                Config.Args.SerialPortArgs.SelectedPortIndex = SerialPortArgs.AvailablePorts.IndexOf(portName);
                            }

                        }
                        break;
                }
            }
            return IntPtr.Zero;
        }
    }
}
