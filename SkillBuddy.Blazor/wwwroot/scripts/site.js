window.sb = {
  getInnerWidth: function() {
    return window.innerWidth || document.documentElement.clientWidth;
  },
  scrollToSection: function(id) {
    try {
      var el = document.getElementById(id);
      if (el) el.scrollIntoView({ behavior: 'smooth', block: 'start' });
    } catch (e) {
      console.error(e);
    }
  },
  showSectionModal: function(id) {
    try {
      var el = document.getElementById(id);
      var modalBody = document.querySelector('#sectionModal .modal-body');
      var modalTitle = document.querySelector('#sectionModal .modal-title');
      if (!el || !modalBody) return;
      // Copy only the inner content (avoid nested interactive ids colliding)
      modalBody.innerHTML = el.innerHTML;
      var h = el.querySelector('h4, h3');
      modalTitle.textContent = h ? h.innerText : id;
      var modalEl = document.getElementById('sectionModal');
      var bsModal = bootstrap.Modal.getInstance(modalEl) || new bootstrap.Modal(modalEl);
      bsModal.show();
    } catch (e) {
      console.error(e);
    }
  }
};