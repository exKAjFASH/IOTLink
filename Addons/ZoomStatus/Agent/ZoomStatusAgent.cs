using IOTLinkAddon.Common;
using IOTLinkAPI.Addons;
using IOTLinkAPI.Helpers;
using IOTLinkAPI.Platform.Events;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.InteropServices;
using System.Text;


namespace IOTLinkAddon.Agent
{


    public class ZoomStatusAgent : AgentAddon
    {

        public override void Init(IAddonManager addonManager)
        {
            base.Init(addonManager);

            OnAgentRequestHandler += OnAgentRequest;

        }

        private void OnAgentRequest(object sender, AgentAddonRequestEventArgs e)
        {
            LoggerHelper.Verbose("ZoomStatusAgent::OnAgentRequest");

            AddonRequestType requestType = e.Data.requestType;
            switch (requestType)
            {
                case AddonRequestType.REQUEST_CHECK_ZOOM:
                    CheckZoom();
                    break;
                default: break;
            }
        }

        private void CheckZoom()
        {
            LoggerHelper.Verbose("ZoomStatusAgent::CheckZoom");
            dynamic addonData = new ExpandoObject();
            addonData.requestType = AddonRequestType.REQUEST_CHECK_ZOOM;
            addonData.requestData = "no";

            IReadOnlyList<int> windows = FindWindowByClassName("ZPContentViewWndClass");
            if (windows.Count > 0)
            {
                addonData.requestData = "yes";
            }
            GetManager().SendAgentResponse(this, addonData);

        }



        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder strText, int maxCount);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        // get class name of specific window
        [DllImport("user32.dll")]
        private static extern int GetClassName(int hWnd, StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindowVisible(IntPtr hWnd);


        // Delegate to filter which windows to include 
        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        /// <summary> Get the text for the window pointed to by hWnd </summary>
        public static string GetWindowText(IntPtr hWnd)
        {
            int size = GetWindowTextLength(hWnd);
            if (size > 0)
            {
                var builder = new StringBuilder(size + 1);
                GetWindowText(hWnd, builder, builder.Capacity);
                return builder.ToString();
            }

            return String.Empty;
        }

        /// <summary> Find all windows that match the given filter </summary>
        /// <param name="filter"> A delegate that returns true for windows
        ///    that should be returned and false for windows that should
        ///    not be returned </param>
        public static List<IntPtr> FindWindows(EnumWindowsProc filter)
        {
            IntPtr found = IntPtr.Zero;
            List<IntPtr> windows = new List<IntPtr>();

            EnumWindows(delegate (IntPtr wnd, IntPtr param)
            {
                if (filter(wnd, param))
                {
                    // only add the windows that pass the filter
                    windows.Add(wnd);
                }

                // but return true here so that we iterate all windows
                return true;
            }, IntPtr.Zero);

            return windows;
        }

        /// <summary> Find all windows that contain the given title text </summary>
        /// <param name="titleText"> The text that the window title must contain. </param>
        public static List<IntPtr> FindWindowsWithText(string titleText)
        {
            return FindWindows(delegate (IntPtr wnd, IntPtr param)
            {
                return GetWindowText(wnd).Contains(titleText);
            });
        }

        public static IReadOnlyList<int> FindWindowByClassName(string className)
        {
            var windowList = new List<int>();
            EnumWindows(delegate (IntPtr hwnd, IntPtr param)
            {
                if (!IsWindowVisible(hwnd)) return true;

                var lpString = new StringBuilder(512);
                GetClassName((int)hwnd, lpString, lpString.Capacity);
                if (lpString.ToString().Equals(className, StringComparison.InvariantCultureIgnoreCase))
                {
                    windowList.Add((int)hwnd);
                }

                return true;
            }, IntPtr.Zero);
            return windowList;

        }

    }
}
