/// options:
/// required - foldersContainerSelector - the element in which the tree will be put in
/// optional - docsContainerSelector - the element in which the docs will be put
/// optional - folderClick(function (event, data)) - default - loads documents in docsContainerSelector
/// optional - isInModal - if the tree will be in a modal
function MyDocumentsHandler(options) {
    if (!options.foldersContainerSelector) {
        throw 'MyDocumentsHandler: Missing selector for folders';
    }

    var rootKey = 'parentNode',
        eventTargetTypesClick = {
            title: 'title',
            prefix: 'prefix',
            expander: 'expander',
            checkbox: 'checkbox',
            icon: 'icon'
        },
        ROUTE_PREFIX = appRootFolder + '/User',
        ROUTES = {
            getFolderData: ROUTE_PREFIX + '/GetFolderData',
            getDocsCountFolder: ROUTE_PREFIX + '/GetDocsCountFolder',
            addFolder: ROUTE_PREFIX + '/CreateFolder',
            renameFolder: ROUTE_PREFIX + '/RenameFolder',
            deleteFolder: ROUTE_PREFIX + '/DeleteFolder',
            listMyDocuments: ROUTE_PREFIX + '/MyDocumentsList',
            changeFolderParent: ROUTE_PREFIX + '/ChangeFolderParent',
            moveDocs: ROUTE_PREFIX + '/MoveDocs'
        },
        rootExtraClasses = "topmost-folder",
        isInModal = options.isInModal,
        folderClickFn,
        treeDiv,
        self = this;

    function defaultClickFn(event, data) {
        var currentFolderId;
        if (data.targetType !== eventTargetTypesClick.expander) { // don't load folder data on the expander
            currentFolderId = data.node.key;
            self.LoadMyDocuments(1, currentFolderId);
        }
    }

    folderClickFn = options.folderClick || defaultClickFn;

    this.LoadMyDocuments = function (page, folderId) {
        var activeNode, currentFolderId, folderName;
        if (folderId) {
            currentFolderId = folderId;
        } else {
            activeNode = treeDiv.fancytree("getActiveNode");
            currentFolderId = activeNode ? activeNode.key : null;
        }

        if ($('#OrderBy').length) {
            orderBy = $('#OrderBy').val();
        }
        else {
            orderBy = 'add_date';
        }

        orderDir = $('#sort_dir').data('dir');
        if (currentFolderId === rootKey) {
            currentFolderId = null;
        }

        folderName = treeDiv.fancytree('getTree').getNodeByKey(currentFolderId || rootKey).title;

        $.ajax({
            type: "POST",
            url: ROUTES.listMyDocuments,
            data: {
                orderBy: orderBy,
                orderDir: orderDir,
                folderId: currentFolderId
            }
        }).then(setupDocuments);

        function setupDocuments(data) {
            var html,
                count,
                $docsContainer,
                $contentContainer = $('.container');

            if (!options.docsContainerSelector) {
                throw "No selector given for documents container.";
            }

            $docsContainer = $(options.docsContainerSelector);
            $docsContainer.html(data);
            if (window.innerWidth >= 1024) {
                $(options.docsContainerSelector).find("#my-documents-list-container .doc-item-container")
                    .draggable({
                        revert: "invalid",
                        cursorAt: { top: -1, left: -1 },
                        connectToFancytree: true,   // let Fancytree accept drag events
                        containment: $('.body-container')
                    })
                    .on('dragstart', function () {
                        $contentContainer.addClass('document-dragging');
                    })
                    .on('dragstop', function (ev) {
                        $contentContainer.removeClass('document-dragging');
                    });
            }

            $docsContainer.find('#my-documents-view-title')
                .html(folderName);

            $docsContainer.off('mydocs:doc-removed')
                .off('mydocs:all-docs-removed')
                .on('mydocs:doc-removed', function () {
                    var activeNode = treeDiv.fancytree("getActiveNode"),
                        key = activeNode ? activeNode.key : rootKey,
                        count = self.getFolderDocsCount(key);

                    self.setFolderDocsCount(key, count - 1);
                })
                .on('mydocs:all-docs-removed', function () {
                    var activeNode = treeDiv.fancytree("getActiveNode"),
                        key = activeNode ? activeNode.key : rootKey;

                    self.setFolderDocsCount(key, 0);
                });

            $docsContainer.data('folder-key', currentFolderId || rootKey);
            html = $docsContainer.find('.my-docs-count').html();
            if (html) {
                count = html.trim();
                self.setFolderDocsCount(currentFolderId || rootKey, count);
            }
        }
    }

    this.getSelectedNodeKey = function () {
        var node = treeDiv.fancytree("getActiveNode");
        var key = node ? node.key : null;
        return key === rootKey ? null : key;
    }

    this.changeFavouriteDocsSortDir = function () {
        var currentDir = $('#sort_dir').data('dir');
        if (currentDir === 'desc') {
            $('#sort_dir').data('dir', 'asc');
        }
        else {
            $('#sort_dir').data('dir', 'desc');
        }

        this.LoadMyDocuments(1);
    }

    this.initFolders = function (readyFn) {
        $.ajax({
            url: ROUTES.getFolderData,
            type: 'POST',
            data: {
                parentId: null,
            },
            dataType: 'json'
        }).then(function (data) {
            $.ajax({
                url: ROUTES.getDocsCountFolder,
                type: 'POST',
                data: {
                    parentId: null,
                },
                dataType: 'json'
            }).then(function (countObj) {
                var len = data.length,
                    root,
                    i;

                root = [{
                    title: Resources.storage['UI_JS_MyDocuments'],
                    key: rootKey,
                    folder: true,
                    lazy: false,
                    expanded: true,
                    children: data,
                    extraClasses: rootExtraClasses,
                    documentsCount: countObj.documentsCount
                }];
                initFancyTree(root, readyFn);
            });
        }, function (err) {
            console.dir(err);
        });
    }

    // fromFolderKey - optional, will take currently selected if not given
    this.moveAllDocsFromFolderToFolder = function (toFolderKey, fromFolderKey) {
        if (toFolderKey) {
            toFolderKey = parseInt(toFolderKey, 10);
        }

        fromFolderKey = fromFolderKey || this.getSelectedNodeKey();
        if (fromFolderKey) {
            fromFolderKey = parseInt(fromFolderKey, 10)
        }
 
        $.ajax({
            url: ROUTES.moveDocs,
            type: 'POST',
            data: {
                fromId: fromFolderKey,
                toId: toFolderKey
            },
            dataType: 'json'
        }).then(function (data) {
            switch (data.status) {
                case 200:
                    var fromDocsCount, toDocsCount;
                    if (fromFolderKey === toFolderKey) {
                        return;
                    }

                    fromDocsCount = self.getFolderDocsCount(fromFolderKey);
                    toDocsCount = self.getFolderDocsCount(toFolderKey);
                    if (toDocsCount !== null) {
                        self.setFolderDocsCount(toFolderKey, toDocsCount + fromDocsCount);
                    }

                    self.setFolderDocsCount(fromFolderKey, 0);
                    $('#my-documents-list-container').empty();
                    $('.my-docs-count').text('0');
                    break;
                case 500:
                    // TODO: show error
                    break;
            }

        });
    }

    // Fancytree only options, functions, callbacks
    function initFancyTree(root, readyFn) {
        var readyCallback = readyFn || function () { };

        treeDiv = $(options.foldersContainerSelector);
        treeDiv.fancytree({
            extensions: ["dnd"],
            source: root,
            lazyLoad: function (event, data) {
                var node = data.node;
                data.result = {
                    url: ROUTES.getFolderData,
                    type: "POST",
                    data: {
                        parentId: data.node.key,
                    },
                    cache: false
                };
            },
            init: function (event, data) {
                readyCallback();
                bindRootContextMenu();
            },
            renderNode: function (event, data) {
                var elem = $(data.node.span).find('.docs-count');
                elem.remove();
                $(data.node.span).append('<span class="docs-count" style="cursor: pointer;">(' + data.node.data.documentsCount + ')</span>');
            },
            dnd: {
                autoExpandMS: 400,
                focusOnClick: false,
                preventVoidMoves: true, // Prevent dropping nodes 'before self', etc.
                preventRecursiveMoves: true, // Prevent dropping nodes on own descendants
                dragStart: function (node, data) {
                    /** This function MUST be defined to enable dragging for the tree.
                     *  Return false to cancel dragging of node.
                     */
                    return false;
                },
                dragEnter: function (node, data) {
                    /** data.otherNode may be null for non-fancytree droppables.
                     *  Return false to disallow dropping on node. In this case
                     *  dragOver and dragLeave are not called.
                     *  Return 'over', 'before, or 'after' to force a hitMode.
                     *  Return ['before', 'after'] to restrict available hitModes.
                     *  Any other return value will calc the hitMode from the cursor position.
                     */

                    return ["over"];
                },
                dragDrop: onDragDrop
            },
            click: folderClickFn,
            loadChildren: function (event, data) {
                bindContextMenu(options.foldersContainerSelector + ' span:not(.' + rootExtraClasses + ').fancytree-node');
            },
        });

        function onDragDrop(node, data) {
            var HIT_MODES = {
                    before: "before",
                    after: "after",
                    over: "over" // into folder 
                },
                currentHitMode = data.hitMode;

            if (currentHitMode === HIT_MODES.over) {
                if (!data.otherNode) {
                    var isSuccessful = dragDropDocument(data, node);
                    return isSuccessful;
                }
            } else if (currentHitMode === HIT_MODES.after || currentHitMode === HIT_MODES.before) {
                if (!data.otherNode) {
                    returnDraggable(data.draggable.element);
                }
            }

            return false;
        }

        function dragDropDocument(data, node) {
            var docItemContainer = data.draggable.element,
                docLangId = docItemContainer.find('.aFavouriteDocRemove').data('itemid'),
                title = docItemContainer.find('.aFavouriteDocRemove').data('itemtitle'),
                currFolderKey = $(options.docsContainerSelector).data('folder-key');

            if (currFolderKey === node.key) {
                returnDraggable(docItemContainer);
                return false;
            }

            delFavouriteDoc(docLangId, docItemContainer.find('.doc-top-bar'), true, false)
                .then(function () {
                    addFavouriteDoc(docLangId, docItemContainer, title, node.key === rootKey ? null : node.key);
                    var count = self.getFolderDocsCount(node.key);
                    self.setFolderDocsCount(node.key, count + 1);
                });

            return true;
        }

        function returnDraggable($draggable) {
            $draggable.animate({
                top: '0px',
                left: '0px'
            });
        }

        function bindContextMenu(selector) {
            $.contextMenu({
                selector: selector,
                items: {
                    add: {
                        name: Resources.storage['UI_JS_Add'],
                        callback: createFolder
                    },
                    rename: {
                        name: Resources.storage['UI_JS_Rename'],
                        callback: renameFolder
                    },
                    delete: {
                        name: Resources.storage['UI_JS_Delete'],
                        callback: deleteFolder
                    }
                }
            });
        }

        function bindRootContextMenu() {
            var rootSelector = options.foldersContainerSelector + ' .' + rootExtraClasses;
            $(rootSelector).off('contextmenu');
            $.contextMenu({
                selector: rootSelector,
                items: {
                    add: {
                        name: Resources.storage['UI_JS_Add'],
                        callback: createFolder
                    },
                }
            });
        }
    }

    this.createFolderBtnCallback = function () {
        var node = treeDiv.fancytree("getActiveNode"),
            key = node ? node.key : undefined;

        if (!key) {
            treeDiv.fancytree("getTree").activateKey(rootKey);
            node = treeDiv.fancytree("getActiveNode");
        }

        createFolder('add', {
            $trigger: $(node.span)
        });
    }

    // if node is not found returns null
    this.getFolderDocsCount = function (key) {
        var node, count;
        if (key === null) {
            key = rootKey;
        }

        node = treeDiv.fancytree('getTree').getNodeByKey(key.toString());
        if (!node) {
            return null;
        }

        count = $(node.span).find('.docs-count').html().replace(/\D/g, '');

        return parseInt(count, 10);
    }

    this.setFolderDocsCount = function (key, count) {
        var node;
        if (key === null) {
            key = rootKey;
        }

        node = treeDiv.fancytree('getTree').getNodeByKey(key.toString());
        node.data.documentsCount = count;
        $(node.span).find('.docs-count').html('(' + count + ')');
    }

    // options :
    // optional - folderName
    // optional - title
    // optional - contentText
    // required - ajaxData
    // required - url
    // required - success (callback on save)
    function openCreateFolderModal(options) {
        if (options.folderName && options.folderName.match(/\S/g)) {
            options.folderName = options.folderName;
        } else {
            options.folderName = '';
        }

        var form = createForm(options),
            buttonsModal = [{
                text: Resources.storage['UI_JS_Save'],
                click: function () {
                    var ajaxData = options.ajaxData,
                        that = this;
                    ajaxData.folderName = $('#create-update-form #folder-name').val();

                    $.ajax({
                        type: 'POST',
                        url: options.url,
                        data: ajaxData
                    }).then(function (data) {
                        success.call(that, data);
                    });
                },
                class: 'btn-large btn-blue f-white f-bold f-0-7-important folders-btn-save'
            }, {
                text: Resources.storage['UI_JS_Cancel'],
                click: function () {
                    $(this).dialog('close');
                },
                class: 'btn-large btn-lgrey f-blue f-bold f-0-7-important'
            }];

        $('<div id="dialog-holder"></div>')
            .append(form)
            .dialog({
                modal: true,
                title: options.title,
                zIndex: 10000,
                width: 'auto',
                maxWidth: 270,
                defaultWidth: 270,
                fluid: true,
                dialogClass: isInModal ? 'modal-in-modal' : '',
                resizable: false,
                draggable: true,
                autoOpen: true,
                open: function () {
                    $("#dialog-holder").keypress(function (e) {
                        if (e.keyCode == $.ui.keyCode.ENTER) {
                            e.preventDefault();
                            $(this).parent().find(".folders-btn-save").trigger("click");
                            return false;
                        }
                    });
                    isAModalOpened = true;
                },
                buttons: buttonsModal,
                close: function (event, ui) {
                    var errorDiv = $('#create-update-form .error-div');
                    setTimeout(function () {
                        if (!isInModal) {
                            isAModalOpened = false;
                        }
                    }, 200);

                    errorDiv.hide();
                    errorDiv.html('');
                    $(this).remove();
                }
            });

        function createForm() {
            var form = $('<form style="font-size: 0.75em" id="create-update-form"></form>'),
                nameInput = $('<input type="text" name="folderName" id="folder-name" value="' + options.folderName + '" class="text ui-widget-content ui-corner-all">'),
                label = $('<label for="folder-name">' + options.contentText + '</label>'),
                errorDiv = $('<div class="error-div" style="color: red"></div>');

            errorDiv.hide();
            form.append(label);
            form.append(nameInput);
            form.append(errorDiv);
            return form;
        }

        function success(data) {
            var errorDiv = $('#create-update-form .error-div');
            switch (data.status) {
                case 200:
                    options.success(data);
                    $(this).dialog('close');
                    break;
                case 400:
                    errorDiv.show();
                    errorDiv.html(data.errorText);
                    break;
                case 500:
                    errorDiv.show();
                    errorDiv.html(data.errorText);
                    break;
            }
        }
    }

    function createFolder(key, options) {
        var node = $.ui.fancytree.getNode(options.$trigger);
        openCreateFolderModal({
            title: Resources.storage['UI_JS_CreateFolderIn'] + ' "' + node.title + '"',
            url: ROUTES.addFolder,
            ajaxData: {
                parentId: node.key
            },
            contentText: Resources.storage['UI_JS_Name'] + ': ',
            success: function (data) {
                node.addChildren(data.folderData, null);
            }
        });
    }

    function renameFolder(key, opts) {
        var node = $.ui.fancytree.getNode(opts.$trigger);
        openCreateFolderModal({
            title: Resources.storage['UI_JS_RenameFolder'] + ' "' + node.title + '"',
            folderName: node.title,
            url: ROUTES.renameFolder,
            ajaxData: {
                folderId: node.key
            },
            contentText: Resources.storage['UI_JS_NewName'] + ': ',
            success: function (data) {
                var elem, activeNode;
                node.setTitle(data.folderName);
                elem = $(node.span).find('.docs-count');
                elem.remove();
                $(node.span).append('<span class="docs-count" style="cursor: pointer;">(' + node.data.documentsCount + ')</span>');

                activeNode = treeDiv.fancytree('getActiveNode');
                if (activeNode && activeNode.span === node.span) {
                    $(options.docsContainerSelector).find('#my-documents-view-title').html(data.folderName);
                }
            }
        });
    }

    function deleteFolder(key, opts) {
        var node = $.ui.fancytree.getNode(opts.$trigger),
            errorDiv = $('<div class="error-div" style="color: red"></div>'),
            buttonsModal = [{
                text: Resources.storage['UI_JS_Yes'],
                click: function () {
                    var that = this;
                    $.ajax({
                        url: ROUTES.deleteFolder,
                        type: 'POST',
                        data: {
                            folderId: node.key,
                        },
                        dataType: 'json'
                    }).then(function (data) {
                        var myDocumentsTopFolder = node.tree.rootNode.children[0],
                            activeNode = $(options.foldersContainerSelector).fancytree("getActiveNode");

                        switch (data.status) {
                            case 200:
                                if (activeNode && (node.key === activeNode.key || checkIfParent(node.children, activeNode.key))) {
                                    $(myDocumentsTopFolder.span).trigger('click');
                                }

                                node.remove();
                                errorDiv.hide();
                                errorDiv.html('');
                                $(that).dialog("close");
                                break;
                            case 500:
                                errorDiv.show(data.errorText);
                                break;
                        }

                        function checkIfParent(childNodes, activeNodeKey) {
                            var children = childNodes,
                                len,
                                i;

                            if (!children) {
                                return false;
                            }

                            len = children.length;

                            for (i = 0; i < len; i++) {
                                if (children[i].key === activeNodeKey) {
                                    return true;
                                }

                                if (checkIfParent(children[i].children, activeNodeKey)) {
                                    return true;
                                }
                            }
                        }
                    });
                },
                class: 'btn-large btn-blue f-white f-bold f-0-7-important'
            }, {
                text: Resources.storage['UI_JS_Cancel'],
                click: function () {
                    $(this).dialog('close');
                },
                class: 'btn-large btn-lgrey f-blue f-bold f-0-7-important'
            }];

        errorDiv.hide();
        $('<div id="delete-dialog"></div>')
            .append(errorDiv)
            .dialog({
                modal: true,
                title: Resources.storage['UI_JS_DeleteFolder'] + ' "' + node.title + '"?',
                zIndex: 10000,
                autoOpen: true,
                width: 'auto',
                dialogClass: isInModal ? 'modal-in-modal' : '',
                resizable: false,
                draggable: true,
                open: function () {
                    isAModalOpened = true;
                },
                buttons: buttonsModal,
                close: function (event, ui) {
                    setTimeout(function () {
                        if (!isInModal) {
                            isAModalOpened = false;
                        }
                    }, 200);

                    $(this).remove();
                }
            });
    }
}

/*favourite doc*/
function favouriteDocAddClick(e) {
    e.preventDefault();

    if ($('body').hasClass('body-demo')) {
        window.location.href = appRootFolder + '/User/MyDocuments';
    }
    else {
        var $innerSpan = $(e.target),
       docLangId = $innerSpan.data('itemid'),
       title = $innerSpan.data('itemtitle'),
       myFoldersHandler = new MyDocumentsHandler({
           foldersContainerSelector: '#my-folders-add-container',
           folderClick: function (event, data) {

           },
           isInModal: true
       });

        var fullWidth = $(window).width();
        if (fullWidth > 899) {
            fullWidth *= 0.25;
        } else if (fullWidth < 899 && fullWidth > 480) {
            fullWidth *= 0.5;
        } else {
            fullWidth *= 0.75;
        }

        $('<div id="add-doc-dialog"><div id="my-folders-add-container" class="my-folders-container"></div></div>')
            .dialog({
                modal: true,
                title: Resources.storage['UI_JS_AddDocToFolder'],
                zIndex: 10000,
                autoOpen: true,
                width: fullWidth,
                resizable: false,
                draggable: true,
                open: function () {
                    isAModalOpened = true;
                },
                buttons: [{
                    text: Resources.storage['UI_JS_Cancel'],
                    click: function () {
                        $(this).dialog('close');
                    },
                    class: 'folders-main-btn-cancel btn btn-lgrey f-blue f-bold f-0-7-important right'
                }, {
                    text: Resources.storage['UI_JS_Save'],
                    click: function () {
                        addFavouriteDoc(docLangId, $innerSpan, title, myFoldersHandler.getSelectedNodeKey());
                        $(this).dialog("close");
                    },
                    class: 'btn btn-blue folders-main-btn-save f-white f-bold f-0-7-important right'
                }],
                close: function (event, ui) {
                    setTimeout(function () {
                        isAModalOpened = false;
                    }, 200);

                    $(this).remove();
                }
            })
            .parent()
            .find('.ui-dialog-buttonset')
            .prepend('<div id="link-wrapper"><a style="color: #148dd4" class="add-new-folder-link f-bold f-0-7-important left">' + Resources.storage['UI_JS_NewFolder'] + '</a>')
            .ready(function () {
                var link = $('.ui-dialog-buttonset .add-new-folder-link');
                myFoldersHandler.initFolders();

                link.click(myFoldersHandler.createFolderBtnCallback);
                link.css('cursor', 'pointer');
            });
        // addFavouriteDoc(docLangId, $innerSpan, title, folderId);
    }
}

function addFavouriteDoc(docLangId, $innerSpan, title, folderId) {
    $.ajax({
        url: appRootFolder + "/User/AddUserDoc",
        type: 'POST',
        data: {
            docLangId: docLangId,
            folderId: folderId
        },
        success: function (data) {
            if (data.status === 202 || data.status === '202') {
                notify(data.title, data.message, 'warn');
            }
            else if (data.status === 401 || data.status === '401') {
                /* $('<div class="modal-product-features-info"></div>').html(data.message).dialog({
                     title: data.title
                 });*/

                window.location.href = "/User/ProductFeaturesInfo/5";
            }
            else if (data.status === 500 || data.status === '500') {
                notify(data.title, data.message, 'error');
            }
            else {
                $innerSpan.addClass('star-yellow');
                $innerSpan.removeClass('star');

                $innerSpan.removeClass('aFavouriteDoc');
                $innerSpan.addClass('aFavouriteDocRemove');

                $innerSpan.prop('title', Resources.storage['UI_JS_MyDocumentsRemove']);

                $innerSpan.unbind();
                $innerSpan.click(favouriteDocRemoveClick);
            }
        }
    });
}

/*--- DEL FAVOURITE DOC ---*/
function favouriteDocRemoveClick(e) {
    e.preventDefault();

    var $innerSpan = $(e.target),
        docLangId = $innerSpan.data('itemid'),
        element = $(e.target);

    delFavouriteDoc(docLangId, element, false, true);
}

function favouriteDocRemoveAndDeleteElementClick(e) {
    e.preventDefault();

    var $innerSpan = $(e.target),
        docLangId = $innerSpan.data('itemid'),
        element = $(e.target).parent();

    delFavouriteDoc(docLangId, element, true, false);
}

function delFavouriteDoc(docLangId, element, remove, fromListView) {
    var curItemId,
        index,
        container,
        showMore,
        showMoreKeywords,
        keywords,
        myDocsCountContainer,
        myDocsCurrentCount;

    return $.ajax({
        url: appRootFolder + "/User/DelUserDoc",
        type: 'POST',
        data: { docLangId: docLangId },
        statusCode: {
            200: function (response) {
                if (fromListView === true) {
                    $(element).addClass('star');
                    $(element).removeClass('star-yellow');
                    $(element).unbind();
                    $(element).click(favouriteDocAddClick);
                    $(element).prop('title', Resources.storage['UI_JS_MyDocumentsAdd']);
                }
                else {
                    element.find('.cbSelectDocument').attr('checked', false);
                    curItemId = element.find('.cbSelectDocument').data('itemid');

                    index = $.inArray(curItemId, selectedDocsIds);
                    if (index >= 0) selectedDocsIds.splice(index, 1);
                    refreshExportLinks();

                    if (remove === true) {
                        container = element.parent();

                        showMore = container.find('.show-more');
                        $(showMore[0]).remove();

                        showMoreKeywords = container.find('.show-more-keywords');
                        $(showMoreKeywords[0]).remove();

                        keywords = container.find('.item-keywords');
                        $(keywords[0]).remove();

                        element.parent().prev().remove();
                        element.parent().remove();

                        myDocsCountContainer = $('.my-docs-count')[0];
                        myDocsCountContainer = $(myDocsCountContainer);
                        myDocsCurrentCount = parseInt(myDocsCountContainer.text());
                        $('#my-documents-list-container').trigger('mydocs:doc-removed');
                        myDocsCountContainer.text((myDocsCurrentCount - 1).toString());
                    }
                }
            },
            202: function (data) {
                notify(data.title, data.message, 'error');
            }
        }
    });
}