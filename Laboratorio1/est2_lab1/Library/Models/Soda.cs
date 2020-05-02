using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Models
{
    public class Soda:IComparable
    {
        public string Name { get; set; } //Nombre de la soda
        public string Flavor { get; set; }//Sabor de la soda
        public int Vol { get; set; } //Volumen de la soda
        public float Price { get; set; }//Precio de la soda
        public string ProductHouse { get; set; } //Casa productora de la soda

        public int CompareTo(object obj) //Comparación del Nombre de las bebidas
                                         //retorna los siguientes 3 valores -1 menor, 0 igual, 1 mayor
        {
         
            return this.Name.CompareTo(((Soda)obj).Name);
          
        }
        public static Comparison<Soda> CompareByName= delegate (Soda s1, Soda s2)
        {
            return s1.CompareTo(s2);
        };
    }
}
