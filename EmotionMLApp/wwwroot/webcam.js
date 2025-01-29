// wwwroot/webcam.js

window.startWebcam = function () {
    const video = document.getElementById("webcam");
    if (navigator.mediaDevices && navigator.mediaDevices.getUserMedia) {
        navigator.mediaDevices.getUserMedia({ video: true })
            .then(function (stream) {
                video.srcObject = stream;
            })
            .catch(function (error) {
                console.error("Error accessing webcam: ", error);
            });
    } else {
        console.error("getUserMedia not supported on your browser!");
    }
}

window.stopWebcam = function () {
    const video = document.getElementById("webcam");
    const stream = video.srcObject;
    if (stream) {
        const tracks = stream.getTracks();
        tracks.forEach(function (track) {
            track.stop();
        });
        video.srcObject = null;
    }
}
