# BestApp in λ#

Sample app that accepts an API Gateway request and stores the data to an S3 bucket.

## Module Definition
```yaml
Module: My.BestApp
Items:

  - Resource: BestAppBucket
    Type: AWS::S3::Bucket
    Scope: BestAppFunction
    Allow: ReadWrite

  - Function: BestAppFunction
    Memory: 256
    Timeout: 30
    Sources:

      - Api: POST:/put
        Invoke: Put
```

## Lambda Function Definition
```csharp

public class PutRequest {

    //--- Properties ---
    [JsonRequired]
    public string Name { get; set; }

    [JsonRequired]
    public string Content { get; set; }
}

public class PutResponse {

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

    public async Task<PutResponse> Put(PutRequest request) {
        await _s3Client.PutObjectAsync(new PutObjectRequest {
            BucketName = _bucketName,
            Key = request.Name,
            ContentBody = request.Content
        });
        return new PutResponse {
            Status = "success"
        };
    }
}
```