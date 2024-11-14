using AccessSecretManagerApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace MyApp.Namespace
{
    [Route("api/[controller]")]
    [ApiController]
    public class awsController : ControllerBase
    {
        private readonly SecretsManagerService _secretManagerService;
        private readonly IMemoryCache _cache;
        private readonly IOptions<AwsSettings> _awsSettings;
        private readonly S3Service _s3Service;

        public awsController(SecretsManagerService secretManagerService,
                            IMemoryCache cache,
                            IOptions<AwsSettings> awsSettings,
                            S3Service s3Service)
        {
            this._secretManagerService = secretManagerService;
            this._cache = cache;
            this._awsSettings = awsSettings;
            this._s3Service = s3Service;
        }


        [HttpGet("get-secrets")]
        public async Task<IActionResult> GetAwsSecrets(string secretName)
        {
            var response = await this._secretManagerService.GetAwsSecretsAsync(secretName);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> UploadFileToS3(IFormFile file)
        {
            var response = await this._s3Service.UploadFile(file);
            return Ok(response);
        }

        [HttpGet("get-presignedUrl")]
        public async Task<IActionResult> GetPresignedUrl(string fileName)
        {
            var response = await this._s3Service.GetPresignedUrl(fileName);
            return Ok(response);
        }
    }
}
