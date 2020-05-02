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
    public class decipher : ControllerBase
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
                                    cif.DesCifrar(buffer);
                                    cif.EscribirCifrado(fullPathC);
                                }
                            }
                        }

                        var stream1 = new FileStream(fullPathC, FileMode.Open);

                        return File(stream1, System.Net.Mime.MediaTypeNames.Application.Octet, file_nameN);

                    }
                    if (type_cipher == "ZigZag")
                    {
                        ZigZag cif = new ZigZag();
                        int niveles = Convert.ToInt32(Request.Form["clave"]);
                        long con = 0;
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
                                    cif.InicializarLista(niveles);
                                    if (buffer.Length == 1000)
                                    {
                                        byte l = br.ReadByte();
                                        cif.aux = Convert.ToInt32(l);
                                    }


                                    if (cif.aux != 0)
                                    {
                                        byte[] aux = new byte[cif.aux];
                                        aux = br.ReadBytes(cif.aux);
                                        byte[] auxi = new byte[Data.BufferLength + cif.aux];
                                        System.Buffer.BlockCopy(buffer, 0, auxi, 0, buffer.Length);
                                        System.Buffer.BlockCopy(aux, 0, auxi, 0, aux.Length);
                                        buffer = new byte[auxi.Length];
                                        System.Buffer.BlockCopy(auxi, 0, buffer, 0, buffer.Length);

                                    }
                                    cif.CalcularM(niveles, buffer.Length);
                                    cif.AsignarCanNiveles(niveles);
                                    cif.DesCifrar(niveles, buffer);
                                    cif.DesCifrarE(niveles);
                                    cif.EscribirDesCifradoZ(fullPathC);
                                    con = con + buffer.Length + cif.aux;


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
                        int restriccion = row * column;
                        Vertical vertical = new Vertical();
                        string request = null;

                        request = vertical.Decipher(file, restriccion, column, row, type_cipher, file_nameN);
                        return StatusCode(3, request);
                    }

                    if (type_cipher == "Ruta_Espiral")
                    {
                        int row = Convert.ToInt32(Request.Form["fila"]);
                        int column = Convert.ToInt32(Request.Form["columna"]);
                        int restriccion = row * column;
                        Espiral espiral = new Espiral();
                        string request = null;
                        request = espiral.Decipher(file, restriccion, column, row, type_cipher, file_nameN);
                        return StatusCode(3, request);

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
