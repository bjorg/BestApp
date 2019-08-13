using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.S3;
using Amazon.S3.Model;
using LambdaSharp;
using LambdaSharp.ApiGateway;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace My.BestApp.BestAppFunction {

    public class UploadRequest {

        //--- Properties ---
        [JsonRequired]
        public string Name { get; set; }

        [JsonRequired]
        public string Content { get; set; }
    }

    public class UploadResponse {

        //--- Properties ---
        public string Status { get; set; }
    }

    public class Function : ALambdaApiGatewayFunction {

        //--- Fields ---
        private string _bucketName;
        private IAmazonS3 _s3Client;

        //--- Methods ---
        public override async Task InitializeAsync(LambdaConfig config) {
            _bucketName = config.ReadS3BucketName("BestAppBucket");
            _s3Client = new AmazonS3Client();
        }

        public async Task<UploadResponse> UploadAsync(UploadRequest request) {
            await _s3Client.PutObjectAsync(new PutObjectRequest {
                BucketName = _bucketName,
                Key = request.Name,
                ContentBody = request.Content,
                ContentType = "text/plain"
            });
            return new UploadResponse {
                Status = "success"
            };
        }
    }
}
