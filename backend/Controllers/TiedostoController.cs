using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SharedLib;

namespace backend.Controllers
{
    [Route("/[controller]")]
    public class TiedostoController : ControllerBase
    {

        private readonly string _azureConnectionString;
        private readonly string _blobContainer;

        public TiedostoController(IConfiguration configuration)
        {
            _azureConnectionString = configuration.GetConnectionString("AzureConnectionString") ?? "";
            _blobContainer = configuration.GetValue<string>("BlobContainerName") ?? "";
        }

        // Tallentaa monta tiedostoa
        [HttpPost("/tiedosto")]
        public async Task<ActionResult<List<UploadResult>>> UploadFile(List<IFormFile> files)
        {
            List<UploadResult> uploadResults = new List<UploadResult>();

            try
            {
                foreach (var file in files)
                {
                    var uploadResult = new UploadResult();

                    
                    var container = new BlobContainerClient(_azureConnectionString, _blobContainer);

                    var blob = container.GetBlobClient(file.FileName);

                    //await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
                    

                    using (var fileStream = file.OpenReadStream())
                    {
                        await blob.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = file.ContentType });
                    }

                    uploadResult.Location = blob.Uri.ToString();
                    uploadResult.FileName = file.FileName;
                    uploadResults.Add(uploadResult);
                }

                return Ok(uploadResults);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal error: {ex}");
            }
        }

        // Lataa yhden tiedoston
        [HttpGet("/tiedosto/lataa/{fileName}")]
        public async Task<IActionResult> DownloadFile(string fileName)
        {
            CloudBlockBlob blockBlob;
            await using (MemoryStream memoryStream = new MemoryStream())
            {
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_azureConnectionString);
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(_blobContainer);
                blockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);
                await blockBlob.DownloadToStreamAsync(memoryStream);
            }
            Stream blobStream = blockBlob.OpenReadAsync().Result;
            return File(blobStream, blockBlob.Properties.ContentType, blockBlob.Name);
        }

        // Poistaa yhden tiedoston
        [HttpDelete("/tiedosto/{fileName}")]
        public async Task<IActionResult> DeleteFile(string fileName)
        {
            var container = new BlobContainerClient(_azureConnectionString, _blobContainer);

            var blob = container.GetBlobClient(fileName);

            await blob.DeleteIfExistsAsync(Azure.Storage.Blobs.Models.DeleteSnapshotsOption.IncludeSnapshots);

            return Ok("Tiedosto poistettu");
        }

    }
}

