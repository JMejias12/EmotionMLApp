using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.Image;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

public class FaceDetectionService
{
    private readonly MLContext _mlContext;
    private readonly ITransformer _model;

    public FaceDetectionService()
    {
        _mlContext = new MLContext();
        _model = LoadModel();
    }

    private ITransformer LoadModel()
    {
        var pipeline = _mlContext.Transforms.LoadImages(outputColumnName: "image", imageFolder: "", inputColumnName: nameof(ImageInputData.ImagePath))
            .Append(_mlContext.Transforms.ResizeImages(outputColumnName: "image", imageWidth: 300, imageHeight: 300))
            .Append(_mlContext.Transforms.ExtractPixels(outputColumnName: "image"))
            .Append(_mlContext.Model.LoadTensorFlowModel("ssd_mobilenet_v1_coco_2017_11_17/saved_model")
                .ScoreTensorName("detection_scores")
                .AddInput("image")
                .AddOutput("detection_boxes", "detection_classes", "detection_scores"));

        return pipeline.Fit(_mlContext.Data.LoadFromEnumerable(new List<ImageInputData>()));
    }

    public List<Rectangle> DetectFaces(Bitmap bitmap)
    {
        using var memoryStream = new MemoryStream();
        bitmap.Save(memoryStream, ImageFormat.Png);
        var imageData = new ImageInputData { ImagePath = memoryStream.ToArray() };

        var predictionEngine = _mlContext.Model.CreatePredictionEngine<ImageInputData, ImagePrediction>(_model);
        var prediction = predictionEngine.Predict(imageData);

        var boxes = new List<Rectangle>();
        for (int i = 0; i < prediction.DetectionScores.Length; i++)
        {
            if (prediction.DetectionScores[i] > 0.5)
            {
                var box = prediction.DetectionBoxes[i];
                boxes.Add(new Rectangle((int)(box[1] * bitmap.Width), (int)(box[0] * bitmap.Height), (int)((box[3] - box[1]) * bitmap.Width), (int)((box[2] - box[0]) * bitmap.Height)));
            }
        }

        return boxes;
    }
}

public class ImageInputData
{
    [ImageType(300, 300)]
    public Bitmap Image { get; set; }
}

public class ImagePrediction : ImageInputData
{
    public float[] DetectionScores { get; set; }
    public float[][] DetectionBoxes { get; set; }
}
