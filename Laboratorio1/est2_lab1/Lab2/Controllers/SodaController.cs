using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Library.Models;
using Lab2.Helpers;

namespace Lab2.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SodaController : ControllerBase
    {
        [HttpGet]
        [Route("{id?}")]
        public IEnumerable<Soda> GetBy(int id = -1)
        {
            DoIt();
            Data.Instance.ListSoda = Data.Instance.BTree.InOrder();
            if (id > -1)
            {
                return id > Data.Instance.ListSoda.Count
                    ? new List<Soda>()
                    : new List<Soda>() { Data.Instance.ListSoda[id] };
            }

            return Data.Instance.ListSoda;
        }

        [HttpPost]
        public Soda Add()
        {
            Soda s = new Soda();
            s.Name = Request.Form["Name"];
            s.Flavor=Request.Form["Flavor"];
            s.Vol= Convert.ToInt32(Request.Form["Vol"]);
            s.Price=Convert.ToInt32(Request.Form["Price"]);
            s.ProductHouse= Request.Form["ProductHouse"];

            Data.Instance.BTree.Insert(s);
            return s;
        }

        void DoIt()
        {
            if (Data.Instance.BTree.Empty())
            {
                Data.Instance.BTree.Insert(new Soda { Vol = 100, Name = "Coca-cola", Flavor = "Cola", Price = 5, ProductHouse = "Coca-cola inc" });
                Data.Instance.BTree.Insert(new Soda { Vol = 100, Name = "Fanta", Flavor = "Orange", Price = 8, ProductHouse = "Coca-cola inc" });
                Data.Instance.BTree.Insert(new Soda { Vol = 100, Name = "Sprite", Flavor = "Lemon", Price = 9, ProductHouse = "Coca-cola inc" }) ;
                Data.Instance.BTree.Insert(new Soda { Vol = 100, Name = "Diet Coke/Coca-Cola Light", Flavor = "Cola", Price = 2, ProductHouse = "Coca-cola inc" });
                Data.Instance.BTree.Insert(new Soda { Vol = 100, Name = "Coca-Cola Zero", Flavor = "Cola", Price = 4, ProductHouse = "Coca-cola inc" });
                Data.Instance.BTree.Insert(new Soda { Vol = 100, Name = "Minute Maid", Flavor = "Orange", Price = 7, ProductHouse = "Coca-cola inc" });
                Data.Instance.BTree.Insert(new Soda { Vol = 100, Name = "Georgia Coffee", Flavor = "Coffee", Price = 4, ProductHouse = "Coca-cola inc" });
                Data.Instance.BTree.Insert(new Soda { Vol = 100, Name = "Powerade", Flavor = "Lemon", Price = 9, ProductHouse = "Coca-cola inc" });
                Data.Instance.BTree.Insert(new Soda { Vol = 100, Name = "Del Valle", Flavor = "Orange", Price = 9, ProductHouse = "Coca-cola inc" });
                Data.Instance.BTree.Insert(new Soda { Vol = 100, Name = "Schweppes", Flavor = "Orange", Price = 9, ProductHouse = "Coca-cola inc" });
                Data.Instance.BTree.Insert(new Soda { Vol = 100, Name = "Aquarius", Flavor = "Aqua", Price = 9, ProductHouse = "Coca-cola inc" });
                Data.Instance.BTree.Insert(new Soda { Vol = 100, Name = "Minute Maid Pulpy", Flavor = "Orange", Price = 9, ProductHouse = "Coca-cola inc" });
                Data.Instance.BTree.Insert(new Soda { Vol = 100, Name = "Dasani", Flavor = "Aqua", Price = 9, ProductHouse = "Coca-cola inc" });
                Data.Instance.BTree.Insert(new Soda { Vol = 100, Name = "Simply", Flavor = "Aqua", Price = 9, ProductHouse = "Coca-cola inc" });
                Data.Instance.BTree.Insert(new Soda { Vol = 100, Name = "Glacéau", Flavor = "Lemon", Price = 9, ProductHouse = "Coca-cola inc" });
                Data.Instance.BTree.Insert(new Soda { Vol = 100, Name = "BonAqua", Flavor = "Aqua", Price = 9, ProductHouse = "Coca-cola inc" });
                Data.Instance.BTree.Insert(new Soda { Vol = 100, Name = "Gold Peak", Flavor = "Coffee", Price = 9, ProductHouse = "Coca-cola inc" });
            }
        }
    }
}