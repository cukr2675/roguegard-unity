if (/iPhone|iPad|iPod|Android/i.test(navigator.userAgent)) {
  if (/iPhone|iPad|iPod/i.test(navigator.userAgent)) {
    const addressBarGlip = document.querySelector('#address-bar-glip');
    addressBarGlip.addEventListener('click', () => {
      const balloon = document.querySelector('#balloon');
      balloon.style.display = (balloon.style.display === 'none' ? 'block' : 'none');
    });
  } else if (/Android/i.test(navigator.userAgent)) {
    const addressBarGlip = document.querySelector('#address-bar-glip');
    addressBarGlip.style.position = 'fixed';
    const span = addressBarGlip.querySelector('span');
    span.innerHTML = '全画面モードON';
    span.style.top = '-40px';
    span.style.transform = 'translateX(-50%)';
    span.style.fontSize = '25px';
    const balloon = document.querySelector('#balloon');
    balloon.style.display = 'none';
    addressBarGlip.addEventListener('click', () => {
      document.body.requestFullscreen();
      if (grippedWebGLTemplate.fullscreenOrientation) {
        screen.orientation.lock(grippedWebGLTemplate.fullscreenOrientation);
      }
    });
  }
  
  grippedWebGLTemplate.resize = unityResize;
  
  window.addEventListener('resize', () => {
    grippedWebGLTemplate.resizeDelay();
  });
  document.addEventListener('fullscreenchange', () => {
    grippedWebGLTemplate.resizeDelay();
  });
  
  function unityResize() {
    const unityCanvas = document.querySelector('#unity-canvas');
    const addressBarGlip = document.querySelector('#address-bar-glip');
    const width = window.innerWidth;
    const height = window.innerHeight;
    const aspectRatio = grippedWebGLTemplate.aspectRatio;
    window.scroll({'top': 0});
    
    if (aspectRatio) {
      const w = Math.min(width, height * aspectRatio);
      unityCanvas.style.width = `${Math.floor(w)}px`;
      unityCanvas.style.height = `${Math.floor(w / aspectRatio)}px`;
      unityCanvas.style.marginTop = `${Math.floor((height - w / aspectRatio) / 2)}px`;
    } else {
      unityCanvas.style.width = `${width}px`;
      unityCanvas.style.height = `${height}px`;
      unityCanvas.style.marginTop = '0';
    }

    if (/iPhone|iPad|iPod/i.test(navigator.userAgent)) {
      const dummyControl = document.querySelector('#dummy-control');
      const addressBarIsVisible = height < dummyControl.clientHeight;
      if (addressBarIsVisible) {
        document.body.style.overflow = null;
        document.body.style.marginBottom = '200vh';
        addressBarGlip.style.display = 'block';
      } else {
        document.body.style.overflow = 'hidden';
        document.body.style.marginBottom = '0';
        addressBarGlip.style.display = 'none';
      }
    } else if (/Android/i.test(navigator.userAgent)) {
      if (document.fullscreenElement || window.matchMedia('(display-mode: standalone)').matches || window.matchMedia('(display-mode: fullscreen)').matches) {
        addressBarGlip.style.display = 'none';
      } else {
        addressBarGlip.style.display = 'block';
      }
    }
  }
}
