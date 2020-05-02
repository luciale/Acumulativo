using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lab0Mod.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lab0Mod.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Peli : ControllerBase
    {
        // GET: api/Peli
        [HttpGet]
        // para recibir dato
        // [Route("{id?}")]
        public IEnumerable<Pelicula> Get()
        {

            if (Datacs.Instance.lista_peliculas.Count > 10)
            {
                int con = Datacs.Instance.lista_peliculas.Count;
                List<Pelicula> lis = new List<Pelicula>();
                for (int x = 0; x < 10; x++)
                {
                    lis.Add(Datacs.Instance.lista_peliculas[con - x]);

                }
                return lis;



            }
            else
            {
                return Datacs.Instance.lista_peliculas;
            }


        }

         // POST: api/Peli
        [HttpPost]
        public void Post()
        {
            Pelicula peli = new Pelicula();
            peli.nombre = Request.Form["nombre"];
            peli.director = Request.Form["director"];
            peli.año = Convert.ToInt32(Request.Form["año"]);
            Datacs.Instance.lista_peliculas.Add(peli);
        }


    }
}
