let blinkTextMenuLinks = document.querySelectorAll(".blink-text-menu a");

blinkTextMenuLinks.forEach((link) => {
  let letters = link.textContent.split("");
  link.textContent = "";
  letters.forEach((letter, i) => {
    i += 1;
    let span = document.createElement("span");
    let delay = i / 40;
    if (i % 2 === 0) {
      delay -= 0.05;
    } else {
      delay += 0.05;
    }
    let letterOut = document.createElement("span");
    letterOut.textContent = letter;
    letterOut.style.transitionDelay = `${delay}s`;
    letterOut.classList.add("out");
    span.append(letterOut);
    let letterIn = document.createElement("span");
    letterIn.textContent = letter;
    letterIn.style.transitionDelay = `${delay}s`;
    letterIn.classList.add("in");
    span.append(letterIn);
    link.append(span);
  });
});


document.querySelectorAll(".blink-text-menu a").forEach((link) => {
  link.addEventListener("click", function () {
    // Remove "active" class from all links before adding it to the clicked one
    document.querySelectorAll(".blink-text-menu a").forEach((el) => {
      el.classList.remove("active");
    });

    // Add "active" class to clicked link
    this.classList.add("active");
  });
});


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


function wait(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

async function delayedFunction() {
blinkTextParent = document.querySelector(".blink-text-menu");
blinkTextParent.style.display = "none";
    await wait(1000); // Wait for 2 seconds
blinkTextParent.style.display = "inherit";
}

delayedFunction();

