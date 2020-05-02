using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net.Http.Headers;
using est2_lab3.Models;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Net;

namespace est2_lab3.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DecompressController : ControllerBase
    {
        

        [HttpPost, DisableRequestSizeLimit]
        public IActionResult Upload()
        {
            try
            {
                var file = Request.Form.Files[0];
                var folderName = Path.Combine("Resources", "Files");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                string extension = Path.GetExtension(file.FileName);
               
                var mensajee = "Archivo con extensión incorrecta";
                var fileNameH = "";
                var fullPathH = "";

                if (extension == ".huff")
                {

                    if (file.Length > 0)
                    {
                        Huffman arch = new Huffman();
                        bool lectura_n = false; //Bandera de la lectura de la cantidad de bytes del nombre
                        bool lectura_n1 = false; //Bandera de la lectura del nombre
                        bool lectura_1 = false; //Bandera de la lectura de los primeros dos bytes;
                        bool lectura_can = false; //bandera de la lectura del numero de codigos asci
                        bool lectura_2 = false;// Bandera de lectura de los codigos ascci;
                        int can_as = 0;
                        //SUBIR ARCHIVO
                        var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                        var fullPath = Path.Combine(pathToSave, fileName);
                        var dbPath = Path.Combine(folderName, fileName);
                        string nom = String.Empty;
                        
                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }
                        int cantidadle = 0;
                        int cantidadn = 0;
                        using (var stream = new FileStream(fullPath, FileMode.Open))
                        {
                            using (BinaryReader br = new BinaryReader(stream))
                            {
                                var buffer = new byte[Data.BufferLength];
                                while (br.BaseStream.Position != br.BaseStream.Length)
                                {
                                    if (lectura_n == false)
                                    {
                                        buffer = br.ReadBytes(1);
                                        cantidadn = Convert.ToInt32(buffer[0]);
                                        lectura_n = true;

                                    }
                                    else if (lectura_n1 == false)
                                    {
                                        buffer = br.ReadBytes(cantidadn);
                                        
                                        for(int x=0; x < cantidadn; x++)
                                        {
                                            nom = nom + Convert.ToChar(buffer[x]);
                                        }
                                        lectura_n1 = true;
                                    }
                                    else if (lectura_1 == false)
                                    {
                                        buffer = br.ReadBytes(2);// Leo los dos primeros bytes
                                        cantidadle = Convert.ToInt32(buffer[0]);
                                        arch.agregados = Convert.ToInt32(buffer[1]);

                                        lectura_1 = true;

                                    }
                                    else if (lectura_can == false)
                                    {
                                        buffer = br.ReadBytes(cantidadle);
                                        string aux = string.Empty;
                                        for (int x = 0; x < cantidadle; x++)
                                        {
                                            aux = aux + Convert.ToChar(buffer[x]);
                                        }
                                        can_as = Convert.ToInt32(aux);
                                        lectura_can = true;
                                    }
                                    else if (lectura_2 == false)
                                    {
                                        buffer = br.ReadBytes(can_as); //Leo la cantidad de codigos ascci agregados
                                        arch.GuardarAscci(buffer);
                                        lectura_2 = true;
                                    }
                                    else
                                    {
                                        buffer = br.ReadBytes(Data.BufferLength);//llenar el buffer
                                        arch.IniciarDicODesc(buffer);
                                    }
                                }
                            }

                            stream.Close();
                        }
                        arch.Start_Dictionary();// diccionario con probabilidades
                        arch.Add_Pr(); //sumar probabilidades


                        arch.OrdenarDic();

                        arch.CrearArbol();
                        //CREACION ARCHIVO .TXT
                        fileNameH = nom + ".txt";
                         fullPathH = Path.Combine(pathToSave, fileNameH);
                        var dbPathH = Path.Combine(folderName, fileNameH);

                        using (var stream = new FileStream(fullPathH, FileMode.Create))
                        {
                            //Escribimos en el archivo
                        }
                        arch.Binarios(fullPathH);
                    
                    }

                    else
                    {
                        return BadRequest();
                    }
                    var stream1 = new FileStream(fullPathH, FileMode.Open);

                    return File(stream1, System.Net.Mime.MediaTypeNames.Application.Octet, fileNameH);

                }
                
                
                if(extension== ".lzw")
                {
                    if (file.Length > 0)
                    {
                        Lzw arch = new Lzw();
                        bool lectura_n = false; //Bandera de la lectura de la cantidad de bytes del nombre
                        bool lectura_n1 = false; //Bandera de la lectura del nombre
                        bool lectura_1 = false; //Bandera de la lectura de los primeros dos bytes;
                        bool lectura_can = false; //bandera de la lectura del numero de codigos asci
                        bool lectura_2 = false;// Bandera de lectura de los codigos ascci;
                        int can_as = 0;
                        //SUBIR ARCHIVO
                        var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                        var fullPath = Path.Combine(pathToSave, fileName);
                        var dbPath = Path.Combine(folderName, fileName);
                        string nom = String.Empty;
                        int cantidadle = 0;
                        
                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        using (var stream = new FileStream(fullPath, FileMode.Open))
                        {
                            using (BinaryReader br = new BinaryReader(stream))
                            {
                                var buffer = new byte[Data.BufferLength];
                                while (br.BaseStream.Position != br.BaseStream.Length)
                                {
                                    if (lectura_n == false)
                                    {
                                        buffer = br.ReadBytes(1);
                                        cantidadle= Convert.ToInt32(buffer[0]);
                                        lectura_n = true;
                                    }
                                    else if (lectura_n1 == false)
                                    {
                                        buffer = br.ReadBytes(cantidadle);
                                        nom = string.Empty;
                                        for (int x = 0; x < cantidadle; x++)
                                        {
                                            nom = nom + Convert.ToChar(buffer[x]);
                                        }
                                        lectura_n1 = true;
                                    }
                                    else if (lectura_1 == false)
                                    {
                                        buffer = br.ReadBytes(1);// Leo los dos primeros bytes
                                        cantidadle = Convert.ToInt32(buffer[0]);
                                        lectura_1 = true;

                                    }
                                    else if (lectura_can == false)
                                    {
                                        buffer = br.ReadBytes(cantidadle);
                                        string aux = string.Empty;
                                        for (int x = 0; x < cantidadle; x++)
                                        {
                                            aux = aux + Convert.ToChar(buffer[x]);
                                        }
                                        can_as = Convert.ToInt32(aux);
                                        lectura_can = true;
                                    }
                                    else if (lectura_2 == false)
                                    {
                                        buffer = br.ReadBytes(can_as); //Leo la cantidad de codigos ascci agregados
                                        arch.GuardarAscci(buffer);
                                        lectura_2 = true;
                                    }
                                    else
                                    {
                                        buffer = br.ReadBytes(Data.BufferLength);//llenar el buffer
                                        arch.IniciarDicODescLZW(buffer);
                                    }
                                }
                            }

                            stream.Close();
                        }
                        fileNameH = nom + ".txt";
                         fullPathH = Path.Combine(pathToSave, fileNameH);
                        var dbPathH = Path.Combine(folderName, fileNameH);

                        using (var stream = new FileStream(fullPathH, FileMode.Create))
                        {
                            //Escribimos en el archivo
                        }
                        arch.EscribirTXTLZW(fullPathH);

                    }
                    else
                    {
                        return BadRequest();
                    }
                    var stream1 = new FileStream(fullPathH, FileMode.Open);

                    return File(stream1, System.Net.Mime.MediaTypeNames.Application.Octet, fileNameH);
                }
                return Ok(new { mensajee });

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}