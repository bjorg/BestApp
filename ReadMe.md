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

      - Api: POST:/upload
        Invoke: Upload

  - Variable: ApiUrl
    Value: !Ref Module::RestApi::Url
    Scope: public
```

## Lambda Function Definition
```csharp
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
```

## Deploy

1. Install [λ# CLI](https://lambdasharp.net)
```
dotnet tool install -g LambdaSharp.Tool
```

2. Create a λ# deployment tier
```
lash init
```

3. Deploy module (compiles the C# code, generates the CloudFormation template, and uploads them)
```
lash deploy
```
