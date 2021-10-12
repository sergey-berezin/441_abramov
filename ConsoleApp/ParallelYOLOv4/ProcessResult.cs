using System.Collections.Generic;
using System.Linq;

namespace ParallelYOLOv4
{
    public struct ProcessResult
    {
        public string imageName;
        public IReadOnlyDictionary<string, int> categoriesCounts;

        public bool IsEmpty() => categoriesCounts.Count() == 0;
        public override string ToString() =>
            $"'{imageName}' contains next objects: {string.Join(", ", categoriesCounts.ToList().Select(pair => $"{pair.Key} x{pair.Value}"))}";
    }
}
