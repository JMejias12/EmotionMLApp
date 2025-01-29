document.addEventListener('DOMContentLoaded', (event) => {
    let video = document.getElementById('webcam');
    let canvas = document.createElement('canvas');
    let context = canvas.getContext('2d');

    async function startWebcam() {
        // Add a small delay to ensure the video element is available
        await new Promise(resolve => setTimeout(resolve, 100));
        video = document.getElementById('webcam');
        if (!video) {
            console.error('Video element not found');
            return;
        }

        let stream = await navigator.mediaDevices.getUserMedia({ video: true });
        video.srcObject = stream;
        video.play();
        requestAnimationFrame(captureFrame);
    }

    function stopWebcam() {
        let stream = video.srcObject;
        let tracks = stream.getTracks();

        tracks.forEach(function (track) {
            track.stop();
        });

        video.srcObject = null;
    }

    async function captureFrame() {
        if (video.paused || video.ended) {
            return;
        }

        canvas.width = video.videoWidth;
        canvas.height = video.videoHeight;
        context.drawImage(video, 0, 0, canvas.width, canvas.height);

        let imageData = canvas.toDataURL('image/png');
        let response = await fetch('/api/FaceDetection/DetectFaces', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ imageData: imageData })
        });

        if (response.ok) {
            let result = await response.json();
            drawRectangles(result.boxes);
        } else {
            console.error('Error detecting faces:', response.statusText);
        }

        requestAnimationFrame(captureFrame);
    }

    function drawRectangles(boxes) {
        context.clearRect(0, 0, canvas.width, canvas.height);
        context.drawImage(video, 0, 0, canvas.width, canvas.height);

        context.strokeStyle = 'red';
        context.lineWidth = 2;

        boxes.forEach(box => {
            context.strokeRect(box.x, box.y, box.width, box.height);
        });
    }

    // Expose startWebcam and stopWebcam to be callable from Blazor
    window.startWebcam = startWebcam;
    window.stopWebcam = stopWebcam;
});



//window.startWebcam = function () {
//    const video = document.getElementById("webcam");
//    if (navigator.mediaDevices && navigator.mediaDevices.getUserMedia) {
//        navigator.mediaDevices.getUserMedia({ video: true })
//            .then(function (stream) {
//                video.srcObject = stream;
//            })
//            .catch(function (error) {
//                console.error("Error accessing webcam: ", error);
//            });
//    } else {
//        console.error("getUserMedia not supported on your browser!");
//    }
//}

//window.stopWebcam = function () {
//    const video = document.getElementById("webcam");
//    const stream = video.srcObject;
//    if (stream) {
//        const tracks = stream.getTracks();
//        tracks.forEach(function (track) {
//            track.stop();
//        });
//        video.srcObject = null;
//    }
//}
