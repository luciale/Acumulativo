using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace est2_lab3.Models
{
    public class Nodo
    {
        public double Value;
        public byte Character;
        public string Name;
        public string Prefijo;
        public Nodo Child_Left;
        public Nodo Child_Right;
        public int No_Nodo;
        public void Start_Nodo(double val, byte car)
        {

            this.Character = car;
            this.Value = val;
            this.Prefijo = string.Empty;
            this.Child_Left = null;
            this.Child_Right= null;
        }



        public void New(Nodo hijod, Nodo hijoi)
        {
            hijod.Prefijo = hijod.Prefijo + '1';
            this.Child_Right = hijod;
            hijoi.Prefijo = hijoi.Prefijo + '0';
            this.Child_Left = hijoi;




        }
    }
}
