using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Transposicion.Models
{
    public class Vertical
    {

        public void Cipher(byte[] buffer, int column, int row,string path)
        {

            int count = 0;
            char cod = '$';
            byte bit = Convert.ToByte(cod);

            byte[,] matriz = new byte[row, column];

            for (int auxfila = 0; auxfila < row; auxfila++)
            {
                for (int auxcolumna = 0; auxcolumna < column; auxcolumna++)
                {
                    if (count < buffer.Length)
                    {
                        matriz[auxfila, auxcolumna] = buffer[count];
                    }
                    else
                    {
                        matriz[auxfila, auxcolumna] = bit;
                    }
                    count++;
                }
            }


            //RECORRIENDO MATRIZ PARA ESCRIBIR
            byte[] escribir = new byte[row * column];
            count = 0;
            for (int auxcolum = 0; auxcolum < column; auxcolum++)
            {
                for (int auxfila = 0; auxfila < row; auxfila++)
                {
                    escribir[count] = matriz[auxfila, auxcolum];
                    count++;
                }
            }

            //Escribir en un archivo
            EscribirCif(path, escribir);
        }

        public void EscribirCif(string path, byte[] escribir)
        {
            using (var stream = new FileStream(path, FileMode.Append))
            {

                using (BinaryWriter wr = new BinaryWriter(stream))
                {
                    wr.Write(escribir);

                }

                stream.Close();
            }
        }

       public void Decipher(byte[] buffer, int column, int row, string path)
        {
            int count = 0;
            char cod = '$';
            byte bit = Convert.ToByte(cod);
            char codq = ' ';
            byte bitq = Convert.ToByte(codq);

            byte[,] matriz = new byte[row, column];

            for (int auxcolumn = 0; auxcolumn < column; auxcolumn++)
            {
                for (int auxfila = 0; auxfila< row; auxfila++)
                {
                    if (count < buffer.Length)
                    {
                        matriz[auxfila, auxcolumn] = buffer[count];
                    }
                    else
                    {
                        matriz[auxfila, auxcolumn] = bit;
                    }
                    count++;
                }
            }


            //RECORRIENDO MATRIZ PARA ESCRIBIR
            byte[] escribir = new byte[row * column];
            count = 0;
            int quitar = 0;
         
            for (int auxfila= 0; auxfila < row; auxfila++)
            {
                for (int auxcolumna = 0; auxcolumna < column; auxcolumna++)
                {
                    
                    escribir[count] = matriz[auxfila, auxcolumna];
                    if (escribir[count] == bit)
                    {
                        quitar++;
                    }
                    count++;
                    

                }
            }
            for(int auxq=quitar; auxq > 0; auxq--)
            {
                escribir[(row * column) - auxq] = bitq;
            }
            //Escribir en un archivo
            EscribirCif(path, escribir);
        }
    }
}
