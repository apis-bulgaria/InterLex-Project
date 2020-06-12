$().ready(function () {
    bindEuroStatColumnHiding();
});

function bindEuroStatColumnHiding() {
    function anyColumnIsHidden() {
        // consider checking tds too
        var tHeads = $('th'),
            anyHidden = false;
        tHeads.each(function () {
            if ($(this).css('display') === 'none') {
                anyHidden = true;
                return false;
            }
        });

        return anyHidden;
    }

    $('.col-show-hide-link').on('click', function (event) {
        event.preventDefault();
        event.stopPropagation();

        var that = $(this);
        var dataColId = that.attr('data-col-id');

        var tHeads = $('th[data-col-id="' + dataColId + '"]');
        var tCols = $('td[data-col-id="' + dataColId + '"]');

        if (that.hasClass('cols-visible')) {
            tHeads.hide(200);
            tCols.hide(200);

            that.addClass('f-grey');
            that.removeClass('f-blue');
            that.removeClass('cols-visible');
            that.addClass('cols-hidden');
            that.attr('title', Resources.storage['UI_JS_Add']); // dobavi
        }
        else {
            tHeads.show(200);
            tCols.show(200);

            that.addClass('f-blue');
            that.removeClass('f-grey');
            that.removeClass('cols-hidden');
            that.addClass('cols-visible');
            that.attr('title', Resources.storage['UI_Remove']); // premahni
        }

        setTimeout(function () {
            if (anyColumnIsHidden()) {
                $('#col-show-all').addClass('visibility-visible');
                $('#col-show-all').removeClass('visibility-hidden');
            }
            else {
                $('#col-show-all').addClass('visibility-hidden');
                $('#col-show-all').removeClass('visibility-visible');
            }
        }, 300);

    });

    $('#col-show-all').on('click', function () {
        var tHeads = $('th'),
            tCols = $('td'),
            that = $(this),
            hiddenColsLinks = $('.cols-hidden');

        tHeads.show(200);
        tCols.show(200);

        hiddenColsLinks.addClass('f-blue');
        hiddenColsLinks.removeClass('f-grey');

        hiddenColsLinks.removeClass('cols-hidden');
        hiddenColsLinks.addClass('cols-visible');

        that.addClass('visibility-hidden');
        that.removeClass('visibility-visible');

        return false;
    });
}

function populateFinsDocTypeBoxTitles() {
    // getting checkboxes
    var allSelectedCheckboxes = $('#finances-document-type-container label input[type="checkbox"]:checked');

    // getting spans
    var allSpans = allSelectedCheckboxes.next().next(); // bad selector; try refactoring later

    // mapping all texts splitted
    var boxText = allSpans.map(function () {
        return $(this).text();
    }).get().join('; ');

    // populating box
    $('#finances-document-type-tb').val(boxText);
    $('#finances-document-type-tb').attr('title', boxText);
    $('#finances-document-type-tb').trigger('change');
}

function financesScrollToChart(event) {
    $('.chart-area.flot-chart').ScrollTo();
}

function rebindScrollToChart() {
    $('#link-show-chart').unbind();

    $('#link-show-chart').on('click', function (event) {
        event.stopPropagation();
        event.preventDefault();

        $('#finances-charts-container').ScrollTo();
    });
}