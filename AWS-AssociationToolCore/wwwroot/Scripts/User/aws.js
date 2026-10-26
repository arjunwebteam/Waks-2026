$(document).ready(function () {
    var tickerLength = $('.sponser-ad-sec ul li').length;
    var tickerHeight = $('.sponser-ad-sec ul li').outerHeight();
    $('.sponser-ad-sec ul li:last-child').prependTo('.sponser-ad-sec ul');
    $('.sponser-ad-sec ul li:last-child').prependTo('.sponser-ad-sec ul'); // add this line to move two list items up
    $('.sponser-ad-sec ul').css('marginTop', -2 * tickerHeight); // change marginTop value to move two list items up
    function moveTop() {
        $('.sponser-ad-sec ul').animate({
            top: -2 * tickerHeight // change top value to move two list items up
        }, 600, function () {
            $('.sponser-ad-sec ul li:first-child').appendTo('.sponser-ad-sec ul');
            $('.sponser-ad-sec ul li:first-child').appendTo('.sponser-ad-sec ul'); // add this line to move two list items up
            $('.sponser-ad-sec ul').css('top', '');
        });
    }
    setInterval(function () {
        moveTop();
    }, 3000);
});
$(document).ready(function () {
    var tickerHeight1 = $('.media-sponser-ad-sec ul li').outerHeight();
    $('.media-sponser-ad-sec ul li:last-child').prependTo('.media-sponser-ad-sec ul');
    $('.media-sponser-ad-sec ul li:last-child').prependTo('.media-sponser-ad-sec ul'); // add this line to move two list items up
    $('.media-sponser-ad-sec ul').css('marginTop', -2 * tickerHeight1); // change marginTop value to move two list items up
    function moveTop() {
        $('.media-sponser-ad-sec ul').animate({
            top: -2 * tickerHeight1 // change top value to move two list items up
        }, 600, function () {
            $('.media-sponser-ad-sec ul li:first-child').appendTo('.media-sponser-ad-sec ul');
            $('.media-sponser-ad-sec ul li:first-child').appendTo('.media-sponser-ad-sec ul'); // add this line to move two list items up
            $('.media-sponser-ad-sec ul').css('top', '');
        });
    }
    setInterval(function () {
        moveTop();
    }, 3000);
});
$(document).ready(function() {
  var screenWidth = $(window).width();

  if (screenWidth < 767) { 
    $('.no-owl-slide').addClass('owl-carousel live1 owl-theme');
    $('.no-owl-slide').owlCarousel({
 autoplay: true,
    loop: true,
     responsive: {
        0: {
            items: 1,
					nav:false,
	dots:false,

        },
        576: {
            items: 2,
					nav:false,
	dots:false,
        },
        768: {
            items: 3,
					nav:false,
	dots:false,

        }
    }
    });
  }
});


 
/* HOTEL & PARKING PAGE CSS */

$(document).ready(function () {
    $("#map-data").hide();
    $("#parking-data").hide();
    $("#map1-data").hide();
    $("#parking1-data").hide();
    $("#map2-data").hide();
    $("#parking2-data").hide();
$("#map3-data").hide();
    $("#parking3-data").hide();
    $("#map").click(function () {
        $("#map-data").show();
        $("#parking-data").hide();
    });
    $("#parking").click(function () {
        $("#parking-data").show();
        $("#map-data").hide();
    });

    $("#map1").click(function () {
        $("#map1-data").show();
        $("#parking1-data").hide();
    });
    $("#parking1").click(function () {
        $("#parking1-data").show();
        $("#map1-data").hide();
    });
    $("#map2").click(function () {
        $("#map2-data").show();
        $("#parking2-data").hide();
    });
    $("#parking2").click(function () {
        $("#parking2-data").show();
        $("#map2-data").hide();
    });
 $("#map3").click(function () {
        $("#map3-data").show();
        $("#parking3-data").hide();
    });
    $("#parking3").click(function () {
        $("#parking3-data").show();
        $("#map3-data").hide();
    });
});



// client section 

$(document).ready(function () {

  var $slider = $('.memberslider1');

  $slider.owlCarousel({
    loop: false,
    autoplay: false,
    nav: false,
    dots: false,
    margin: 8,
    smartSpeed: 1000,
    autoplayTimeout: 1200,
    responsive: {
      0: { items: 2 },
      576: { items: 3 },
      992: { items: 4 },
      1024: { items: 5 }
    }
  });

  function controlAutoplay(event) {
    var carousel = event.relatedTarget;

    // ✅ total items
    var totalItems = carousel.items().length;

    // ✅ items for current breakpoint
    var breakpoint = carousel._breakpoint;
    var visibleItems = carousel.options.responsive[breakpoint].items;

    if (totalItems > visibleItems) {
      $slider.trigger('play.owl.autoplay');
      carousel.settings.loop = true;
    } else {
      $slider.trigger('stop.owl.autoplay');
      carousel.settings.loop = false;
    }
  }

  // 🔥 Bind ONLY once
  $slider.on(
    'initialized.owl.carousel resized.owl.carousel',
    controlAutoplay
  );

});

// Slimming Clinic Gallery section 

$('.live2').owlCarousel({
    nav:false,
	dots:true,
	margin:20,
	autoplay:true,
	loop:true,
	autoplayTimeout:5000,
	smartSpeed:1000,
    autoplayHoverPause:true,
    responsive:{
        0:{
            items:1,
			dots:false
        },
		400:{
            items:2,
			dots:false
        },
		720:{
            items:3
        },
		992:{
            items:3
        },
		1244:{
            items:4
        },
		
    }
});
 
// client section 

$('.live3').owlCarousel({
    nav:false,
	dots:true,
	margin:15,
	autoplay:true,
	loop:true,
	autoplayTimeout:5000,
	smartSpeed:1000,
    autoplayHoverPause:true,
    responsive:{
        0:{
            items:1,
			dots:false
        },
		400:{
            items:1,
			dots:false
        },
		540:{
            items:2
        },
		1024:{
            items:3
        },
		1144:{
            items:4
        },
		
    }
});

// home page tabs section code

$('#slick1').slick({	
		rows: 1,
		dots: false,
		arrows: false,
		infinite: false,
		autoplay: true,
		autoplaySpeed:1000,
		speed:700,
		margin:30, 
		slidesToShow:2,
		slidesToScroll: 2,
		 responsive: [
    { breakpoint: 360,
      settings: {
        slidesToShow: 1,
		slidesToScroll: 1,
      }
    },
	{ breakpoint: 580,
      settings: {
        slidesToShow: 1,
		slidesToScroll: 1
      }
    },
	{ breakpoint: 992,
      settings: {
        slidesToShow: 1,
		slidesToScroll: 1,
      }
    },
    { breakpoint: 776,
      settings: {
        slidesToShow: 1,
		slidesToScroll: 1,
        rows: 1 // This doesn't appear to work in responsive (Mac/Chrome)
      }
	  
    }]
});


// onload popup code 
window.onload = function() {
  document.getElementById('button').onclick = function() {
	document.getElementById('modalOverlay').style.display = 'none'
  };
};


// innerpages common popup code 

var within_first_modal = false;
$('.btn-second-modal').on('click', function() {
  if ($(this).hasClass('within-first-modal')) {
    within_first_modal = true;
    $('#first-modal').modal('hide');
  }
  $('#second-modal').modal('show');
});

$('.btn-second-modal-close').on('click', function() {
  $('#second-modal').modal('hide');
  // if (within_first_modal) {
    // $('#first-modal').modal('show');
    // within_first_modal = false;
  // }
});

// light gallery code
$(document).ready(function(){
            $('#lightgallery').lightGallery();
        });
		  $(document).ready(function() {
	$("#showme").click(function() {
	   $(".search-box").toggle("slide");
	});
});



  
  // Sponners  

$('.live4').owlCarousel({
    nav: false,
    dots: false,
    margin: 10,
    gap:10,
    autoplay: true,
    loop: true,
    autoplayTimeout: 1500,
    smartSpeed: 1000,
    autoplayHoverPause: true,
    responsive: {
        0: {
            items: 1,

        },
	360: {
            items: 2,
        },
        576: {
            items: 2,
        },
        768: {
            items: 3,

        },
        1000: {
            items: 5,

        },
1200: {
            items: 6,

        }

    }
});

$(".nav-dropdown > li > a").click(function () {
    $('.nav-dropdown').css('display', 'none');
});


// search-box open close js code
let navbar = document.querySelector(".navbar");
// sidebar open close js code
let navLinks = document.querySelector(".nav-links");
let menuOpenBtn = document.querySelector(".navbar .bx-menu");
let menuCloseBtn = document.querySelector(".nav-links span.bxx-x");
menuOpenBtn.onclick = function () {
    navLinks.style.left = "0";
}
menuCloseBtn.onclick = function () {
    navLinks.style.left = "-100%";
}


// sidebar submenu open close js code
let htmlcssArrow = document.querySelector(".htmlcss-arrow");
htmlcssArrow.onclick = function () {
    navLinks.classList.toggle("show1");
}
let moreArrow = document.querySelector(".more-arrow");
moreArrow.onclick = function () {
    navLinks.classList.toggle("show2");
}
let jsArrow = document.querySelector(".js-arrow");
jsArrow.onclick = function () {
    navLinks.classList.toggle("show3");
}



// /////////////////////////////////////






$(document).ready(function () {
  $("a.btn-gallery").on("click", function (event) {
    event.preventDefault();
    var gallery = $(this).attr("href");

    $(gallery)
      .magnificPopup({
        delegate: "a",
        type: "image",
        gallery: {
          enabled: true,
        },
      })
      .magnificPopup("open");
  });
});
$(function () {
  // Move the em tag with class "loading" after the fieldset element
  $("fieldset").after($(".error.help-block"));
});
$(document).ready(function () {
  // Initialize Magnific Popup
  $("#video-gallery").magnificPopup({
    delegate: "a",
    type: "iframe",
    iframe: {
      patterns: {
        youtube: {
          index: "youtu",
          id: function (url) {
            // Extract video ID from URL
            var match = url.match(/[^\w-]([-\w]{11})[^-\w]?/);
            if (match && match[1]) {
              return match[1];
            }
          },
          src: "https://www.youtube.com/embed/%id%?autoplay=1",
        },
        vimeo: {
          index: "vimeo.com/",
          id: "/",
          src: "https://player.vimeo.com/video/%id%?autoplay=1",
        },
        // Add more patterns for other video platforms if needed
      },
    },
    gallery: {
      enabled: true,
    },
  });
});

$(document).ready(function() {
  $(document).on("input", ".numberkey", function() {
    var inputVal = $(this).val();
    $(this).val(inputVal.replace(/\D/g, ""));
});
});

// -----------------------------------------------------------------------------

// additional validator common methods start
  // Format credit card number with spaces every 4 digits
  $("#CardNumber").on("input", function () {
    var value = $(this).val().replace(/\D/g, ''); // Remove non-numeric characters
    if (value.length > 0) {
        value = value.match(new RegExp('.{1,4}', 'g')).join(' ');
    }
    $(this).val(value);
});

// Restrict CVV input to numbers only
//$("#Cvv").on("input", function () {
//    var value = $(this).val().replace(/\D/g, ''); // Remove non-numeric characters
//    $(this).val(value);
//});

// Custom method for CVV format
$.validator.addMethod("cvvformat", function (value, element) {
    // Check if the value consists of exactly 3 digits
    return /^\d{3}$/.test(value);
}, "Please enter a valid 3-digit CVV.");




$.validator.addMethod("validate_email", function(value, element) {
    if (value === "") {
      return true;
  }
  if (/^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/.test(value)) {
      return true;
  } else {
      return false;
  }
}, "Please enter a valid Email.");
$.validator.addMethod("validImageExtension", function (value, element) {
  if (element.files && element.files.length > 0) {
      var allowedExtensions = ["jpg", "png", "gif", "jpeg"];
      var fileName = element.files[0].name;
      var extension = fileName.split('.').pop().toLowerCase();
      return $.inArray(extension, allowedExtensions) !== -1;
  }
  return true;
}, "Only allowed extensions are: jpg, png, gif, jpeg");
$.validator.addMethod("valueNotEquals", function (value, element, arg) {
   return arg !== value;
}, "Value must not equal arg.");
$.validator.addMethod("creditcard", function (value, element) {
   // Remove spaces and dashes from the card number
   value = value.replace(/ /g, "").replace(/-/g, "");

   // The Luhn algorithm for credit card number validation
   var sum = 0;
   var shouldDouble = false;
   for (var i = value.length - 1; i >= 0; i--) {
       var digit = parseInt(value.charAt(i));

       if (shouldDouble) {
           digit *= 2;
           if (digit > 9) {
               digit -= 9;
           }
       }

       sum += digit;
       shouldDouble = !shouldDouble;
   }

   return sum % 10 === 0;
}, "Please enter a valid credit card number");
$.validator.addMethod("phoneFormat", function(value, element) {
  if (value === "") {
    return true;
}
   var phoneNumber = value.replace(/\D/g, '');
   var phoneNumberLength = phoneNumber.length;
   if (phoneNumberLength >= 3 && phoneNumberLength <= 6) {
       phoneNumber = phoneNumber.replace(/^(\d{3})(\d+)/, '$1-$2');
   } else if (phoneNumberLength > 6) {
       phoneNumber = phoneNumber.replace(/^(\d{3})(\d{3})(\d+)/, '$1-$2-$3');
   }
   $(element).val(phoneNumber);
   var pattern = /^\d{3}-\d{3}-\d{4}$/;
   return pattern.test(phoneNumber);
}, "Please enter format xxx-xxx-xxxx.");
$.validator.addMethod("alpha", function(value, element) {
    if (value === "") {
      return true;
  }
   var formattedText = value.replace(/[^a-zA-Z ]+/g, '').replace(/\s{2,}/g, ' ');
   $(element).val(formattedText);
   var pattern = /^[a-zA-Z]+( [a-zA-Z]+)*$/;
   return pattern.test(formattedText);
}, "Please enter only letters with a single space between words.");
$.validator.addMethod("validpassword", function (t, e) {
  return this.optional(e) || /^(?=.*\d+)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*(),.?":{}|<>])[0-9a-zA-Z!@#$%^&*(),.?":{}|<>]{8,}$/i.test(t)
}, "Enter Strong Password.");

// this is for tabs input require hidden
$.validator.setDefaults({
  ignore: function(index, element) {
      // Check if the element's parent (or any ancestor) has display: none
      return $(element).closest('div:hidden').length > 0;
  }
});

// additional validator common methods  end
// -----------------------------------------------------------------------------
