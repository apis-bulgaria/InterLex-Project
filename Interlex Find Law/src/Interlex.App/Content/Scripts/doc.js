currentMatch = 0;

function PracticeByArticle(articleId) {
    $.ajax({
        url: appRootFolder + "PracticeByArticle?articleId=" + articleId,
        type: 'GET',
        success: function (result) {

            // TODO
            //$.ajax({
            //    url: "/Search/SearchResultIncomingLegal?articleId=" + articleId,
            //    type: 'GET',
            //    success: function (result) {
            //        $("#dIncomingLegalByArticle").html(result);
            //        $("#accordion").accordion('option', 'active', 2);
            //    },
            //    error: function (xhr, ajaxOptions, thrownError) {
            //        alert(xhr.status);
            //        alert(thrownError);
            //    }
            //
            $("#dIncomingLegalByArticle").html('By Article ' + articleId + '...<br/><br/>');
            $("#accordion").accordion('option', 'active', 2);

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}

function SetAllHints() {
    //var links = $('a[id^="hlRef_"]');
    //for (i = 0; i < links.length; i++) {
    //    var link = links[i];
    //    $(link).qtip({
    //        content: {
    //            text: 'Loading...', // The text to use whilst the AJAX request is loading
    //            ajax: {
    //                url: 'GetTextAsHint?url=' + links.attr("href"), // URL to the local file
    //                type: 'GET', // POST or GET
    //                data: {} // Data to pass along with your request
    //            }
    //        }
    //    });
    //}
}

function sectionItemClick(sectionId, subItemId) {
    sec = $('#' + sectionId);
    subitem = $('#' + subItemId);
    if (sec.css('display') == 'none') {
        sec.parent().children('header').trigger('click');
    }

    if (isIE) {
        $("html, body").animate({
            scrollTop: $('#' + subItemId).first().offset().top - 128
        }, 1);
    }
    else {
        $(subitem).ScrollTo();
    }
}

function sectionItemClickParent(sectionId) {
    sec = $('#' + sectionId);

    if (isIE) {
        $("html, body").animate({
            scrollTop: $('#' + sectionId).first().offset().top - 128
        }, 1);
    }
    else {
        $(sec).ScrollTo();
    }
}

function loadDocContents(docLangId) {
    if ($('#doc-view-container').length > 0) {
        //alert($('#doc-view-container > .docmenu-item').length);
        nav_item = $('#docContents > .doc-nav-category-items');
        //nav_item.attr('style', '');
        nav_item.html('');
        $('#doc-view-container').children('.docmenu-item').each(function () {


            li = $('<li class="f-black" style="font-size: 1.0em;"></li>');
            title = $(this).children().first().children('.docmenu-item-text');
            li.html('<a href="javascript:sectionItemClickParent(\'' + title.attr('id') + '\');" style="color:#148DD4">' + title.text() + '</a>');
            //console.log(title.text());

            // check for second level
            if ($(this).children().eq(1).length == 1) {
                sectionId = $(this).children().eq(1).attr('id');
                ul = $('<ul style="list-style-type:none;padding-left:4px;"></ul>');
                $(this).children().eq(1).find('.docmenu-item').each(function () {
                    subitem = $(this).children('.docmenu-item-text');


                    // special case for eufins documents, related ot the simliar cases tab structutre
                    if ($(this).attr('id') === 'similiarcases-sub-group')
                        subitem = $(this).children('#similiar-cases-header-container').children('.docmenu-item-text')

                    // this check for attribute is used when the sub rubric in the content pane of the document is different from the 
                    //  the rubric that should be visualized in the document
                    // NOTE: EU-FINS specific. Other documents should not depend on this attribute('ui_lang_rubric'), the Xslt predprocessor itself won't produce the attribute for other type of documents
                    var subItemText = subitem.text();
                    if (subitem.attr('ui_lang_rubric'))
                        subItemText = subitem.attr('ui_lang_rubric')

                    ul.append($('<li><a href="javascript:sectionItemClick(\'' + sectionId + '\', \'' + subitem.attr('id') + '\');">' + subItemText + '</a></li>'));
                });

                li.append(ul);
            }

            // Text menu
            if ($(this).find('#textSec').length > 0) {
                nav_item.attr('padding-left', '0px !important');
                tree = $('<div id="DocContents_Tree"></div>');
                li.append(tree);

                // Contents tree
                tree.fancytree(
                    {
                        source:
                        {
                            type: 'GET',
                            dataType: 'json',
                            url: appRootFolder + '/api/Entity/DocContents/' + docLangId,
                            success: function (data) {
                                // if no proper tree data is returned fancytree fire error and global event ajaxComplete is not fired. This is used to check if user has rights
                                UnAuthJsonCheck(data);
                            },
                            cache: false
                        },
                        checkbox: false,
                        clickFolderMode: 2,
                        click: function (event, data) {
                            if (data.targetType == 'title') {
                                scrollToSelector = "a[eid='" + data.node.data.eid + "']";
                                if (isIE) {
                                    $("html, body").animate({
                                        scrollTop: $(scrollToSelector).first().offset().top
                                    }, 1);
                                }
                                else {
                                    $(scrollToSelector).ScrollTo();
                                }
                            }
                        },
                        icons: false,
                        strings: {
                            loading: "Loading...",
                            loadError: "Load error!"
                        }
                    }
                );

            }

            nav_item.append(li);
        });

        nav_item.addClass('selected');
        nav_item.show();
    }
    else {
        $('#docContents').hide();
    }
}



function prepairInlineLinks(linkInNewTab) {
    // language of the opened document is prefered for the link!
    var currentDoc_LangId = $('#doc-view-container').data('langid');
    //$('.d-apis.d-ref.d-inline').addClass('inline_link');

    //$('.d-c-apisweb').addClass('inline_link');
    //$('.d-apisweb').addClass('inline_link');

    addHints('inline_link', currentDoc_LangId, linkInNewTab);
}

// V.O [19.08.2015] Attaches hints to the source ulr of the document
function addSourcesHints() {
    // iterates each source and gets its hidden hint and applies it to the qtip
    $('.d-document-base-source-span').each(function () {
        var mainSourceUrl = $(this).find('.d-document-base-source-a'); // the main source name (EXAMPLE: 'Eur-Lex')
        var mainSourceHint = $(this).find('.d-document-source-hint-span'); // hidden hint

        if (mainSourceHint !== undefined && mainSourceHint.length > 0) {
            var name = $(mainSourceUrl).attr('name');

            var height = 175; // by default for the long texts use 175, for shorter use 50
            var isLongTexted = ($.inArray(name, ['EURLEX', 'DecNat', 'Jure', 'HUDOC', 'Reflets', 'InfoLugano', 'JuriFast', 'Rechtsinformationssystem des Bundes (RIS)', 'Legifrance', 'APIS']) >= 0);

            if (!isLongTexted) {
                height = 50;
            }

            mainSourceUrl.qtip({
                content: mainSourceHint,
                position: {
                    viewport:
                        $(window),
                    target: 'mouse',
                    adjust: { mouse: false, y: -15 },
                    my: 'bottom left'
                },
                show: {
                    effect: false,
                    delay: 200,
                    solo: true
                },
                hide: {
                    fixed: true,
                    delay: 300,
                    effect: false
                },
                style: {
                    classes: 'qtip-light d-source-hint-controll',
                    def: true,
                    width: 500,
                    //height: height,
                }
            });
        }
    });
}

// V.O [20.08.2015] attaches hints to the conrete urls of the document
function addSourceDirectHints() {
    $('.d-document-direct-source-span').each(function () {
        var directSourceUrl = $(this).find('.d-document-direct-source-a');
        var directSourceHint = $(this).find('.d-document-source-hint-span');

        if (directSourceHint !== undefined && directSourceHint.length > 0) {

            directSourceUrl.qtip({
                content: directSourceHint,
                position: {
                    viewport:
                        $(window),
                    target: 'mouse',
                    adjust: { mouse: false, y: -15 },
                    my: 'bottom left'
                },
                show: {
                    effect: false,
                    delay: 200,
                    solo: true
                },
                hide: {
                    fixed: true,
                    delay: 300,
                    effect: false
                },
                style: {
                    classes: 'qtip-light d-source-hint-controll',
                    def: true,
                    width: 500,
                    height: 50,
                }
            });
        }
    })
}

// V.O [20.08.2015] attaches hints to the keyword sources
function addKeywordSoruceHints() {
    $('.btn-lang-keyword').each(function () {
        var hint = $(this).attr('hint');

        // if the hint exists as attribute attched it
        if (hint !== undefined && hint.length > 0) {

            $(this).qtip({
                content: hint,
                position: {
                    viewport:
                        $(window),
                    target: 'mouse',
                    adjust: { mouse: false, y: -15 },
                    my: 'bottom left'
                },
                show: {
                    effect: false,
                    delay: 200,
                    solo: true
                },
                hide: {
                    fixed: true,
                    delay: 300,
                    effect: false
                },
                style: {
                    classes: 'qtip-light d-source-hint-controll',
                    def: true,
                    width: 200,
                    height: 40,
                }
            });
        }
    });
}

// V.O [21.08.2015] attaches hints to the keyword sources
function addSummarySourceHints() {
    $('.btn-lang-summary').each(function () {
        var hint = $(this).attr('hint');

        if (hint !== undefined && hint.length > 0) {
            $(this).qtip({
                content: hint,
                position: {
                    viewport:
                        $(window),
                    target: 'mouse',
                    adjust: { mouse: false, y: -15 },
                    my: 'bottom left'
                },
                show: {
                    effect: false,
                    delay: 200,
                    solo: true
                },
                hide: {
                    fixed: true,
                    delay: 300,
                    effect: false
                },
                style: {
                    classes: 'qtip-light d-source-hint-controll',
                    def: true,
                    width: 200,
                    height: 40,
                }
            });
        }
    });
}

/*search in doc*/
function showNextMatch(modifier) {
    var minusOffset = 157;

    if (matches.length > 0) {
        currentMatch = currentMatch + modifier;
        if (currentMatch < 0) {
            currentMatch = resultCount - 1;
        }
        else if (currentMatch >= resultCount)
            currentMatch = 0;

        $('.matchborder').removeClass('matchborder');
        $('.matchborder-memoryze').removeClass('matchborder-memoryze');
        $(matches[currentMatch]).addClass('matchborder');
        $(matches[currentMatch]).addClass('matchborder-memoryze');

        $('#span-matches-count').text(Resources.storage['UI_JS_Match'] + ': ' + (currentMatch + 1) + '/' + resultCount);

        if (getDeviceWidth() <= 480) {
            minusOffset = 200;
        }

        $('.matchborder').first().ScrollToMinusOffset(minusOffset);
    }
}

function showFirstMatch() {
    var minusOffset = 157;

    if (matches.length > 0) {
        currentMatch = 0;

        $('.matchborder').removeClass('matchborder');
        $('.matchborder-memoryze').removeClass('matchborder-memoryze');
        $(matches[0]).addClass('matchborder');
        $(matches[0]).addClass('matchborder-memoryze');

        $('#span-matches-count').text(Resources.storage['UI_JS_Match'] + ': ' + (currentMatch + 1) + '/' + resultCount);

        if (getDeviceWidth() <= 480) {
            minusOffset = 200;
        }

        $('.matchborder').first().ScrollToMinusOffset(minusOffset);
    }
}

function showLastMatch() {
    var minusOffset = 157;
    if (matches.length > 0) {
        currentMatch = matches.length - 1;

        $('.matchborder').removeClass('matchborder');
        $('.matchborder-memoryze').removeClass('matchborder-memoryze');
        $(matches[currentMatch]).addClass('matchborder');
        $(matches[currentMatch]).addClass('matchborder-memoryze');

        $('#span-matches-count').text(Resources.storage['UI_JS_Match'] + ': ' + (currentMatch + 1) + '/' + resultCount);

        var isIE = /*@cc_on!@*/false || !!document.documentMode;
        $('.matchborder').first().ScrollToMinusOffset(157);


        if (getDeviceWidth() <= 480) {
            minusOffset = 200;
        }

        $('.matchborder').first().ScrollToMinusOffset(minusOffset);
    }
}

function attachEuFinsSimliarCasesTabHanlders() {
    var similiarCasesBtn = $('#similiarcases-btn');

    // at the init state the simliar cases table is present so, this tab should be focues
    similiarCasesBtn.addClass('selected');

    var reverseSimliarCasesBtn = $('#reverse-similiarcases-btn');

    var simliarCasesTable = $('#xslt-simliar-cases');
    var reverseSimliarCasesTable = $('#s-reverse-similiar-cases');

    var isSimliarCasesBtnPresent = similiarCasesBtn !== null && similiarCasesBtn !== undefined;
    var isReverseSimliarCasesBtnPresent = reverseSimliarCasesBtn != null && reverseSimliarCasesBtn !== undefined;

    if (isSimliarCasesBtnPresent && isReverseSimliarCasesBtnPresent) {

        var toggleSelectedClass = function (el) {
            if (el.hasClass('selected'))
                el.removeClass('selected')
            else
                el.addClass('similiarCasesBtn');
        }

        var toggleDisplay = function (target) {

            var toShow = simliarCasesTable;
            var toHide = reverseSimliarCasesTable;
            if ($(target).is(reverseSimliarCasesBtn)) {

                toShow = reverseSimliarCasesTable;
                toHide = simliarCasesTable;

                reverseSimliarCasesBtn.addClass('selected');
                similiarCasesBtn.removeClass('selected');
            }
            else {
                similiarCasesBtn.addClass('selected');
                reverseSimliarCasesBtn.removeClass('selected');
            }

            toShow.show();
            toShow.addClass('selected');

            toHide.hide();
            toHide.removeClass('selected');
        }

        var handler = function (ev) {
            // toggleSelectedClass(similiarCasesBtn);
            // toggleSelectedClass(reverseSimliarCasesBtn);

            toggleDisplay(ev.target);
        }

        similiarCasesBtn.click(handler);
        reverseSimliarCasesBtn.click(handler);
    }
}

function attachInternalReferenceHandlers() {
    var $internalRefs = $('#textSec a[href^="#"]').not('[class="doc-anchor"]');
    $internalRefs.click(function (ev) {
        var $element = $(ev.target);
        var eId = $element.attr('href').replace('#', '');
        navigateToEId(eId);
    });
}

function navigateToEId(eId) {
    var $scrollTo = $("a[eid='" + eId + "']");

    // for cases when the eid is nested we can try to find appropriate match
    // but only if there is single path which contains the current eid
    if ($scrollTo.length === 0) {
        var $possibleScrollTo = $('a[eid]').filter(function (index, element) {
            return $(element).attr('eid').contains(eId);
        });

        if ($possibleScrollTo.length === 1) {
            $scrollTo = $possibleScrollTo;
        }
    }

    $scrollTo.ScrollTo();
}

function attachFitVideo() {
    var $iframes = $('.d-iframe');
    $iframes.parent('p').addClass('fluid-width-video-wrapper');
    $iframes.fitVids();
}
