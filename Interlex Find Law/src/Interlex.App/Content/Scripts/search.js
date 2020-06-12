function Search() {
    $.ajax({
        url: appRootFolder + "/Search/DocList/Search-" + searchId + '/Page1',
        type: 'GET',
        success: function (result) {
            $("#dSearchResult").html(result);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $("#dSearchResult").html(thrownError);
        }
    });
}

function ReSearch(pageNo, refreshFilters) {
    listLoadMsg($("#doc_list"));
    sort = $('#sort').val();
    sort_dir = $('#sort_dir').data('dir');
    $.ajax({
        url: appRootFolder + "/Search/DocList/Search-" + searchId + "/Page" + pageNo + "/" + sort + "/" + sort_dir,
        type: 'GET',
        success: function (result) {

            if (result.length > 23) { // status:unauth
                $("#doc_list").html(result);
                if (refreshFilters) {
                    populateFilters();
                }
                /*  $('input').iCheck({
                      checkboxClass: 'icheckbox_square-blue'
                  });*/
                //refreshPagerView(pageNo);
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $("#doc_list").html(thrownError);
        }
    });
}

function listLoadMsg(obj) {
    var loadingResource = Resources.storage["UI_JS_Loading"];
    obj.html('<span class="f-blue f-0-8">' + loadingResource + '...</span>');
}

function changeSortDir(fromRecentDocs) {
    if ($('#sort_dir').data('dir') === 'asc' || $('#sort_dir').data('dir') === '') {
        $('#sort_dir').data('dir', 'desc');

        $('#chevron-sort').addClass('chevron-sort-down');
        $('#chevron-sort').removeClass('chevron-sort-up');
    }
    else {
        $('#sort_dir').data('dir', 'asc');

        $('#chevron-sort').addClass('chevron-sort-up');
        $('#chevron-sort').removeClass('chevron-sort-down');
    }

    if (fromRecentDocs === true) {
        orderDirFilter($('#sort_dir').data('dir'));
    }
    else {
        goToPage(1);
    }
}

//function refreshPagerView(page) {
//    index = parseInt(page) - 1; // eq() expect zero-based index
//    $('ul.jPag-pages').each(function () {
//        $(this).find('li:eq(' + index + ') > a').trigger("click", false);
//    });
//}


function goToPage(page) {
    ReSearch(page);
    //$('#go-to-page-input-top').val('');
    //$('#go-to-page-input-bottom').val('');

    //index = parseInt(page) - 1; // eq() expect zero-based index
    //a = $('ul.jPag-pages > li:eq(' + index + ') > a');
    //refreshPage = true;
    ////refreshViewOnly = (refreshViewOnly !== 'undefined' && refreshViewOnly == true) ? false : true;
    //if (a.length > 0) {
    //    $('ul.jPag-pages').each(function () {
    //        //TODO: check why this is not invoked
    //        $(window).ScrollToNoOffset(); //positioning again on top

    //        $(this).find('li:eq(' + index + ') > a').trigger("click", refreshPage);
    //        refreshPage = false;
    //    });
    //}
    //else {
    //    ReSearch(page);
    //}
}


function goToExactPage(pager, maxPage) {
    var desiredPage;
    if (pager === 'top') {
        desiredPage = $('#go-to-page-input-top').val();

    }
    else if (pager === 'bottom') {
        desiredPage = $('#go-to-page-input-bottom').val();
    }
    else {
        console.log('invalid pager'); //consider stopping program flow
    }

    if (desiredPage !== null && desiredPage !== undefined && desiredPage !== "" && isNaN(desiredPage) === false && desiredPage > 0) {
        if (desiredPage > maxPage) {
            goToPage(maxPage);
            notify('There is no such page', 'The desired page does not exist. You were redirected to the last one found.', 'warn');
        }
        else {
            goToPage(desiredPage);
        }
    }
}

function clearEnactmentModal() {
    $('#PAL_Tree .fancytree-selected').removeClass('fancytree-selected'); // I am sorry, JavaScript <3
    $('#PAL_Tree .fancytree-active').removeClass('fancytree-active');
    $('#pal_docs_list').html('');
    $('#pal_pages_top').html('');
    $('#input-enactment-search').val('');
    $('#input-enactment-number').val('');
    $('#input-enactment-year').val('');
    $('#input-enactment-celex').val('');
}