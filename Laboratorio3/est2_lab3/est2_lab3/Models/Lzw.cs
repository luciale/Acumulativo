using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace est2_lab3.Models
{
    public class Lzw
    {
        public string Name { get; set; } //Nombre nuevo del archivo
        public long Size { get; set; } //Tamaño del archivo antes de comprimir
        public Dictionary<byte[], int> dicLZW = new Dictionary<byte[], int>();
        public Dictionary<byte, int> dicOLZW = new Dictionary<byte, int>();
        public Stack<Datos> dat = new Stack<Datos>();
        int con_lzw = 0; //cuenta los caracteres en la tabla
        public byte[] prev = new byte[0];
        public byte[] nuevo;
        public byte[] buffer_escr; //buffer para escribir en el archivo
        List<int> codigos_as = new List<int>();// lista de asccis

    
        public void Lista(string path)
        {


            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            Name = "Historial.txt";
            string filePath = Path.Combine(path, Name);
            string cadena = System.IO.File.ReadAllText(filePath);
            if (cadena.Length != 0)
            {
                string[] cadenas = cadena.Split('/');

                for (int g = 0; g < cadenas.Length; g++)
                {
                    string[] datos_l = cadenas[g].Split(';');

                    Datos aux = new Datos();
                    aux.nombre = datos_l[0];
                    aux.razon = Convert.ToDouble(datos_l[1]);
                    aux.factor = Convert.ToDouble(datos_l[2]);
                    aux.porcentaje = Convert.ToDouble(datos_l[3]);
                    aux.metodo = datos_l[4];
                    dat.Push(aux);

                }
            }


        }

        public byte[] DescomprimirLZW()
        {
            byte[] texto = new byte[0];

            int cod_v = codigos_as[0];
            int cod_n = 0;

            byte[] caracter = new byte[1];
            byte[] cadena = new byte[0];
            foreach (var item in dicOLZW)
            {
                if (item.Value == cod_v)
                {
                    caracter[0] = item.Key;
                    byte[] aux = new byte[texto.Length + caracter.Length];
                    System.Buffer.BlockCopy(texto, 0, aux, 0, texto.Length);
                    System.Buffer.BlockCopy(caracter, 0, aux, texto.Length, caracter.Length);
                    texto = aux;

                    break;
                }
            }
            for (int u = 1; u < codigos_as.Count(); u++)
            {
                cod_n = codigos_as[u];
                bool bandera = false;
                foreach (var item in dicLZW)
                {
                    if (item.Value == cod_n)
                    {
                        bandera = true;
                        break;
                    }
                }
                if (bandera == false)
                {
                    foreach (var item1 in dicLZW)
                    {
                        if (item1.Value == cod_v)
                        {
                            cadena = item1.Key;
                            byte[] auxi = new byte[cadena.Length + caracter.Length];
                            System.Buffer.BlockCopy(cadena, 0, auxi, 0, cadena.Length);
                            System.Buffer.BlockCopy(caracter, 0, auxi, cadena.Length, caracter.Length);
                            break;
                        }
                    }
                }
                else if (bandera == true)
                {
                    bool banderafallo = false;
                    foreach (var item1 in dicLZW)
                    {
                        if (item1.Value == cod_n)
                        {
                            cadena = item1.Key;
                            banderafallo = true;
                            break;
                        }
                    }
                    if (banderafallo == false)
                    {
                        byte[] p = new byte[1];
                        p[0] = cadena[0];
                        byte[] auxfallo = new byte[cadena.Length + p.Length];
                        System.Buffer.BlockCopy(p, 0, auxfallo, 0, p.Length);
                        System.Buffer.BlockCopy(cadena, 0, auxfallo, p.Length, cadena.Length);
                    }
                }
                byte[] aux = new byte[texto.Length + cadena.Length];
                System.Buffer.BlockCopy(texto, 0, aux, 0, texto.Length);
                System.Buffer.BlockCopy(cadena, 0, aux, texto.Length, cadena.Length);
                texto = aux;
                caracter[0] = cadena[0];

                foreach (var it in dicLZW)
                {
                    if (it.Value == cod_v)
                    {
                        byte[] agregar = new byte[it.Key.Length + caracter.Length];
                        System.Buffer.BlockCopy(it.Key, 0, agregar, 0, it.Key.Length);
                        System.Buffer.BlockCopy(caracter, 0, agregar, it.Key.Length, caracter.Length);
                        dicLZW.Add(agregar, con_lzw);
                        con_lzw++;
                        cod_v = cod_n;

                        break;
                    }

                }




            }
            return texto;
        }
        public void EscribirTXTLZW(string path)
        {

            byte[] escribir = DescomprimirLZW();
        
            using (var stream = new FileStream(path, FileMode.Open))
            {

                using (BinaryWriter wr = new BinaryWriter(stream))
                {
                    wr.Write(escribir);

                }

                stream.Close();
            }

        }
        public void IniciarDicODescLZW(byte[] buffer) //Ingresa la tabla al diccionario
        {
            int valaux = 0;
            for (int x = 0; x < buffer.Length; x = x + 3)
            {
                byte[] aux = new byte[1];
                aux[0] = buffer[x];
                valaux = Convert.ToInt32(buffer[x + 1]);
                if (Convert.ToInt32(buffer[x + 1]) != 0)
                {
                    string extra = DecimalaBinario(Convert.ToInt32(buffer[x + 1]), 8);

                    valaux = BinaryToDecimal2(extra) + Convert.ToInt32(buffer[x + 2]);
                }
                else
                {
                    valaux = Convert.ToInt32(buffer[x + 2]);
                }

                dicOLZW.Add(buffer[x], valaux);
                dicLZW.Add(aux, valaux);
                con_lzw++;
            }

        }
        public void IniciarDicLZW(byte[] buffer) //Ingresa los caracteres a la tabla LZW
        {


            for (int x = 0; x < buffer.Length; x++)
            {
                byte[] aux = new byte[1];
                aux[0] = buffer[x];
                bool bandera = false;

                foreach (var item in dicLZW)
                {
                    if (aux.SequenceEqual(item.Key) == true)
                    {
                        bandera = true;
                    }
                }
                if (bandera == false)
                {
                    dicLZW.Add(aux, con_lzw);
                    dicOLZW.Add(buffer[x], con_lzw);
                    con_lzw++;
                }


            }
        }

        public void TraducirLZW(byte[] buffer)
        {
            string salida = string.Empty;

            for (int x = 0; x < buffer.Length; x++)
            {
                byte[] actual = new byte[1];
                actual[0] = buffer[x];
                nuevo = new byte[prev.Length + 1];
                System.Buffer.BlockCopy(prev, 0, nuevo, 0, prev.Length);
                System.Buffer.BlockCopy(actual, 0, nuevo, prev.Length, 1);
                bool bandera = false;
                foreach (var item in dicLZW)
                {

                    if (nuevo.SequenceEqual(item.Key) == true)
                    {
                        bandera = true;
                        break;
                    }
                }
                if (bandera == true)
                {
                    prev = nuevo;
                }
                else if (bandera == false)
                {
                    dicLZW.Add(nuevo, con_lzw);
                    foreach (var item in dicLZW)
                    {

                        if (prev.SequenceEqual(item.Key) == true)
                        {
                            Cod_ASCII(item.Value);
                            break;
                        }
                    }

                    prev = actual;
                    con_lzw++;
                }


            }
            foreach (var item in dicLZW)
            {

                if (prev.SequenceEqual(item.Key) == true)
                {
                    Cod_ASCII(item.Value);
                    break;
                }
            }
        }
        public void Cod_ASCII(int dec) //decimal a ascci
        {
            int tamactual = buffer_escr.Length;
            byte[] aux = new byte[tamactual];
            System.Buffer.BlockCopy(buffer_escr, 0, aux, 0, tamactual);
            byte bit = Convert.ToByte(dec);
            char asc = Convert.ToChar(bit);
            //    codigos = codigos + asc;
            buffer_escr = new byte[tamactual + 1];
            System.Buffer.BlockCopy(aux, 0, buffer_escr, 0, tamactual);
            buffer_escr[tamactual] = Convert.ToByte(asc);

            // agrego el ascci a el buffer de escritura


        }
        public void GuardarAscci(byte[] buffer)
        {
            for (int x = 0; x < buffer.Length; x++)
            {
                codigos_as.Add(Convert.ToInt32(buffer[x]));
            }
        }
        public void EscribirLZW(string path, string path1)
        {

            byte[] nombre = new byte[Name.Length - 4];
            byte[] diccionario = DicaBytesLZW();
            int cancodigos = buffer_escr.Length; //cantidad de codigos ascci que escribimos
            string cant = cancodigos.ToString(); //cantidad de datos a leer para obtener no de asccis
            char[] nuevo = new char[cant.Length]; // char con cantidad de asccis
            for (int x = 0; x < cant.Length; x++)
            {
                nuevo[x] = cant[x];
            }
            byte[] no;
            no = Encoding.UTF8.GetBytes(nuevo);


            for (int n = 0; n < Name.Length - 4; n++)
            {

                nombre[n] = Convert.ToByte(Name[n]);
            }
            using (var stream = new FileStream(path, FileMode.Open))
            {

                using (BinaryWriter wr = new BinaryWriter(stream))
                {
                    wr.Write(Convert.ToByte(Name.Length - 4));
                    wr.Write(nombre);
                    wr.Write(Convert.ToByte(cant.Length));
                    wr.Write(no);
                    wr.Write(buffer_escr);
                    wr.Write(diccionario);

                }

                stream.Close();
            }

            FileInfo file = new FileInfo(path);
            long f = file.Length;
            int t = Convert.ToInt32(f);

            Datos data = new Datos();
            data.nombre = Name;

            double aux = Convert.ToDouble(t) / Size;
            data.razon = Math.Round(aux, 2);
            aux = Size / Convert.ToDouble(t);
            data.factor = Math.Round(aux, 2);
            aux = Convert.ToDouble(t) * 100 / Size;
            aux = 100 - aux;
            data.porcentaje = Math.Round(aux, 2);
            data.metodo = "lzw";
            dat.Push(data);
            EscribirList(data,path1);
        }
        public string DecimalaBinario(int deci, int val) //convierte el decimal enviado a un binario
        {
            string binario = string.Empty;
            int residuo = 0;
            for (int x = 0; deci > 1; x++)
            {
                residuo = deci % 2;
                deci = deci / 2;
                binario = residuo.ToString() + binario;
            }

            if (deci == 1)
            {
                binario = deci.ToString() + binario;
            }
            if (binario.Length != val)
            {
                for (int d = 0; d < (val - binario.Length); deci++)
                {
                    binario = '0' + binario;
                }
            }
            return binario;
        }

        public byte BinaryToDecimalInt(char[] binario)
        {
            int valor_decimal = 0; // valor del decimal a convertir
            for (int c = 7; c >= 0; c--)
            {
                int d = 7 - c;
                double v = Convert.ToDouble(binario[d].ToString()) * Math.Pow(2, c);
                valor_decimal = valor_decimal + Convert.ToInt32(v);

            }
            byte bit = Convert.ToByte(valor_decimal);
            return bit;
        }
        public byte[] DicaBytesLZW()
        {
            byte[] bytesdic = new byte[dicOLZW.Count() * 3];
            int x = 0;
            foreach (var item in dicOLZW)
            {
                bytesdic[x] = item.Key;
                if (item.Value > 255)
                {
                    string extra = DecimalaBinario(item.Value, 16);
                    char[] ex = new char[8];
                    for (int i = 0; i < 8; i++)
                    {
                        ex[i] = extra[i];
                    }
                    bytesdic[x + 1] = BinaryToDecimalInt(ex);
                    for (int i = 0; i < 8; i++)
                    {
                        ex[i] = extra[i + 8];
                    }
                    bytesdic[x + 2] = BinaryToDecimalInt(ex);

                }
                else
                {
                    bytesdic[x + 1] = Convert.ToByte(0);
                    bytesdic[x + 2] = Convert.ToByte(item.Value);
                }

                x = x + 3;

            }
            return bytesdic;
        }
        public void EscribirList(Datos dati, string path)
        {

            string filePath = "Historial.txt";
            var fullPathH = Path.Combine(path, filePath);


            string exis = System.IO.File.ReadAllText(fullPathH);

            StreamWriter newfile = new StreamWriter(fullPathH);


            if (exis.Length != 0)
            {

                newfile.Write(exis);
                newfile.Write("/");

            }
            newfile.Write(dati.nombre + ";" + (dati.razon).ToString() + ";" + (dati.factor).ToString() + ";" + (dati.porcentaje).ToString() + ";" + dati.metodo);

            newfile.Close();



        }

        public int BinaryToDecimal2(string binario)
        {
            int valor_decimal = 0; // valor del decimal a convertir
            for (int c = 15; c >= 8; c--)
            {
                int d = 15 - c;
                double v = Convert.ToDouble(binario[d].ToString()) * Math.Pow(2, c);
                valor_decimal = valor_decimal + Convert.ToInt32(v);

            }
            return valor_decimal;
        }
    }



}

