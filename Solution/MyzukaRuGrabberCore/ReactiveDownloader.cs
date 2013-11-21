using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MyzukaRuGrabberCore.DataModels;

namespace MyzukaRuGrabberCore
{
    public class ReactiveDownloader : IObservable<KeyValuePair<OneSongHeader, Exception>>
    {
        #region Fields

        private IObserver<KeyValuePair<OneSongHeader, Exception>> _observer;
        #endregion

        private ReactiveDownloader(CancellationToken CancToken)
        {
            
        }



        public IDisposable Subscribe(IObserver<KeyValuePair<OneSongHeader, Exception>> observer)
        {
            this._observer = observer;
            return null;
        }
    }
}
