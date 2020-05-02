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
    [Route("api/cipher/getPublicKey")]
    [ApiController]
    public class getkey : ControllerBase
    {

        [HttpGet]
        public FileResult Upload()
        {
            var folderName = Path.Combine("Resources", "Files");
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            string pathToSaveK = Path.Combine(pathToSave, "Keys");
            string pathToSaveZip = Path.Combine(Directory.GetCurrentDirectory(), "KeyComprimido");
            string archzip = Path.Combine(pathToSaveZip, "keys.zip");
            if (Directory.Exists(pathToSaveZip))
            {
                FileInfo f = new FileInfo(archzip);
                if (f.Exists)
                {
                    f.Delete();
                }
               
                ZipFile.CreateFromDirectory(pathToSaveK, archzip);
                var stream1 = new FileStream(archzip, FileMode.Open);

                return File(stream1, System.Net.Mime.MediaTypeNames.Application.Octet, "keys.zip");
            }
            else
            {
                Directory.CreateDirectory(pathToSaveZip);
                ZipFile.CreateFromDirectory(pathToSaveK, archzip);
                var stream1 = new FileStream(archzip, FileMode.Open);

                return File(stream1, System.Net.Mime.MediaTypeNames.Application.Octet, "archivo.zip");
            }
        }
               
    }
}
