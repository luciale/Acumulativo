using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Transposicion.Models;

namespace Transposicion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class cipher : ControllerBase
    {


        [HttpPost, DisableRequestSizeLimit]
        public IActionResult Upload()
        {

            var file = Request.Form.Files[0];
            string file_nameN = Request.Form["nombre"];
            string type_cipher = Request.Form["tipo"];

            //lectura archivo
            var folderName = Path.Combine("Resources", "Files");
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            if (!Directory.Exists(pathToSave))
            {
                Directory.CreateDirectory(pathToSave);
            }
            string extension = Path.GetExtension(file.FileName);
            var buffer = new byte[Data.BufferLength];


            if (extension == ".txt")
            {
                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName);
                    file_nameN = file_nameN + ".txt";
                    string fullPathC = Path.Combine(pathToSave, file_nameN);
                    var dbPathH = Path.Combine(folderName, file_nameN);



                    if (type_cipher == "Cesar")
                    {
                        string clave = Request.Form["clave"];
                        int cl = 0;
                        bool result = int.TryParse(clave, out cl);
                        Cesar cif = new Cesar();
                        cif.LlenarDiccionarios();
                        if (result == false) //la clave no es númerica
                        {
                            cif.LlenarClave(clave);
                        }
                        else //la clave es númerica 
                        {
                            cif.LlenarClaveInt(cl);

                        }


                        //esto se repite en cada if 
                        using (var stream = new FileStream(fullPathC, FileMode.Create))
                        {
                            //ya se creó el archivo
                        }
                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        using (var stream = new FileStream(fullPath, FileMode.Open))
                        {

                            using (BinaryReader br = new BinaryReader(stream))
                            {

                                while (br.BaseStream.Position != br.BaseStream.Length)
                                {
                                    buffer = br.ReadBytes(Data.BufferLength); //llenar el buffer  
                                    cif.Cifrar(buffer);
                                    cif.EscribirCifrado(fullPathC);
                                }
                            }
                        }

                        var stream1 = new FileStream(fullPathC, FileMode.Open);

                        return File(stream1, System.Net.Mime.MediaTypeNames.Application.Octet, file_nameN);

                    }
                    if (type_cipher == "ZigZag")
                    {
                        int niveles = Convert.ToInt32(Request.Form["clave"]);
                        ZigZag cif = new ZigZag();

                        //esto se repite en cada if 
                        using (var stream = new FileStream(fullPathC, FileMode.Create))
                        {
                            //ya se creó el archivo
                        }
                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        using (var stream = new FileStream(fullPath, FileMode.Open))
                        {

                            using (BinaryReader br = new BinaryReader(stream))
                            {

                                while (br.BaseStream.Position != br.BaseStream.Length)
                                {
                                    buffer = br.ReadBytes(Data.BufferLength);
                                    cif.InicializarLista(niveles);
                                    cif.LLenarNiveles(buffer, niveles);
                                    cif.Cifrar();
                                    cif.EscribirZigZag(fullPathC);
                                }
                            }
                        }

                        var stream1 = new FileStream(fullPathC, FileMode.Open);

                        return File(stream1, System.Net.Mime.MediaTypeNames.Application.Octet, file_nameN);

                    }
                    if (type_cipher == "Ruta_Vertical")
                    {
                        int row = Convert.ToInt32(Request.Form["fila"]);
                        int column = Convert.ToInt32(Request.Form["columna"]);
                        int total = row * column; //cantidad de bytes a leer o que van en la matriz
                        buffer = new byte[Data.BufferLength];
                        Vertical cif = new Vertical();
                        //esto se repite en cada if 
                        using (var stream = new FileStream(fullPathC, FileMode.Create))
                        {
                            //ya se creó el archivo
                        }
                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        using (var stream = new FileStream(fullPath, FileMode.Open))
                        {

                            using (BinaryReader br = new BinaryReader(stream))
                            {

                                while (br.BaseStream.Position != br.BaseStream.Length)
                                {
                                    buffer = br.ReadBytes(total);
                                    cif.Cipher(buffer, column, row, fullPathC);
                                    
                                }
                            }
                        }

                        var stream1 = new FileStream(fullPathC, FileMode.Open);

                        return File(stream1, System.Net.Mime.MediaTypeNames.Application.Octet, file_nameN);



                    }
                    if (type_cipher == "Ruta_Espiral")
                    {
                       
                        return  BadRequest();

                    }
                    else
                    {
                        return StatusCode(3, "El tipo de cifrado ingresado no es valido para ruta");
                    }


                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return BadRequest();
            }
        }

    }
}
