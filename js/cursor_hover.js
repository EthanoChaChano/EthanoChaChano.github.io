let scrollX = 0;
let scrollY = 0;

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
        }, index * 100); // Adjust delay for stagger effect
    });
});
