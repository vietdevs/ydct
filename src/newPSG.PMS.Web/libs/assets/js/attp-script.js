/*********************************************************************

    Note: style js.

*********************************************************************/

/*************************************************************************
                            INDEX
*************************************************************************/
/*
    01. wow js
    02. js scrollup
    03. Sticky Header
    04. jQuery MeanMenu
    05. testimonial-slide
    06. testimonial-three-slide
    07. slide-partner-log	
    08. mainservice-slide
    09. team-slide	
	10.isotope activation 	
	11. Counter up activation
	12. google map activation 
	13. popup-youtube
	14. bootstrap-touch-slider
*/
/*********************************************************************/
jQuery(document).ready(function ($) {
 "use strict";
//----------------------------------
/*-------------------------------------------
01. wow js
--------------------------------------------- */
 new WOW().init();
/*-------------------------------------------
02. js scrollup
--------------------------------------------- */
$.scrollUp({
	scrollText: '<i class="fa fa-angle-up"></i>',
	easingType: 'linear',
	scrollSpeed: 900,
	animation: 'fade'
}); 
/*-------------------------------------------
03. Sticky Header
--------------------------------------------- */
$(window).on('scroll', function(){
	if( $(window).scrollTop()>120 ){
		$('#sticky').addClass('stick');
	} else {
		$('#sticky').removeClass('stick');
	}
}); 
/*-------------------------------------------
 04. jQuery MeanMenu
--------------------------------------------- */
jQuery(".main-menu").meanmenu();	
/*-------------------------------------------
05. testimonial-slide
--------------------------------------------- */
$(".testimonial-slide").owlCarousel({
	autoPlay: true, 
	slideSpeed:1000,
	pagination:false,
	navigation:false,	  
	items : 2,
	navigationText:["<i class='fa fa-long-arrow-left'></i>","<i class='fa fa-long-arrow-right'></i>"],
	itemsDesktop : [1199,2],
	itemsDesktopSmall : [980,2],
	itemsTablet: [768,1],
	itemsMobile : [479,1],
}); 
/*-------------------------------------------
06. testimonial-three-slide
--------------------------------------------- */
$(".testimonial-three-slide").owlCarousel({
	autoPlay: true, 
	slideSpeed:1000,
	pagination:false,
	navigation:true,	  
	items : 1,
	navigationText:["<i class='fa fa-long-arrow-left'></i>","<i class='fa fa-long-arrow-right'></i>"],
	itemsDesktop : [1199,1],
	itemsDesktopSmall : [980,1],
	itemsTablet: [768,1],
	itemsMobile : [479,1],
}); 
/*-------------------------------------------
07. slide-partner-log
--------------------------------------------- */
$(".slide-partner-log").owlCarousel({
	autoPlay: true, 
	slideSpeed:1000,
	pagination:false,
	navigation:false,	  
	items : 6,
	navigationText:["<i class='fa fa-long-arrow-left'></i>","<i class='fa fa-long-arrow-right'></i>"],
	itemsDesktop : [1199,6],
	itemsDesktopSmall : [980,4],
	itemsTablet: [768,3],
	itemsMobile : [479,2],
}); 
/*-------------------------------------------
08. mainservice-slide
--------------------------------------------- */
$(".mainservice-slide").owlCarousel({
	autoPlay: true, 
	slideSpeed:1000,
	pagination:false,
	navigation:false,	  
	items : 3,
	navigationText:["<i class='fa fa-long-arrow-left'></i>","<i class='fa fa-long-arrow-right'></i>"],
	itemsDesktop : [1199,3],
	itemsDesktopSmall : [980,2],
	itemsTablet: [768,1],
	itemsMobile : [479,1],
});
/*-------------------------------------------
09. team-slide
--------------------------------------------- */
$(".team-slide").owlCarousel({
	autoPlay: true, 
	slideSpeed:1000,
	pagination:false,
	navigation:true,	  
	items : 3,
	navigationText:["<i class='fa fa-long-arrow-left'></i>","<i class='fa fa-long-arrow-right'></i>"],
	itemsDesktop : [1199,3],
	itemsDesktopSmall : [980,2],
	itemsTablet: [768,1],
	itemsMobile : [479,1],
});
/*-------------------------------------------
11. Counter up activation
--------------------------------------------- */
$('.counter').counterUp({
	delay: 10,
	time: 5000
});
/*-------------------------------------------
13. popup-youtube
--------------------------------------------- */
if ($('.popup-youtube').length > 0) {

	$('.popup-youtube').magnificPopup({
		type: 'iframe',
		mainClass: 'mfp-fade',
		removalDelay: 160,
		preloader: false,
		fixedContentPos: false
	});
}
}); 


