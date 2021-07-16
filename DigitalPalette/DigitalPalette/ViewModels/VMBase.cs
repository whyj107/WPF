using System;
using System.ComponentModel;

namespace DigitalPalette.ViewModels
{
    public abstract class VMBase : INotifyPropertyChanged, IDisposable
    {
        protected VMBase() { }

        #region [INotifyPropertyChanged]
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
        #endregion

        #region [IDisposable]
        bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // 관리 리소스 해지
            }

            _disposed = true;
        }

        ~VMBase()
        {
            Dispose(false);
        }
        #endregion
    }
}
