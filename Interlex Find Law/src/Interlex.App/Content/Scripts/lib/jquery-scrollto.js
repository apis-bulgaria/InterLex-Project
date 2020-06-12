(function ($) {
    $.fn.ScrollTo = function () {
        $("html, body").animate({
            scrollTop: $(this).first().offset().top - 128
        }, 1);

        return this;
    };
}(jQuery));

(function ($) {
    $.fn.ScrollToMinusOffset = function (offset) {
        $("html, body").animate({
            scrollTop: $(this).first().offset().top - offset
        }, 1);

        return this;
    };
}(jQuery));

(function ($) {
    $.fn.ScrollToNoOffset = function () {
        window.scrollTo(0, 0);

        return this;
    };
}(jQuery));