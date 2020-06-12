(function ($) {
    function checkArrowOffset(element, originalBottomOffset) {
        var footerVisibleHeight = getElementVisibleHeight($('footer'));
           
        if (($(document).scrollTop() + window.innerHeight) < $('footer').offset().top) {
            element.css('bottom', originalBottomOffset); // restore when scrolling up
        }
        else {
            element.css('bottom', (footerVisibleHeight + 10) + 'px'); // reaching footer
        }
    }

    $.fn.backToTop = function (options) {
        //Default options
        var settings = $.extend({
            element: '<span class="fa fa-chevron-circle-up"></span>',
            right: '10px',
            bottom: '15px',
            size: '1.8em',
            color: '#148dd4',
            opacity: '0.7',
            opacityHoverered: '1.0',
            showAfterPx: 150
        }, options);

        var body = $('body');
        var elementToAppend = $(settings.element);
        elementToAppend.addClass('fixed');
        elementToAppend.css('right', settings.right);
        elementToAppend.css('bottom', settings.bottom);
        elementToAppend.css('font-size', settings.size);
        elementToAppend.css('color', settings.color);
        elementToAppend.css('opacity', settings.opacity);
        elementToAppend.css('cursor', 'pointer');
        elementToAppend.css('display', 'none');

        elementToAppend.hover(function () {
            var that = $(this);
            that.css('opacity', settings.opacityHoverered);
        }, function () {
            var that = $(this);
            that.css('opacity', settings.opacity);
        });

        elementToAppend.click(function () {
            $("html, body").animate({ scrollTop: 0 }, "fast");
            return false;
        });

        $(body).append(elementToAppend);

        $(document).scroll(function () {
            var y = $(this).scrollTop();
            if (y > parseInt(settings.showAfterPx)) {
                elementToAppend.fadeIn(200);
            } else {
                elementToAppend.fadeOut(200);
            }

            checkArrowOffset(elementToAppend, settings.bottom);
        });

        return this;
    };

}(jQuery));