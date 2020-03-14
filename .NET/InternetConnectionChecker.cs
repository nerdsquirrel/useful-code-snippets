using System;
using System.Runtime.InteropServices.ComTypes;
using NETWORKLIST;

namespace Network.Helpers
{
    public class InternetConnectionChecker : INetworkListManagerEvents, IDisposable
    {
        private int _cookie;
        private bool _isMonitoring;
        private IConnectionPoint _connectionPoint;
        private readonly INetworkListManager _networkListManager;

        private InternetConnectionChecker()
        {
            _networkListManager = new NetworkListManager();
        }

        public static Lazy<InternetConnectionChecker> Instance = new Lazy<InternetConnectionChecker>(()=> new InternetConnectionChecker());

        public bool IsConnected()
        {
            return _networkListManager.IsConnectedToInternet;
        }

        public bool StartMonitoringConnection()
        {
            try
            {
                if (_isMonitoring)
                    return _isMonitoring;
                var container = _networkListManager as IConnectionPointContainer;
                if (container == null)
                    throw new Exception("connection container is null");
                var riid = typeof(INetworkListManagerEvents).GUID;
                container.FindConnectionPoint(ref riid, out _connectionPoint);
                _connectionPoint.Advise(this, out _cookie);
                _isMonitoring = true;
                return _isMonitoring;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool StopMonitoringConnection()
        {
            try
            {
                if (!_isMonitoring)
                    return true;
                _connectionPoint.Unadvise(_cookie);
                _isMonitoring = false;
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public void ConnectivityChanged(NLM_CONNECTIVITY newConnectivity)
        {
            if (_networkListManager.IsConnectedToInternet)
            {
                // do something based on internet connection
            }
                
        }

        public void Dispose()
        {
            _connectionPoint.Unadvise(_cookie);
        }
    }
}
