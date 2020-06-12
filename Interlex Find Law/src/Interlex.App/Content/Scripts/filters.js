/*search result functions */
var FILTERS_INITIALLY_SHOWN = 10;

function populateFilters() {
    $.ajax({
        type: 'POST',
        dataType: "JSON",
        url: appRootFolder + '/Search/GetFilterClassifiers/Search-' + searchId
    }).done(function (response) {
        console.log(response.IsCaseLawFolder);
        let data = response.Data;
        var filters = new Array(),
            ul = null;

        if (!data.some((element) => {
            return element.Type === 3031
        }) && !$('.selected-filters span[data-typeid="3031"]').length && response.IsCaseLawFolder) {
            let fake3031 = {
                DocsCount: 0,
                Id: "0e82da2c-20cf-4fbb-a2c2-4006ae0342da",
                Name: 'does not even matter, it will get replaced',
                OrderValue: 1,
                Type: 3031
            };

            data.push(fake3031);
        };

        $.each(data, function (i, obj) {
            if (!filters[obj.Type] && obj.Type !== 3031) {
                filters[obj.Type] = $('<div class="search-filter f-blue" style="padding-left:10px; height: auto;"></div>');
                ul = $(document.createElement('ul'));
                ul.addClass('filter-ul-holder');
                filters[obj.Type].append(ul);
            }

            var li = $('<li></li>');
            var link = $('<a/>');
            link.data('id', obj.Id);
            if (obj.Type === 3031) {
                console.log(Resources.storage);
                obj.Name = Resources.storage['UI_PILCases'];
            }
            link.html(obj.Name + '&nbsp;(' + obj.DocsCount + ')');
            link.attr('href', 'javascript:void(true);');
            link.addClass('f-blue');
            link.addClass('filterItem');
            if (obj.Type !== 3031 || (obj.Type === 3031 && obj.DocsCount > 0)) {
                link.click(function () {
                    $.ajax({
                        type: 'POST',
                        url: appRootFolder + '/Search/SetFilterClassifier/Search-' + searchId + '/' + obj.Type + '/' + obj.Id,
                        data: { name: $(this).text() }
                    }).done(function () {
                        window.location.reload();
                    });
                });
            }

            li.append(link);

            if (obj.Type === 3031) {
                ul = $(document.createElement('ul'));
                li.prepend('<span class="www-line"></span>');
                li.addClass('m4p');
                if (obj.DocsCount === 0) {
                    li.addClass('inactive');
                }
                ul.append(li);
                $('#pAvaiableFiltersBody').prepend(ul);
            }
            else {
                ul.append(li);
            }
        });

        $('#filters-loading').hide();
        $.each(filters, function (typeid, filter) {
            if (filter) {
                $('#filterTitle' + typeid).append(filter);
                $('#filterTitle' + typeid).show();
            }
        });

        $.each(filters, function (index, value) {
            var curUl = $(value).find('ul');
            var filterItemCounts = curUl.children().length;

            if (filterItemCounts > 0) {
                if (true) {
                    var allFilterLinks = curUl.find('li');

                    var moreLi = $('<li></li>');
                    var moreLink = $('<a>');
                    moreLink.attr('href', 'javascript:void(true);');
                    moreLink.html('<span class="chevron-filters-down inline-block"></span>');
                    moreLink.addClass('filter-more-link');
                    moreLink.click(function (e) {
                        var parentId = $(e.target).parent().parent().attr('id');

                        if ($(e.target).hasClass('chevron-filters-down')) {
                            curUl.hide();
                            $(e.target).addClass('chevron-filters-right');
                            $(e.target).removeClass('chevron-filters-down');

                            eraseCookie(parentId + '-expanded');
                            setCookie(parentId + '-expanded', 'false', 365);
                        }
                        else {
                            curUl.show();

                            $(e.target).addClass('chevron-filters-down');
                            $(e.target).removeClass('chevron-filters-right');

                            eraseCookie(parentId + '-expanded');
                            setCookie(parentId + '-expanded', 'true', 365);
                        }
                    });

                    var curTitle = curUl.parent().prev('.filter-title');
                    moreLink.insertAfter(curTitle);

                    curTitle.click(function (e) {
                        e.preventDefault();

                        if ($(e.target).hasClass('filter-title')) {
                            $(e.target).next().children().first().trigger('click'); //used to select inner span - not triggering 'a' click cause of image changing for the span
                        }
                    });
                }
            }
        });

        $.each($('.filter'), function () {
            var currentId = $(this).attr('id');
            var filterUlHolder = $(this).find('.filter-ul-holder');
            var currentCookie = getCookie(currentId + '-expanded');
            var expandLink = $(this).find('.filter-more-link').children('span').first();
            if (currentCookie !== undefined && currentCookie !== '') {
                if (currentCookie === 'false') {
                    filterUlHolder.hide();

                    $(expandLink).addClass('chevron-filters-right');
                    $(expandLink).removeClass('chevron-filters-down');
                }
                else {
                    filterUlHolder.show();

                    $(expandLink).addClass('chevron-filters-down');
                    $(expandLink).removeClass('chevron-filters-right');
                }
            }
        });

        return;
    });
}

function showMoreFilters(allFilterLinks, filterItemCounts) {
    for (var i = FILTERS_INITIALLY_SHOWN; i < filterItemCounts; i++) {
        $(allFilterLinks[i]).show();
    }
}

function hideMoreFilters(allFilterLinks, filterItemCounts) {
    for (var i = FILTERS_INITIALLY_SHOWN; i < filterItemCounts; i++) {
        $(allFilterLinks[i]).hide();
    }
}