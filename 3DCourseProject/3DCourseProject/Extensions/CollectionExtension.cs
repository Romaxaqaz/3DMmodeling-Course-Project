using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace _3DCourseProject.Extensions
{
    public static  class CollectionExtension
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> list) => new ObservableCollection<T>(list);

        public static T AddAndReturn<T>(this IList<T> list, T item)
        {
            list.Add(item);
            return item;
        }
    }
}
