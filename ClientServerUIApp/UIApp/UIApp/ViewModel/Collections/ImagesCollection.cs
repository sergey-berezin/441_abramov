using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UIApp.ViewModel.FileEntities;


namespace UIApp.ViewModel.Collections
{
    public class ImagesCollection : 
        Dictionary<string, ImageViewModel>, INotifyCollectionChanged, INotifyPropertyChanged, ICloneable
    {
        #region Private Fields

        private bool isEmpty;

        #endregion

        #region Constructor

        public ImagesCollection() : base() => IsEmpty = true;

        #endregion

        #region Properties

        // Необходимо проверять коллекцию на пустоту, чтобы знать, можно ли начать распознавание объектов
        public bool IsEmpty 
        { 
            get => isEmpty; 
            set
            {
                if (isEmpty != value)
                {
                    isEmpty = value;
                    OnPropertyChanged(nameof(IsEmpty));
                }
            }
        }

        public ImageViewModel this[int index]
        {
            get => Values.ToList()[index];
        }

        #endregion

        #region Events

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged; // отслеживает изменения IsEmpty

        #endregion

        #region Public Methods

        public new void Add(string key, ImageViewModel val)
        {
            base.Add(key, val);
            IsEmpty = false;
            OnCollectionChanged();
        }

        public new void Clear()
        {
            base.Clear();
            IsEmpty = true;
            OnCollectionChanged();
        }

        public object Clone()
        {
            var clone = new Dictionary<string, ImageViewModel>();
            foreach (var pair in this)
            {
                clone.Add(new string(pair.Key), new ImageViewModel(pair.Value.FullName));
            }
            return clone;
        }

        public void SimulateCollectionChanged() => OnCollectionChanged();

        #endregion

        #region Private Methods

        private void OnCollectionChanged()
        {
            NotifyCollectionChangedEventArgs args = 
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            CollectionChanged?.Invoke(this, args);
        }

        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));        

        #endregion

    }

}

