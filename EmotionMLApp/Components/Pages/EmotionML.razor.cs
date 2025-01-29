using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace EmotionMLApp.Components.Pages
{
    public partial class EmotionML
    {
        private string webcamStatus = "Not Started";
        private string emotionDetectBtn = "Start EmotionML";
        private string webcamBtn = "Start WebCam";
        private bool isWebcamStarted = false;

        private async Task toggleWebcamState()
        {
            if (!isWebcamStarted)
            {
                await StartWebcam();
            }
            else
            {
                await StopWebcam();
            }
        }

        private async Task StartWebcam()
        {
            await JS.InvokeVoidAsync("startWebcam");
            isWebcamStarted = true;
            webcamStatus = "Live!";
            webcamBtn = "Stop WebCam";
        }

        private async Task StopWebcam()
        {
            await JS.InvokeVoidAsync("stopWebcam");
            isWebcamStarted = false;
            webcamStatus = "Not Started";
            webcamBtn = "Start WebCam";
        }
    }
}
