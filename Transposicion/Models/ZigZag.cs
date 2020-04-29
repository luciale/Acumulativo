using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Transposicion.Models
{
    public class ZigZag
    {
        public List<ListZigZag> nivel = new List<ListZigZag>(); //Lista de los niveles y sus listas
        public List<int> caracteres_nivel = new List<int>(); //Lista con el numero de caracteres a leer en cada nivel
        int espacios = 0;
        int m = 0; //valor de m
        byte[] escribir;

        public int aux = 0;
        public void InicializarLista(int niveles) //Creo Niveles
        {
            nivel = new List<ListZigZag>();
            for (int x = 0; x < niveles; x++)
            {
                nivel.Add(new ListZigZag());
            }
        }

        public void LLenarNiveles(byte[] buffer, int niveles)
        {
            m = 0;
            char cod = '$';
            byte bit = Convert.ToByte(cod);
            int x = 0;
            int can = 0;
            while (x < buffer.Length)
            {
                //recorrer niveles abajo
                for (int y = 0; y < niveles; y++)
                {
                    if (y == 1)
                    {
                        can++;
                    }
                    if (x < buffer.Length)
                    {

                        nivel[y].bytes.Add(buffer[x]);
                        x++;
                    }
                    else
                    {

                        break;

                    }

                }
                //recorrer niveles arriba 
                for (int y = niveles - 2; y > 0; y--)
                {
                    if (x < buffer.Length)
                    {
                        nivel[y].bytes.Add(buffer[x]);
                        x++;
                    }
                    else
                    {
                        break;
                    }
                }

            }
            bool falta = false;
            m = nivel[nivel.Count() - 1].bytes.Count();

            for (int r = 1; r <= niveles - 2; r++)
            {
                int esp = nivel[r].bytes.Count();
                if (esp > (m * 2))
                {
                    falta = true;
                    break;
                }
            }
            if (falta == true)
            {
                nivel[niveles - 1].bytes.Add(bit);

                m = nivel[nivel.Count() - 1].bytes.Count();
            }

            while (nivel[0].bytes.Count() != m + 1)
            {
                nivel[0].bytes.Add(bit);

            }

            for (int r = 1; r <= niveles - 2; r++)
            {
                int esp = nivel[r].bytes.Count();
                if (esp != (m * 2))
                {
                    int ag = (m * 2) - esp;
                    for (int p = 0; p < ag; p++)
                    {
                        nivel[r].bytes.Add(bit);
                    }
                }
            }
        }

        public void CalcularEspacios(byte[] buffer, int niveles)
        {
            int div = buffer.Length % niveles;
            if (div != 0)
            {
                div = (buffer.Length / niveles) + 1;

                int con = div * niveles;
                espacios = con - buffer.Length;

            }

        }

        public void Cifrar()
        {
            List<byte> aux = new List<byte>();
            for (int x = 0; x < nivel.Count(); x++)
            {
                for (int y = 0; y < nivel[x].bytes.Count(); y++)
                {
                    aux.Add(nivel[x].bytes[y]);
                }
            }
            int tam = aux.Count();
            escribir = new byte[tam];
            for (int z = 0; z < tam; z++)
            {
                escribir[z] = aux[z];
            }
        }


        public void EscribirZigZag(string path)
        {
            using (var stream = new FileStream(path, FileMode.Append))
            {
                int extra = 0;
                if (escribir.Length > 1000)
                {
                    extra = escribir.Length - 1000;
                }
                byte[] ne = new byte[escribir.Length - extra];
                for (int x = 0; x < ne.Length; x++)
                {
                    ne[x] = escribir[x];
                }
                byte[] ne1 = new byte[extra];
                for (int x = 0; x < extra; x++)
                {
                    ne1[x] = escribir[1000 + x];
                }
                byte bit = Convert.ToByte(extra);
                char asc = Convert.ToChar(bit);
                using (BinaryWriter wr = new BinaryWriter(stream))
                {
                    wr.Write(ne);
                    wr.Write(Convert.ToByte(asc));// escribir el codigo en ascci;
                    wr.Write(ne1);

                }

                stream.Close();
            }
            escribir = new byte[0];
        }

        public void CalcularM(int niveles, int tam)
        {
            int medios = niveles - 2;
            aux = 0;
            decimal arr = Convert.ToDecimal(tam) + 1 + (2 * medios);
            decimal ab = (2 + (2 * medios));
            decimal ma = decimal.Divide(arr, ab);
            //decimal aum = Math.Round(ma);
            m = Convert.ToInt32(ma);
            int sumatoria = m + (m - 1) + (medios * (2 * (m - 1)));
            if (sumatoria != tam)
            {
                int aux1 = Math.Abs(sumatoria - tam);


                espacios = aux;


            }

        }
        public void AsignarCanNiveles(int niveles)
        {
            caracteres_nivel = new List<int>();

            caracteres_nivel.Add(m);
            for (int x = 0; x < niveles - 2; x++)
            {
                caracteres_nivel.Add(2 * (m - 1));
            }
            caracteres_nivel.Add(m - 1);
        }
        public void DesCifrar(int niveles, byte[] buffer)
        {
            char cod = '$';

            if (espacios != 0)
            {
                byte[] aux = new byte[buffer.Length + espacios];
                System.Buffer.BlockCopy(buffer, 0, aux, 0, buffer.Length);
                byte[] aux1 = new byte[espacios];
                for (int z = 0; z < espacios; z++)
                {
                    aux1[z] = Convert.ToByte(cod);
                }
                System.Buffer.BlockCopy(aux1, 0, aux, buffer.Length, aux1.Length);
                buffer = new byte[aux.Length];
                System.Buffer.BlockCopy(aux, 0, buffer, 0, aux.Length);
            }
            int p = 0;

            while (p < buffer.Length)
            {
                for (int x = 0; x < nivel.Count(); x++)
                {

                    for (int z = 0; z < caracteres_nivel[x]; z++)
                    {
                        if (p < buffer.Length)
                        {
                            nivel[x].bytes.Add(buffer[p]);
                            p++;
                        }

                    }
                }
            }
        }

        public void DesCifrarE(int niveles)
        {

            List<byte> auxi = new List<byte>();
            int medios = niveles - 2;
            for (int p = 0; p < m - 1; p++)
            {
                auxi.Add(nivel[0].bytes[p]); //recorriendo abajo
                for (int n = 1; n <= medios; n++)
                {
                    auxi.Add(nivel[n].bytes[p * 2]);
                }
                if (p < nivel[niveles - 1].bytes.Count())
                {
                    auxi.Add(nivel[niveles - 1].bytes[p]); //recorriendo arriba
                }

                for (int n = medios; n >= 1; n--)
                {
                    if (p < nivel[n].bytes.Count())
                    {
                        auxi.Add(nivel[n].bytes[(p * 2) + 1]); //recorriendo arriba
                    }

                }
            }

            auxi.Add(nivel[0].bytes[m - 1]);
            BorrarAgregados(auxi);

        }
        public void BorrarAgregados(List<byte> es)
        {
            int can = 0;
            char cod = '$';
            byte bit = Convert.ToByte(cod);

            for (int r = 0; r < es.Count(); r++)
            {
                if (es[r] == bit)
                {
                    can++;
                }
            }

            escribir = new byte[es.Count() - can];
            for (int z = 0; z < escribir.Length; z++)
            {
                escribir[z] = es[z];
            }

        }

        public void EscribirDesCifradoZ(string path)
        {
            using (var stream = new FileStream(path, FileMode.Append))
            {

                using (BinaryWriter wr = new BinaryWriter(stream))
                {
                    wr.Write(escribir);

                }

                stream.Close();
            }

            aux = 0;
        }
    }
}
