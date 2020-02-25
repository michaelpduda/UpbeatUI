/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace UpbeatUI.Context
{
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        private static readonly Dictionary<string, PropertyChangedEventArgs> _eventArgCache = new Dictionary<string, PropertyChangedEventArgs>();

        public event PropertyChangedEventHandler PropertyChanged;

        public static PropertyChangedEventArgs GetPropertyChangedEventArgs(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentException(
                    "propertyName cannot be null or empty.");

            PropertyChangedEventArgs e;
            lock (_eventArgCache)
            {
                if (!_eventArgCache.ContainsKey(propertyName))
                    _eventArgCache.Add(propertyName, new PropertyChangedEventArgs(propertyName));
                e = _eventArgCache[propertyName];
            }

            return e;
        }

        protected void RaisePropertyChanged(params string[] values)
        {
            foreach (string value in values)
                RaisePropertyChanged(value);
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                PropertyChangedEventArgs e = GetPropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
    }
}