function reveal() {
  var reveals = document.querySelectorAll(".reveal");

  for (var i = 0; i < reveals.length; i++) {
    var windowHeight = window.innerHeight;
    var elementTop = reveals[i].getBoundingClientRect().top;
    var elementVisible = reveals[i].dataset.visible || 50;

    var leftChildren = reveals[i].querySelectorAll(".left");
    var rightChildren = reveals[i].querySelectorAll(".right");

    if (elementTop <= windowHeight - elementVisible) {
      reveals[i].classList.add("active");

      leftChildren.forEach((child) => {
        child.classList.add("active");
      });
      rightChildren.forEach((child) => {
        child.classList.add("active");
      });

    } else {
      reveals[i].classList.remove("active");

      leftChildren.forEach((child) => {
        child.classList.remove("active");
      });
      rightChildren.forEach((child) => {
        child.classList.remove("active");
      });
    }
  }
}

window.addEventListener("scroll", reveal);

// To check the scroll position on the page load
reveal();