namespace Helpers
{
    class ObserverManager
    {
        private event Action? _listObservers;
        private readonly Dictionary<int, Action?> _specificObservers = new();

        internal void AddListObserver(Action observer) => _listObservers += observer;

        internal void RemoveListObserver(Action observer) => _listObservers -= observer;

        internal void AddObserver(int id, Action observer)
        {
            if (_specificObservers.ContainsKey(id))
                _specificObservers[id] += observer;
            else
                _specificObservers[id] = observer;
        }

        internal void RemoveObserver(int id, Action observer)
        {
            if (_specificObservers.ContainsKey(id) && _specificObservers[id] is not null)
            {
                _specificObservers[id] -= observer;
                if (_specificObservers[id]?.GetInvocationList().Length == 0)
                    _specificObservers.Remove(id);
            }
        }

        internal void NotifyListUpdated()
        {
            _listObservers?.Invoke();
        }

        internal void NotifyItemUpdated(int id)
        {
            if (_specificObservers.ContainsKey(id))
                _specificObservers[id]?.Invoke();
        }
    }
}
