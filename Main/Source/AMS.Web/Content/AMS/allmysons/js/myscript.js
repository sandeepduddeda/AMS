
// JavaScript Document
new WOW().init();
jQuery(document).ready(function (event) {

    jQuery("#preloader").delay(100).fadeOut(500);

    $("html").click(function (event) {
        $("nav").removeClass('nav-open');
        $('.body-overlay').fadeOut();
    });

    // check if the platform is not android
    var ua = navigator.userAgent.toLowerCase();
    if (ua.indexOf("android") < 0) {
        jQuery(window).stellar({
            horizontalScrolling: false
        });
    }


    $(".menu-icon").click(function (event) {
        $("nav").toggleClass('nav-open');
        $('.body-overlay').fadeIn();
        event.stopPropagation();
    });
    $("nav").click(function (event) {
        event.stopPropagation();
    });

    jQuery('#owl-partners').owlCarousel({
        margin: 40,
        navigation: true,
        navigationText: ["<i class='fa fa-angle-left'></i>", "<i class='fa fa-angle-right'></i>"],
        autoplay: false,
        autoplayHoverPause: true,
        dots: false,
        responsive: {

            1200: { items: 8 },
            540: { items: 2 }
        }
    });

    jQuery('#owl-reviews-index').owlCarousel({
        margin: 40,
        navigation: true,
        navigationText: ["<i class='fa fa-angle-left'></i>", "<i class='fa fa-angle-right'></i>"],
        autoplay: false,
        autoplayHoverPause: true,
        dots: false,
        responsive: {

            1200: { items: 4 }
        }
    });

    jQuery('.location-areas-index-more').click(function (e) {
        jQuery('.location-areas-index .col-lg-2+.col-lg-2').slideToggle('slow', function () {
            if ($('.location-areas-index .col-lg-2+.col-lg-2').is(':visible')) {
                $('.location-areas-index-more').addClass('open');
            } else {
                $('.location-areas-index-more').removeClass('open');
            }
        });
    });


    /// Scroll Back To Top 
    // browser window scroll (in pixels) after which the "back to top" link is shown
    var offset = 150,
		//browser window scroll (in pixels) after which the "back to top" link opacity is reduced
		offset_opacity = 1200,
		//duration of the top scrolling animation (in ms)
		scroll_top_duration = 700,
		//grab the "back to top" link
		jQueryback_to_top = jQuery('.cd-top');

    //hide or show the "back to top" link
    jQuery(window).scroll(function () {
        (jQuery(this).scrollTop() > offset) ? jQueryback_to_top.addClass('cd-is-visible') : jQueryback_to_top.removeClass('cd-is-visible cd-fade-out');
        if (jQuery(this).scrollTop() > offset_opacity) {
            jQueryback_to_top.addClass('cd-fade-out');
        }
    });

    //smooth scroll to top
    jQueryback_to_top.on('click', function (event) {
        event.preventDefault();
        jQuery('body,html').animate({
            scrollTop: 0,
        }, scroll_top_duration
		);
    });

    $('#datepicker').datepicker({
        inline: true,
        showOtherMonths: true,
        dateFormat: 'dd MM yy',
        dayNamesMin: ['Su', 'Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa'],
        showOn: "button",
        buttonImage: "img/calendar-blue.png",
        buttonImageOnly: true,
        minDate: 0,
        showOtherMonths: true,
        selectOtherMonths: true,
        onSelect: function () {
            var date = $(this).datepicker('getDate');
            $('#MoveDate').val(date.getMonth() + 1 + '/' + date.getDate() + '/' + date.getFullYear());
            $('.pricing-details-step4').hide();
            $('.pricing-details-step5').show();
            $('.pricings-form-tab ul li:nth-child(5) a').addClass('active');
        }
    });

/*
    jQuery(".pricing-main").each(function () {
        $('.pricing-details-step1 input').click(function (e) {
            $('.pricing-details-step1').hide() && $('.pricing-details-step2').show() && $('.pricings-form-tab ul li:nth-child(2) a').addClass('active');
        });
        $('.pricing-details-step2 input').click(function (e) {
            $('.pricing-details-step2').hide() && $('.pricing-details-step3').show() && $('.pricings-form-tab ul li:nth-child(3) a').addClass('active');
        });
        $('.pricing-details-step3 input').click(function (e) {
            $('.pricing-details-step3').hide() && $('.pricing-details-step4').show() && $('.pricings-form-tab ul li:nth-child(4) a').addClass('active');
        });
		// Out of State function .pricing-details-step5-ld
		$('.pricing-details-step5-ld').hide();
        $('.select_house').click(function (e) { $('.pricing-details-step2-house').show(); });
        $('.select_apart').click(function (e) { $('.pricing-details-step2-apartment').show(); });
        $('.select_office').click(function (e) { $('.pricing-details-step2-office').show(); });
        $('.select_other').click(function (e) { $('.pricing-details-step2-other').show(); });
		// Out of State function .select_ld
        $('.select_ld').click(function (e) {
            $('.pricing-details-step5-ld input').attr("required", "required");
            $('.pricing-details-step5-ld').show();
        });
        $('form.steps-main').submit(function () {
            $("#btnSubmit").button("loading");
        });
        $('#phone').mask("(999) 999-9999");
    });*/

    $('form.steps-main').each(function () {

        var form = $(this),
            container = form.parent('.pricing-main'),
            containerExist = container.length > 0;

        step1 = form.find('.pricing-details-step1'),
        step2 = form.find('.pricing-details-step2'),
            step2house = form.find('.pricing-details-step2-house'),
            step2apartment = form.find('.pricing-details-step2-apartment'),
            step2office = form.find('.pricing-details-step2-office'),
            step2other = form.find('.pricing-details-step2-other'),
        step3 = form.find('.pricing-details-step3'),
        step4 = form.find('.pricing-details-step4'),
        step5 = form.find('.pricing-details-step5');
        step5ld = form.find('.pricing-details-step5-ld'),
        step6 = form.find('.pricing-details-step6'),
        submitBtn = form.find('#btnSubmit');

        step1.find('input').click(function (e) {
            step1.hide();
            step2.show();
            if (containerExist)
                container.find('.pricings-form-tab ul li:nth-child(2) a').addClass('active')
        });

        step2.find('input').click(function (e) {
            step2.hide();
            step3.show();
            if (containerExist)
                container.find('.pricings-form-tab ul li:nth-child(3) a').addClass('active');
        });

        step3.find('input[type="radio"]').click(function (e) {
/*
            if (step3.find('input[type="text"]').valid()) {
                step3.hide();
                step4.show();
                if (containerExist)
                    container.find('.pricings-form-tab ul li:nth-child(4) a').addClass('active');
            }
*/
            if ($('input[name="OriginZip"]')[0].checkValidity() && $('input[name="DestinationZip"]')[0].checkValidity())
            {
                step3.hide();
                step4.show();
                if (containerExist)
                    container.find('.pricings-form-tab ul li:nth-child(4) a').addClass('active');
            } else {
                submitBtn.click();
            }
        });


        step5.find('button').click(function () {
            if ($('input[name="FullName"]')[0].checkValidity() && $('input[name="Phone"]')[0].checkValidity() && $('input[name="Email"]')[0].checkValidity()) {
                step5.hide();
                step6.show();
                if (containerExist)
                    container.find('.pricings-form-tab ul li:nth-child(6) a').addClass('active');
            } else {
                submitBtn.click();
            }            
        });

        // Out of State function .pricing-details-step5-ld
        step5ld.hide();
        form.find('.select_house').click(function (e) { step2house.show(); });
        form.find('.select_apart').click(function (e) { step2apartment.show(); });
        form.find('.select_office').click(function (e) { step2office.show(); });
        form.find('.select_other').click(function (e) { step2other.show(); });
        // Out of State function .select_ld
        form.find('.select_ld').click(function (e) {
            step5ld.find('input').attr("required", "required");
            step5ld.show();
        });

        form.submit(function () {
            form.find("#btnSubmit").button("loading");
        });

        form.find('#phone').mask("(999) 999-9999");
    });

    if ($(window).width() > 767) {

        $('.our-family-toggle').click(function (e) {
            $('.our-family-cont').slideToggle() && $('.our-friends-cont,.our-community-cont,.cc-cont,.employee-own-cont').slideUp();
        });

        $('.our-friends-toggle').click(function (e) {
            $('.our-friends-cont').slideToggle() && $('.our-family-cont,.our-community-cont,.cc-cont,.employee-own-cont').slideUp();
        });

        $('.our-community-toggle').click(function (e) {
            $('.our-community-cont').slideToggle() && $('.our-family-cont,.our-friends-cont,.cc-cont,.employee-own-cont').slideUp();
        });

        $('.cc-toggle').click(function (e) {
            $('.cc-cont').slideToggle() && $('.our-family-cont,.our-friends-cont,.our-community-cont,.employee-own-cont').slideUp();
        });

        $('.employe-toggle').click(function (e) {
            $('.employee-own-cont').slideToggle() && $('.our-family-cont,.our-friends-cont,.our-community-cont,.cc-cont').slideUp();
        });
        function disableLinks() {
            var allLinks = document.getElementsByClassName("disableLinks");

            for (var i = 0; i < allLinks.length; i++) {
                if (allLinks[i].href !== '#') {
                    allLinks[i].href = "javascript:return false";
                }
            }
        }

        window.onload = disableLinks;

    }
	

});

