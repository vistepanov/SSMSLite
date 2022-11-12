using System;

namespace SsmsLite.Result
{
    public class ResultPlugin
    {
        private bool _isRegistered;
        //private readonly QueryTracker _queryTracker;
        //private readonly HistoryUi _historyUi;

        public ResultPlugin()   //QueryTracker queryTracker, HistoryUi historyUi)
        {
            //_queryTracker = queryTracker;
            //_historyUi = historyUi;
        }


        public void Register()
        {
            if (_isRegistered)
            {
                throw new Exception("HistoryPlugin is already registered");
            }

            _isRegistered = true;
            //_queryTracker.StartTracking();
            //_historyUi.Register();
        }
    }
}
