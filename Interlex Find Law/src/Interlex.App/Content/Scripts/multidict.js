selectedMultiDictIds = {};
selectedQueryTexts = {};
deleteCookie('selectedMultiDictIds');

function multiDictClear(tabPrefix) {
  //  $('#formCases').is(':visible') ? 'Cases_' : $('#formLaw').is(':visible') ? 'Law_' : ''; // this is not so reliable in this case, consider iterrating all multidict elements
    selectedQueryTexts = {};
    selectedMultiDictIds = {};
    deleteCookie('selectedMultiDictIds');
    $('#multilingual-dictionary-query-content').html('');
    $('.fa-check-square-o').addClass('fa-square-o');
    $('.fa-check-square-o').removeClass('fa-check-square-o');
    $('#list-multilingual-dictionary-terms-langs li.selected').removeClass('selected');
    $('#list-multilingual-dictionary-terms li.selected').removeClass('selected');
    $('#Cases_tb-multilingual-dictionary').val('');
    $('#Cases_tb-multilingual-dictionary').attr('title', '');
    $('#Law_tb-multilingual-dictionary').val('');
    $('#Law_tb-multilingual-dictionary').attr('title', '');
    $('#tb-multilingual-dictionary').val('');
    $('#tb-multilingual-dictionary').attr('title', '');
    //$('#' + tabPrefix + 'MultiDict_Text').val('');
    //$('#' + tabPrefix + 'MultiDict_LogicalType').val('');
    $('#MultiDict_Text').val('');
    $('#MultiDict_LogicalType').val('');
    $('#Cases_MultiDict_Text').val('');
    $('#Cases_MultiDict_LogicalType').val('');
    $('#Law_MultiDict_Text').val('');
    $('#Law_MultiDict_LogicalType').val('');
    $('#tb-multilingual-dictionary-search').val('');
    // refreshing alphabet
    $('.multilingual-dictionary-letter').removeClass('selected');
    $('.multilingual-dictionary-letter[data-letter="all"]').addClass('selected');

    $('#tb-multilingual-dictionary-search').val('c');
    $('#btn-multilingual-dictionary-search').trigger('click'); // refreshing result
    $('#tb-multilingual-dictionary-search').val('');

    /* Refreshing "Or" radio button as primary */
    $('#rd-multilingual-dictionary-or').prop('checked', true);
    $('#rd-multilingual-dictionary-and').removeAttr('checked');
}

function bindMultiDictContentEvents() {
    $('#list-multilingual-dictionary-terms li').on('click', function (event) {
        event.preventDefault();
        event.stopPropagation();

        var $target = $(event.target),
            id = $(this).attr('data-id');

        $('#list-multilingual-dictionary-terms li').removeClass('active');
        $(this).addClass('active');

        $.ajax({
            type: 'POST',
            url: appRootFolder + '/Search/MultiDictGetTranslations',
            data: {
                id: id
            }
        }).done(function (response) {
            $('#multilingual-dictionary-translations-container').html(response);
            bindMultiDictTranslationsEvents();

            if ($target.hasClass('fa')) {
                var allLangs = $('#list-multilingual-dictionary-terms-langs li'),
                    primaryLangElements = allLangs.filter(function () {
                        return ($(this).attr('data-langid') >= 1 && $(this).attr('data-langid') <= 5) // primary langs without Italian
                               || $(this).attr('data-langid') == $('#slct-multilingual-dictionary-lang option:selected').val(); // currently selected search lang
                    }),
                    allLangCheckboxes = allLangs.find('.fa'),
                    primaryLangCheckboxes = primaryLangElements.find('.fa'),
                    allLangTexts = allLangs.find('.multilingual-dictionary-translation-text'),
                    primaryLangTexts = primaryLangElements.find('.multilingual-dictionary-translation-text'),
                    allLangsSelected = $('#list-multilingual-dictionary-terms-langs li.selected');

                if ($target.hasClass('fa-square-o')) { // adding all primary langs
                    multiDictMarkSelected(id);

                    primaryLangElements.each(function () {
                        multiDictSelectItem(id, $(this).attr('data-langid'));
                    });

                    primaryLangElements.addClass('selected');
                    primaryLangCheckboxes.addClass('fa-check-square-o');
                    primaryLangCheckboxes.removeClass('fa-square-o');

                    primaryLangElements.each(function () {
                        selectedQueryTexts[id + ':' + $(this).attr('data-langid')] = '"' + $(this).find('.multilingual-dictionary-translation-search-text').text() + '"';
                    });

                    populateBuildedQuery();
                }
                else {
                    allLangs.each(function () {
                        delete selectedQueryTexts[id + ':' + $(this).attr('data-langid')];
                    });

                    allLangsSelected.each(function () {
                        multiDictRemoveItem(id, $(this).attr('data-langid'));
                    });

                    allLangs.removeClass('selected');
                    allLangCheckboxes.addClass('fa-square-o');
                    allLangCheckboxes.removeClass('fa-check-square-o');

                    populateBuildedQuery();
                }
            }
        });

        return false;
    });

    function bindMultiDictTranslationsEvents() {
        $('#list-multilingual-dictionary-terms-langs li').on('click', function () {
            var cbInner = $(this).find('span.fa'), // inner "checkbox"
                forItemId = $(this).parent().attr('data-itemid'),
                langId = $(this).attr('data-langid'),
                translationText = $(this).find('.multilingual-dictionary-translation-text').text();

            if ($(this).hasClass('selected')) { // selecting
                $(this).removeClass('selected');
                cbInner.addClass('fa-square-o');
                cbInner.removeClass('fa-check-square-o');

                delete selectedQueryTexts[forItemId + ':' + langId];

                multiDictRemoveItem(forItemId, langId);
            }
            else { // deselecting
                $(this).addClass('selected');
                cbInner.addClass('fa-check-square-o');
                cbInner.removeClass('fa-square-o');

                selectedQueryTexts[forItemId + ':' + langId] = '"' + translationText + '"';
                multiDictMarkSelected(forItemId);
                multiDictSelectItem(forItemId, langId);
            }

            populateBuildedQuery();
        });
    }
}

function populateBuildedQuery(tabPrefix) {
    tabPrefix = $('#formCases').is(':visible') ? 'Cases_' : $('#formLaw').is(':visible') ? 'Law_' : '';
    console.log('###' + tabPrefix);
  
    var logicalLabel = $('input[name="rd-multilingual-dictionary"]:checked').val();
    var fullLogicalLabel = '<span class="lb-multilingual-dictionary-logical">' + logicalLabel + '</span>';
    var selectedQueryContentSpans = {};
    for (var queryText in selectedQueryTexts) {
        selectedQueryContentSpans[queryText] = '<span class="lb-multilingual-dictionary-content relative" data-itemdata="' + queryText + '">'
            + selectedQueryTexts[queryText] + '<span class="fa fa-times-circle lb-multilingual-dictionary-delete visibility-hidden"></span></span>';
    }

    var buildedQuery = associativeJoin(selectedQueryContentSpans, ' ' + fullLogicalLabel + ' ');
    console.log('###' + buildedQuery);
    $('#multilingual-dictionary-query-content').html(buildedQuery);
    var multiDictionaryText = associativeJoin(selectedQueryTexts, ' ' + logicalLabel + ' ');
    console.log(multiDictionaryText);
    $('#' + tabPrefix + 'MultiDict_Text').val(multiDictionaryText);

    $('.lb-multilingual-dictionary-content').unbind();
    $('.lb-multilingual-dictionary-content').hover(function () {
        $(this).find('.lb-multilingual-dictionary-delete').removeClass('visibility-hidden');
    }, function () {
        $(this).find('.lb-multilingual-dictionary-delete').addClass('visibility-hidden');
    });

    $('.lb-multilingual-dictionary-delete').unbind();
    $('.lb-multilingual-dictionary-delete').on('click', function (event) {
        event.preventDefault();
        event.stopPropagation();

        var prev = $(this).parent().prev(),
            spanText = $(this).parent().find('.lb-multilingual-dictionary-content').text(),
            itemData = $(this).parent().attr('data-itemdata'),
            itemGuid = itemData.split(':')[0],
            itemLang = itemData.split(':')[1];

        delete selectedQueryTexts[itemData];
        multiDictRemoveItem(itemGuid, itemLang);

        if (prev.length && prev.hasClass('lb-multilingual-dictionary-logical')) {
            prev.remove(); // clearing previous logical span
        }
        else {
            $(this).parent().next().remove(); // if the first element is selected clearing the next logical span
        }

        $(this).parent().remove();

        if ($('#list-multilingual-dictionary-terms li[data-id="' + itemGuid + '"]').hasClass('active')) {
            var langItemFromList = $('#list-multilingual-dictionary-terms-langs li[data-langid="' + itemLang + '"]'),
                langItemFromListIcon = langItemFromList.find('.fa');
            langItemFromList.removeClass('selected');
            langItemFromListIcon.addClass('fa-square-o');
            langItemFromListIcon.removeClass('fa-check-square-o');
        }

        populateBuildedQuery();
    });

    $('#' + tabPrefix + 'MultiDict_SelectedQueryTexts').val(JSON.stringify(selectedQueryTexts));
    $('#' + tabPrefix + 'MultiDict_SelectedQueryTexts').trigger('change');
   
}

function multiDictSelectItem(itemId, translationId) {
    if (typeof selectedMultiDictIds['item-' + itemId] === 'undefined') {
        selectedMultiDictIds['item-' + itemId] = [];
    }

    selectedMultiDictIds['item-' + itemId].push(translationId);
    setCookie('selectedMultiDictIds', JSON.stringify(selectedMultiDictIds), 1);
}

function multiDictRemoveItem(itemId, translationId) {
    selectedMultiDictIds['item-' + itemId].remove(translationId);
    if (selectedMultiDictIds['item-' + itemId].length === 0) {
        delete selectedMultiDictIds['item-' + itemId];
        multiDictMarkUnselected(itemId);
    }

    setCookie('selectedMultiDictIds', JSON.stringify(selectedMultiDictIds), 1);
}

function multiDictMarkSelected(id) {
    var item = $('#list-multilingual-dictionary-terms li[data-id="' + id + '"]');
    item.addClass('selected');
    var innerCheckbox = item.find('.fa');
    innerCheckbox.addClass('fa-check-square-o');
    innerCheckbox.removeClass('fa-square-o');
}

function multiDictMarkUnselected(id) {
    var item = $('#list-multilingual-dictionary-terms li[data-id="' + id + '"]');
    item.removeClass('selected');
    var innerCheckbox = item.find('.fa');
    innerCheckbox.addClass('fa-square-o');
    innerCheckbox.removeClass('fa-check-square-o');
}

function bindMultiDictLetterEvents() {
    $('.multilingual-dictionary-letter:not(.disabled)').click(function (event) {
        event.preventDefault();
        event.stopPropagation();

        $('.multilingual-dictionary-letter').removeClass('selected');
        $(this).addClass('selected');

        $('#btn-multilingual-dictionary-search').trigger('click');
    });
}