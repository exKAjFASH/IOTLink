using IOTLinkAddon.Common;
using IOTLinkAPI.Addons;
using IOTLinkAPI.Helpers;
using IOTLinkAPI.Platform.Events;
using System.Dynamic;
using System.Timers;

namespace IOTLinkAddon.Service
{
    public class ZoomStatusService : ServiceAddon
    {
        private System.Timers.Timer _monitorTimer;
        public override void Init(IAddonManager addonManager)
        {
            base.Init(addonManager);

            SetupTimers();

            OnAgentResponseHandler += OnAgentResponse;
        }

        private void SetupTimers()
        {
            if (_monitorTimer == null)
            {
                _monitorTimer = new System.Timers.Timer();
                _monitorTimer.Elapsed += new ElapsedEventHandler(OnMonitorTimerElapsed);
            }

            _monitorTimer.Stop();
            _monitorTimer.Interval = 10000;
            _monitorTimer.Start();

            LoggerHelper.Info("System monitor is activated.");
        }
        private void OnMonitorTimerElapsed(object source, ElapsedEventArgs e)
        {
            LoggerHelper.Debug("OnMonitorTimerElapsed: Started");

            dynamic addonData = new ExpandoObject();
            addonData.requestType = AddonRequestType.REQUEST_CHECK_ZOOM;
            GetManager().SendAgentRequest(this, addonData);

            LoggerHelper.Debug("OnMonitorTimerElapsed: Completed");
        }
        private void OnAgentResponse(object sender, AgentAddonResponseEventArgs e)
        {
            if (string.Compare(PlatformHelper.GetCurrentUsername().Trim().ToLowerInvariant(), e.Username.Trim().ToLowerInvariant()) == 0)
            {
                GetManager().PublishMessage(this, "ZoomStatus", (string)(e.Data.requestData));
            }
        }
    }
}
