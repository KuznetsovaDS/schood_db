using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    internal class subjects : ObservableCollection<object>
    {
        public void AddRange(IEnumerable<object> collection)
        {
            foreach (var item in collection)
            {
                Add(item);
            }
        }
    }
}