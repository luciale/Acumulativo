using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace est2_lab3.Models
{
    public class Huffman
    {
        public string Name { get; set; } //Nombre nuevo del archivo

        public long Size { get; set; } //Tamaño del archivo antes de comprimir
        public Dictionary<byte, int> Dic_O = new Dictionary<byte, int>(); //diccionario original, con bytes y ocurrencia 
        public Dictionary<byte, double> Dic = new Dictionary<byte, double>(); //diccionario con probabilidades
        public List<KeyValuePair<byte, double>> List_Order = new List<KeyValuePair<byte, double>>(); // Lista con las probabilidades ordenadas
        public double Probability; //suma de probabilidades 
        public List<Nodo> List_Nodo = new List<Nodo>(); //Lista de Nodos 
        int Nodos_count = 1; //contador de nodos creados
        bool Full_Arbol = false; //Bandera que indica si el arbol esta lleno
        public Dictionary<byte, string> Prefijos = new Dictionary<byte, string>(); //Diccionario con prefijos
        public Nodo root = new Nodo(); //raiz del arbol
        public byte[] Write_Buffer; //buffer para escribir en el archivo
        public string pre_restantes=string.Empty; // prefijos restantes del buffer
        public Stack<Datos> dat = new Stack<Datos>();
        public void Lista(string path)
        {
            
            
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                Name = "Historial.txt";
                string filePath = Path.Combine(path,Name);
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
        public void Dictionary_Original(byte[] buffer) //Llena el diccionario original con el buffer de datos
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                byte valaux = buffer[i];
                if (Dic_O.ContainsKey(valaux) == true)
                {
                    Dic_O[valaux] = Dic_O[valaux] + 1;

                }
                else
                {
                    Dic_O.Add(valaux, 1);
                }
            }

        }
        public void Start_Dictionary() //inicia el diccionario calculando probabilidades
        {
            foreach (var item in Dic_O)
            {
                byte valaux = item.Key;
                double val = (Convert.ToDouble(item.Value)) / Size;
                val = Math.Round(val, 6);
                Dic.Add(valaux, val);
            }

        }

        
        public void OrdenarDic() //Ordena el diccionario 
        {
            List<KeyValuePair<byte, double>> Listaux =Dic.ToList();
            Dic.Clear();
            Listaux.Sort(
                delegate (KeyValuePair<byte, double> pair1,
                KeyValuePair<byte, double> pair2)
                {
                    return pair1.Value.CompareTo(pair2.Value);
                }
            );
            List_Order = Listaux;
        }
       
        public void Add_Pr() //Suma la probabilidad total en las ocurrencias
        {
            double sum = 0;
            foreach (var item in Dic)
            {
                sum = sum + item.Value;
            }
            Probability = Math.Round(sum, 6);
        }
        public void CrearArbol() //Creacion fdel arbol 
        {

            Nodo der = new Nodo();
            Nodo izq = new Nodo();
            Nodo nodo = new Nodo();


            if (List_Nodo.Count == 0) //Iniciando arbol porque aun no hay nodos creados
            {
                KeyValuePair<byte, double> aux = new KeyValuePair<byte, double>();
                aux = List_Order[0];
                der.Start_Nodo(aux.Value, aux.Key); //primer nodo derecho
                List_Order.Remove(aux);
                aux = List_Order[0];
                izq.Start_Nodo(aux.Value, aux.Key); //primer nodo izquierdo
                List_Order.Remove(aux);
            }
            else // ya se agregaron nodos a la lista
            {
                // NODO DERECHO
                if (List_Order.Count() != 0)
                {
                    KeyValuePair<byte, double> aux1 = new KeyValuePair<byte, double>();
                    aux1 = List_Order[0];
                    double valaux1 = List_Nodo[0].Value;
                    if (aux1.Value < valaux1) //si en la lista de caracteres esta la menor probabilidad
                    {
                        der.Start_Nodo(aux1.Value, aux1.Key);
                        List_Order.Remove(aux1);
                    }
                    else // en la lista de nodos esta la menor probabilidad
                    {
                        der = List_Nodo[0];
                        List_Nodo.Remove(der);
                    }
                }
                else
                {
                    if (List_Nodo.Count() != 0)
                    {
                        der = List_Nodo[0];
                        List_Nodo.Remove(der);
                    }
                    else
                    {
                        Full_Arbol= true;
                    }
                }
                //NODO IZQUIERDO
                if (List_Order.Count() != 0)
                {
                    KeyValuePair<byte, double> aux2 = List_Order[0];
                    double valaux2 = List_Nodo[0].Value;
                    if (aux2.Value < valaux2) //si en la lista de caracteres esta la menor probabilidad
                    {
                        izq.Start_Nodo(aux2.Value, aux2.Key);
                        List_Order.Remove(aux2);
                    }
                    else if (List_Nodo.Count() != 0) // en la lista de nodos esta la menor probabilidad
                    {
                        izq = List_Nodo[0];
                        List_Nodo.Remove(izq);
                    }
                }
                else
                {
                    if (List_Nodo.Count() != 0)
                    {
                        izq = List_Nodo[0];
                        List_Nodo.Remove(izq);
                    }
                    else
                    {
                        Full_Arbol = true;
                    }
                }

            }
            //CREAR NODO PADRE
            nodo.Name = 'n' + Nodos_count.ToString();
            nodo.Value = Math.Round(der.Value + izq.Value, 6);
            nodo.New(der, izq);
            List_Nodo.Add(nodo);
            Nodos_count++;
            if ((nodo.Value) < Probability - 0.05 && Full_Arbol == false)
            {
                CrearArbol();
            }
            else
            {
                root = nodo;
                AsignarPrefijos(root);
            }
        }
        public void AsignarPrefijos(Nodo nodo) //Asignacion de prefijos al arbol
        {
            if (nodo.Child_Left != null)
            {
                nodo.Child_Left.Prefijo = nodo.Prefijo + nodo.Child_Left.Prefijo;
                if (nodo.Child_Left.Name == null)
                {
                    Prefijos.Add(nodo.Child_Left.Character, nodo.Child_Left.Prefijo);
                }
                if (nodo.Child_Left.Child_Left != null)
                {
                    AsignarPrefijos(nodo.Child_Left);
                }
            }
            if (nodo.Child_Right != null)
            {
                nodo.Child_Right.Prefijo = nodo.Prefijo + nodo.Child_Right.Prefijo;
                if (nodo.Child_Right.Name == null)
                {
                    Prefijos.Add(nodo.Child_Right.Character, nodo.Child_Right.Prefijo);
                }
                if (nodo.Child_Right != null)
                {
                    AsignarPrefijos(nodo.Child_Right);
                }
            }

        }

        public int agregados = 0; //0.s agregados al ultimo binario

        List<int> codigos_as = new List<int>();// lista de asccis
        public void Traduction(byte[] buffer)
        {
            string writing = String.Empty;
            for (int r = 0; r < buffer.Length; r++)
            {
                if (Prefijos.ContainsKey(buffer[r]) == true)
                {
                    writing = writing + Prefijos[buffer[r]];

                }
            }
            //convertir el buffer traducido a binario
            BinaryToDecimal(writing);



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
        public void BinaryToDecimal(string codigospre) // separar el buffer y enviarlo como decimales
        {


            int posrecorrido = 0; // variable que define la posicion en que empezaremos a recorrer el buffer
            char[] binario = new char[8]; //obtiene un codigo binario
            int restantes = 0; // cantidad de restantes del buffer
            int candec = 0; //cantidad de decimales en el buffer

            if (codigospre.Length == 0)  //hay restantes y el buffer esta vacio
            {
                for (int x = 0; x < pre_restantes.Length; x++)//lleno el binario con los restantes
                {
                    binario[x] = pre_restantes[x];
                }
                for (int x = pre_restantes.Length; x < 8; x++)
                {
                    binario[x] = '0'; //completo el binario con el nuevo buffer
                    //agregados++;
                }
                Cod_Decimal(binario);

            }
            else //buffer no esta vacio
            {
                if (pre_restantes != null)  // si hay restantes del buffer anterior
                {

                    for (int x = 0; x < pre_restantes.Length; x++)//lleno el binario con los restantes
                    {
                        binario[x] = pre_restantes[x];
                    }

                    for (int x = pre_restantes.Length; x < 8; x++)
                    {
                        binario[x] = codigospre[posrecorrido]; //completo el binario con el nuevo buffer
                        posrecorrido++;
                    }

                    Cod_Decimal(binario);
                }// hayan restantes del pasado o no 
                restantes = (codigospre.Length - posrecorrido) % 8;
                if (restantes != 0)// los rrstantes en el buffer nuevo no son divisibles en 8
                {
                    pre_restantes = codigospre.Substring(codigospre.Length - restantes, restantes);//agrego los restantes en el siguiente buffer
                    candec = (codigospre.Length - posrecorrido - restantes) / 8;


                }
                else// los restantes del buffer si son divisibles en 8.
                {
                    candec = (codigospre.Length - posrecorrido) / 8;


                }

                for (int x = 0; x < candec; x++)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        binario[y] = codigospre[(x * 8 + posrecorrido) + y];
                    }
                    Cod_Decimal(binario);

                }
            }

        }

        public void Cod_Decimal(char[] binario) //convertir los binarios a decimales
        {
            int valor_decimal = 0; // valor del decimal a convertir
            for (int c = 7; c >= 0; c--)
            {
                int d = 7 - c;
                double v = Convert.ToDouble(binario[d].ToString()) * Math.Pow(2, c);
                valor_decimal = valor_decimal + Convert.ToInt32(v);

            }
            Cod_ASCII(valor_decimal); //enviar el decimal
        }

        public void Cod_ASCII(int dec) //decimal a ascci
        {
            int tamactual = Write_Buffer.Length;
            byte[] aux = new byte[tamactual];
            System.Buffer.BlockCopy(Write_Buffer, 0, aux, 0, tamactual);
            byte bit = Convert.ToByte(dec);
            char asc = Convert.ToChar(bit);
            //    codigos = codigos + asc;
            Write_Buffer = new byte[tamactual + 1];
            System.Buffer.BlockCopy(aux, 0, Write_Buffer, 0, tamactual);
            Write_Buffer[tamactual] = Convert.ToByte(asc);

            // agrego el ascci a el buffer de escritura


        }
        public void EscribirArch(string path, string path1) //escribimos por buffers
        {
            byte[] nombre = new byte[Name.Length-4];
            byte[] diccionario = DicaBytes();
            int cancodigos = Write_Buffer.Length; //cantidad de codigos ascci que escribimos
            string cant = cancodigos.ToString(); //cantidad de datos a leer para obtener no de asccis
            char[] nuevo = new char[cant.Length]; // char con cantidad de asccis
            for (int x = 0; x < cant.Length; x++)
            {
                nuevo[x] = cant[x];
            }
            byte[] no;
            no = Encoding.UTF8.GetBytes(nuevo);
            byte bit = Convert.ToByte(agregados);
            char asc = Convert.ToChar(bit);

            for(int n=0; n < Name.Length-4; n++)
            {

                nombre[n] = Convert.ToByte(Name[n]);
            }
            using (var stream = new FileStream(path, FileMode.Open))
            {

                using (BinaryWriter wr = new BinaryWriter(stream))
                {
                    wr.Write(Convert.ToByte(Name.Length-4));
                    wr.Write(nombre);
                    wr.Write(Convert.ToByte(cant.Length));
                    wr.Write(Convert.ToByte(asc));
                    wr.Write(no);
                    wr.Write(Write_Buffer);
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
            data.metodo = "huffman";
            dat.Push(data);
            EscribirList(data,path1);
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
                newfile.Write(dati.nombre + ";" + (dati.razon).ToString() + ";" + (dati.factor).ToString() + ";" + (dati.porcentaje).ToString() +";" + dati.metodo);

                newfile.Close();
            
          

        }

     
        public byte[] DicaBytes()
        {
            byte[] bytesdic = new byte[Dic_O.Count() * 3];
            
            int x = 0;
            foreach (var item in Dic_O)
            {
                bytesdic[x] = item.Key;
                if (item.Value > 255)
                {
                    string extra = DecimalaBinario(item.Value, 16);
                    char[] ex = new char[8];
                    for(int i = 0; i < 8; i++)
                    {
                        ex[i] = extra[i];
                    }
                    bytesdic[x + 1] = BinaryToDecimalInt(ex);
                    for (int i = 0; i < 8; i++)
                    {
                        ex[i] = extra[i+8];
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
        public void GuardarAscci(byte[] buffer)
        {
            for (int x = 0; x < buffer.Length; x++)
            {
                codigos_as.Add(Convert.ToInt32(buffer[x]));
            }
        }

        public void IniciarDicODesc(byte[] buffer) //Ingresa la tabla al diccionario
        {
            int valaux = 0;
            for (int x = 0; x < buffer.Length; x = x + 3)
            {
                if(Convert.ToInt32(buffer[x + 1])!= 0){
                    string extra = DecimalaBinario(Convert.ToInt32(buffer[x + 1]), 8);

                    valaux = BinaryToDecimal2(extra) + Convert.ToInt32(buffer[x + 2]);
                }
                else
                {
                    valaux = Convert.ToInt32(buffer[x + 2]);
                }
                
               
                Dic_O.Add(buffer[x], valaux);
                Size = Size + valaux;
            }

        }
        public void Binarios(string path)
        {
            string binarios = string.Empty;
            for (int x = 0; x < codigos_as.Count(); x++)
            {
                binarios = binarios + DecimalaBinario(codigos_as[x],8);
            }
            Descomprimir(binarios,path);
        }
        public void Descomprimir(string binarios, string path)
        {
           
            byte[] texto = new byte[100];
            int x = 0;
            int y = 0;
            string aux = string.Empty;

            using (var stream = new FileStream(path, FileMode.Open))
            {

                using (BinaryWriter wr = new BinaryWriter(stream))
                {

                    while (x < binarios.Length - agregados)
                    {
                        aux = aux + binarios[x];
                        if (Prefijos.ContainsValue(aux) == true)
                        {
                            foreach (var item in Prefijos)
                            {
                                if (item.Value == aux)
                                {
                                    texto[y] = item.Key;
                                    y++;
                                    aux = string.Empty;
                                }
                            }
                        }
                        if (y == 100)
                        {
                            wr.Write(texto);
                            y = 0;
                        }
                        x++;
                    }
                    if (y != 0)
                    {
                        wr.Write(texto);
                    }

                }

                stream.Close();
            }



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

      
    }
}
