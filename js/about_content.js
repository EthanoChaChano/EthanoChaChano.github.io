const buttons = document.querySelectorAll(".selectors button");
const contents = document.querySelectorAll(".content");

buttons.forEach(button => {
  button.addEventListener("click", () => {
    const targetClass = button.getAttribute("data-target");

    buttons.forEach(button=> button.classList.remove("active"));
    button.classList.add("active");

    contents.forEach(content => content.classList.remove("active"));
    document.querySelector(`.${targetClass}`).classList.add("active");
  });
});
