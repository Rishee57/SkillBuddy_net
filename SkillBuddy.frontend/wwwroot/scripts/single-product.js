 
/**
 * Enhanced Responsive Service Carousel
 * ====================================
 * - Mobile (≤576px): horizontal swipe; arrows hidden
 * - Tablet (577–768px): 2 cards + swipe + arrows
 * - Desktop (≥769px): 3 cards + arrows
 * - Drag guard: Prevents accidental selection while scrolling
 */

document.addEventListener('DOMContentLoaded', function() {
  const leftBtn = document.querySelector('.carousel-btn-left');
  const rightBtn = document.querySelector('.carousel-btn-right');
  const serviceCards = document.querySelectorAll('.service-carousel-track .service-card');
  const carouselContainer = document.querySelector('.service-carousel-container');
  const carouselTrack = document.querySelector('.service-carousel-track');
  
  if (!carouselContainer || !carouselTrack || !serviceCards.length) {
    console.warn('Carousel elements not found');
    return;
  }

  // State management
  let currentIndex = 0;
  let selectedCard = 2; // Default to "RO Not Working"
  let cardsPerView = 3;
  let viewMode = 'desktop'; // 'mobile', 'tablet', 'desktop'
  
  // Touch/drag handling variables
  let startX = 0;
  let startY = 0;
  let scrollLeft = 0;
  let isDragging = false;
  let dragStartTime = 0;
  let dragDistance = 0;
  let hasDragged = false;

  // Debounced resize handler
  let resizeTimeout;
  
  /**
   * Determines the current view mode based on window width
   */
  function getViewMode() {
    const width = window.innerWidth;
    if (width <= 576) return 'mobile';
    if (width <= 768) return 'tablet';
    return 'desktop';
  }

  /**
   * Updates carousel configuration based on screen size
   */
  function updateCarouselConfig() {
    const newViewMode = getViewMode();
    const width = window.innerWidth;
    
    viewMode = newViewMode;
    
    switch (viewMode) {
      case 'mobile':
        cardsPerView = 1;
        setupTouchHandlers();
        hideArrows();
        break;
        
      case 'tablet':
        cardsPerView = 2;
        setupTouchHandlers();
        showArrows();
        break;
        
      case 'desktop':
        cardsPerView = 3;
        removeTouchHandlers();
        showArrows();
        break;
    }
    
    // Reset current index if it exceeds new limits
    const maxIndex = Math.max(0, serviceCards.length - cardsPerView);
    if (currentIndex > maxIndex) {
      currentIndex = maxIndex;
    }
    
    updateCarouselDisplay();
  }

  /**
   * Shows navigation arrows
   */
  function showArrows() {
    if (leftBtn && rightBtn) {
      leftBtn.style.display = 'flex';
      rightBtn.style.display = 'flex';
      updateArrowStates();
    }
  }

  /**
   * Hides navigation arrows
   */
  function hideArrows() {
    if (leftBtn && rightBtn) {
      leftBtn.style.display = 'none';
      rightBtn.style.display = 'none';
    }
  }

  /**
   * Updates arrow button states (enabled/disabled)
   */
  function updateArrowStates() {
    if (!leftBtn || !rightBtn) return;
    
    const maxIndex = Math.max(0, serviceCards.length - cardsPerView);
    
    leftBtn.disabled = currentIndex <= 0;
    rightBtn.disabled = currentIndex >= maxIndex;
    
    leftBtn.style.opacity = leftBtn.disabled ? '0.4' : '1';
    rightBtn.style.opacity = rightBtn.disabled ? '0.4' : '1';
  }

  /**
   * Sets up touch/drag handlers for mobile and tablet
   */
  function setupTouchHandlers() {
    // Remove existing handlers first
    removeTouchHandlers();
    
    // Mouse events (for desktop testing and tablet hybrid devices)
    carouselContainer.addEventListener('mousedown', handleDragStart, { passive: false });
    document.addEventListener('mousemove', handleDragMove, { passive: false });
    document.addEventListener('mouseup', handleDragEnd, { passive: false });
    
    // Touch events (for mobile devices)
    carouselContainer.addEventListener('touchstart', handleDragStart, { passive: false });
    document.addEventListener('touchmove', handleDragMove, { passive: false });
    document.addEventListener('touchend', handleDragEnd, { passive: false });
    
    // Prevent default drag behavior
    carouselContainer.addEventListener('dragstart', e => e.preventDefault());
  }

  /**
   * Removes touch/drag handlers
   */
  function removeTouchHandlers() {
    carouselContainer.removeEventListener('mousedown', handleDragStart);
    document.removeEventListener('mousemove', handleDragMove);
    document.removeEventListener('mouseup', handleDragEnd);
    carouselContainer.removeEventListener('touchstart', handleDragStart);
    document.removeEventListener('touchmove', handleDragMove);
    document.removeEventListener('touchend', handleDragEnd);
    carouselContainer.removeEventListener('dragstart', e => e.preventDefault());
  }

  /**
   * Handles the start of drag/touch interaction
   */
  function handleDragStart(e) {
    if (viewMode === 'desktop') return;
    
    isDragging = true;
    hasDragged = false;
    dragStartTime = Date.now();
    
    const clientX = e.type === 'touchstart' ? e.touches[0].clientX : e.clientX;
    const clientY = e.type === 'touchstart' ? e.touches[0].clientY : e.clientY;
    
    startX = clientX;
    startY = clientY;
    scrollLeft = carouselContainer.scrollLeft;
    
    carouselContainer.classList.add('dragging');
    
    // Prevent text selection during drag
    document.body.style.userSelect = 'none';
  }

  /**
   * Handles drag/touch movement
   */
  function handleDragMove(e) {
    if (!isDragging || viewMode === 'desktop') return;
    
    const clientX = e.type === 'touchmove' ? e.touches[0].clientX : e.clientX;
    const clientY = e.type === 'touchmove' ? e.touches[0].clientY : e.clientY;
    
    const deltaX = clientX - startX;
    const deltaY = clientY - startY;
    
    // Determine if this is a horizontal or vertical gesture
    if (Math.abs(deltaY) > Math.abs(deltaX)) {
      // Vertical scroll - don't interfere
      return;
    }
    
    // Horizontal drag detected
    if (Math.abs(deltaX) > 3) {
      hasDragged = true;
      e.preventDefault(); // Prevent scrolling
      
      dragDistance = deltaX;
      
      // Update scroll position with momentum
      const momentum = viewMode === 'mobile' ? 1 : 0.7;
      carouselContainer.scrollLeft = scrollLeft - (deltaX * momentum);
    }
  }

  /**
   * Handles the end of drag/touch interaction
   */
  function handleDragEnd(e) {
    if (!isDragging || viewMode === 'desktop') return;
    
    isDragging = false;
    carouselContainer.classList.remove('dragging');
    
    // Restore text selection
    document.body.style.userSelect = '';
    
    const dragDuration = Date.now() - dragStartTime;
    const isQuickSwipe = dragDuration < 200 && Math.abs(dragDistance) > 30;
    
    // Handle swipe gestures for tablet/mobile
    if (isQuickSwipe || Math.abs(dragDistance) > 100) {
      if (dragDistance > 0 && currentIndex > 0) {
        // Swipe right - go to previous
        navigateToIndex(currentIndex - 1);
      } else if (dragDistance < 0 && currentIndex < serviceCards.length - cardsPerView) {
        // Swipe left - go to next
        navigateToIndex(currentIndex + 1);
      }
    }
    
    // Reset drag state
    dragDistance = 0;
    hasDragged = false;
  }

  /**
   * Navigates to a specific carousel index
   */
  function navigateToIndex(newIndex) {
    const maxIndex = Math.max(0, serviceCards.length - cardsPerView);
    currentIndex = Math.max(0, Math.min(newIndex, maxIndex));
    updateCarouselDisplay();
  }

  /**
   * Resets all card styles to default
   */
  function resetCardStyles(card) {
    const content = card.querySelector('.service-card-content');
    const title = card.querySelector('.service-title');
    const rating = card.querySelector('.service-rating');
    const price = card.querySelector('.service-price');
    const btn = card.querySelector('.service-add-btn');
    
    if (!content) return;
    
    content.style.background = '#fff';
    content.style.border = '2px solid #e5e7eb';
    content.style.color = 'initial';
    
    if (title) {
      title.style.color = 'initial';
      title.classList.remove('text-white');
    }
    
    if (rating) {
      rating.style.color = '#6c757d';
    }
    
    if (price) {
      price.style.color = 'initial';
      price.classList.remove('text-white');
    }
    
    if (btn) {
      btn.style.background = '#f8f9ff';
      btn.style.color = '#6c63ff';
      btn.style.border = '1px solid #6c63ff';
    }
  }

  /**
   * Highlights the selected card
   */
  function highlightCard(card) {
    const content = card.querySelector('.service-card-content');
    const title = card.querySelector('.service-title');
    const rating = card.querySelector('.service-rating');
    const price = card.querySelector('.service-price');
    const btn = card.querySelector('.service-add-btn');
    
    if (!content) return;
    
    content.style.background = '#3b82f6';
    content.style.border = '2px solid #3b82f6';
    content.style.color = 'white';
    
    if (title) {
      title.style.color = 'white';
      title.classList.add('text-white');
    }
    
    if (rating) {
      rating.style.color = '#e5e7eb';
    }
    
    if (price) {
      price.style.color = 'white';
      price.classList.add('text-white');
    }
    
    if (btn) {
      btn.style.background = 'white';
      btn.style.color = '#3b82f6';
      btn.style.border = '1px solid white';
    }
  }

  /**
   * Updates the carousel display based on current mode and index
   */
  function updateCarouselDisplay() {
    // Reset all cards
    serviceCards.forEach((card, index) => {
      resetCardStyles(card);
      
      if (viewMode === 'mobile') {
        // Mobile: show all cards in flex layout
        card.style.display = 'block';
      } else {
        // Tablet/Desktop: show cards based on current index
        const isVisible = index >= currentIndex && index < currentIndex + cardsPerView;
        card.style.display = isVisible ? 'block' : 'none';
      }
    });
    
    // Highlight selected card
    if (serviceCards[selectedCard]) {
      highlightCard(serviceCards[selectedCard]);
    }
    
    // Update arrow states
    updateArrowStates();
  }

  /**
   * Handles card selection with drag guard
   */
  function handleCardClick(card, index, e) {
    // Prevent selection if user was dragging
    if (hasDragged) {
      e.preventDefault();
      return;
    }
    
    selectedCard = index;
    updateCarouselDisplay();
  }

  // Setup event listeners
  serviceCards.forEach((card, index) => {
    card.addEventListener('click', (e) => handleCardClick(card, index, e));
  });

  // Arrow navigation
  if (leftBtn) {
    leftBtn.addEventListener('click', () => {
      if (currentIndex > 0) {
        navigateToIndex(currentIndex - 1);
      }
    });
  }

  if (rightBtn) {
    rightBtn.addEventListener('click', () => {
      const maxIndex = Math.max(0, serviceCards.length - cardsPerView);
      if (currentIndex < maxIndex) {
        navigateToIndex(currentIndex + 1);
      }
    });
  }

  // Handle window resize with debouncing
  window.addEventListener('resize', () => {
    clearTimeout(resizeTimeout);
    resizeTimeout = setTimeout(() => {
      updateCarouselConfig();
    }, 150);
  });

  // Keyboard navigation support
  carouselContainer.addEventListener('keydown', (e) => {
    if (viewMode === 'mobile') return;
    
    switch (e.key) {
      case 'ArrowLeft':
        e.preventDefault();
        if (currentIndex > 0) navigateToIndex(currentIndex - 1);
        break;
      case 'ArrowRight':
        e.preventDefault();
        const maxIndex = Math.max(0, serviceCards.length - cardsPerView);
        if (currentIndex < maxIndex) navigateToIndex(currentIndex + 1);
        break;
    }
  });

  // Make carousel container focusable for keyboard navigation
  if (viewMode !== 'mobile') {
    carouselContainer.setAttribute('tabindex', '0');
  }

  // Initialize the carousel
  updateCarouselConfig();
  
  console.log('Enhanced Responsive Carousel initialized:', {
    viewMode,
    cardsPerView,
    totalCards: serviceCards.length
  });
});

(function () {
  document.querySelectorAll('.service-carousel-container').forEach(container => {
    const track = container.querySelector('.service-carousel-track');
    const leftBtn = container.querySelector('.carousel-btn-left');
    const rightBtn = container.querySelector('.carousel-btn-right');

    if (!track || !leftBtn || !rightBtn) return;

    // We’ll use the container itself as the scroll area (simplest, smooth, no transform math)
    const scrollEl = container;

    // Ensure horizontal scroll is enabled (doesn't change your visual styles)
    scrollEl.style.overflowX = scrollEl.style.overflowX || 'auto';

    // Step = one card width + gap
    const firstCard = track.querySelector('.service-card');
    const gap = parseFloat(getComputedStyle(track).columnGap || getComputedStyle(track).gap || 12) || 12;

    function cardWidth() {
      if (!firstCard) return 320;
      const rect = firstCard.getBoundingClientRect();
      // Include the gap so each click lands nicely at the next card
      return Math.ceil(rect.width + gap);
    }

    function updateArrows() {
      const max = Math.max(0, scrollEl.scrollWidth - scrollEl.clientWidth - 1);
      if (leftBtn) leftBtn.disabled = scrollEl.scrollLeft <= 0;
      if (rightBtn) rightBtn.disabled = scrollEl.scrollLeft >= max;
    }

    function scrollByStep(dir) {
      scrollEl.scrollBy({ left: dir * cardWidth(), behavior: 'smooth' });
      // Re-check after the smooth scroll
      setTimeout(updateArrows, 400);
    }

    leftBtn.addEventListener('click', () => scrollByStep(-1));
    rightBtn.addEventListener('click', () => scrollByStep(1));

    // Keep state correct on resize/scroll
    scrollEl.addEventListener('scroll', updateArrows, { passive: true });
    window.addEventListener('resize', () => setTimeout(updateArrows, 100));

    // Init
    updateArrows();
  });
})();

