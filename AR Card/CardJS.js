AFRAME.registerComponent('markerhandler', {

    init: function () {
        const animatedMarker = document.querySelector("#animated-marker");

        const portfolio = document.querySelector("#portfolio");
        const linkedIn = document.querySelector("#linkedIn");
        const itchIO = document.querySelector("#itchIO");
        const email = document.querySelector("#email");


        animatedMarker.addEventListener('click', function (ev, target) {
            const intersectedElement = ev && ev.detail && ev.detail.intersectedEl;
            if (portfolio && intersectedElement === portfolio) {
                window.open('https://ethanochachano.github.io', '_blank');
            }

            if (linkedIn && intersectedElement === linkedIn) {
                window.open('https://www.linkedin.com/in/ethanchan603', '_blank');
            }

            if (itchIO && intersectedElement === itchIO) {
                window.open('https://ethano-chachano.itch.io', '_blank');
            }

            if (email && intersectedElement === email) {
                window.location.href = "mailto:ethanochachano@gmail.com"
            }
        });

        const vid = document.querySelector("#cardVideo");
        vid.pause();
        animatedMarker.addEventListener('markerFound', function () {
            video.play();
        });
        animatedMarker.addEventListener('markerLost', function () {
            video.pause();
        });

    }
});