using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;
using Tensorflow;
using Microsoft.ML.Vision;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using static Tensorflow.Binding;
using Google.Protobuf;
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
        var modelPath = "ssd_mobilenet_v2/saved_model";
        var graphDef = new GraphDef();
        using (var file = File.OpenRead(modelPath))
        {
            graphDef.MergeFrom(new CodedInputStream(file));
            tf.import_graph_def(graphDef, name: "");
        }
        // Assuming you need to return an ITransformer object, you might need to adjust this part
        // to properly load and return the model as an ITransformer.
        return _mlContext.Model.Load(modelPath, out var modelInputSchema);
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
    public Bitmap Image { get; set; }
    public byte[] ImagePath { get; set; }
}

public class ImagePrediction : ImageInputData
{
    public float[] DetectionScores { get; set; }
    public float[][] DetectionBoxes { get; set; }
}
