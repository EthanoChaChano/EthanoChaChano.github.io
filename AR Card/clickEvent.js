AFRAME.registerComponent('markerhandler', {

    init: function() {
        const animatedMarker = document.querySelector("#animated-marker");
        
        const aEntity = document.querySelector("#animated-model");
        const linkedIn = document.querySelector("#linkedIn");
        const portfolio = document.querySelector("#portfolio");

        animatedMarker.addEventListener('click', function(ev, target){
            const intersectedElement = ev && ev.detail && ev.detail.intersectedEl;
            if (aEntity && intersectedElement === aEntity) {
              window.open('https://example.com', '_blank');
            }
        });

        animatedMarker.addEventListener('click', function(ev, target){
            const intersectedElement = ev && ev.detail && ev.detail.intersectedEl;
            if (linkedIn && intersectedElement === linkedIn) {
              window.open('https://www.linkedin.com/in/ethanchan603', '_blank');
            }
        });

        animatedMarker.addEventListener('click', function(ev, target){
            const intersectedElement = ev && ev.detail && ev.detail.intersectedEl;
            if (portfolio && intersectedElement === portfolio) {
              window.open('https://ethanochachano.github.io', '_blank');
            }
        });

}});