const imageCount = 16; // Change this to the total number of images you have
const imageFolder = "img/slideshow/"; // Adjust this to your image folder path

// Automatically generate image filenames (e.g., image1.png, image2.png, ..., imageN.png)
const images = Array.from({ length: imageCount }, (_, i) => `${imageFolder}image${i + 1}.png`);
let currentIndex = 0, isImage1Active = true;

function startSlideshow() {
    const img1 = document.getElementById("image1"), img2 = document.getElementById("image2");

    // Set the first image and apply the 'active' class
    img1.src = images[0];
    img1.classList.add("active");

    setInterval(() => {
        currentIndex = (currentIndex + 1) % images.length;
        const nextImage = images[currentIndex];

        if (isImage1Active) {
            img2.src = nextImage;
            img2.classList.add("active");
            img1.classList.remove("active");
        } else {
            img1.src = nextImage;
            img1.classList.add("active");
            img2.classList.remove("active");
        }
        isImage1Active = !isImage1Active;
    }, 5000);
}

window.onload = startSlideshow;