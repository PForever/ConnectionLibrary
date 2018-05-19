namespace ConnectionLibrary.Abstract.DataObjects.Containers
{
    public class SynchronizeData<T>
    {
        private bool _isEmty = true;
        private SynchronizeResult _resultInfo;
        public SynchronizeResult ResultInfo => _resultInfo;
        public bool IsEmty
        {
            get => _isEmty;
            set
            {
                lock (this)
                {
                    if (!_isEmty) _resultInfo = SynchronizeResult.SyncLocked;
                    _isEmty = value;
                    _resultInfo = SynchronizeResult.Successful;
                }
            }
        }

        public SynchronizeResult Err => _resultInfo;
        private T _data;
        public T Data
        {
            get => _data;
            set
            {
                lock (this)
                {
                    if (!_isEmty) _resultInfo = SynchronizeResult.SyncLocked;
                    _data = value;
                    _isEmty = false;
                    _resultInfo = SynchronizeResult.Successful;
                }
            }
        }
    }
    public class SynchronizeData
    {
        private bool _isEmty = true;
        private SynchronizeResult _resultInfo;
        public SynchronizeResult ResultInfo => _resultInfo;

        public bool IsEmty
        {
            get => _isEmty;
            set
            {
                lock (this)
                {
                    if (!_isEmty) _resultInfo = SynchronizeResult.SyncLocked;
                    _isEmty = value;
                    _resultInfo = SynchronizeResult.Successful;
                }
            }
        }
    }
}