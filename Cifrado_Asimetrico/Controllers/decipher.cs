using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Cifrado_Asimetrico.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cifrado_Asimetrico.Controllers
{
    [Route("api/decipher/cesar2")]
    [ApiController]
    public class decipher : ControllerBase
    {

        [HttpPost, DisableRequestSizeLimit]
        public FileResult Upload()
        {

            var file = Request.Form.Files[0];
            string type_cipher = Request.Form["tipo"];
            int BufferLength = 100;

            //lectura archivo
            var folderName = Path.Combine("Resources", "Files");
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            if (!Directory.Exists(pathToSave))
            {

                Directory.CreateDirectory(pathToSave);
            }
            string extension = Path.GetExtension(file.FileName);
            var buffer = new byte[100];

            if (extension == ".txt")
            {
                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var fullPath = Path.Combine(pathToSave, fileName);
                    string file_nameN = "descifrado.txt";
                    string pathToSave1 = Path.Combine(pathToSave, "Archivos"); //Carpeta donde estarán los archivos que devuelve cesar
                    

                    //Archivos que devuelve cesar
                    string fullPathC = Path.Combine(pathToSave1, file_nameN);
                    if (Directory.Exists(pathToSave1))
                    {

                        FileInfo f = new FileInfo(fullPathC);
                        if (f.Exists)
                        {
                            f.Delete();
                        }
                        
                    }
                    else
                    {
                        Directory.CreateDirectory(pathToSave1);
                    }

                    string clave = Request.Form["clave"];
                    int cl = int.Parse(clave);

                    if (type_cipher == "rsa")
                    {
                        string auxe = Request.Form["e"];
                        string auxn = Request.Form["n"];
                        CifradoRSA cifa = new CifradoRSA();
                        cifa.e = Convert.ToInt32(auxe);
                        cifa.n = Convert.ToInt32(auxn);
                        cl= cifa.DesCifrar(cl);
                    }
                    else if (type_cipher == "diffie")
                    {
                        string auxB = Request.Form["B"];
                        Diffie cifa = new Diffie();
                        cifa.priv = Convert.ToInt32(cl);
                        cifa.publ = Convert.ToInt32(auxB);
                        cifa.CalcularPubl();
                        cl = cifa.k;
                       

                    }
                    else
                    {
                        return null;
                    }
                  
                    Cesar2 cif = new Cesar2();
                    cif.LlenarDiccionarios();
                    //clave mayor a 26
                    if (cl>25)
                    {
                        int n = -(25 - cl);
                        cl = n;
                    }
                    cif.LlenarClaveInt(cl);

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
                                buffer = br.ReadBytes(BufferLength); //llenar el buffer  
                                cif.DesCifrar(buffer);
                                cif.EscribirCifrado(fullPathC);
                            }
                        }
                    }

                    var stream1 = new FileStream(fullPathC, FileMode.Open);

                    return File(stream1, System.Net.Mime.MediaTypeNames.Application.Octet, "archivo.zip");

                }
                else
                {
                    return null;
                }
                }
                else
                {
                    return null;
                }

            
            
        }
    }
}
