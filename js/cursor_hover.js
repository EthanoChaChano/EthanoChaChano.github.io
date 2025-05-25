let scrollX = 0;
let scrollY = 0;
let mouseMoveTimeout;

document.addEventListener("scroll", () => {
    scrollX = window.scrollX;
    scrollY = window.scrollY;
});

document.addEventListener("mousemove", (event) => {
    const cursor = document.querySelector(".cursor");
    const shapes = document.querySelectorAll(".shape");

    const { clientX: x, clientY: y } = event;

    // Adjust the mouse position based on the scroll position
    const mouseX = x + scrollX;
    const mouseY = y + scrollY;

    // Move the cursor instantly
    cursor.style.transform = `translate(${mouseX}px, ${mouseY}px)`;

    // Move the shapes with a delay effect
    shapes.forEach((shape, index) => {
        setTimeout(() => {
            shape.style.transform = `translate(${mouseX}px, ${mouseY}px)`;
            shape.classList.remove("fade-out"); // Remove fade-out if active
        }, index * 100);
    });

    // Clear the previous inactivity timer
    clearTimeout(mouseMoveTimeout);

    // Set a timeout to fade out shapes when the mouse stops moving
    mouseMoveTimeout = setTimeout(() => {
        shapes.forEach((shape) => {
            shape.classList.add("fade-out");
        });
    }, 1000); // 1 second delay for inactivity
});
