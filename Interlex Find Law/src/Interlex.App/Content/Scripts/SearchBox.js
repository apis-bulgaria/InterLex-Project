function treeDeselAll(treeId) {
    if ($(treeId).html() != '' && $(treeId).html() !== undefined) {
        $(treeId).fancytree("getRootNode").visit(function (node) {
            if (node.selected) {
                node.setSelected(false);
            }
        });
    }
}

function clearTreeFilter(prefixName) {
    $('#' + prefixName + '_SelectedIds').val('');
    treeDeselAll('#' + prefixName + '_Tree');
    $('#tb' + prefixName + 'Text').val('');
    $('#tb' + prefixName + 'Text').attr('title', '');
}

var trimSize = 200;
function trimFilterTitle(title) {
    if (title.length > trimSize) {
        return title.substr(0, trimSize) + '...';
    }
    return title;
}

function setFilterDataToForm(prefixName, titles, delim) {
    if (titles.length > 0) {
        if (titles[titles.length - 1] == delim)
            titles = titles.substr(0, titles.length - 1);
        $('#tb' + prefixName + 'Text').val(trimFilterTitle(titles.replace(/\|/g, ", ")));
        $('#tb' + prefixName + 'Text').attr('title', titles.replace(/\|/g, "\n")); // \n - За да се показва всяко на нов ред
    }
    else {
        $('#tb' + prefixName + 'Text').val('');
        $('#tb' + prefixName + 'Text').attr('title', '');
    }
}

function loadFilterDataTitles(prefixName, clientSelectedIds) {
    $.ajax({
        type: "POST",
        dataType: 'json',
        url: appRootFolder + "/api/Entity/GetSelectedTitlePaths",
        data: { clientSelectedIds: clientSelectedIds }
    }).done(function (data) {
        setFilterDataToForm(prefixName, data, '|');
    });
}

function loadError(e, data) {
    UnAuthJsonCheck(data);
}

function GetTree(prefixName, searchId, keyPaths, selectedIds) {
    var productId = getCookie('SelectedProductId');

    caseLawArea = '0';//all
    caseLawArea = $('input[name="Cases.CaseLawType"]:checked').val();
    var legislationArea = '-1';//invalid
    legislationArea = $('input[name="Law.LegislationType"]:checked').val();
    if (prefixName == 'Cases_CourtsFolders' && $('input[name="Cases.CaseLawType"]:checked').length == 1) {
        if ($("#" + prefixName + "_Tree").html() != '') {
            if ($("#" + prefixName + "_Tree").fancytree('getNodeByKey', '7eff6a8f-2a80-4128-a3d6-16687a5a54ae') != null) // European Union
                tree_caseLawArea = 0;
            else if ($("#" + prefixName + "_Tree").fancytree('getNodeByKey', '83e31960-fece-4e9f-8758-511fe0e58a07') != null) // Austria
                tree_caseLawArea = 2;
            else
                tree_caseLawArea = 1; // search in EU Case Law
            if (tree_caseLawArea != caseLawArea)
                $("#" + prefixName + "_Tree").html('');
        }
    }

    caseLawDirCodesFull = 0;
    if ($('#Cases_DirectoryCaseLawFull').is(":checked"))
        caseLawDirCodesFull = 1;


    if ($("#" + prefixName + "_Tree").html() == '') {
        $("#" + prefixName + "_Tree").remove();
        var newDiv = document.createElement("div");
        newDiv.id = prefixName + "_Tree";
        $("#" + prefixName + "_Container").append(newDiv);

        // Used for WebApi method name below
        getMethodName = prefixName.substr(prefixName.indexOf('_') + 1);


        searchParam = '';
        if (searchId != null)
            searchParam = '/' + searchId;
        tab = prefixName.substr(0, prefixName.indexOf('_'));

        //console.log('/api/Entity/GetClassifier/' + getMethodName + '/null/' + tab + searchParam);
        tree = $("#" + prefixName + "_Tree").fancytree(
            {
                source:
                {
                    type: 'POST',
                    dataType: 'json',
                    url: appRootFolder + '/api/Entity/GetClassifier/' + getMethodName + '/null/' + tab + searchParam,
                    data: {
                        selectedIds: selectedIds,
                        caseLawArea: caseLawArea,
                        legislationArea: legislationArea,
                        caseLawDirCodesFull: caseLawDirCodesFull,
                        productId: productId
                    },
                    success: function (data) {
                        // if no proper tree data is returned fancytree fire error and global event ajaxComplete is not fired. This is used to check if user has rights
                        console.log(data);
                        UnAuthJsonCheck(data);
                    },
                    cache: false
                },
                lazyLoad: function (event, data) {
                    var node = data.node;
                    // Load child nodes via ajax GET /getTreeData?mode=children&parent=1234
                    data.result = {
                        type: 'POST',
                        dataType: 'json',
                        url: appRootFolder + '/api/Entity/GetClassifier/' + getMethodName + '/' + node.key + '/' + tab + searchParam,
                        data: {
                            selectedIds: selectedIds,
                            caseLawArea: caseLawArea
                        },
                        success: function (data) {
                            // if no proper tree data is returned fancytree fire error and global event ajaxComplete is not fired. This is used to check if user has rights
                            UnAuthJsonCheck(data);
                        },
                        cache: false
                    };
                },
                loadError: loadError,
                loadChildren: function (event, ctx) {
                    // if parent node is checked all loaded children should be checked
                    if (ctx.node.selected)
                        ctx.node.fixSelection3AfterClick();
                    else // if not, loading state from session and fix parent selected state
                    {
                        $("#" + prefixName + "_Tree").fancytree("getRootNode").fixSelection3FromEndNodes();
                    }
                },
                checkbox: true,
                selectMode: 3, //three state checkboxes
                clickFolderMode: 2,
                init: function (event, data) {
                    treeLoadKeyPath(prefixName, keyPaths);

                },
                // Check item on title click
                click: function (event, data) {
                    if (data.targetType == 'title')
                        data.node.toggleSelected();
                },
                icons: false
            }
        );
    }
}

function OnTreeOK(prefixName) {
    $('#tb' + prefixName + 'Text').val('');
    titles = '';
    ids = '';

    $("#" + prefixName + "_Tree").fancytree("getRootNode").visit(function (node) {
        if (node.selected && node.hideCheckbox == false && (node.getParent().parent == null || node.getParent().selected == false)) {
            title = node.title;
            n = node.getParent();
            while (n != null && n.parent != null) {
                title = n.title + ' / ' + title;
                n = n.getParent();
            }
            titles += title + '|';
            ids += node.data.id + ',';
        }
    });
    //alert('titles:'+titles);
    if (ids != '')
        ids = ids.substr(0, ids.length - 1);
    $('#' + prefixName + '_SelectedIds').val(ids).trigger('change');
    setFilterDataToForm(prefixName, titles, '|');
    $('#div' + prefixName).dialog('close');
}

function ShowPAL_Tree(productId) {
    console.log(appRootFolder);

    if ($("#PAL_Tree").html() == '') {
        if (!productId || productId === 1) {
            $("#PAL_Tree").fancytree(
                {
                    source:
                    {
                        type: 'POST',
                        dataType: 'json',
                        url: appRootFolder + '/api/Entity/GetClassifier/DocumentTypes/null/PAL',
                        data: {
                            //selectedIds: selectedIds,
                            //caseLawArea: caseLawArea,
                            //caseLawDirCodesFull: caseLawDirCodesFull
                        },
                        success: function (data) {
                            // if no proper tree data is returned fancytree fire error and global event ajaxComplete is not fired. This is used to check if user has rights
                            UnAuthJsonCheck(data);
                        },
                        cache: false
                    },
                    lazyLoad: function (event, data) {
                        var node = data.node;
                        // Load child nodes via ajax GET /getTreeData?mode=children&parent=1234
                        data.result = {
                            type: 'POST',
                            dataType: 'json',
                            url: appRootFolder + '/api/Entity/GetClassifier/DocumentTypes/' + node.key + '/PAL',
                            data: {
                                //selectedIds: selectedIds,
                                //caseLawArea: caseLawArea
                            },
                            success: function (data) {
                                // if no proper tree data is returned fancytree fire error and global event ajaxComplete is not fired. This is used to check if user has rights
                                UnAuthJsonCheck(data);
                            },
                            cache: false
                        };
                    },
                    loadError: loadError,
                    loadChildren: function (event, ctx) {
                        // if parent node is checked all loaded children should be checked
                        //if (ctx.node.selected)
                        //    ctx.node.fixSelection3AfterClick();
                        //else // if not, loading state from session and fix parent selected state
                        //{
                        //    $("#" + prefixName + "_Tree").fancytree("getRootNode").fixSelection3FromEndNodes();
                        //}
                    },
                    checkbox: false,
                    selectMode: 1, //three state checkboxes
                    select: function (event, data) {
                        // Display list of selected nodes
                        var s = data.tree.getSelectedNodes();
                        if (s.length == 1) {
                            $("#hClassifierId").val(s[0].data.id);
                            $("#input-enactment-search").val('');
                            $("#input-enactment-number").val('');
                            $("#input-enactment-year").val('');
                            $("#input-enactment-celex").val('');

                            $('#modal-choose-provision').html('');

                            palSearch();
                        }
                        else {
                            $("#hClassifierId").val('');
                        }
                    },
                    clickFolderMode: 3,
                    init: function (event, data) {
                        $("#PAL_Tree").fancytree("getTree").visit(function (node) {
                            //console.log('node');
                            //console.log(node);
                            if (node.selected) {
                                // not hacks at all
                                node.toggleSelected();
                                node.toggleSelected();
                            }

                        });
                        //treeLoadKeyPath(prefixName, keyPaths);

                    },
                    // Check item on title click
                    click: function (event, data) {
                        if (data.targetType == 'title')
                            data.node.toggleSelected();

                    },
                    icons: false,
                    classNames: {
                        active: "pal_note_selected"
                    }
                }
            );
        } else {
            $("#PAL_Tree").fancytree({
                source: [
                    {
                        title: Resources.storage['UI_JS_FinStandarts'],
                        data: {
                            id: 'f87f25b58de34f68b2fe7670aea7cc3c'
                        }
                    },
                    {
                        title: Resources.storage['UI_JS_Reglaments'],
                        data: {
                            query: '+props:dtact +props:baseact +props:ceu +docnumber:3????r*'
                        }
                    },
                    {
                        title: Resources.storage['UI_JS_Directives'],
                        data: {
                            query: '+props:dtact +props:baseact +props:ceu +docnumber:3????l*'
                        }
                    },
                ],
                select: function (event, data) {
                    // Display list of selected nodes
                    var s = data.tree.getSelectedNodes();
                    if (s.length == 1) {
                        $("#hClassifierId").val(s[0].data.id);
                        $("#hPalQuery").val(s[0].data.query);
                        $("#input-enactment-search").val('');
                        $("#input-enactment-number").val('');
                        $("#input-enactment-year").val('');
                        $("#input-enactment-celex").val('');

                        palSearch();
                    }
                    else {
                        $("#hClassifierId").val('');
                        $("#hPalQuery").val('');
                    }

                },
                loadError: loadError,
                checkbox: false,
                selectMode: 1, //three state checkboxes
                clickFolderMode: 3,
                click: function (event, data) {
                    if (data.targetType == 'title')
                        data.node.toggleSelected();

                },
                icons: false,
                classNames: {
                    active: "pal_note_selected"
                }
            });
        }
    }
}

var palClassifierId = '';
var palSearchText = '';
var palSearchNumber = '';
var palSearchYear = '';
var palDocNumber = '';
var palQuery = '';
var palPage = 1;

function palDoc(obj, doc_lang_id, celex, isFromIndex, productId) {
    var suffix = '',
        prefix;

    //if (isFromIndex === true) {
    //    suffix = '_Index';
    //}

    if (productId === 1) {
        prefix = 'Cases';
    } else {
        prefix = 'Finances';
    }

    $('#' + prefix + '_EnactmentText' + suffix).val($(obj).text());
    $('#' + prefix + '_EnactmentText' + suffix).attr('title', $(obj).text());
    $('#' + prefix + '_EnactmentDocLangId' + suffix).val(doc_lang_id).trigger('change');
    $('#' + prefix + '_EnactmentCelex' + suffix).val(celex).trigger('change');

    $('#' + prefix + '_ProvisionText' + suffix).val(Resources.storage["UI_JS_AllProvisions"]);
    $('#' + prefix + '_ProvisionText' + suffix).attr('title', Resources.storage["UI_JS_AllProvisions"]);
    $('#' + prefix + '_ProvisionId' + suffix).val('');

    $('#list-enactment-choose li').removeClass('selected');

    let $liParent = $(obj).parents('li');
    $liParent.addClass('selected');

    if (!isFromIndex) {
        $('#link-choose-enactment-index').val($(obj).text());
        $('#link-choose-enactment-index').attr('title', $('#link-choose-enactment-index').val());
    }

    showProvisionDlg(isFromIndex);

    //  $('#modal-choose-enactment').dialog('close');
}

function palProvision(obj, provisionId, parOriginal, isFromIndex, productId) {
    var suffix = '';
    if (isFromIndex === true) {
        suffix = '_Index';
    }

    if (productId === 1) {
        prefix = 'Cases';
    } else {
        prefix = 'Finances';
    }

    $('#' + prefix + '_ProvisionText' + suffix).val($(obj).text());
    $('#' + prefix + '_ProvisionText' + suffix).attr('title', $(obj).text());
    $('#' + prefix + '_ProvisionId' + suffix).val(provisionId).trigger('change');
    $('#' + prefix + '_ProvisionParOriginal' + suffix).val(parOriginal).trigger('change');

    $('#list-provision-choose li').removeClass('selected');

    let $liParent = $(obj).parents('li');
    $liParent.addClass('selected');

    if (!isFromIndex) {
        $('#link-choose-enactment-index').val($('#link-choose-enactment-index').val() + ' ' + $(obj).text());
        $('#link-choose-enactment-index').attr('title', $('#link-choose-enactment-index').val());
    }

    //  $('#modal-choose-provision').dialog('close');
}

function palSearch() {
    //console.log($("#hClassifierId").val());

    //save search values
    palClassifierId = $("#hClassifierId").val();
    palSearchText = $("#input-enactment-search").val();
    palSearchNumber = $("#input-enactment-number").val();
    palSearchYear = $("#input-enactment-year").val();
    palDocNumber = $("#input-enactment-celex").val();
    palQuery = $("#hPalQuery").val();

    if (palClassifierId != '' || palSearchText != '' || palSearchNumber != '' || palSearchYear != '' || palDocNumber != '' || palQuery != '') {

        $('#pal_pages_top').html('');
        var loadingResource = Resources.storage["UI_JS_Loading"] + '...';
        $("#pal_docs_list").html(loadingResource);

        $.ajax({
            url: appRootFolder + '/Search/PalSearchTotalCount/',
            type: 'GET',
            data: {
                classifierId: palClassifierId,
                searchText: palSearchText,
                searchNumber: palSearchNumber,
                searchYear: palSearchYear,
                searchDocNumber: palDocNumber,
                query: palQuery
            }
        }).done(function (totalCount) {
            if (totalCount == 0) {
                $('#pal_pages_top').html(Resources.storage['UI_JS_NoDocumentsFound']);
                $("#pal_docs_list").html('');
            }
            else {
                $("#pal_pages_top").paging(totalCount, {
                    format: '[< ncnnn >]',
                    perpage: 20,
                    onSelect: function (page) {
                        palDocList(page);

                        $('.enactment-choose-current-page').removeClass('enactment-choose-current-page');
                        $('#pal_pages_top a[data-page="' + page + '"].pager-not-control').addClass('enactment-choose-current-page');
                    },
                    onFormat: function (type) {
                        switch (type) {
                            case 'block': // n and c
                                return '<a href="javascript:void(true);" class="f-0-8 pager-not-control">' + this.value + '</a>&nbsp;';
                            case 'next': // >
                                // return '<a>&gt;</a>';
                                return '<a>›</a>';
                            case 'prev': // <
                                //  return '<a>&lt;</a>';
                                return '<a>‹</a>';
                            case 'first': // [
                                return '<a>«</a>';
                            case 'last': // ]
                                return '<a>»</a>';
                        }
                    }
                });
            }
        }).fail(function () {
            $("#pal_docs_list").html('Error retrieving data.');
        });
    }
    else {
        $("#pal_docs_list").html(Resources.storage['UI_JS_Pal_NoSearchCriteria']);
    }
}

function palDocList(page, isIndexSearch) {
    palPage = page;
    if (palClassifierId != '' || palSearchText != '' || palSearchNumber != '' || palSearchYear != '' || palDocNumber != '' || palQuery != '') {
        var loadingResource = Resources.storage["UI_JS_Loading"] + '...';
        $("#pal_docs_list").html(loadingResource);
        var isIndexSearch = parseInt($('#modal-choose-enactment').attr('data-index-search'));
        if (isIndexSearch === 1) {
            isIndexSearch = true;
        }
        else {
            isIndexSearch = false;
        }

        $.ajax({
            url: appRootFolder + '/Search/PalSearch/',
            type: 'POST',
            data: {
                classifierId: palClassifierId,
                searchText: palSearchText,
                searchNumber: palSearchNumber,
                searchYear: palSearchYear,
                searchDocNumber: palDocNumber,
                page: palPage,
                isIndexSearch: isIndexSearch,
                query: palQuery
            }
        }).
            done(function (data) {
                $('#link-choose-enactment-index').val('');
                $('#link-choose-enactment-index').attr('title', '');
                $("#pal_docs_list").html(data);
            });
    }
}

function loadPalProvisions(doc_lang_id, isIndexSearch) {
    var loadingResource = Resources.storage["UI_JS_Loading"] + '...';
    $("#modal-choose-provision").html(loadingResource);

    $.ajax({
        url: appRootFolder + '/Search/PalProvisions/',
        type: 'POST',
        data: {
            docLangId: doc_lang_id,
            isIndexSearch: isIndexSearch
        }
    }).done(function (data) {
        $("#modal-choose-provision").html(data);
    });
}

var referedActApplicant = '';
var referedActApplicationNumber = '';
var referedActECLI = '';

function referedActECHRSearch() {
    //console.log($("#hClassifierId").val());

    //save search values
    referedActApplicant = $('#echr-refered-act-applicant').val();
    referedActApplicationNumber = $('#echr-refered-act-application-number').val();
    referedActECLI = $('#echr-refered-act-ecli').val();

    if (referedActApplicant !== '' || referedActApplicationNumber !== '' || referedActECLI !== '') {
        $('#refered-act-pager').html('');
        var loadingResource = Resources.storage["UI_JS_Loading"] + '...';
        $("#refered-act-search-result").html(loadingResource);

        $.ajax({
            url: appRootFolder + '/Search/ReferedActECHRSearchCount/',
            type: 'GET',
            data: {
                applicant: referedActApplicant,
                applicationNumber: referedActApplicationNumber,
                ecli: referedActECLI
            }
        }).done(function (totalCount) {
            if (totalCount == 0) {
                $('#refered-act-pager').html(Resources.storage['UI_JS_NoDocumentsFound']);
                $("#refered-act-search-result").html('');
            }
            else {
                $("#refered-act-pager").paging(totalCount, {
                    format: '[< ncnnn >]',
                    perpage: 20,
                    onSelect: function (page) {
                        referedActECHRSearchList(page);

                        $('.refered-act-choose-current-page').removeClass('refered-act-choose-current-page');
                        $('#refered-act-pager a[data-page="' + page + '"].pager-not-control').addClass('refered-act-choose-current-page');
                    },
                    onFormat: function (type) {
                        switch (type) {
                            case 'block': // n and c
                                return '<a href="javascript:void(true);" class="f-0-8 pager-not-control">' + this.value + '</a>&nbsp;';
                            case 'next': // >
                                // return '<a>&gt;</a>';
                                return '<a>›</a>';
                            case 'prev': // <
                                //  return '<a>&lt;</a>';
                                return '<a>‹</a>';
                            case 'first': // [
                                return '<a>«</a>';
                            case 'last': // ]
                                return '<a>»</a>';
                        }
                    }
                });
            }
        }).fail(function () {
            $("#refered-act-search-result").html('Error retrieving data.');
        });
    }
    else {
        $("#refered-act-search-result").html('No search criteria');
    }
}

function referedActECHRSearchList(page) {
    palPage = page;
    if (referedActApplicant !== '' || referedActApplicationNumber !== '' || referedActECLI !== '') {
        var loadingResource = Resources.storage["UI_JS_Loading"] + '...';
        $("#refered-act-search-result").html(loadingResource);
        $.ajax({
            url: appRootFolder + '/Search/ReferedActECHRSearch/',
            type: 'POST',
            data: {
                applicant: referedActApplicant,
                applicationNumber: referedActApplicationNumber,
                ecli: referedActECLI,
                page: palPage
            }
        }).done(function (data) {
            $("#refered-act-search-result").html(data);
        });
    }
}

function referedActECHRChoose(callElement, docLangId) {
    // populating echr type
    $('#Cases_ECHRReferedActType').val(3);
    // populating doc lang id of the act to be later sent to model
    $('#Cases_ReferedActECHRDocLangId').val(docLangId);

    // populating title text in box
    var elementText = $(callElement).text();
    $('#input-refered-act').val(elementText);
    $('#Cases_ReferedActTitle').val(elementText);

    // disabling provision choosement
    $('.refered-act-provision-input-container input').val('');

    // showing correct privison div
    $('#cases-echr-refered-act-provision').removeClass('display-none');
    $('#cases-echr-court-rules').addClass('display-none');
    $('#cases-echr-hudoc-articles').addClass('display-none');

    // clearing possibly populated values by the classifiers
    if ($('#tbCases_HudocArticlesText').length) {
        clearTreeFilter('Cases_HudocArticles');
    }

    if ($('#tbCases_RulesOfTheCourtText').length) {
        clearTreeFilter('Cases_RulesOfTheCourt');
    }

    $('#Cases_ECHRReferedActType').trigger('change');

    // closing modal
    $('#modal-choose-refered-act').dialog('close');
}

function validateSearch(activeTab) {
    var isValid = false;

    if ($('#SearchText').val().trim() != '')
        return true;

    switch (activeTab) {
        case 'simple':
            if ($('#MultiDict_Text').val().trim() !== '') {
                console.log('..invoking valid set shiet');
                isValid = true;
            }
            break;
        case 'tabCases':
            $('#formCases .check_tree_ids, #formCases .text_field, #Cases_EnactmentText, #formCases .text-box, #Cases_ReferedActTitle, #Cases_MultiDict_Text').each(function () {
                if ($(this).val() != '') {
                    isValid = true;
                    return false; //loop break
                }
            });

            if (isValid === false) {
                if ($('#SearchText').val().trim() != '') {
                    DoSearch('simple');
                }
            }

            break;
        case 'tabLaw':
            $('#formLaw .check_tree_ids, #formLaw .text_field, #formLaw .text-box, #formLaw .ftext_field, #Law_MultiDict_Text').each(function () {
                if ($(this).val() != '') {
                    isValid = true;
                    return false; //loop break
                }
            });

            if (isValid === false) {
                if ($('#SearchText').val().trim() != '') {
                    DoSearch('simple');
                }
            }

            break;

        case 'tabFinances':
            $('#formFinances .check_tree_ids, #formFinances .text_field, #formFinances .text-box, #formFinances .ftext_field, #Finances_EnactmentText').each(function () { // Added Enactment
                if ($(this).val() != '') {
                    isValid = true;
                    return false; //loop break
                }
            });

            $('#finances-document-type-container label input[type="checkbox"]').each(function () {
                if ($(this).prop('checked') === true) {
                    isValid = true;
                    return false;
                }
            });

            if (isValid === false) {
                if ($('#SearchText').val().trim() != '') {
                    DoSearch('simple');
                }
            }

            break;
    }
    return isValid;
}

function DoSearch(activeTab) {
    $('#Cases_IsIndexEnactmentSearch').val('False'); // checked later

    switch (activeTab) {
        case 'simple': // no tabs used
            if (validateSearch(activeTab)) {
                console.log('valid shiet');
                $.post(appRootFolder + "/Search/Search", $("#SearchForm").serialize(), function (response) {
                    if (response === "Error") {
                        notify(Resources.storage["UI_JS_SearchError"], Resources.storage["UI_JS_SearchErrorText"], 'warn');
                    }
                    else if (response.result == 'Redirect') {
                        window.location = response.url;
                    }
                });
            }
            break;
        case 'tabCases': // Cases
            if (validateSearch(activeTab)) {
                $.post(appRootFolder + "/Search/Search", $("#formCases").serialize() + '&' + $('#SearchText').serialize(), function (response) {
                    if (response === "Error") {
                        notify(Resources.storage["UI_JS_SearchError"], Resources.storage["UI_JS_SearchErrorText"], 'warn');
                    }
                    else if (response.result == 'Redirect') {
                        window.location = response.url;
                    }
                });
            } else {
                // var oneSearchDetails = Resources.getJSResource('OneSearchDetail');
                $('#adv-search-notifications-cases').text(Resources.storage["UI_JS_OneSearchDetail"]);

                setTimeout(function () {
                    $('#adv-search-notifications-cases').html('&nbsp;');
                }, 4000);
            }
            break;
        case 'tabLaw': // Law
            if (validateSearch(activeTab)) {
                $.post(appRootFolder + "/Search/Search", $("#formLaw").serialize() + '&' + $('#SearchText').serialize(), function (response) {
                    if (response === "Error") {
                        notify(Resources.storage["UI_JS_SearchError"], Resources.storage["UI_JS_SearchErrorText"], 'warn');
                    }
                    else if (response.result == 'Redirect') {
                        window.location = response.url;
                    }
                });
            } else {
                $('#adv-search-notifications-law').text(Resources.storage["UI_JS_OneSearchDetail"]);

                setTimeout(function () {
                    $('#adv-search-notifications-law').html('&nbsp;');
                }, 4000);
            }
            break;
        case 'tabFinances': // Finances
            if (validateSearch(activeTab)) {
                console.log($('#formFinances').serialize());
                var docTypeCheckboxes = $('#finances-document-type-container label input[type="checkbox"]');

                $.post(appRootFolder + "/Search/Search", $("#formFinances").serialize() + '&' + $('#SearchText').serialize() + '&' + docTypeCheckboxes.serialize(), function (response) {
                    if (response === "Error") {
                        notify(Resources.storage["UI_JS_SearchError"], Resources.storage["UI_JS_SearchErrorText"], 'warn');
                    }
                    else if (response.result == 'Redirect') {
                        window.location = response.url;
                    }
                });
            } else {
                $('#adv-search-notifications-finances').text(Resources.storage["UI_JS_OneSearchDetail"]);

                setTimeout(function () {
                    $('#adv-search-notifications-finances').html('&nbsp;');
                }, 4000);
            }
            break;
        case 'enactmentIndex':
            console.log($('#Cases_EnactmentText').val());
            if ($('#Cases_EnactmentText').val() !== undefined && $('#Cases_EnactmentText').val() !== null && $('#Cases_EnactmentText').val() !== '') {
                let enactmentTextMemo = $('#Cases_EnactmentText').val(),
                    enactmentDocLangIdMemo = $('#Cases_EnactmentDocLangId').val(),
                    enactmentCelexMemo = $('#Cases_EnactmentCelex').val(),
                    provisionTextMemo = $('#Cases_ProvisionText').val(),
                    provisionIdMemo = $('#Cases_ProvisionId').val(),
                    provisionParIdOriginalMemo = $('#Cases_ProvisionParOriginal').val();

                clearAdvSearchFilters('tabCases');

                $('#Cases_EnactmentText').val(enactmentTextMemo);
                $('#Cases_EnactmentDocLangId').val(enactmentDocLangIdMemo);
                $('#Cases_EnactmentCelex').val(enactmentCelexMemo);
                $('#Cases_ProvisionText').val(provisionTextMemo);
                $('#Cases_ProvisionId').val(provisionIdMemo);
                $('#Cases_ProvisionParOriginal').val(provisionParIdOriginalMemo);

                $('#Cases_IsIndexEnactmentSearch').val('True');

                $.post("/Search/Search", $("#formCases").serialize() + '&' + $('#SearchText').serialize(), function (response) {
                    if (response === "Error") {
                        notify(Resources.storage["UI_JS_SearchError"], Resources.storage["UI_JS_SearchErrorText"], 'warn');
                    }
                    else if (response.result == 'Redirect') {
                        window.location = response.url;
                    }
                });
            }
            else {
                notify(Resources.storage["UI_JS_SearchError"], Resources.storage["UI_JS_OneSearchDetail"], 'warn');
            }
            break;
    }
}

function closeDropdown(obj, e) {
    if (obj.id) {
        var isActive = $('#' + obj.id + '[data-toggle="dropdown"]').parent().hasClass('open');
        $('[data-toggle="dropdown"]').parent().removeClass('open');
        if (isActive) {
            e.stopPropagation();
        }
    }
    else {
        $('[data-toggle="dropdown"]').parent().removeClass('open');
    }
}

$('.dropdown-modal').on('hide.bs.dropdown', function (e) {
    return false;
});

function treeLoadKeyPath(prefix, keyPaths) {
    if (keyPaths != undefined) {
        $("#" + prefix + "_Tree").fancytree("getTree").loadKeyPath(keyPaths, function (node, status) {
            if (status === "loaded") {
                //console.log("loaded intermiediate node " + node);
                //node.setExpanded(true);
                //node.setSelected(true);
            } else if (status === "ok") {
                node.setActive(true);
            }
        });
    }
}

function tog(v) { return v ? 'addClass' : 'removeClass'; }
$(document).on('input', '.clearable-new', function () {
    $(this)[tog(this.value)]('x');
}).on('mousemove', '.x', function (e) {
    $(this)[tog(this.offsetWidth - 18 < e.clientX - this.getBoundingClientRect().left)]('onX');
}).on('touchstart click', '.onX', function (ev) {
    ev.preventDefault();
    $(this).removeClass('x onX').val('').change();
});

function loadMySearch(id, async, deletePrev, globalId, siteSearchId) {
    var searchType = 'simple';

    $.ajax({
        type: "POST",
        async: async,
        url: appRootFolder + "/User/MySearchesView",
        traditional: true,
        data: { viewId: id, globalId: globalId, siteSearchId: siteSearchId }
    }).done(function (data) {
        var result, inSession;

        if (typeof data !== 'string') {
            //IN SESSION LOAD
            result = data;
            inSession = true;
        }
        else {
            //OUT OF SESSION LOAD
            result = $.parseJSON(data);
            inSession = false;
        }

        console.log(result);

        // Product 2
        if (result.Finances != null) {
            // Clear all Finances filters
            clearAdvSearchFilters('tabFinances');
        }
        else { // Product 1
            // Clear all Law filters
            clearAdvSearchFilters('tabLaw');
            // Clear all Cases filters
            clearAdvSearchFilters('tabCases');
        }

        if (result.SearchText != null)
            $('#SearchText').val(result.SearchText);


        // Multilingual Dictionary
        if (typeof result.MultiDict !== 'undefined' && result.MultiDict !== null && result.MultiDict.SelectedIds != null && result.MultiDict.SelectedIds !== '') {
            console.log(result.MultiDict);
            $('#MultiDict_SelectedIds').val(result.MultiDict.SelectedIds);
            $('#MultiDict_SelectedIds').trigger('change');
            setCookie('selectedMultiDictIds', result.MultiDict.SelectedIds);

            if (result.MultiDict.LogicalType == true) { // Refreshing "And" radio button
                $('#rd-multilingual-dictionary-and').prop('checked', true);
                $('#rd-multilingual-dictionary-or').removeAttr('checked');
            }
            else { // Refreshing "Or" radio button
                $('#rd-multilingual-dictionary-or').prop('checked', true);
                $('#rd-multilingual-dictionary-and').removeAttr('checked');
            }

            $('#tb-multilingual-dictionary').val(result.MultiDict.Text);
            $('#tb-multilingual-dictionary').attr('title', result.MultiDict.Text);
            $('#MultiDict_Text').val(result.MultiDict.Text);
            $('#MultiDict_Text').trigger('change'); // for clear button to be shown

            selectedMultiDictIds = JSON.parse(result.MultiDict.SelectedIds);
            selectedQueryTexts = JSON.parse(result.MultiDict.SelectedQueryTexts);

            populateBuildedQuery();

            $('#multi-dict-main-trigger').trigger('click');
        }


        //Populating Law tab
        if (result.Law != null) {
            searchType = 'tabLaw';

            //Populating boolean checkboxes
            if (result.Law.OnlyInTitles === true) {
                $('#Law_OnlyInTitles').prop('checked', true);
            }

            if (result.Law.TranslateSearchText === true) {
                $('#Law_TranslateSearchText').prop('checked', true);
            }

            if (result.Law.OnlyInActualActs === true) {
                $('#Law_OnlyInActualActs').prop('checked', true);
            }

            if (result.Law.OnlyInBasicActs === true) {
                $('#Law_OnlyInBasicActs').prop('checked', true);
            }

            //Populating dropdowns
            $('#select-oj-series option[value="' + result.Law.OJSeries + '"]').prop('selected', true);
            $('#law-date-type-select option[value="' + result.Law.LawDateType + '"]').prop('selected', true);

            //Populating textboxes
            $('#Law_NatID_ELI').val(result.Law.NatID_ELI);
            $('#Law_Year').val(result.Law.Year);
            $('#Law_DocNumber').val(result.Law.DocNumber);
            $('#Law_OJYear').val(result.Law.OJYear);
            $('#Law_Number').val(result.Law.Number);
            $('#Law_PageNumber').val(result.Law.PageNumber);

            //Populating dates
            if (result.Law.DatePeriodType != null) {
                $('input[name="Law.DatePeriodType"]:not(:checked)').removeAttr('checked');

                if (result.Law.DatePeriodType === 0 || result.Law.DatePeriodType === '0') {
                    $(':radio[name="Law.DatePeriodType"][value=date]').prop('checked', 'checked');
                    $(':radio[name="Law.DatePeriodType"][value=date]').attr('checked', 'checked');
                }
                else if (result.Law.DatePeriodType === 1 || result.Law.DatePeriodType === '1') {
                    $(':radio[name="Law.DatePeriodType"][value=period]').prop('checked', 'checked');
                    $(':radio[name="Law.DatePeriodType"][value=period]').attr('checked', 'checked');
                    $('#Law_DateTo').prop('disabled', false);
                }
                else {
                    throw 'Invalid DatePeriodType';
                }
            }

            if (result.Law.DatePeriodType === '0' || result.Law.DatePeriodType === 0) {
                $('#Law.DatePeriodType_Date').prop('checked', true);
            }
            else if (result.Law.DatePeriodType === '1' || result.Law.DatePeriodType === 1) {
                $('#Law.DatePeriodType_Period').prop('checked', true);
            }

            if (result.Law.DateFrom != undefined && result.Law.DateFrom != null && result.Law.DateFrom.toString() !== '0001-01-01T00:00:00' && result.Law.DateFrom.toString() != '/Date(-62135596800000)/') {
                if (inSession) {
                    var microtime = result.Law.DateFrom.toString().split('/').join('').split('Date').join('').split('(').join('').split(')').join(''); //INSTEAD OF REGEX
                    $('#Law_DateFrom').val(getDateFromMicrotime(microtime));
                }
                else {
                    var tIndex = result.Law.DateFrom.toString().indexOf('T');
                    var realDate = result.Law.DateFrom.toString().substring(0, tIndex);
                    $('#Law_DateFrom').val(realDate);
                }
            }

            if (result.Law.DateTo != undefined && result.Law.DateTo != null && result.Law.DateTo.toString() !== '0001-01-01T00:00:00' && result.Law.DateTo.toString() != '/Date(-62135596800000)/') {
                if (inSession) {
                    var microtime = result.Law.DateTo.toString().split('/').join('').split('Date').join('').split('(').join('').split(')').join(''); //INSTEAD OF REGEX
                    $('#Law_DateTo').val(getDateFromMicrotime(microtime));
                }
                else {
                    var tIndex = result.Law.DateTo.toString().indexOf('T');
                    var realDate = result.Law.DateTo.toString().substring(0, tIndex);
                    $('#Law_DateTo').val(realDate);
                }
            }

            //Populating trees
            populateTree('Law_DocumentTypes', result.Law.DocumentTypes);
            populateTree('Law_DirectoryLegislation', result.Law.DirectoryLegislation);
            populateTree('Law_Eurovoc', result.Law.Eurovoc);
            populateTree('Law_SubjectMatter', result.Law.SubjectMatter);

            $('#tbLaw_ActJurisdictionsText').length && populateTree('Law_ActJurisdictions', result.Law.ActJurisdictions);

            if ($('#tbLaw_SyllabusText').length) {
                populateTree('Law_Syllabus', result.Law.Syllabus);
            }
            if ($('#tbLaw_JurisdictionsText').length) {
                populateTree('Law_Jurisdictions', result.Law.Jurisdictions);
            }

            $(':radio[name="Law.LegislationType"][value="' + result.Law.LegislationType + '"]').trigger('click');

            // Multilingual Dictionary
            if (result.Law.MultiDict !== 'undefined' && result.Law.MultiDict !== null && result.Law.MultiDict.SelectedIds != null && result.Law.MultiDict.SelectedIds !== '') {
                $('#Law_MultiDict_SelectedIds').val(result.Law.MultiDict.SelectedIds);
                $('#Law_MultiDict_SelectedIds').trigger('change');
                setCookie('selectedMultiDictIds', result.Law.MultiDict.SelectedIds);

                if (result.Law.MultiDict.LogicalType == true) { // Refreshing "And" radio button
                    $('#rd-multilingual-dictionary-and').prop('checked', true);
                    $('#rd-multilingual-dictionary-or').removeAttr('checked');
                }
                else { // Refreshing "Or" radio button
                    $('#rd-multilingual-dictionary-or').prop('checked', true);
                    $('#rd-multilingual-dictionary-and').removeAttr('checked');
                }

                $('#Law_tb-multilingual-dictionary').val(result.Law.MultiDict.Text);
                $('#Law_tb-multilingual-dictionary').attr('title', result.Law.MultiDict.Text);
                $('#Law_MultiDict_Text').val(result.Law.MultiDict.Text);
                $('#Law_MultiDict_Text').trigger('change'); // for clear button to be shown

                selectedMultiDictIds = JSON.parse(result.Law.MultiDict.SelectedIds);
                selectedQueryTexts = JSON.parse(result.Law.MultiDict.SelectedQueryTexts);

                populateBuildedQuery();
            }

            if ($('#secAdvSearch').css('display') === 'none') {
                $('#btnAdvSearch').trigger('click');
            }

            $('.legislation-expand-link').trigger('click');

            $('.clearable').trigger('keyup');
        }


        //Populating Cases tab
        if (result.Cases != null) {
            searchType = 'tabCases';

            //Populating boolean checkboxes
            if (result.Cases.OnlyInTitles === true) {
                $('#Cases_OnlyInTitles').prop('checked', true);
            }

            if (result.Cases.TranslateSearchText === true) {
                $('#Cases_TranslateSearchText').prop('checked', true);
            }

            if (result.Cases.DirectoryCaseLawFull === true) {
                $('#Cases_DirectoryCaseLawFull').prop('checked', true);
            }

            if (result.Cases.Applicant !== undefined && result.Cases.Applicant !== null && result.Cases.Applicant !== '') {
                $('#Cases_Applicant').val(result.Cases.Applicant);
            }

            if (result.Cases.ApplicationNumber !== undefined && result.Cases.ApplicationNumber !== null && result.Cases.ApplicationNumber !== '') {
                $('#Cases_ApplicationNumber').val(result.Cases.ApplicationNumber);
            }

            //Populating textboxes
            $('#Cases_NatID_ECLI').val(result.Cases.NatID_ECLI);
            $('#Cases_Parties').val(result.Cases.Parties);
            $('#Cases_CaseNumber').val(result.Cases.CaseNumber);
            $('#Cases_Year').val(result.Cases.Year);


            $(':radio[name="Cases.CaseLawType"][value=' + result.Cases.CaseLawType + ']').trigger('click');

            //loading DatePeriodType
            if (result.Cases.DatePeriodType != null) {
                $('input[name="Cases.DatePeriodType"]:not(:checked)').removeAttr('checked');

                if (result.Cases.DatePeriodType === 0 || result.Cases.DatePeriodType === '0') {
                    $(':radio[name="Cases.DatePeriodType"][value=date]').prop('checked', 'checked');
                    $(':radio[name="Cases.DatePeriodType"][value=date]').attr('checked', 'checked');
                }
                else if (result.Cases.DatePeriodType === 1 || result.Cases.DatePeriodType === '1') {
                    $(':radio[name="Cases.DatePeriodType"][value=period]').prop('checked', 'checked');
                    $(':radio[name="Cases.DatePeriodType"][value=period]').attr('checked', 'checked');
                    $('#Cases_DateTo').prop('disabled', false);
                }
                else {
                    throw 'Invalid DatePeriodType';
                }
            }

            //Populating enactment
            populateEnactment(result, 'Cases');

            //Populating dates
            if (result.Cases.DateFrom != undefined && result.Cases.DateFrom != null && result.Cases.DateFrom.toString() !== '0001-01-01T00:00:00' && result.Cases.DateFrom.toString() != '/Date(-62135596800000)/') {
                if (inSession) {
                    var microtime = result.Cases.DateFrom.toString().split('/').join('').split('Date').join('').split('(').join('').split(')').join(''); //INSTEAD OF REGEX
                    $('#Cases_DateFrom').val(getDateFromMicrotime(microtime));
                }
                else {
                    var tIndex = result.Cases.DateFrom.toString().indexOf('T');
                    var realDate = result.Cases.DateFrom.toString().substring(0, tIndex);
                    $('#Cases_DateFrom').val(realDate);
                }
            }

            if (result.Cases.DateTo != undefined && result.Cases.DateTo != null && result.Cases.DateTo.toString() !== '0001-01-01T00:00:00' && result.Cases.DateTo.toString() != '/Date(-62135596800000)/') {
                if (inSession) {
                    var microtime = result.Cases.DateTo.toString().split('/').join('').split('Date').join('').split('(').join('').split(')').join(''); //INSTEAD OF REGEX
                    $('#Cases_DateTo').val(getDateFromMicrotime(microtime));
                }
                else {
                    var tIndex = result.Cases.DateTo.toString().indexOf('T');
                    var realDate = result.Cases.DateTo.toString().substring(0, tIndex);
                    $('#Cases_DateTo').val(realDate);
                }
            }

            //Populating trees
            populateTree('Cases_CourtsFolders', result.Cases.CourtsFolders);
            populateTree('Cases_DirectoryCaseLaw', result.Cases.DirectoryCaseLaw);
            populateTree('Cases_EuroCases', result.Cases.EuroCases);
            populateTree('Cases_Eurovoc', result.Cases.Eurovoc);
            if ($('#tbCases_SyllabusText').length) {
                populateTree('Cases_Syllabus', result.Cases.Syllabus);
            }
            populateTree('Cases_SubjectMatter', result.Cases.SubjectMatter);

            // if ($('#tbCases_DocumentTypesText').length) {
            populateTree('Cases_DocumentTypes', result.Cases.DocumentTypes);
            //  }

            //  if ($('#tbCases_ProcedureTypeText').length) {
            populateTree('Cases_ProcedureType', result.Cases.ProcedureType);
            //  }

            // if ($('#tbCases_AdvocateGeneralText').length) {
            populateTree('Cases_AdvocateGeneral', result.Cases.AdvocateGeneral);
            //  }

            // if ($('#tbCases_JudgeRapporteurText').length) {
            populateTree('Cases_JudgeRapporteur', result.Cases.JudgeRapporteur);
            //  }

            populateTree('Cases_Courts', result.Cases.Courts);

            // ECHR
            if ($('#tbCases_HudocText').length) {
                populateTree('Cases_Hudoc', result.Cases.Hudoc);
            }

            if ($('#tbCases_HudocImportanceText').length) {
                populateTree('Cases_HudocImportance', result.Cases.HudocImportance);
            }

            if ($('#tbCases_HudocApplicabilityText').length) {
                populateTree('Cases_HudocApplicability', result.Cases.HudocApplicability);
            }

            if ($('#tbCases_HudocArticleViolationText').length) {
                populateTree('Cases_HudocArticleViolation', result.Cases.HudocArticleViolation);
            }

            if ($('#tbCases_StatesText').length) {
                populateTree('Cases_States', result.Cases.States);
            }

            if (result.Cases.ReferedActTitle !== undefined && result.Cases.ReferedActTitle !== null && result.Cases.RereferedActTitle !== '') {
                $('#input-refered-act').val(result.Cases.ReferedActTitle);
                $('#Cases_ReferedActTitle').val(result.Cases.ReferedActTitle);
                var referedActType = result.Cases.ECHRReferedActType;
                $('#Cases_ECHRReferedActType').val(referedActType);
                if (referedActType === '0' || referedActType === 0) {
                    $('#select-refered-act option[value=0]').prop('selected', true);
                }
                else if (referedActType === '1' || referedActType === 1) {
                    $('#cases-echr-refered-act-provision').addClass('display-none');
                    $('#cases-echr-hudoc-articles').removeClass('display-none');
                    $('#cases-echr-court-rules').addClass('display-none');

                    $('#select-refered-act option[value=1]').prop('selected', true);

                    $('#Cases_ReferedActECHRDocLangId').val('');
                    populateTree('Cases_HudocArticles', result.Cases.HudocArticles);
                }
                else if (referedActType === '2' || referedActType === 2) {
                    $('#cases-echr-refered-act-provision').addClass('display-none');
                    $('#cases-echr-hudoc-articles').addClass('display-none');
                    $('#cases-echr-court-rules').removeClass('display-none');

                    $('#select-refered-act option[value=2]').prop('selected', true);

                    $('#Cases_ReferedActECHRDocLangId').val('');
                    populateTree('Cases_RulesOfTheCourt', result.Cases.RulesOfTheCourt);
                }
                else if (referedActType === '3' || referedActType === 3) {
                    $('#Cases_ReferedActECHRDocLangId').val(result.Cases.ReferedActECHRDocLangId);
                    $('#cases-echr-refered-act-provision').removeClass('display-none');
                    $('#cases-echr-hudoc-articles').addClass('display-none');
                    $('#cases-echr-court-rules').addClass('display-none');
                }

                $('#Cases_ECHRReferedActType').trigger('change');
            }

            // Multilingual Dictionary
            if (result.Cases.MultiDict.SelectedIds != null && result.Cases.MultiDict.SelectedIds !== '') {
                console.log(result.Cases.MultiDict);

                $('#Cases_MultiDict_SelectedIds').val(result.Cases.MultiDict.SelectedIds);
                $('#Cases_MultiDict_SelectedIds').trigger('change');
                setCookie('selectedMultiDictIds', result.Cases.MultiDict.SelectedIds);

                if (result.Cases.MultiDict.LogicalType == true) { // Refreshing "And" radio button
                    $('#rd-multilingual-dictionary-and').prop('checked', true);
                    $('#rd-multilingual-dictionary-or').removeAttr('checked');
                }
                else { // Refreshing "Or" radio button
                    $('#rd-multilingual-dictionary-or').prop('checked', true);
                    $('#rd-multilingual-dictionary-and').removeAttr('checked');
                }

                $('#Cases_tb-multilingual-dictionary').val(result.Cases.MultiDict.Text);
                $('#Cases_tb-multilingual-dictionary').attr('title', result.Cases.MultiDict.Text);
                $('#Cases_MultiDict_Text').val(result.Cases.MultiDict.Text);
                $('#Cases_MultiDict_Text').trigger('change'); // for clear button to be shown

                selectedMultiDictIds = JSON.parse(result.Cases.MultiDict.SelectedIds);
                selectedQueryTexts = JSON.parse(result.Cases.MultiDict.SelectedQueryTexts);

                populateBuildedQuery();
            }

            if ($('#secAdvSearch').css('display') === 'none') {
                $('#btnAdvSearch').trigger('click');
            }

            $('.clearable').trigger('keyup');
        }

        if (result.Finances != null) {
            searchType = 'tabFinances';

            // Populating boolean checkboxes
            if (result.Finances.OnlyInTitles === true) {
                $('#Finances_OnlyInTitles').prop('checked', true);
            }

            if (result.Finances.Keywords === true) {
                $('#Finances_Keywords').prop('checked', true);
            }

            if (result.Finances.SearchInSummaries === true) {
                $('#Finances_SearchInSummaries').prop('checked', true);
            }

            // Populating doc type booleans
            if (result.Finances.DocTypeStandarts === true) {
                $('#Finances_DocTypeStandarts').prop('checked', true);
            }

            if (result.Finances.DocTypeReglaments === true) {
                $('#Finances_DocTypeReglaments').prop('checked', true);
            }

            if (result.Finances.DocTypeDirectives === true) {
                $('#Finances_DocTypeDirectives').prop('checked', true);
            }

            if (result.Finances.DocTypeEuCaseLaw === true) {
                $('#Finances_DocTypeEuCaseLaw').prop('checked', true);
            }

            if (result.Finances.DocTypeNationalCaseLaw === true) {
                $('#Finances_DocTypeNationalCaseLaw').prop('checked', true);
            }

            if (result.Finances.DocTypeSummaries === true) {
                $('#Finances_DocTypeSummaries').prop('checked', true);
            }

            //Populating enactment
            populateEnactment(result, 'Finances');
            populateFinsDocTypeBoxTitles();

            // Populating date types
            if (result.Finances.DatePeriodType != null) {
                $('input[name="Finances.DatePeriodType"]:not(:checked)').removeAttr('checked');

                if (result.Finances.DatePeriodType === 0 || result.Finances.DatePeriodType === '0') {
                    $(':radio[name="Finances.DatePeriodType"][value=date]').prop('checked', 'checked');
                    $(':radio[name="Finances.DatePeriodType"][value=date]').attr('checked', 'checked');
                }
                else if (result.Finances.DatePeriodType === 1 || result.Finances.DatePeriodType === '1') {
                    $(':radio[name="Finances.DatePeriodType"][value=period]').prop('checked', 'checked');
                    $(':radio[name="Finances.DatePeriodType"][value=period]').attr('checked', 'checked');
                    $('#Cases_DateTo').prop('disabled', false);
                }
                else {
                    throw 'Invalid DatePeriodType';
                }
            }

            if (result.Finances.DatePeriodType === '0' || result.Finances.DatePeriodType === 0) {
                $('#Finances.DatePeriodType_Date').prop('checked', true);
            }
            else if (result.Finances.DatePeriodType === '1' || result.Finances.DatePeriodType === 1) {
                $('#Finances.DatePeriodType_Period').prop('checked', true);
                $('#Finances_DateTo').prop('disabled', false);
            }

            // Populating dates
            if (result.Finances.DateFrom != undefined && result.Finances.DateFrom != null && result.Finances.DateFrom.toString() !== '0001-01-01T00:00:00' && result.Finances.DateFrom.toString() != '/Date(-62135596800000)/') {
                if (inSession) {
                    var microtime = result.Finances.DateFrom.toString().split('/').join('').split('Date').join('').split('(').join('').split(')').join(''); //INSTEAD OF REGEX
                    $('#Finances_DateFrom').val(getDateFromMicrotime(microtime));
                }
                else {
                    var tIndex = result.Finances.DateFrom.toString().indexOf('T');
                    var realDate = result.Finances.DateFrom.toString().substring(0, tIndex);
                    $('#Finances_DateFrom').val(realDate);
                }
            }

            if (result.Finances.DateTo != undefined && result.Finances.DateTo != null && result.Finances.DateTo.toString() !== '0001-01-01T00:00:00' && result.Finances.DateTo.toString() != '/Date(-62135596800000)/') {
                if (inSession) {
                    var microtime = result.Finances.DateTo.toString().split('/').join('').split('Date').join('').split('(').join('').split(')').join(''); //INSTEAD OF REGEX
                    $('#Finances_DateTo').val(getDateFromMicrotime(microtime));
                }
                else {
                    var tIndex = result.Finances.DateTo.toString().indexOf('T');
                    var realDate = result.Finances.DateTo.toString().substring(0, tIndex);
                    $('#Finances_DateTo').val(realDate);
                }
            }

            // Populating classifiers
            // populateTree('Finances_DocumentTypes', result.Finances.DocumentTypes);
            populateTree('Finances_EuroFinance', result.Finances.EuroFinance);

            if ($('#secAdvSearch').css('display') === 'none') {
                $('#btnAdvSearch').trigger('click');
            }

            $('.clearable').trigger('keyup');
        }

        if (deletePrev === true) {
            var delid = id;

            $.ajax({
                type: "POST",
                url: appRootFolder + "/User/MySearchesDelete",
                traditional: true,
                data: {
                    delid: delid
                }
            });
        }
    });

    // type: string - Cases or Finances
    function populateEnactment(result, type) {
        $('#' + type + '_EnactmentText').val(result[type].EnactmentText);
        $('#' + type + '_EnactmentDocLangId').val(result[type].EnactmentDocLangId).trigger('change');
        $('#' + type + '_EnactmentCelex').val(result[type].EnactmentCelex).trigger('change');
        $('#' + type + '_ProvisionText').val(result[type].ProvisionText);
        $('#' + type + '_ProvisionId').val(result[type].ProvisionId).trigger('change');
        $('#' + type + '_ProvisionParOriginal').val(result[type].ProvisionParOriginal).trigger('change');
    }

    return searchType;
}

function populateTree(prefix, treeFilter) {
    $('#' + prefix + '_Tree').empty();
    //console.log(treeFilter);
    if (treeFilter && treeFilter.SelectedIds != '') {
        var selectyedIds = treeFilter.SelectedIds;
        $('#' + prefix + '_SelectedIds').val(selectyedIds).trigger('change');
        var keyPaths = treeFilter.KeyPaths;
        $('#' + prefix + '_KeyPaths').val(keyPaths);
        $('#tb' + prefix + 'Text').val('loading...');
        loadFilterDataTitles(prefix, selectyedIds);
    }
    else {
        clearTreeFilter(prefix);
    }
}

function populateSearchTemplate(templatePrefix, targetPrefix) {
    var templates = $('div[id^="' + templatePrefix + '"]'), elementName, currentId;

    $.each(templates, function (i, val) {
        currentId = $(this).attr('id');
        elementName = currentId.split(templatePrefix)[1];
        $('#' + targetPrefix + elementName).append($(this));
    });
}

// Select all for search folders
// treeIds - array of ids of folders trees
function addCheckAllButton(treeIds) {
    var i,
        $checkBox = $('<input type="checkbox" class="check-all-checkbox">'),
        $label = $('<label class="check-all-label">' + Resources.getResource('UI_JS_SelectAll') + '</label>'),
        len = treeIds.length;

    for (i = 0; i < len; i++) {
        $checkBox.data('target', treeIds[i]);
        $checkBox.attr('id', treeIds[i] + 'CheckBox');
        $checkBox.click(function () {
            var $this = $(this),
                target = $this.data('target'),
                tree = $(target).fancytree('getTree');

            if ($this.is(':checked')) {
                tree.visit(function (node) {
                    node.setSelected(true);
                });
            } else {
                tree.visit(function (node) {
                    node.setSelected(false);
                });
            }

            $(target).unbind("fancytreeclick", fancytreeStateChange);
            $(target).bind("fancytreeclick", fancytreeStateChange);

            function fancytreeStateChange(event, data) {
                var selected = !data.node.selected,
                    isParent;

                if (data.targetType === 'expander') {
                    return;
                }

                tree.visit(function (node) {
                    if (node.selected === false && node.key != data.node.key) {
                        selected = false;
                    }
                });

                if (!selected) {
                    $this.prop('checked', false);
                }
            }
        });

        $checkBox.insertBefore(treeIds[i]);
        $label.insertBefore(treeIds[i]);
        $('<hr/>').insertBefore(treeIds[i]);
    }
}

function setClasificatorTreesHeight() {
    var screenHeight = document.documentElement.clientHeight;
    if (screenHeight < 480) {
        $('.tree-dialog').css('height', ((screenHeight / 3) * 2) + 'px');
    }
}