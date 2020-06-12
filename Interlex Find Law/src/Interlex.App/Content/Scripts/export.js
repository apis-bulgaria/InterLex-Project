var selectedDocsIds = [];
var MAX_SELECTED_DOCUMENTS = 10;

function bindSelectsForExport() {
    $('.checkbox-document').click(checkboxDocumentClick);
}

function checkboxDocumentClick(e) {
    var checkbox = $(this),
        curItemId = checkbox.data('itemid'),
        divToColor = $(e.target).parent().next(),
        exportLimitTitle = Resources.storage['UI_JS_MaxExportCountTitle'],
        exportLimitMessage = Resources.storage['UI_JS_MaxExportCountMessage'];

    if (selectedDocsIds.length + 1 > MAX_SELECTED_DOCUMENTS) {
        notify(exportLimitTitle,
      exportLimitMessage + MAX_SELECTED_DOCUMENTS,
      'warn');
    }
    else {
        selectedDocsIds.push(curItemId);
        $('#btn-clear-export-list').fadeIn(200);

        refreshExportLinks();

        divToColor.parent().css('background-color', '#F0F8FB');
        divToColor.parent().children('.doc-keywords').children('.doc-tab-container').children('.not-active-link-keywords').css('background-color', '#F0F8FB');
        divToColor.parent().children('.doc-summary').children('.doc-tab-container').children('.not-active-link-summaries').css('background-color', '#F0F8FB');
        divToColor.parent().children('.doc-summary').children('.doc-tab-container').children('.not-active-link-summaries').css('border-bottom', '3px solid #F0F8FB');
        divToColor.parent().children('.doc-keywords').children('.doc-tab-container').children('.not-active-link-keywords').css('border-bottom', '3px solid #F0F8FB');

        checkbox.addClass('checkbox-document-selected');
        checkbox.removeClass('checkbox-document');
        checkbox.unbind();
        checkbox.click(checkboxDocumentSelectedClick);
    }
}

function checkboxDocumentSelectedClick(e) {
    var checkbox = $(this),
        curItemId = checkbox.data('itemid'),
        divToColor = $(e.target).parent().next(),
        index = $.inArray(curItemId, selectedDocsIds);
 
    if (index >= 0) selectedDocsIds.splice(index, 1);
    refreshExportLinks();

    if (selectedDocsIds.length <= 0) {
        $('#btn-clear-export-list').fadeOut(200);
    }

    divToColor.parent().css('background-color', 'white');
    divToColor.parent().children('.doc-keywords').children('.doc-tab-container').children('.not-active-link-keywords').css('background-color', 'white');
    divToColor.parent().children('.doc-keywords').children('.doc-tab-container').children('.not-active-link-keywords').css('border-bottom', '3px solid white');
    divToColor.parent().children('.doc-summary').children('.doc-tab-container').children('.not-active-link-summaries').css('background-color', 'white');
    divToColor.parent().children('.doc-summary').children('.doc-tab-container').children('.not-active-link-summaries').css('border-bottom', '3px solid white');

    checkbox.addClass('checkbox-document');
    checkbox.removeClass('checkbox-document-selected');
    checkbox.unbind();
    checkbox.click(checkboxDocumentClick);
}

function refreshExportLinks() {
    var pdfLink = $('#pdf-export'),
        rtfLink = $('#rtf-export'),
        hrefStart = '/Export/ExportMultiDocs?idsString=',
        idsJoined = selectedDocsIds.join('-'),
        newHref = hrefStart.concat(idsJoined);

    newPdfHref = newHref.concat('&type=pdf');
    newRtfHref = newHref.concat('&type=rtf');

    pdfLink.attr('href', newPdfHref);
    rtfLink.attr('href', newRtfHref);
}