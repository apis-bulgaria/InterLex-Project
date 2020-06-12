var isAModalOpened = false;
var lastOpenedTab = 'cases';
var paginationOptions = {
    count: 40,
    start: 1,
    display: 10,
    border: false,
    text_color: '#148dd4',
    background_color: 'none',
    text_hover_color: '#2573AF',
    background_hover_color: 'none',
    images: false,
    mouse: 'press',
    onChange: function (page) {
        ReSearch(page);
    }
};

function changeLang(lang) {
    $.cookie('sitelang', lang, {
        expires: 365,
        path: '/'
    });

    window.location.reload(false);
}

function getURIParam(name) {
    var results = new RegExp('[\?&amp;]' + name + '=([^&amp;#]*)').exec(window.location.href);
    if (results !== null) {
        return results[1] || 0;
    } else {
        return null;
    }
}

// Handle UserAuthorize attribute response
function UnAuthJsonCheck(data) {
    //console.log(data);
    if (typeof data !== 'undefined') {

        if (typeof data.status === 'undefined') {
            try {
                data = JSON && JSON.parse(data) || $.parseJSON(data);
            }
            catch (e) {
                return;
            }
        }
        //console.log(parsedData);
        if (data && data.status === 'unauth') {
            top.window.location = appRootFolder + '/Login/Index?returnUrl=' + top.window.location.href;
            return;
        }
    }
}

function addHints(linkClass, langId, linkInNewTab) {
    $('.' + linkClass + ':not(.type-apisweb,.type-hdappnr,.type-undef,.no-hint)').qtip({
        content: {
            text: function (event, api) {
                var closeIcon = $('#' + api._id).find('.qtip-close.qtip-icon').hide();
                api.set('content.title', '');
                api.set('content.text', 'Loading...');
                linkType = 'undef';
                if ($(this).hasClass('type-docnr'))
                    linkType = 'docnr';
                else if ($(this).hasClass('type-doclangid'))
                    linkType = 'doclangid';
                else if ($(this).hasClass('type-hdappnr'))
                    linkType = 'hdappnr';
                else if ($(this).hasClass('type-celex'))
                    linkType = 'celex';

                docNumber = $(this).data('dn');
                //console.log(docNumber);
                if ($(this).hasClass('type-guid')) {
                    toUrl = appRootFolder + '/Doc/DocHintByIdentifier/' + docNumber;
                }
                else if (linkType == 'celex' || linkType == 'docnr' || linkType == 'hdappnr' || linkType == 'doclangid') {
                    //console.log($(this).hasClass('mod-ref'));
                    if ($(this).hasClass('mod-ref')) {
                        modType = $(this).attr('modtype');
                        toUrl = appRootFolder + '/Doc/ModHint/' + $('#doc-view-container').data('docnumber').replace(/\//g, '_') + '/' + langId + '/' + docNumber.replace(/\//g, '_') + '/' + modType;
                    }
                    else {
                        toPar = $(this).data('topar');
                        //if ($(this).hasClass('type-celex') && $(this).parents('#textSec').length == 1)
                        if ($(this).hasClass('type-celex') && $(this).hasClass('d-no-cons') == false)
                            toUrl = appRootFolder + '/Doc/ParHint/' + linkType + '/LastConsTxt/Lang' + langId + '/' + docNumber.replace(/\//g, '_') + '/' + toPar;
                        else if ($(this).hasClass('type-doclangid')) {
                            toUrl = appRootFolder + '/Doc/ParHintById/' + docNumber + (toPar ? "/" + toPar : ""); // topar is eid for Vat320060L112
                        }
                        else
                            toUrl = appRootFolder + '/Doc/ParHint/' + linkType + '/Lang' + langId + '/' + docNumber.replace(/\//g, '_') + '/' + toPar;
                    }
                    //console.log(toUrl);
                }
                else {
                    console.log('Hint Error in link. Css class type* not found');
                    return 'Hint Error in link. Css class type* not found';
                }

                var linkObj = $(this);
                //console.log(toUrl);
                $.ajax({ url: toUrl })
                    .done(function (data) {

                        closeIcon.show();

                        var docNumber = data.DocNumber;

                        if (data.DocType != 0) {
                            var isPdfBlob = data.BloblInfo && data.BloblInfo.IsBlob && data.BloblInfo.IsPdf;
                            var title = '<div style="display: inline-block; " class="qtip - label" />';
                            if (!isPdfBlob) {
                                var countryFlagImg = '';
                                if (data.Country != '' && data.Country != undefined) {
                                    countryFlagImg = '<img src="' + appRootFolder + '/Content/Images/flags/' + data.Country + '.png" title="' + data.CountryName + '" style="border:1px solid #AAA" class="qtip-flag"/>';
                                }

                                if (docNumber === 'undefined') {
                                    docNumber = '';
                                }
                                docTypeImgSrc = (data.DocType == '1') ? 'icon-cases-small-white.png' : 'icon-law-small-white.png';
                                title = '<img src="' + appRootFolder + '/Content/Images/' + docTypeImgSrc + '" alt="" border="0" style="display: inline;"/><div style="display: inline-block;" class="qtip-label">' + docNumber + '</div>' + countryFlagImg;
                            }

                            api.set('content.title', title);

                            if (data.ArticlePathJSON) {
                                var articlePaths = $.parseJSON(data.ArticlePathJSON),
                                    $container = $('<div id="breadcrumbs-container-qtip"></div>'),
                                    $breadCrumsUl = $('<ul class="eucs-breadcrumbs"></ul>'),
                                    $titleSpan = $('<span class="title-container-qtip"></span>'),
                                    $contentSpan = $('<span class="content"></span>'),
                                    len = articlePaths.length,
                                    content,
                                    i;

                                for (i = len - 1; i >= 0; i--) {
                                    $titleSpan = $titleSpan.clone();
                                    $titleSpan.html('');
                                    $contentSpan = $contentSpan.clone();
                                    $contentSpan.html('');

                                    content = articlePaths[i].num + ' ' + articlePaths[i].heading;
                                    $titleSpan.attr('title', content);
                                    $titleSpan.attr('data-doc-lang-id', articlePaths[i].docLangId);
                                    $titleSpan.attr('data-eid', articlePaths[i].eid);
                                    $titleSpan.attr('data-open-vat', data.DocNumber.toLowerCase() === '32006L0112'.toLowerCase() ? 1 : 0);
                                    $contentSpan.html(articlePaths[i].num);
                                    $titleSpan.append($contentSpan);

                                    $breadCrumsUl.append($('<li></li>').append($titleSpan));
                                }

                                $container.append($breadCrumsUl);
                                api.set('content.text', $container[0].outerHTML + data.Text);
                            } else {
                                api.set('content.text', data.Text);
                            }
                        }
                        // when the document is not found
                        else {
                            alt_link = linkObj.attr('alternative');

                            if (typeof alt_link !== typeof undefined && alt_link !== false && alt_link != '') {
                                // check for web.apis.bg format
                                if (alt_link.indexOf('http://') == -1 && alt_link.indexOf('Base') > -1) {
                                    linkObj.css('color', '#F7941F');
                                    alt_link = 'http://web.apis.bg/p.php?' + alt_link.replace('./', '').replace('&amp;', '&');
                                }
                                linkObj.attr('href', alt_link);
                                api.set('content.title', 'External link');
                                api.set('content.text', '<a href="' + alt_link + '" target="_blank">' + alt_link + '</a>');
                            }
                            else {
                                api.set('content.title', docNumber);
                                api.set('content.text', "<div class='d-hint-wrapper'>" + data.Text + "</div>");
                            }
                        }
                    })
                    .fail(function (xhr, status, error) {
                        api.set('content.text', status + ': ' + error)
                    });

                return 'Loading...';
            },
            button: true
        },
        position: {
            viewport: $(window),
            target: 'mouse',
            adjust: {
                mouse: false,
                y: 5
            }
        },
        show: {
            effect: false,
            delay: 200,
            solo: true
        },
        events: {
            show: function (event, api) {
                $(this).draggable({
                    handle: ".qtip-titlebar"
                });
            }
        },
        hide: {
            fixed: true,
            delay: 300,
            effect: false,
        },
        style: {
            classes: 'qtip-light'
        }
    });
}

function globalDocumentUIBinding() {
    controlOverflowing();
    bindSelectsForExport();

    $('.aFavouriteDoc').click(favouriteDocAddClick);
    $('.aFavouriteDocRemove').click(favouriteDocRemoveClick);

    // $(this).css('border-left', '2px solid #FCB647');

    $('.doc-item-container').hover(function () {
        $(this).prev().css('height', $(this).css('height'));
        $(this).prev().css('visibility', 'visible');

        $(this).css('border-left', '2px solid #12ADFF');
        // $(this).children($('.doc-top-bar').css('background', '#12ADFF'));

    }, function () {
        $(this).prev().css('visibility', 'hidden');

        $(this).css('border-left', '2px solid #E0E0E0');
        $(this).css('background', '#fff');
        $(this).children($('.doc-top-bar').css('background', '#E8F5FF'));
    });

    $('.doc-title-a-container a').click(function () {
        $(this).addClass('f-bold');
        $(this).addClass('f-italic');
    });
}

function controlFixedTopMargining() {
    var advSearchHeight = $('#secAdvSearch').css('height');
    var advSearchHeightPixels = parseInt(advSearchHeight.substring(0, advSearchHeight.length - 2));
    $('.body-container').css('margin-top', advSearchHeightPixels + 60);
}

/* Consider removing this function... only present here because the another is not loaded out of the document */
function prepairRelatedDocsLinks(preferedLangId, linkInNewTab) {
    $('.d-apis.d-ref.d-inline').addClass('inline_link');

    addHints('inline_link', preferedLangId, linkInNewTab);
}
