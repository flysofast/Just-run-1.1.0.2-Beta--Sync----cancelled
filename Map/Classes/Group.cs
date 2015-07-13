using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Map
{
    public class Group<T> : List<T>
    {
        public Group(string name, IEnumerable<T> items)
            : base(items)
        {
            this.Title = name;
        }

        public string Title
        {
            get;
            set;
        }
    }
}
