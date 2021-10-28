namespace UIApp.ViewModel
{
    public class ImagesSize : BaseViewModel
    {
        private Size _currentSize;

        public ImagesSize() => MakeNormal();           

        static public Size Normal { get; } = new Size(150, 150);
        static public Size Large { get; } = new Size(300, 300);

        public Size CurrentSize
        {
            get => _currentSize;
            set
            {
                if (_currentSize != value)
                {
                    _currentSize = value;
                    OnPropertyChanged(nameof(CurrentSize));
                }
            }
        }

        public void MakeNormal() => CurrentSize = Normal;

        public void MakeLarge() => CurrentSize = Large;

        public struct Size
        {
            public uint width;
            public uint height;

            public Size(uint h, uint w)
            {
                width = w;
                height = h;
            }

            public static bool operator ==(Size size1, Size size2)
            {
                return size1.width == size2.width && size1.height == size2.height;
            }

            public static bool operator !=(Size size1, Size size2)
            {
                return !(size1 == size2);
            }

            public override bool Equals(object size2)
            {
                return this == (Size)size2;
            }

            public override int GetHashCode()
            {
                return (height + width).GetHashCode();
            }
        }
    }

}

