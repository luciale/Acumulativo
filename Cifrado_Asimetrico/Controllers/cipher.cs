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
    [Route("api/cipher/cesar2")]
    [ApiController]
    public class cipher : ControllerBase
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
                    string file_nameN = "cifrado.txt";
                    string pathToSave1 = Path.Combine(pathToSave, "Archivos"); //Carpeta donde estarán los archivos que devuelve cesar
                    string pathToSaveK = Path.Combine(pathToSave, "Keys"); //Carpeta donde estarán los archivos de las llaves
                    

                    //Archivos que devuelve cesar
                    string fullPathC = Path.Combine(pathToSave1, file_nameN);
                    string PathKey = Path.Combine(pathToSave1, "key.txt");
                    if (Directory.Exists(pathToSave1))
                    {

                        FileInfo f = new FileInfo(fullPathC);
                        if (f.Exists)
                        {
                            f.Delete();
                        }
                        f = new FileInfo(PathKey);
                        if (f.Exists)
                        {
                            f.Delete();
                        }
                       
                    }
                    else
                    {
                        Directory.CreateDirectory(pathToSave1);
                    }

                    //Archivos con las llaves
                    string PathPubl = Path.Combine(pathToSaveK, "public.txt");
                    string PathPriv = Path.Combine(pathToSaveK, "private.txt");
                    if (Directory.Exists(pathToSaveK))
                    {

                        FileInfo f = new FileInfo(PathPubl);
                        f.Delete();
                        f = new FileInfo(PathPriv);
                        f.Delete();
                    }
                    else
                    {
                        Directory.CreateDirectory(pathToSaveK);
                    }


          

                    
                  


                    string clave = Request.Form["clave"];
                    int cl = int.Parse(clave);
                    Cesar2 cif = new Cesar2();
                    cif.LlenarDiccionarios();
                   

                    if (type_cipher == "rsa")
                    {
                        //clave mayor a 26
                        if (cl > 25)
                        {
                            int n = -(26 - cl);
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
                                    cif.Cifrar(buffer);
                                    cif.EscribirCifrado(fullPathC);
                                }
                            }
                        }
                        string auxp = Request.Form["p"];
                        string auxq = Request.Form["q"];
                        int p = int.Parse(auxp);
                        int q = int.Parse(auxq);
                        CifradoRSA cifa = new CifradoRSA();

                        if (cifa.VerificarPrimo(p) && cifa.VerificarPrimo(q))
                        {
                            cifa.p = p;
                            cifa.q = q;
                            cifa.n = (cifa.p) * (cifa.q); //generando N
                            cifa.z = (cifa.p - 1) * (cifa.q - 1); //Generando phu
                            cifa.GenerandoE();
                            //cifa.e = 17;
                            cifa.CalcularD();
                            cifa.Cifrar(PathKey,cl);
                            using (var stream = new FileStream(PathPubl, FileMode.Create))
                            {
                                //ya se creó el archivo
                            }
                            using (var stream = new FileStream(PathPriv, FileMode.Create))
                            {
                                //ya se creó el archivo
                            }

                            cifa.EscribirArchPriv(PathPriv);
                            cifa.EscribirArchPubl(PathPubl);
                            // cifa

                            string pathToSaveZip = Path.Combine(Directory.GetCurrentDirectory(), "Comprimido");
                            string archzip = Path.Combine(pathToSaveZip, "archivos.zip");
                            if (Directory.Exists(pathToSaveZip))
                            {

                                FileInfo f = new FileInfo(archzip);
                                f.Delete();
                            }
                            else
                            {
                                Directory.CreateDirectory(pathToSaveZip);
                            }
                            ZipFile.CreateFromDirectory(pathToSave1,archzip);
                            
                            var stream1 = new FileStream(archzip, FileMode.Open);

                            return File(stream1, System.Net.Mime.MediaTypeNames.Application.Octet, "archivo.zip");

                        }
                        else
                            return null;
                    }
                    
                    else if(type_cipher=="diffie")
                    {
                       
                        string auxB = Request.Form["B"];
                        Diffie cifa = new Diffie();
                        cifa.priv = Convert.ToInt32(cl);
                        cifa.publ = Convert.ToInt32(auxB);
                        cifa.CalcularPubl();
                        cl = cifa.k;
                        //clave mayor a 26
                        if (cl > 25)
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
                                    cif.Cifrar(buffer);
                                    cif.EscribirCifrado(fullPathC);
                                }
                            }
                        }
                        cifa.EscribirArch(PathPubl,PathPriv);

                        var stream1 = new FileStream(fullPathC, FileMode.Open);

                        return File(stream1, System.Net.Mime.MediaTypeNames.Application.Octet, "cifrado.txt");
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
            else
            {
                return null;
            }
        }
    }
}
