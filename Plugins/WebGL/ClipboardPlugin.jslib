mergeInto(LibraryManager.library, {
  CopyTextToClipboard: function (text) {
    var str = UTF8ToString(text);
    if (navigator.clipboard && navigator.clipboard.writeText) {
      navigator.clipboard.writeText(str).then(function() {
        console.log('Internal: Copy success');
      }).catch(function(err) {
        console.error('Internal: Copy failed', err);
      });
    } else {
      // Fallback for older browsers
      var textArea = document.createElement("textarea");
      textArea.value = str;
      document.body.appendChild(textArea);
      textArea.select();
      document.execCommand("copy");
      document.body.removeChild(textArea);
    }
  }
});