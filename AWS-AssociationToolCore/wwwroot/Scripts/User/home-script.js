
// HEADER MENU CODE STARTS HERE

/**
   * Mobile nav toggle
   */

const mobileNavShow = document.querySelector('.mobile-nav-show');
const mobileNavHide = document.querySelector('.mobile-nav-hide');

document.querySelectorAll('.mobile-nav-toggle').forEach(el => {
  el.addEventListener('click', function(event) {
    event.preventDefault();
    mobileNavToogle();
  })
});

function mobileNavToogle() {
  document.querySelector('body').classList.toggle('mobile-nav-active');
  mobileNavShow.classList.toggle('d-none');
  mobileNavHide.classList.toggle('d-none');
}

/**
 * Hide mobile nav on same-page/hash links
 */
document.querySelectorAll('#navbar a').forEach(navbarlink => {

  if (!navbarlink.hash) return;

  let section = document.querySelector(navbarlink.hash);
  if (!section) return;

  navbarlink.addEventListener('click', () => {
    if (document.querySelector('.mobile-nav-active')) {
      mobileNavToogle();
    }
  });

});

/**
 * Toggle mobile nav dropdowns
 */
const navDropdowns = document.querySelectorAll('.navbar .menu-item-has-children > a');

navDropdowns.forEach(el => {
  el.addEventListener('click', function(event) {
    if (document.querySelector('.mobile-nav-active')) {
      const indicator = this.querySelector('.dropdown-indicator');

      if (event.target !== indicator) {
        // Clicked on the anchor tag, navigate to its href
        return;
      }

      event.preventDefault();

      // Toggle the 'active' class on the indicator
      indicator.classList.toggle('bi-chevron-up');
      indicator.classList.toggle('bi-chevron-down');

      // Toggle the 'dropdown-active' class on the next 'ul' element (sub-menu)
      const subMenu = this.parentElement.querySelector('.sub-menu');
      if (subMenu) {
        subMenu.classList.toggle('dropdown-active');
      }
    }
  });
});
// HEADER MENU CODE ENDS HERE




// VIDEO GALLERY CODE
$(document).ready(function() {
    // Initialize Magnific Popup
    $('#home-video-gallery').magnificPopup({
      delegate: 'a',
      type: 'iframe',
      iframe: {
        patterns: {
          youtube: {
            index: 'youtu',
            id: function(url) {
              // Extract video ID from URL
              var match = url.match(/[^\w-]([-\w]{11})[^-\w]?/);
              if (match && match[1]) {
                return match[1];
              }
            },
            src: 'https://www.youtube.com/embed/%id%?autoplay=1'
          },
          vimeo: {
            index: 'vimeo.com/',
            id: '/',
            src: 'https://player.vimeo.com/video/%id%?autoplay=1'
          }
          // Add more patterns for other video platforms if needed
        }
      },
      gallery: {
        enabled: true
      }
    });
  });

 // Sponsors ticker
var visibleItems = 5;

if (window.innerWidth <= 1200) {
    visibleItems = 3;
}
if (window.innerWidth <= 768) {
    visibleItems = 2;
}

$('.sponsors').easyTicker({
    direction: 'up',
    easing: 'swing',
    speed: 'slow',
    interval: 2500,
    height: 'auto',
    visible: visibleItems,
    mousePause: 2,
    controls: {
        up: '',
        down: '',
        toggle: '',
        playText: 'Play',
        stopText: 'Stop'
    }
});
 

var visibleMadiaItems = 3;

if (window.innerWidth <= 1200) {
    visibleMadiaItems = 3;
}
if (window.innerWidth <= 768) {
    visibleMadiaItems = 2;
}
$('.media').easyTicker({
	direction: 'up',
	easing: 'swing',
	speed: 'slow',
	interval: 2500,
	height: 'auto',
	visible:visibleMadiaItems,
	mousePause: 2,
	controls: {
		up: '',
		down: '',
		toggle: '',
		playText: 'Play',
		stopText: 'Stop'
	}
}); 

$(document).ready(function () {
  $('.exhibits-slider').owlCarousel({
    items: 1,
    loop: true,
    margin: 10,
    nav: false,
    dots: true,
    autoplay: true,
    autoplayTimeout: 3000,
    smartSpeed: 900
  });
});

// NEWS SCROLL CODE
$(function(){  
	$('.news').easyTicker({
		visible: 1,
		
		interval: 3000
	});
});


///////////////

$(document).ready(function () {
  var owl = $('.programs-slider');

  owl.owlCarousel({
    loop: true,
    margin: 10,
    nav: false,
    dots: false,
    autoplay: true,
    autoplayTimeout: 2500,
    smartSpeed: 800,
    responsive: {
      0: { items: 2, margin: 5 },
      600: { items: 2 },
      992: { items: 3 }
    }
  });

  $('.prog-next').click(function () {
    owl.trigger('next.owl.carousel');
  });

  $('.prog-prev').click(function () {
    owl.trigger('prev.owl.carousel');
  });
});
$(document).ready(function () {
  var owl = $('.photo-gallery');

  owl.owlCarousel({
    loop: true,
    margin: 10,
    nav: false,
    dots: false,
    autoplay: true,
    autoplayTimeout: 2500,
    smartSpeed: 800,
    responsive: {
      0: { items: 2, margin: 5 },
      600: { items: 2 },
      992: { items: 4 }
    }
  });

  $('.prog-next').click(function () {
    owl.trigger('next.owl.carousel');
  });

  $('.prog-prev').click(function () {
    owl.trigger('prev.owl.carousel');
  });
});

// ////
// Sponser ticker

$('.aws-ticker1').easyTicker({
	direction: 'up',
	easing: 'swing',
	speed: 'slow',
	interval: 2000,
	height: 'auto',
	margin:5,
	visible:1,
	mousePause: 1,
	controls: {
		up: '',
		down: '',
		toggle: '',
		playText: 'Play',
		stopText: 'Stop'
	}
});
$('.aws-ticker2').easyTicker({
	direction: 'up',
	easing: 'swing',
	speed: 'slow',
	interval: 2000,
	height: 'auto',
	margin:5,
	visible:2,
	controls: {
		up: '',
		down: '',
		toggle: '',
		playText: 'Play',
		stopText: 'Stop'
	}
});
$('.aws-ticker3').easyTicker({
	direction: 'up',
	easing: 'swing',
	speed: 'slow',
	interval: 2000,
	height: 'auto',
	margin:5,
	visible:3,
	controls: {
		up: '',
		down: '',
		toggle: '',
		playText: 'Play',
		stopText: 'Stop'
	}
});
$('.aws-ticker4').easyTicker({
	direction: 'up',
	easing: 'swing',
	speed: 'slow',
	interval: 2000,
	height: 'auto',
	margin:5,
	visible:8,
	mousePause: 2,
	controls: {
		up: '',
		down: '',
		toggle: '',
		playText: 'Play',
		stopText: 'Stop'
	}
});


// 
$(document).ready(function() {
    $(window).scroll(function() {
      if ($(document).scrollTop() > 210) {
        $(".main_header").addClass("fixed_menu");
      } else {
        $(".main_header").removeClass("fixed_menu");
      }
    });
  });