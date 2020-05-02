using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace Cifrado_Asimetrico.Models
{
    
        public class CifradoRSA
        {
            //CLase para generar llaves
            public int p = 0;
            public int q = 0;
            public int n = 0;
            public int z = 0;
            public int e = 0;
            public int d = 0;
            public bool VerificarPrimo(int num)
            {
                int i = 0;
                int a = 0;
                for (i = 1; i < (num + 1); i++)
                {
                    if (num % i == 0)
                    {
                        a++;
                    }
                }
                if (a != 2)
                {
                    return false;
                }
                else
                {
                    return true;
                }

            }

            public void GenerandoE()
            {
                for (int i = 2; i < z; i++)
                {
                    if (VerificarPrimo(i) && (z % i != 0))
                    {
                        e = i;
                        break;
                    }
                }
            }
            public void CalcularD()
            {
                int col1 = z;
                int col2 = z;
                int col11 = e; //cambiar a e
                int col12 = 1;
                int aux = 0;
                int aux1 = 0;
                int aux2 = 0;

                aux = col1 / col11;
                aux1 = col1 - (col11 * aux);
                aux2 = col2 - (col12 * aux);
                if (aux2 < 0)
                {
                    aux2 = z + aux2;
                }
                col1 = col11;
                col2 = col12;
                col11 = aux1;
                col12 = aux2;

                while (aux1 > 1)
                {
                    aux = col1 / col11;
                    aux1 = col1 - (col11 * aux);
                    aux2 = col2 - (col12 * aux);
                    if (aux2 < 0)
                    {
                        aux2 = z + aux2;
                    }
                    col1 = col11;
                    col2 = col12;
                    col11 = aux1;
                    col12 = aux2;

                }
                d = aux2;
            }



        public void EscribirArchPriv(string path) //escribimos por buffers
        {


            using (StreamWriter stream1 = new StreamWriter(path, true))
            {
                stream1.WriteLine(d.ToString() + "," + n.ToString());
            }

        }

       

        public void EscribirArchPubl(string path) //escribimos por buffers
        {
        

            using (StreamWriter stream1 = new StreamWriter(path, true))
            {
                stream1.WriteLine(e.ToString() + "," + n.ToString());
            }

        }



        public void Cifrar(string path, int bi)
        {


           
            BigInteger cd = BigInteger.Pow(bi,d);
           
            BigInteger p = cd % n;
            int re = (int)p;

            EscribirArch(path, re.ToString());
            
        }
        public int DesCifrar( int bi)
        {


           
            BigInteger cd = BigInteger.Pow(bi, e);

            BigInteger p = cd % n;
            int re = (int)p;

            return re;

        }




        public void EscribirArch(string path, string nu) //escribimos por buffers
        {

           

                using (StreamWriter stream1 = new StreamWriter(path, true))
                {
                    stream1.WriteLine(nu);
                }
               
                
            
            
        }


        //public void EscribirArchTXT() //escribimos por buffers
        //{
        //    string path = System.Web.Hosting.HostingEnvironment.MapPath("~/Cifrados/");
        //    string filePath = path + "archivo.txt";



        //    using (var stream = new FileStream(filePath, FileMode.Append))
        //    {
        //        using (BinaryWriter wr = new BinaryWriter(stream))
        //        {
        //            wr.Write(escribir);


        //        }
        //        stream.Close();
        //    }
        //    escribir = new byte[0];
        //}
    }
    }

