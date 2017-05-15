using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

// Amazon Specific headers
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

namespace SimpleFile.Controllers
{
    [Route("[controller]")]
    public class S3FileController : Controller
    {
        static private uint         _PRESIGNED_URL_LIFE_MINUTES = 2;
        static private HttpClient   _httpClient                 = new HttpClient(); // Instantiate the httpClient here and share it, so that we don't waste sockets.
        static private string       bucketName                  = "<BLOB>";

        public IAmazonS3    _S3Client { get; set; }
        
        public S3FileController(IAmazonS3 s3FileClient)
        {
            _S3Client = s3FileClient;
        }

        // GET S3File/list
        [HttpGet("list")]
        public Task<string> ListFiles()
        {
            return ListKeysInBucket();
        }

        public async Task<string> ListKeysInBucket()
        { 
            StringBuilder sb = new StringBuilder();; 
            try
            {
                ListObjectsV2Request request = new ListObjectsV2Request
                {
                    BucketName = bucketName,
                    MaxKeys = 10
                };
                ListObjectsV2Response response;
                do
                {
                    response = await _S3Client.ListObjectsV2Async(request);

                    // Process response.
                    int count = 1;
                    foreach (S3Object entry in response.S3Objects)
                    {
                        sb.Append($"{count}. {entry.Key} ({entry.Size} bytes)\n");
                        Console.WriteLine("{0}. {1} ({2} bytes)",
                            count, entry.Key, entry.Size);

                            count++;
                    }
                    Console.WriteLine("Next Continuation Token: {0}", response.NextContinuationToken);
                    request.ContinuationToken = response.NextContinuationToken;
                } while (response.IsTruncated == true);
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                    ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    Console.WriteLine("Check the provided AWS Credentials.");
                    Console.WriteLine("To sign up for service, go to http://aws.amazon.com/s3");
                }
                else
                {
                    Console.WriteLine("Error occurred. Message:'{0}' when listing objects", amazonS3Exception.Message);
                }
            }

            return sb.ToString();
        }

        // GET S3File/redirect/{filename}
        [HttpGet("redirect/{filename}")]
        public IActionResult GetWithRedirect(string filename)
        {
            // Generate a PreSigned URL with a fixed lifetime and redirect the user to the URL
            var url = GeneratePreSignedURL(filename);
            return Redirect(url);
        }

        // GET S3File/download/{filename}
        [HttpGet("download/{filename}")]
        public async Task<IActionResult> GetWithStreamingDownload(string filename)
        {
            // Generate a PreSigned URL with a fixed lifetime and stream the file to the user.
            var url = GeneratePreSignedURL(filename);
            var stream = await _httpClient.GetStreamAsync(url);
            return new FileStreamResult(stream, "application/octet-stream")
            {
                FileDownloadName = filename
            };
        }

        // File Uploading 
        [HttpGet("UploadFiles")]
        public IActionResult UploadFiles()
        {
            return View();
        }

        // POST api/S3File/UploadFiles
        [HttpPost("UploadFiles")]
        public async Task<IActionResult> UploadFiles(IList<IFormFile> files)
        {
            Console.WriteLine("In Upload Files Post method");
            Task<bool>[] requests = files.Select( file =>
                                            {
                                                var filename = ContentDispositionHeaderValue
                                                                .Parse(file.ContentDisposition)
                                                                .FileName
                                                                .Trim('"');

                                                Console.WriteLine($"Uploading {filename}");
                                                return UploadSingleFileToS3(filename, file.OpenReadStream(), file.ContentType);   
                                            }                        
                                          ).ToArray();
            
            await Task.WhenAll(requests);

            int filesUploadedCount = 0;
            foreach (var uploadRequests in requests)
            {
                if (uploadRequests.Result)
                    filesUploadedCount ++;
            }

            ViewBag.Message = $"{filesUploadedCount} file(s) uploaded successfully!";
            return View();
        }

        private async Task<bool> UploadSingleFileToS3(string filename, Stream streamOfFileToUpload, string contentType)
        {
            bool success = false;
            try
            {
                PutObjectRequest request = new PutObjectRequest()
                {
                    BucketName = bucketName,
                    Key = filename,
                    InputStream = streamOfFileToUpload,
                    ContentType = contentType
                };

                var response = await _S3Client.PutObjectAsync(request);
                success = true;
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                    ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    Console.WriteLine("Check the provided AWS Credentials.");
                    Console.WriteLine("To sign up for service, go to http://aws.amazon.com/s3");
                }
                else
                {
                    Console.WriteLine(  "Error occurred. Message:'{0}' when uploading the file {1}",
                                        amazonS3Exception.Message, filename);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return success;
        }

        // Open a public URL to the file on S3 with a fixed life timer.
        private string GeneratePreSignedURL(string filename)
        {
            string urlString = "";
            GetPreSignedUrlRequest request1 = new GetPreSignedUrlRequest
            {
                 BucketName = bucketName,
                 Key = filename,
                 Expires = DateTime.Now.AddMinutes(_PRESIGNED_URL_LIFE_MINUTES)
                 
            };

            try
            {
                urlString = _S3Client.GetPreSignedURL(request1);
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                    ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    Console.WriteLine("Check the provided AWS Credentials.");
                    Console.WriteLine("To sign up for service, go to http://aws.amazon.com/s3");
                }
                else
                {
                    Console.WriteLine(  "Error occurred. Message:'{0}' when generating a presigned url for {1}",
                                        amazonS3Exception.Message, filename);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return urlString;

        }
    }
}
