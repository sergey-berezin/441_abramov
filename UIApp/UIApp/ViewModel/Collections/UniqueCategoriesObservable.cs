using System.Collections.Generic;
using System.Collections.Specialized;

namespace UIApp.ViewModel.Collections
{
    public class UniqueCategoriesObservable : SortedSet<string>, INotifyCollectionChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public new bool Add(string elem)
        {
            var result = base.Add(elem);
            OnCollectionChange();
            return result;
        }

        public override void Clear()
        {
            base.Clear();
            OnCollectionChange();
        }

        private void OnCollectionChange() =>
            CollectionChanged?.Invoke(this, 
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }


}