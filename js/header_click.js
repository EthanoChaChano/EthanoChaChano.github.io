// Add active class to the current button (highlight it)
var header = document.getElementById("header");
var headerBtn = header.getElementsByClassName("headerBtn");
for (var i = 0; i < headerBtn.length; i++) {
    headerBtn[i].addEventListener("click", function () {
        var current = document.getElementsByClassName("active");
        current[0].className = current[0].className.replace(" active", "");
        this.className += " active";
    });
}

// Function to check if the element is visible in the viewport
function isElementVisible(element) {
  const elementTop = element.offsetTop;
  const elementBottom = elementTop + element.offsetHeight;
  const viewportTop = window.scrollY;
  const viewportBottom = viewportTop + window.innerHeight;

  return (
    elementBottom > viewportTop &&
    elementTop < viewportBottom
  );
}

// Function to deactivate all headers by removing the "active" class
function deactivateAllHeaders() {
  const allHeaders = document.querySelectorAll('.headerBtn'); // Ensure all headers have class 'header'
  allHeaders.forEach(function(header) {
    header.classList.remove('active');
  });
}

// Function to toggle the "active" class on the corresponding header
function toggleElementActive(id, header) {
  const elementToCheck = id;
  var btn = header;

  // Check if the section is visible and its header is not active
  if (isElementVisible(elementToCheck)) {
    if (!btn.classList.contains("active")) {
      // Deactivate all headers before activating the current one
      deactivateAllHeaders();
      btn.classList.add("active");
      return true;  // Return true to indicate the header was activated
    }
  } else {
    // If the section is not visible and the header is active, deactivate it
    if (btn.classList.contains("active")) {
      btn.classList.remove("active");
    }
  }

  return false;
}

// Variable to track the last active section's header
let lastActiveHeader = null;

// Scroll event listener to check which section is in view and activate the corresponding header
window.addEventListener('scroll', function() {

  // Call the function for each section and its corresponding header
  if (toggleElementActive(document.getElementById('contact'), document.getElementById('headerContact'))) {
    lastActiveHeader = document.getElementById('headerContact');
  }
  if (toggleElementActive(document.getElementById('project'), document.getElementById('headerProject'))) {
    lastActiveHeader = document.getElementById('headerProject');
  }
  if (toggleElementActive(document.getElementById('about'), document.getElementById('headerAbout'))) {
    lastActiveHeader = document.getElementById('headerAbout');
  }
  if (toggleElementActive(document.getElementById('home'), document.getElementById('headerHome'))) {
    lastActiveHeader = document.getElementById('headerHome');
  }
});

