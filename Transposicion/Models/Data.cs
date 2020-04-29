using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Transposicion.Models
{
    public class Data
    {
        private static Data _instance = null;

        public static Data Instance
        {
            get
            {
                if (_instance == null) _instance = new Data();
                return _instance;
            }
        }

        public const int BufferLength = 1000;
    }
}
