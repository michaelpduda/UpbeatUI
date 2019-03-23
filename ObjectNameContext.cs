using System.Collections.Generic;

namespace UpbeatUI
{
    public class ObjectNameContext<T> : ObservableObject
    {
        private KeyValuePair<T, string> _keyValuePair;

        public ObjectNameContext(T target, string name)
        {
            Synchronize(new KeyValuePair<T, string>(target, name));
        }

        public ObjectNameContext(KeyValuePair<T, string> kvp)
        {
            Synchronize(kvp);
        }

        public string Name { get { return _keyValuePair.Value; } }
        public T Target { get { return _keyValuePair.Key; } }

        public void Synchronize(KeyValuePair<T, string> newModelObject)
        {
            _keyValuePair = newModelObject;
            RaisePropertyChanged(nameof(Name), nameof(Target));
        }

        public void Synchronize(T target, string name)
            => Synchronize(new KeyValuePair<T, string>(target, name));
    }
}