Array.prototype.unique = function () {
    var o = {}, i, l = this.length, r = [];
    for (i = 0; i < l; i += 1) o[this[i]] = this[i];
    for (i in o) r.push(o[i]);
    return r;
};

Array.prototype.remove = function () {
    var what, a = arguments, L = a.length, ax;
    while (L && this.length) {
        what = a[--L];
        while ((ax = this.indexOf(what)) !== -1) {
            this.splice(ax, 1);
        }
    }
    return this;
};

String.prototype.contains = function (it) { return this.indexOf(it) != -1; };

/*--- Start of invoked helper functions ---*/
function getDateFromMicrotime(microtime) {
    //ticks are recorded from 1/1/1; get microtime difference from 1/1/1/ to 1/1/1970
    var epochMicrotimeDiff = 2208988800000;

    //new date is ticks, converted to microtime, minus difference from epoch microtime
    var tickDate = new Date(microtime - epochMicrotimeDiff);
    console.log(tickDate);

    var day = tickDate.getDate();
    var month = tickDate.getMonth() + 1;
    var year = tickDate.getFullYear() + 70;

    var parsedDate = day + '/' + month + '/' + year;

    //tickDate = tickDate.format('dd/mm/yyyy');

    return parsedDate;
}

function generateGuid() {
    var guid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });

    return guid;
}

function printDiv(divName) {
    window.print();
}

function setCookie(name, value, days) {
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        var expires = "; expires=" + date.toGMTString();
    }
    else var expires = "";
    document.cookie = name + "=" + value + expires + "; path=/";
}

function getCookie(name) {
    var value = "; " + document.cookie;
    var parts = value.split("; " + name + "=");
    if (parts.length == 2) return parts.pop().split(";").shift();
}

function eraseCookie(name) {
    document.cookie = name + '=; expires=Thu, 01 Jan 1970 00:00:01 GMT;';
}

function deleteCookie(name) {
    setCookie(name, "", -1);
}

notificationsArray = [];

function notify(title, message, type) {
    var randomId = Math.floor((Math.random() * 10000) + 1); /* JESUS CAME TO EARTH */

    notificationsArray[randomId] = $.notify({
        title: '<strong><i class="fa fa-exclamation-circle"></i> ' + title + '</strong>',
        message: message
    }, {
        type: type,
        allow_dismiss: true,
        offset: {
            y: 55
        },
        placement: {
            align: 'center'
        },
        template: '<div data-notify="container" data-randomid="'+ randomId + '" class="notify-container alert alert-{0}" role="alert">' +
		'<span data-notify="title">{1}</span>' +
		'<span data-notify="message">{2}</span>' +
        '<span data-randomid="' + randomId + '" class="fa fa-times-circle close notification-close f-white" aria-hidden="true"></span>' +
	'</div>'
    });

    $('.notification-close[data-randomid="' + randomId + '"]').on('click', function () {
        notificationsArray[randomId].close();
        $('.notify-container[data-randomid="' + randomId + '"]').hide();
        return false;
    });
}

function insertAtCaret(areaId, text) {
    var txtarea = document.getElementById(areaId);
    var scrollPos = txtarea.scrollTop;
    var strPos = 0;
    var br = ((txtarea.selectionStart || txtarea.selectionStart == '0') ?
        "ff" : (document.selection ? "ie" : false));
    if (br == "ie") {
        txtarea.focus();
        var range = document.selection.createRange();
        range.moveStart('character', -txtarea.value.length);
        strPos = range.text.length;
    }
    else if (br == "ff") strPos = txtarea.selectionStart;

    var front = (txtarea.value).substring(0, strPos);
    var back = (txtarea.value).substring(strPos, txtarea.value.length);
    txtarea.value = front + text + back;
    strPos = strPos + text.length;
    if (br == "ie") {
        txtarea.focus();
        var range = document.selection.createRange();
        range.moveStart('character', -txtarea.value.length);
        range.moveStart('character', strPos);
        range.moveEnd('character', 0);
        range.select();
    }
    else if (br == "ff") {
        txtarea.selectionStart = strPos;
        txtarea.selectionEnd = strPos;
        txtarea.focus();
    }
    txtarea.scrollTop = scrollPos;
}

/*--- Start of helper classes ---*/
UTF8 = {
    encode: function (s) {
        for (var c, i = -1, l = (s = s.split("")).length, o = String.fromCharCode; ++i < l;
            s[i] = (c = s[i].charCodeAt(0)) >= 127 ? o(0xc0 | (c >>> 6)) + o(0x80 | (c & 0x3f)) : s[i]
        );
        return s.join("");
    },
    decode: function (s) {
        for (var a, b, i = -1, l = (s = s.split("")).length, o = String.fromCharCode, c = "charCodeAt"; ++i < l;
            ((a = s[i][c](0)) & 0x80) &&
            (s[i] = (a & 0xfc) == 0xc0 && ((b = s[i + 1][c](0)) & 0xc0) == 0x80 ?
            o(((a & 0x03) << 6) + (b & 0x3f)) : o(128), s[++i] = "")
        );
        return s.join("");
    }
};

GlobalValidator = {
    inputHasValue: function inputHasValue(selector) {
        var val = $(selector).val();

        if (val === undefined || val === null || val === '') {
            return false;
        }
        else {
            return true;
        }
    }
}

/* Start of globalization utility */
Globalization = function () {
    this.monthsShort = [];
    this.monthsFull = [];
    this.DEFAULT_LANG_CODE = 'en-GB';

    var months_BG = ['Януари', 'Февруари', 'Март', 'Април', 'Май', 'Юни', 'Юли', 'Август', 'Септември', 'Октомври', 'Ноември', 'Декември'],
        months_EN = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'],
        months_FR = ['janvier', 'février', 'mars', 'avril', 'mai', 'juin', 'juillet', 'août', 'septembre', 'octobre', 'novembre', 'décembre'],
        months_DE = ['Januar', 'Februar', 'März', 'April', 'Mai', 'Juni', 'Juli', 'August', 'September', 'Oktober', 'November', 'Dezember'],
        months_IT = ['gennaio', 'febbraio', 'marzo', 'aprile', 'maggio', 'giugno', 'luglio', 'agosto', 'settembre', 'ottobre', 'novembre', 'dicembre'],
        months_NL = ['januari', 'februari', 'maart', 'april', 'mei', 'juni', 'juli', 'augustus', 'september', 'oktober', 'november', 'december'],
        months_ES = ['enero', 'febrero', 'marzo', 'abril', 'mayo', 'junio', 'julio', 'agosto', 'septiembre', 'octubre', 'noviembre', 'diciembre'],
        months_PT = ['janeiro', 'fevereiro', 'março', 'abril', 'maio', 'junho', 'julho', 'agosto', 'setembro', 'outubro', 'novembro', 'dezembro'],
        months_PL = ['styczeń', 'luty', 'marzec', 'kwiecień', 'maj', 'czerwiec', 'lipiec', 'sierpień', 'wrzesień', 'październik', 'listopad', 'grudzień'];

    var months_BG_Short = ['Ян', 'Фев', 'Мар', 'Апр', 'Май', 'Юни', 'Юли', 'Авг', 'Сеп', 'Окт', 'Ное', 'Дек'], // Bulgarian
 months_EN_Short = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'], // English
 months_FR_Short = ['Janv', 'Févr', 'Mars', 'Avril', 'Mai', 'Juin', 'Juil', 'Août', 'Sept', 'Oct', 'Nov', 'Déc'], // French
 months_DE_Short = ['Jän', 'Feb', 'März', 'Apr', 'Mai', 'Juni', 'Juli', 'Aug', 'Sept', 'Okt', 'Nov', 'Dez'], // German
 months_IT_Short = ['Genn', 'Febbr', 'Mar', 'Apr', 'Magg', 'Giugno', 'Luglio', 'Ag', 'Sett', 'Ott', 'Nov', 'Dic'], // Italian
 months_NL_Short = ['Jan', 'Feb', 'Maart', 'Apr', 'Mei', 'Juni', 'Juli', 'Aug', 'Sept', 'Oct', 'Nov', 'Dec'], // Dutch
 months_ES_Short = ['Enero', 'Feb', 'Marzo', 'Abr', 'Mayo', 'Jun', 'Jul', 'Agosto', 'Sept', 'Oct', 'Nov', 'Dic'], // Spanish
 months_PT_Short = ['Jan', 'Fev', 'Março', 'Abril', 'Maio', 'Junho', 'Julho', 'Agosto', 'Set', 'Out', 'Nov', 'Dez'], // Portuguese
 months_PL_Short = ['Stycz', 'Luty', 'Mar', 'Kwiec', 'Maj', 'Czerw', 'Lip', 'Sierp', 'Wrzes', 'Pazdz', 'Listop', 'Grudz']; // Polish

    /* Country codes are based on the siteLang cookie */
    this.monthsFull['bg-BG'] = months_BG;
    this.monthsFull['en-GB'] = months_EN;
    this.monthsFull['fr-FR'] = months_FR;
    this.monthsFull['de-DE'] = months_DE;
    this.monthsFull['it-IT'] = months_IT;
    this.monthsFull['nl-NL'] = months_NL;
    this.monthsFull['es-ES'] = months_ES;
    this.monthsFull['pt-PT'] = months_PT;
    this.monthsFull['pl-PL'] = months_PL;

    this.monthsShort['bg-BG'] = months_BG_Short;
    this.monthsShort['en-GB'] = months_EN_Short;
    this.monthsShort['fr-FR'] = months_FR_Short;
    this.monthsShort['de-DE'] = months_DE_Short;
    this.monthsShort['it-IT'] = months_IT_Short;
    this.monthsShort['nl-NL'] = months_NL_Short;
    this.monthsShort['es-ES'] = months_ES_Short;
    this.monthsShort['pt-PT'] = months_PT_Short;
    this.monthsShort['pl-PL'] = months_PL_Short;

};

/* Start of revealing prototype */
Globalization.prototype = (function () {
    // Returns an array with short abbreviations for months; Returns english months if specified language is not present.
    function getMonthsShort(fullCode) {
        if (typeof this.monthsShort[fullCode] !== 'undefined') {
            return this.monthsShort[fullCode];
        }
        else {
            return this.monthsShort[this.DEFAULT_LANG_CODE];
        }
    }

    function getMonths(fullCode) {
        if (typeof this.monthsFull[fullCode] !== 'undefined') {
            return this.monthsFull[fullCode];
        }
        else {
            return this.monthsFull[this.DEFAULT_LANG_CODE];
            // dick
        }
    }

    function getMonthNameByLanguage(language, monthNumber) {
        if (typeof this.monthsFull[language] !== 'undefined') {
            return this.monthsFull[language][monthNumber];
        }
        else {
            return this.monthsFull[this.DEFAULT_LANG_CODE][monthNumber];
        }
    }

    return {
        getMonthsShort: getMonthsShort,
        getMonths: getMonths,
        getMonthNameByLanguage: getMonthNameByLanguage
    }
})();
/* End of revealing prototype */
GlobalizationHandler = new Globalization(); // use this in apps
/* End of globalization utility */

$.fn.enterKey = function (fnc) {
    return this.each(function () {
        $(this).keypress(function (ev) {
            var keycode = (ev.keyCode ? ev.keyCode : ev.which);
            if (keycode == '13') {
                fnc.call(this, ev);
            }
        })
    })
}

Date.prototype.getMonthName = function (fullCode) {
    //  lang = lang && (lang in Date.locale) ? lang : 'en';
    //   return Date.locale[lang].month_names[this.getMonth()];
    return GlobalizationHandler.getMonthNameByLanguage(fullCode, this.getMonth());
};

Date.prototype.getMonthNameShort = function (lang) {
    lang = lang && (lang in Date.locale) ? lang : 'en';
    return Date.locale[lang].month_names_short[this.getMonth()];
};

Date.locale = {
    en: {
        month_names: ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'],
        month_names_short: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec']
    },
    bg: {
        month_names: ['Януари', 'Февруари', 'Март', 'Април', 'Май', 'Юни', 'Юли', 'Август', 'Септември', 'Октомври', 'Ноември', 'Декември'],
        month_names_short: ['Ян', 'Фев', 'Мар', 'Апр', 'Май', 'Юни', 'Юли', 'Авг', 'Сеп', 'Окт', 'Ное', 'Дек']
    }
};

/* jQuery UI Enabling Title HTML Content */
$.widget("ui.dialog", $.extend({}, $.ui.dialog.prototype, {
    _title: function (title) {
        if (!this.options.title) {
            title.html("&#160;");
        } else {
            title.html(this.options.title);
        }
    }
}));

/* Itterates over associative array elements and joins them by given concatenation string */
function associativeJoin(assObject, concatString) {
    if (typeof assObject === 'undefined' || assObject === null || assObject === {}) {
        return '';
    }

    var joinedString = '',
        i = 0,
        assObjectlength = Object.keys(assObject).length;

    for (itemIndex in assObject) {
        joinedString = joinedString + assObject[itemIndex];

        if (i !== assObjectlength - 1) {
            joinedString = joinedString + concatString;
        }

        i++;
    }

    return joinedString;
}

setDelay = (function () {
    var timer = 0;
    return function (callback, ms) {
        clearTimeout(timer);
        timer = setTimeout(callback, ms);
    };
})();

function fluidDialog() {
    var $visible = $(".ui-dialog:visible");
    // each open dialog
    $visible.each(function () {
        var $this = $(this);
        var dialog = $this.find(".ui-dialog-content").data("ui-dialog");
        // if fluid option == true
        if (dialog.options.fluid) {
            var wWidth = $(window).width(),
                defaultWidth = dialog.options.defaultWidth,
                $contentContainer = $('#' + $this.attr('aria-describedby')),
                maxHeight = parseInt($contentContainer.attr('data-max-height'), 10),
                $buttonPane = $this.find('.ui-dialog-buttonpane');

            // preventing modal size change if there is enough space
            if (defaultWidth <= wWidth) {
                $this.css('width', defaultWidth + 'px');
            }
            else {
                $this.css('width', 'auto');
            }

            // check window width against dialog width
            if (wWidth < (parseInt(dialog.options.maxWidth) + 50)) {
                // keep dialog from filling entire screen
                $this.css("max-width", "90%");
            } else {
                // fix maxWidth bug
                $this.css("max-width", dialog.options.maxWidth + "px");
            }
            //reposition dialog
            dialog.option("position", dialog.options.position);


            if (maxHeight && ($this.height() > $(window).height()
                        || $this.height() + 2 < maxHeight)) { // $(this).height() + 2 - modals are initialiazed with 2px less than the height they are given wtf
                var paddingsHeight = 0;

                if ($contentContainer.css('padding-top')) {
                    paddingsHeight += parseInt($contentContainer.css('padding-top').replace('px', ''), 10);
                }

                if ($contentContainer.css('padding-bottom')) {
                    paddingsHeight += parseInt($contentContainer.css('padding-bottom').replace('px', ''), 10);
                }

                if ($buttonPane.css('padding-bottom')) {
                    paddingsHeight += parseInt($buttonPane.css('padding-bottom').replace('px', ''), 10);
                } 

                if ($buttonPane.css('padding-top')) {
                    paddingsHeight += parseInt($buttonPane.css('padding-top').replace('px', ''), 10);
                }                               

                $this.height($(window).height() > maxHeight ? maxHeight : $(window).height());
                $contentContainer.css('overflow-y', 'auto');
                $contentContainer.height($this.height() - ($this.find('.ui-dialog-titlebar').height() + $buttonPane.height() + paddingsHeight));
            } else if (maxHeight && $this.height() === maxHeight) {
                $this.height('auto');
            }
        }
    });
}

function escapeRegExpSafe(str) {
    return str.replace(/([.*+?^=!:${}()|\[\]\/\\])/g, "\\$1");
}

String.prototype.replaceAll = function (search, replacement) {
    var target = this;
    return target.replace(new RegExp(escapeRegExpSafe(search), 'g'), replacement);
};

function getDeviceWidth() {
    var deviceWidth = (window.innerWidth > 0) ? window.innerWidth : screen.width;
    return deviceWidth;
}

/**
 * jQuery mousehold plugin - fires an event while the mouse is clicked down.
 * Additionally, the function, when executed, is passed a single
 * argument representing the count of times the event has been fired during
 * this session of the mouse hold.
 *
 * @author Remy Sharp (leftlogic.com)
 * @date 2006-12-15
 * @example $("img").mousehold(200, function(i){  })
 * @desc Repeats firing the passed function while the mouse is clicked down
 *
 * @name mousehold
 * @type jQuery
 * @param Number timeout The frequency to repeat the event in milliseconds
 * @param Function fn A function to execute
 * @cat Plugin
 */

jQuery.fn.mousehold = function (timeout, f) {
    if (timeout && typeof timeout == 'function') {
        f = timeout;
        timeout = 100;
    }
    if (f && typeof f == 'function') {
        var timer = 0;
        var fireStep = 0;
        return this.each(function () {
            jQuery(this).mousedown(function () {
                fireStep = 1;
                var ctr = 0;
                var t = this;
                timer = setInterval(function () {
                    ctr++;
                    f.call(t, ctr);
                    fireStep = 2;
                }, timeout);
            })

            clearMousehold = function () {
                clearInterval(timer);
                if (fireStep == 1) f.call(this, 1);
                fireStep = 0;
            }

            jQuery(this).mouseout(clearMousehold);
            jQuery(this).mouseup(clearMousehold);
        })
    }
}

function highlightMenu(item) {

}

function getElementVisibleHeight($el) {
    var scrollTop = $(this).scrollTop(),
        scrollBot = scrollTop + $(this).height(),
        elTop = $el.offset().top,
        elBottom = elTop + $el.outerHeight(),
        visibleTop = elTop < scrollTop ? scrollTop : elTop,
        visibleBottom = elBottom > scrollBot ? scrollBot : elBottom;

    return visibleBottom - visibleTop;
}