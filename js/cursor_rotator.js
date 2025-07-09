var o = $(".card-image");
$("#top").on("mousemove", function (t) {
  var e = -($(window).innerWidth() / 2 - t.pageX) / 30,
    n = ($(window).innerHeight() / 0.5 - t.pageY) / 10;
  o.attr(
    "style",
    "transform: rotateY(" +
    e +
    "deg) rotateX(" +
    n +
    "deg);-webkit-transform: rotateY(" +
    e +
    "deg) rotateX(" +
    n +
    "deg);-moz-transform: rotateY(" +
    e +
    "deg) rotateX(" +
    n +
    "deg)"
  );
});

function resetRot(){
    o.attr(
    "style",
    "transform: rotateY(0deg) rotateX(0deg);-webkit-transform: rotateY(0deg) rotateX(0deg);-moz-transform: rotateY(0deg) rotateX(0deg)"
  );
}

$("#top").on("mouseleave", resetRot);
window.addEventListener("scroll", resetRot);