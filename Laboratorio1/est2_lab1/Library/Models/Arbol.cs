using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Models
{

    public class Arbol<T> where T : IComparable
    {
        private Nodo<T> raiz { get; set; } // raiz del arbol

        List<T> ListValuesToShow = new List<T>();

        public Arbol()
        {
            raiz = null;
        }

        public bool Empty()
        {
            if (raiz == null) { return true; } else { return false; };
        }

   

        private void InOrder(Nodo<T> nodo_actual)
        {

            if (!object.Equals(nodo_actual.Children[0], default(T)))
            {
                for (int i = 0; i < Nodo<T>.m; i++)
                {
                    if (!object.Equals(nodo_actual.Children[i], default(T)))
                    {
                        InOrder(nodo_actual.Children[i]);

                        if (i < Nodo<T>.m - 1)
                        {
                            if (!object.Equals(nodo_actual.Values[i], default(T)))
                            {
                                ListValuesToShow.Add(nodo_actual.Values[i]);
                            }
                        }

                    }

                }
            }
            else
            {
                for(int i=0; i< Nodo<T>.m - 1; i++)
                {
                    if (!object.Equals(nodo_actual.Values[i], default(T)))
                    {
                        ListValuesToShow.Add(nodo_actual.Values[i]);
                    }
                }
            }
        

        }
        public List<T> InOrder()
        {
            ListValuesToShow.Clear();
            InOrder(raiz);
            return ListValuesToShow;
        }

        public void Insert(T value)
        {
            if (raiz == null) //Si el árbol esta vacío
            {
                Nodo<T> nodo = new Nodo<T>();
                nodo.father = null;
                nodo.Values[0] = value;
                raiz = nodo;
            }
            else //El árbol ya tiene algun dato.
            {
                Insert(raiz, value, null);
            }
        }

        public void Insert(Nodo<T> nodo_actual, T value, List<Nodo<T>> list)
        {

            if (object.Equals(nodo_actual.Children[0],default(T)) && object.Equals(nodo_actual.Children[1], default(T))) //El nodo es hoja
            {
                if (object.Equals(nodo_actual.Values[Nodo<T>.m - 2],default(T))) //Hay espacio en el nodo hoja
                {
                    nodo_actual.Insert_Aux(value);
                    var sortedList = GenericComparation<T>.SortedList(nodo_actual.Values, Soda.CompareByName);
                    for (int x = 0; x < sortedList.Length; x++)
                    {
                        nodo_actual.Values[x] = sortedList[x]; //Inserto los valores ordenados por burbuja en el vector
                    }
                }
                else //no hay espacio en el nodo hoja
                {
                    int medio = Nodo<T>.m / 2;
                    T[] aux = new T[Nodo<T>.m];
                    for (int x = 0; x < aux.Length - 1; x++) //Inserto los valores que ya tengo en un arreglo auxiliar
                    {
                        aux[x] = nodo_actual.Values[x];
                    }
                    aux[aux.Length - 1] = value;
                    var sortedList = GenericComparation<T>.SortedList(aux, Soda.CompareByName);


                    if (object.Equals(nodo_actual.father,default(T)))
                    { //No existe padre
                        Nodo<T> actual_father = Nodo<T>.Insert_NewFather(nodo_actual,sortedList, medio, null) ;
                        raiz = actual_father;

                    }
                    else
                    { //Si existe padre
                        Nodo<T> nodo_aux = new Nodo<T>();
                        for (int x = 0; x < medio; x++)
                        {
                            nodo_aux.Values[x] = sortedList[x];
                        }
                        Nodo<T> nodo_aux1 = new Nodo<T>();
                        int x_aux = 0;
                        for (int x = medio+1; x < sortedList.Length; x++)
                        {
                            nodo_aux1.Values[x_aux] = sortedList[x];
                            x_aux++;
                        }
                        
                        List<Nodo<T>> list1 = new List<Nodo<T>>();
                        list1.Add(nodo_aux);
                        list1.Add(nodo_aux1);
                        //Eliminamos el nodo en la lista de Nodos del padre
                        int eliminar = nodo_actual.father.Delete_Nodo_InList(nodo_actual);
                        if (eliminar != -1)
                        {
                            nodo_actual.father.Children[eliminar] = null;
                        }
                        Insert(nodo_actual.father, sortedList[medio], list1); //enviamos valores nuevos para ingresar

                    }

                }


            }

            else //El nodo no es hoja
            {
                if (list != null) //El nodo ya trae hijos si no trae hijos es porque es un valor nuevo
                {
                    if (object.Equals(nodo_actual.Values[Nodo<T>.m - 2], default(T))) //Hay espacio en el nodo 
                    {
                        nodo_actual.Insert_Aux(value);
                        var sortedList = GenericComparation<T>.SortedList(nodo_actual.Values, Soda.CompareByName);
                        for (int x = 0; x < sortedList.Length; x++)
                        {
                            nodo_actual.Values[x] = sortedList[x]; //Inserto los valores ordenados por burbuja en el vector
                        }
                        //Ordenar los hijos
                        Nodo<T>.Children_Order(nodo_actual, list);


                    }
                    else //No hay espacio en el nodo
                    {
                        int medio = Nodo<T>.m / 2;
                        T[] aux = new T[Nodo<T>.m];
                        for (int x = 0; x < Nodo<T>.m-1; x++) //Inserto los valores que ya tengo en un arreglo auxiliar
                        {
                            aux[x] = nodo_actual.Values[x];
                        }
                        aux[aux.Length - 1] = value;
                        var sortedList = GenericComparation<T>.SortedList(aux, Soda.CompareByName);
                       
                        if (object.Equals(nodo_actual.father, default(T)))
                        { //No existe padre
                           
                            Nodo<T> actual_father = Nodo<T>.Insert_NewFather(nodo_actual,sortedList, medio,list);
                            raiz = actual_father;

                            //unir todos los hijos de los hijos del padre

                        }
                        else
                        { //Si existe padre
                            Nodo<T> nodo_aux = new Nodo<T>();
                            for (int x = 0; x < medio - 1; x++)
                            {
                                nodo_aux.Values[x] = sortedList[x];
                            }
                            Nodo<T> nodo_aux1 = new Nodo<T>();
                            int x_aux = 0;
                            for (int x = medio; x < sortedList.Length; x++)
                            {
                                nodo_aux1.Values[x_aux] = sortedList[x];
                                x_aux++;
                            }
                            
                            list.Add(nodo_aux);
                            list.Add(nodo_aux1);
                            //Eliminamos el nodo en la lista de Nodos del padre
                            int eliminar = nodo_actual.father.Delete_Nodo_InList(nodo_actual);
                            if (eliminar != -1)
                            {
                                nodo_actual.father.Children[eliminar] = null;
                            }

                            //reordenar hijos de los nodos 
                            Insert(nodo_actual.father, sortedList[medio - 1], list); //enviamos valores nuevos para ingresar

                        }
                    }
                }
                else
                {
                    int pos = GenericComparation<T>.Position(nodo_actual.Values, Soda.CompareByName, value);
                    Insert(nodo_actual.Children[pos], value, null);

                }
            }


        }
    }
}

