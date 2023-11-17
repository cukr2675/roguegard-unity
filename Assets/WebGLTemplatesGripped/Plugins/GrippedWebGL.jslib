mergeInto(LibraryManager.library, {

  GrippedWebGLLockAspectRatio: function (aspectRatio) {
    grippedWebGLTemplate.setAspectRatio(aspectRatio);
  },

  GrippedWebGLFreeAspectRatio: function () {
    grippedWebGLTemplate.setAspectRatio(null);
  },

  GrippedWebGLSetPixelated: function (pixelated) {
    grippedWebGLTemplate.setPixelated(pixelated);
  },

});