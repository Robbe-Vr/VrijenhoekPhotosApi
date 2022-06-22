using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VrijenhoekPhotos.Exchange.FactoryInterfaces
{
    public interface IIdentifier<T>
    {
        public T Id { get; set; }
    }
}
