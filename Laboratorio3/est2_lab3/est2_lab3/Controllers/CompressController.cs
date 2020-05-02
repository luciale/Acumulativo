using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.IO;
using System.Net.Http.Headers;
using est2_lab3.Models;
using System.Net;

namespace est2_lab3.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CompressController : ControllerBase
    {
        public object MimeMapping { get; private set; }

        public IEnumerable<Datos> Get()
        {
            var folderName = Path.Combine("Resources", "Files");
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

            Huffman arch = new Huffman();
            arch.Lista(pathToSave);
            return arch.dat;


        }

        [HttpPost, DisableRequestSizeLimit]
        public IActionResult Upload()
        {
            try
            {
                var file = Request.Form.Files[0];
                string file_nameN = Request.Form["name"]; 
                string metodo = Request.Form["metodo"]; //Recibe el metodo por el cual se va a comprimir
                var folderName = Path.Combine("Resources", "Files");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                string extension = Path.GetExtension(file.FileName);
               
               
                if (extension == ".txt" )
                {
                    if (file.Length > 0)
                    {
                        //SUBIR ARCHIVO
                        var fileName = System.Net.Http.Headers.ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                        var fullPath = Path.Combine(pathToSave, fileName);
                        var dbPath = Path.Combine(folderName, fileName);
                        var error = "Escriba metodo para comprimir huffman o lzw";

                        //LECTURA ARCHIVO SUBIDO
                        if (metodo == "huffman")
                        {
                            Huffman arch = new Huffman();
                            using (var stream = new FileStream(fullPath, FileMode.Create))
                            {
                                arch.Name = file.FileName;
                                file.CopyTo(stream);
                            }
                            arch.Size = file.Length;
                            using (var stream = new FileStream(fullPath, FileMode.Open))
                            {

                                using (BinaryReader br = new BinaryReader(stream))
                                {
                                    var buffer = new byte[Data.BufferLength];
                                    while (br.BaseStream.Position != br.BaseStream.Length)
                                    {
                                        buffer = br.ReadBytes(Data.BufferLength); //llenar el buffer
                                        arch.Dictionary_Original(buffer); //enviar buffer al diccionario

                                    }

                                }

                                stream.Close();
                            }
                            arch.Start_Dictionary();// diccionario con probabilidades
                            arch.Add_Pr(); //sumar probabilidades



                            arch.OrdenarDic(); //ordena el diccionario y lo agrega a una lista
                            arch.CrearArbol(); //Creacion de arbol

                            //CREACION ARCHIVO .HUFF
                            var fileNameH = file_nameN + ".huff";
                            var fullPathH = Path.Combine(pathToSave, fileNameH);
                            var dbPathH = Path.Combine(folderName, fileNameH);


                            using (var stream = new FileStream(fullPathH, FileMode.Create))
                            {
                                //Escribimos en el archivo
                            }

                            arch.Write_Buffer = new byte[0];
                            using (var stream = new FileStream(fullPath, FileMode.Open))
                            {
                                using (BinaryReader br = new BinaryReader(stream))
                                {

                                    var buffer = new byte[Data.BufferLength];
                                    while (br.BaseStream.Position != br.BaseStream.Length)
                                    {
                                        buffer = br.ReadBytes(Data.BufferLength); //llenar el buffer
                                        arch.Traduction(buffer);


                                    }
                                    if (arch.pre_restantes.Length != 0)
                                    {
                                        string auxiliar = string.Empty;
                                        arch.BinaryToDecimal(auxiliar);

                                    }
                                }
                                arch.EscribirArch(fullPathH, pathToSave);
                                stream.Close();
                            }


                            var stream1= new FileStream(fullPathH, FileMode.Open);

                            return File(stream1, System.Net.Mime.MediaTypeNames.Application.Octet, fileNameH);

                        }
                        if (metodo == "lzw")
                        {

                            Lzw arch = new Lzw();
                            using (var stream = new FileStream(fullPath, FileMode.Create))
                            {
                                arch.Name = file.FileName;
                                file.CopyTo(stream);
                            }
                            arch.Size = file.Length;
                            using (var stream = new FileStream(fullPath, FileMode.Open))
                            {

                                using (BinaryReader br = new BinaryReader(stream))
                                {
                                    var buffer = new byte[Data.BufferLength];
                                    while (br.BaseStream.Position != br.BaseStream.Length)
                                    {
                                        buffer = br.ReadBytes(Data.BufferLength); //llenar el buffer
                                        arch.IniciarDicLZW(buffer);//enviar buffer al diccionario

                                    }
                                }

                                stream.Close();
                            }
                            //CREACION ARCHIVO .lzw
                            var fileNameH = file_nameN + ".lzw";
                            var fullPathH = Path.Combine(pathToSave, fileNameH);
                            var dbPathH = Path.Combine(folderName, fileNameH);


                            using (var stream = new FileStream(fullPathH, FileMode.Create))
                            {
                                //Escribimos en el archivo
                            }

                            arch.buffer_escr = new byte[0];
                            using (var stream = new FileStream(fullPath, FileMode.Open))
                            {
                                using (BinaryReader br = new BinaryReader(stream))
                                {

                                    var buffer = new byte[Data.BufferLength];
                                    while (br.BaseStream.Position != br.BaseStream.Length)
                                    {
                                        buffer = br.ReadBytes(Data.BufferLength); //llenar el buffer
                                        arch.TraducirLZW(buffer); //traducir en lzw


                                    }

                                }
                                arch.EscribirLZW(fullPathH,pathToSave); //escribo en mi archivo LZW
                                stream.Close();
                            }
                            var stream1 = new FileStream(fullPathH, FileMode.Open);

                            return File(stream1, System.Net.Mime.MediaTypeNames.Application.Octet, fileNameH);
                        }
                        
                        return Ok(new { error});
                    }
                    else
                    {
                        return BadRequest();
                    }

                }

                return BadRequest();

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        
    }
}