window.setImage = async (imageElementId, imageStream) => {

    console.log("Setting image for " + imageElementId)

    const arrayBuffer = await imageStream.arrayBuffer();
    console.log("Length was " + arrayBuffer.length)
    const blob = new Blob([arrayBuffer]);
    const url = URL.createObjectURL(blob);
    const image = document.getElementById(imageElementId);
    image.onload = () => {
        URL.revokeObjectURL(url);
    }
    image.src = url;
}