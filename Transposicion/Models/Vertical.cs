using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Transposicion.Models
{
    public class Vertical
    {
        public string Cipher(Microsoft.AspNetCore.Http.IFormFile file, int restriccion, int column, int row, string type_cipher, string file_nameN)
        {
            if (restriccion > file.Length)
            {
                var folderName = Path.Combine("Resources", "Files");
                byte[,] matriz = new byte[row, column];
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                string extension = Path.GetExtension(file.FileName);
                var buffer = new byte[Data.BufferLength];
                int count = 0;
                int i = 0;
                int j = 0;

                

                if (extension == ".txt")
                {
                    if (file.Length > 0)
                    {
                        var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                        var fullPath = Path.Combine(pathToSave, fileName);
                        var dbPath = Path.Combine(folderName, fileName);

                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        //LECTURA ARCHIVO SUBIDO
                        using (var stream = new FileStream(fullPath, FileMode.Open))
                        {

                            using (BinaryReader br = new BinaryReader(stream))
                            {

                                while (br.BaseStream.Position != br.BaseStream.Length)
                                {
                                    buffer = br.ReadBytes(Data.BufferLength); //llenar el buffer  
                                }

                                count = 0;
                                i = 0;

                                while (i < row)
                                {
                                    j = 0;
                                    while (j < column)
                                    {
                                        if (count < buffer.Length)
                                        {
                                            matriz[i, j] = buffer[count];
                                        }
                                        else
                                        {
                                            matriz[i, j] = Convert.ToByte(36);
                                        }
                                        count++;
                                        j++;
                                    }
                                    i++;
                                }

                            }

                            stream.Close();
                           
                        }

                        //CREACION ARCHIVO
                        var fileNameH = file_nameN + "-ruta-" + type_cipher + ".txt";
                        var fullPathH = Path.Combine(pathToSave, fileNameH);
                        var dbPathH = Path.Combine(folderName, fileNameH);
                        i = 0;
                        j = 0;

                        using (var stream = new FileStream(fullPathH, FileMode.Create))
                        {
                            //Escribimos en el archivo
                        }

                        using (var stream = new FileStream(fullPathH, FileMode.Open))
                        {

                            using (BinaryWriter wr = new BinaryWriter(stream))
                            {
                                count = 0;
                                j = 0;

                                while (j < column)
                                {
                                    i = 0;
                                    while (i < row)
                                    {
                                        wr.Write(matriz[i, j]);
                                        count++;
                                        i++;
                                    }
                                    j++;
                                }

                            }

                            stream.Close();
                            

                        }

                        return "El archivo fue cifrado con exito en : " + fullPathH;
                    }
                    else
                    {
                        return "BadRequest()";
                    }
                }
                else
                {
                    return "El archivo no es de texto";
                }
            }
            else
            {
                return "La matriz que ha creado no es capaz de almacenar el archivo ingresado";
            }
        }

        public string Decipher(Microsoft.AspNetCore.Http.IFormFile file, int restriccion, int column, int row, string type_cipher, string file_nameN)
        {
            if (restriccion >= file.Length)
            {
                var folderName = Path.Combine("Resources", "Files");
                byte[,] matriz = new byte[row, column];
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                string extension = Path.GetExtension(file.FileName);
                var buffer = new byte[Data.BufferLength];
                int count = 0;
                int i = 0;
                int j = 0;

                

                if (extension == ".txt")
                {
                    if (file.Length > 0)
                    {
                        var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                        var fullPath = Path.Combine(pathToSave, fileName);
                        var dbPath = Path.Combine(folderName, fileName);

                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        //LECTURA ARCHIVO SUBIDO
                        using (var stream = new FileStream(fullPath, FileMode.Open))
                        {

                            using (BinaryReader br = new BinaryReader(stream))
                            {

                                while (br.BaseStream.Position != br.BaseStream.Length)
                                {
                                    buffer = br.ReadBytes(Data.BufferLength); //llenar el buffer  
                                }

                                count = 0;
                                i = 0;

                                while (j < column)
                                {
                                    i = 0;
                                    while (i < row)
                                    {
                                        if (count < buffer.Length)
                                        {
                                            matriz[i, j] = buffer[count];
                                        }
                                        else
                                        {
                                            matriz[i, j] = Convert.ToByte(36);
                                        }
                                        count++;
                                        i++;
                                    }
                                    j++;
                                }

                            }

                            stream.Close();
                     
                        }

                        //CREACION ARCHIVO
                        var fileNameH = file_nameN + "-ruta-" + type_cipher + ".txt";
                        var fullPathH = Path.Combine(pathToSave, fileNameH);
                        var dbPathH = Path.Combine(folderName, fileNameH);
                        i = 0;
                        j = 0;

                        using (var stream = new FileStream(fullPathH, FileMode.Create))
                        {
                            //Escribimos en el archivo
                        }

                        using (var stream = new FileStream(fullPathH, FileMode.Open))
                        {

                            using (BinaryWriter wr = new BinaryWriter(stream))
                            {
                                count = 0;
                                j = 0;

                                while (i < row)
                                {
                                    j = 0;
                                    while (j < column)
                                    {
                                        wr.Write(matriz[i, j]);
                                        count++;
                                        j++;
                                    }
                                    i++;
                                }

                            }

                            stream.Close();
                            

                        }

                        return "El archivo fue cifrado con exito en : " + fullPathH;
                    }
                    else
                    {
                        return "BadRequest()";
                    }
                }
                else
                {
                    return "El archivo no es de texto";
                }
            }
            else
            {
                return "La matriz que ha creado no es capaz de almacenar el archivo ingresado";
            }
        }
    }
}
