using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab0Mod.Models
{
    public class Datacs
    {
        public List<Pelicula> lista_peliculas = new List<Pelicula>();
        private static Datacs _instance = null;


        public static Datacs Instance
        {
            get { if (_instance == null) { _instance = new Datacs(); } return _instance; }

        }
    }
}
