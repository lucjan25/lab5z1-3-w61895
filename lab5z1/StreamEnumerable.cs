using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System.Reflection;

namespace lab5z1
{
    public class StreamEnumerable<T> : IEnumerable<T>
    {
        private readonly StreamReader sr;

        public StreamEnumerable(StreamReader sr)
        {
            this.sr = sr;
        }

        protected Int64 start = 0;
        protected Int64 current = 0;
        private T data;
        /*
        private unsafe Int64 typelength<T>() where T : unmanaged 
        {
            return sizeof(T);
        }
        */

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }


    }

}
