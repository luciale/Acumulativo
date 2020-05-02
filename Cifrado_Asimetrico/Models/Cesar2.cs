using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Cifrado_Asimetrico.Models
{
    public class Cesar2
    {
        List<byte> Mayus = new List<byte>(); //lista con valores mayuscula originales
        List<byte> Min = new List<byte>(); //lista con valores minuscula originales
        List<byte> Mayus_N = new List<byte>(); //lista con valores mayuscula nuevos
        List<byte> Min_N = new List<byte>(); //lista con valores minuscula nuevos
        byte[] cifrado;
        public void LlenarDiccionarios() //lleno listas originales
        {
            string ab = "abcdefghijklmnopqrstuvwxyz";
            for (int x = 0; x < ab.Length; x++)
            {
                Min.Add(Convert.ToByte(ab[x]));

            }
            ab = ab.ToUpper();
            for (int x = 0; x < ab.Length; x++)
            {
                Mayus.Add(Convert.ToByte(ab[x]));
            }
        }
        public void LlenarClaveInt(int clave)
        {
            for (int i = clave; i < Min.Count(); i++)
            {
                Mayus_N.Add(Mayus[i]);
                Min_N.Add(Min[i]);
            }
            for (int i = 0; i < clave; i++)
            {
                Mayus_N.Add(Mayus[i]);
                Min_N.Add(Min[i]);
            }

        }
        public void LlenarClave(string clave) //Llenar lista nueva con la clave
        {
            clave = clave.ToUpper(); //paso toda la clave a mayusculas
            //agrego la clave a la nueva lista
            for (int x = 0; x < clave.Length; x++)
            {
                Mayus_N.Add(Convert.ToByte(clave[x]));
            }
            //termino de llenar la lista
            for (int z = 0; z < Mayus.Count(); z++)
            {
                if (Mayus_N.Contains(Mayus[z]) == false)
                {
                    Mayus_N.Add(Mayus[z]);
                }
            }

            clave = clave.ToLower();// paso toda la clave a minusculas
            //agrego toda la clave a la nueva lista de minusculas
            for (int x = 0; x < clave.Length; x++)
            {
                Min_N.Add(Convert.ToByte(clave[x]));
            }
            //termino de llenar la lista minusculas
            for (int z = 0; z < Min.Count(); z++)
            {
                if (Min_N.Contains(Min[z]) == false)
                {
                    Min_N.Add(Min[z]);
                }
            }
        }

        public void Cifrar(byte[] buffer)
        {
            cifrado = new byte[buffer.Length];
            for (int x = 0; x < buffer.Length; x++)
            {
                if (Min.Contains(buffer[x]))
                {
                    for (int r = 0; r < Min.Count(); r++)
                    {
                        if (Min[r] == buffer[x])
                        {
                            cifrado[x] = Min_N[r];
                            break;
                        }
                    }
                }
                else if (Mayus.Contains(buffer[x]))
                {

                    for (int r = 0; r < Mayus.Count(); r++)
                    {
                        if (Mayus[r] == buffer[x])
                        {
                            cifrado[x] = Mayus_N[r];
                            break;
                        }
                    }
                }
                else
                {
                    cifrado[x] = buffer[x];
                }
            }
        }

        public void EscribirCifrado(string path)
        {
            using (var stream = new FileStream(path, FileMode.Append))
            {

                using (BinaryWriter wr = new BinaryWriter(stream))
                {
                    wr.Write(cifrado);

                }

                stream.Close();
            }

        }

        public void DesCifrar(byte[] buffer)
        {

            cifrado = new byte[buffer.Length];

            for (int x = 0; x < buffer.Length; x++)
            {
                if (Min_N.Contains(buffer[x]))
                {
                    for (int r = 0; r < Min.Count(); r++)
                    {
                        if (Min_N[r] == buffer[x])
                        {
                            cifrado[x] = Min[r];
                            break;
                        }
                    }
                }
                else if (Mayus_N.Contains(buffer[x]))
                {

                    for (int r = 0; r < Mayus.Count(); r++)
                    {
                        if (Mayus_N[r] == buffer[x])
                        {
                            cifrado[x] = Mayus[r];
                            break;
                        }
                    }
                }
                else
                {
                    cifrado[x] = buffer[x];
                }
            }
        }
    }
}
