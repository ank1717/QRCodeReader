using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FileUpload.Controllers
{
    public class FileUploadController : Controller
    {
        [HttpPost("FileUpload")]
        public async Task<string> Index(Microsoft.AspNetCore.Http.IFormFile file)
        {
            long size = file.Length;
            string response = null;
            string[] imageExtensions = { ".JPG", ".JPE", ".GIF", ".PNG", ".jpg", ".jpe", ".gif", ".png" };
            string extension = Path.GetExtension(file.FileName);
            byte[] fileBytes;
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                fileBytes = ms.ToArray();
            }
            string externalURL = "http://api.qrserver.com/v1/read-qr-code/";
            var destinationUrl = System.Web.HttpUtility.UrlEncode(externalURL);
            if (size < 1048576 && (Array.IndexOf(imageExtensions, extension) >= 0))
            {
                if (file.Length > 0)
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(externalURL);
                        using (var content = new MultipartFormDataContent("Upload" + DateTime.Now.ToString()))
                        {
                            content.Add(new StreamContent(new MemoryStream(fileBytes)));
                            using (var message = await client.PostAsync(destinationUrl, content))
                            {
                                response = await message.Content.ReadAsStringAsync();
                            }
                        }

                    }
                }
            }
            return response;
        }
    }
}