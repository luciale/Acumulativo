using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace Cifrado_Asimetrico.Models
{
    public class Diffie
    {
        int p = 107;
        int g = 43;
        public int priv = 0; //a
        public int k = 0;
        public int publ = 0; //A
        public void CalcularPubl()
        { 
            //encontrando K para poder cifrar
            BigInteger ka = BigInteger.Pow(publ,priv);
            ka = ka % p;
            k= (int)ka;
            //encontrando publica
            BigInteger pu = BigInteger.Pow(g, priv);
            pu = pu % p;
            publ = (int)pu;

           // EscribirArch(path, publ.ToString());
            //EscribirArch(pathpriv, k.ToString());

        }
        public void EscribirArch(string path, string pathpriv) //escribimos por buffers
        {

            using (StreamWriter stream1 = new StreamWriter(path, true))
            {
                stream1.WriteLine(publ);
            }
            using (StreamWriter stream1 = new StreamWriter(pathpriv, true))
            {
                stream1.WriteLine(priv);
            }
        }
    }
}
