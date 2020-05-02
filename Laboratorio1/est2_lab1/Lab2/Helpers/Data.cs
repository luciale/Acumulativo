using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.Models;

namespace Lab2.Helpers
{
    public class Data
    {
        private static Data _instance = null;

        public static Data Instance
        {
            get
            {
                if (_instance == null) _instance = new Data();
                return _instance;
            }
        }

        public int id = 0;
        public Stack<Soda> StackSoda = new Stack<Soda>();
        public List<Soda> ListSoda = new List<Soda>();
        public List<Soda> ByIDListSoda = new List<Soda>();
        public Stack<Soda> StackBackUp = new Stack<Soda>();
        public Arbol<Soda> BTree = new Arbol<Soda>();
    }
}
