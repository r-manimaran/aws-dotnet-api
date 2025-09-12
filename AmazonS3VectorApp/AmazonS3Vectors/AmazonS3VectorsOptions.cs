namespace AmazonS3Vectors;

public class AmazonS3VectorsOptions
{
    public static string SectionName = "AmazonS3VectorsOptions";
    public string EmbeddingModel { get; set; }
    public string VectorBucketName { get; set; }
    public string VectorIndexName { get; set; }

}
