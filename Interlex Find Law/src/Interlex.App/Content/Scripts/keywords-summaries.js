function changeSourceTab(element, type) {
    var $target = $(element),
      $parentContainer = $target.parent(),
      currentOverflow,
      summariesToDisplay,
      shouldExpandAfterChange,
      elementToExpand,
      mainTabContainer;

    if (type === 'summaries') {
        mainTabContainer = 'summary';
    }
    else {
        mainTabContainer = 'keywords';
    }

    $parentContainer.children('.not-active-link-' + type).removeClass('f-black');
    $parentContainer.children('.not-active-link-' + type).addClass('f-blue-important');
    $parentContainer.children('.not-active-link-' + type).removeClass('not-active-link-' + type);

    $target.removeClass('f-blue-important');
    $target.addClass('f-black');
    $target.addClass('not-active-link-' + type);

    $parentContainer.parent().children('.' + type + '-content').hide();

    currentOverflow = $parentContainer.parent().parent().children('.doc-' + mainTabContainer).first().css('overflow');
    if (currentOverflow === 'hidden') {
        shouldExpandAfterChange = false;
    }
    else {
        shouldExpandAfterChange = true;
        elementToExpand = $parentContainer.parent().parent().children('.doc-' + mainTabContainer).first();
    }

    $parentContainer.parent().parent().children('.doc-' + mainTabContainer).css('height', '9.0ex');
    $parentContainer.parent().parent().children('.doc-' + mainTabContainer).css('overflow', 'hidden');

    summariesToDisplay = $('#' + type + '-' + $target.data('target'));
    summariesToDisplay.show();

    if (type === 'summaries') {
        controlOverflowingSummary();
    }
    else if (type === 'keywords') {
        controlOverflowingKeywords();
    }
    else {
        throw 'Invalid doc list tab type';
    }

    if (shouldExpandAfterChange === true) {
        elementToExpand.css('height', 'auto');
        elementToExpand.css('overflow', 'auto');
        elementToExpand.children('.show-more-' + mainTabContainer + '-div').children('a').first().removeClass('fa-expand');
        elementToExpand.children('.show-more-' + mainTabContainer + '-div').children('a').first().addClass('fa-compress');
    }
}

function changeKeywordsTab(element) {
    changeSourceTab(element, 'keywords');
}

function changeSummariesTab(element) {
    changeSourceTab(element, 'summaries');
}

function controlOverflowing() {
    controlOverflowingTitles();
    controlOverflowingKeywords();
    controlOverflowingSummary();
}

/*Title overflows*/
function controlOverflowingTitles() {
    var titles = $('.doc-title'),
        divModel = $(document.createElement('div')),
        linkModel = $(document.createElement('a')),
        overflowing,
        div,
        link;

    divModel.addClass('show-more-div');
    divModel.addClass('absolute');
    linkModel.addClass('show-more');
    linkModel.addClass('h3');
    linkModel.html('&#x25BC;');
    linkModel.attr('href', '#');
    linkModel.attr('title', Resources.storage['UI_JS_ExpandTitle']);

    titles.each(function () {
        overflowing = isOverflowing($(this));

        if (overflowing) {
            div = divModel.clone();
            link = linkModel.clone();

            div.append(link);

            $(this).append(div);
        }
    });

    $('.show-more').click(showMore);
}

/*Keywords overflow*/
function controlOverflowingKeywords() {
    var keywords = $('.doc-keywords'),
        divModel = $(document.createElement('div')),
        linkModel = $(document.createElement('a')),
        overflowing,
        div,
        link;

    $('.show-more-keywords-div').remove();

    divModel.addClass('show-more-keywords-div');
    divModel.addClass('absolute');

    linkModel.addClass('show-more-keywords');
    linkModel.addClass('h3');
    linkModel.addClass('fa');
    linkModel.addClass('fa-expand');
    linkModel.attr('href', '#');
    linkModel.attr('title', Resources.storage['UI_JS_ExpandKeywords']);

    keywords.each(function () {
        overflowing = isOverflowing($(this));

        if (overflowing) {
            div = divModel.clone();
            link = linkModel.clone();

            div.append(link);
            $(this).append(div);
        }
    });

    $('.show-more-keywords').click(showMoreKeywords);
}

function controlOverflowingSummary() {
    var summaries = $('.doc-summary'),
        divModel = $(document.createElement('div')),
        linkModel = $(document.createElement('a')),
        overflowing,
        div,
        link;

    $('.show-more-summary-div').remove();

    divModel.addClass('show-more-summary-div');
    divModel.addClass('absolute');

    linkModel.addClass('show-more-summary');
    linkModel.addClass('h3');
    linkModel.addClass('fa');
    linkModel.addClass('fa-expand');
    linkModel.attr('href', '#');
    linkModel.attr('title', Resources.storage['UI_JS_ExpandSummary']);

    summaries.each(function () {
        overflowing = isOverflowing($(this));

        if (overflowing) {
            div = divModel.clone();
            link = linkModel.clone();

            div.append(link);

            $(this).append(div);
        }
    });

    $('.show-more-summary').click(showMoreSummary);
}

function showMore(e) {
    var title = $(e.target).parent().parent(),
        itemcontainer;

    e.preventDefault();

    if (title.css('overflow') === 'hidden') {
        title.css('overflow', 'visible');
        title.css('max-height', '100%');
        title.css('height', 'auto');

        $(e.target).html('&#x25B2;');
        $(e.target).attr('title', Resources.storage['UI_JS_CompressTitle']);
    }
    else {
        title.css('overflow', 'hidden');
        title.css('max-height', '4.7ex');
        title.css('height', '4.7ex');

        $(e.target).html('&#x25BC;');
        $(e.target).attr('title', Resources.storage['UI_JS_ExpandTitle']);
    }

    itemcontainer = title.parent();

    $(itemcontainer).prev().css('height', $(itemcontainer).css('height'));
}

function showMoreKeywords(e) {
    e.preventDefault();

    var keywords = $(e.target).parent().parent(),
        itemcontainer;

    if (keywords[0].style.overflow !== 'auto') {

        keywords[0].style.overflow = 'auto';
        keywords[0].style.height = 'auto';

        $(e.target).addClass('fa-compress');
        $(e.target).removeClass('fa-expand');
        $(e.target).attr('title', Resources.storage['UI_JS_CompressKeywords']);
    }
    else {
        keywords[0].style.overflow = 'hidden';
        keywords[0].style.height = '9.0ex';

        $(e.target).addClass('fa-expand');
        $(e.target).removeClass('fa-compress');
        $(e.target).attr('title', Resources.storage['UI_JS_ExpandKeywords']);
    }

    itemcontainer = keywords.parent();

    $(itemcontainer).prev().css('height', $(itemcontainer).css('height'));
}

//consider unifiying showMoreSummaries and showMoreKeywords
function showMoreSummary(e) {
    e.preventDefault();

    var summary = $(e.target).parent().parent(),
        itemcontainer

    if (summary[0].style.overflow !== 'auto') {
        summary[0].style.overflow = 'auto';
        summary[0].style.height = 'auto';

        $(e.target).addClass('fa-compress');
        $(e.target).removeClass('fa-expand');

        $(e.target).attr('title', Resources.storage['UI_JS_CompressSummary']);
    }
    else {
        summary[0].style.overflow = 'hidden';
        summary[0].style.height = '9.0ex';

        $(e.target).addClass('fa-expand');
        $(e.target).removeClass('fa-compress');

        $(e.target).attr('title', Resources.storage['UI_JS_ExpandSummary']);
    }

    itemcontainer = summary.parent();

    $(itemcontainer).prev().css('height', $(itemcontainer).css('height'));
}

function isOverflowing($s) {
    var $s,
        hidden;

    $s.wrapInner('<div />'); // wrap inner contents
    hidden = $s.height() < $s.children('div').height();

    $s.children('div').replaceWith($s.children('div').html()); //unwrap

    return hidden;
}
