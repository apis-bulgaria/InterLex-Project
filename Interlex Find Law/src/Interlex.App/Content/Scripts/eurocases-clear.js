(function ($) {

    $.fn.euClear = function (options) {
        var userAgentString = navigator.userAgent;
        //Detect browsers
        var isEdge = userAgentString.indexOf("Edge") > -1;
        var isOpera = !!window.opera || navigator.userAgent.indexOf(' OPR/') >= 0;
        // Opera 8.0+ (UA detection to detect Blink/v8-powered Opera)
        var isFirefox = typeof InstallTrigger !== 'undefined';   // Firefox 1.0+
        var isSafari = Object.prototype.toString.call(window.HTMLElement).indexOf('Constructor') > 0;
        // At least Safari 3+: "[object HTMLElementConstructor]"
        var isChrome = !!window.chrome && !isOpera;              // Chrome 1+
        var isIE = /*@cc_on!@*/false || !!document.documentMode;

        //Default options
        var settings = $.extend({
            linkClass: 'x-grey-on-white',
            right: '0px',
            top: '0px',
            type: "simple"
        }, options);

        return this.each(function () {
            //TODO: Get parsed default values
            if (isFirefox) {
                if (settings.type === 'simple') {
                    settings.right = '3px';
                }
                else {
                    settings.right = '30px';
                }
            }

            if (isIE) {
                if (settings.type === 'simple') {
                    settings.right = '4px';
                }
                else {
                    settings.right = '73px';
                }
            }

            if (isChrome) {
                settings.right = '4px';
            }

            var $input = $(this),
                $clearLink = $('.eucs-clear[data-element="' + $(this).attr('id') + '"]');
   
            // Visibility
            if ($input.val() !== undefined && $input.val() !== '' && $input.val() !== null) {
                $clearLink.css('visibility', 'visible');
            }

            $input.keyup(function () {
                if ($input.val() !== undefined && $input.val() !== '' && $input.val() !== null) {
                    $clearLink.css('visibility', 'visible');
                }
                else {
                    $clearLink.css('visibility', 'hidden');
                }
            });

            var productId = settings.productId,
                suffix;

            if (productId) {
                suffix = productId === 1 ? 'Cases' : 'Finances';
            } else {
                suffix = 'Cases';
            }

            if (settings.type === 'classifier') { // Classifier
                var extractedName = $input.attr('id').match('tb(.*)Text')[1];
                $clearLink.attr('id', extractedName + '_ClearLink');

                var $selectedIdsElement = $('#' + extractedName.concat('_SelectedIds'));

                $selectedIdsElement.change(function () {
                    if ($selectedIdsElement.val() !== undefined && $selectedIdsElement.val() !== null && $selectedIdsElement.val() !== '') {
                        $clearLink.css('visibility', 'visible');
                    }
                });
            } else if (settings.type === 'enactment') { // Simple enactment
                $('#' + suffix + '_EnactmentDocLangId').change(function () {
                    var $that = $(this);
                    if ($that.val() !== undefined && $that.val() !== null && $that.val() !== '' && $that.val() !== Resources.storage['UI_JS_AllProvisions']) {
                        $clearLink.css('visibility', 'visible');
                    }
                });
            }
            else if (settings.type === 'enactment-index') { // Enactment index
                $('#Cases_EnactmentDocLangId_Index').change(function () {
                    var $that = $(this);
                    if ($that.val() !== undefined && $that.val() !== null && $that.val() !== '' && $that.val() !== Resources.storage['UI_JS_AllProvisions']) {
                        $clearLink.css('visibility', 'visible');
                    }
                });
            } else if (settings.type === 'provision') { // Simple provision
                $('#' + suffix + '_ProvisionId').change(function () {
                    var $that = $(this);
                    if ($that.val() !== undefined && $that.val() !== null && $that.val() !== '') {
                        $clearLink.css('visibility', 'visible');
                    }
                });

                $clearLink.attr('id', 'Cases_Provision_ClearLink');
            }
            else if (settings.type === 'provision-index') { // Provision index
                $('#Cases_ProvisionId_Index').change(function () {
                    var $that = $(this);
                    if ($that.val() !== undefined && $that.val() !== null && $that.val() !== '') {
                        $clearLink.css('visibility', 'visible');
                    }
                });

                $clearLink.attr('id', 'Cases_Provision_ClearLink_Index');
            }
            else if (settings.type === 'refered-act-echr') { // Refered act echr
                $('#Cases_ECHRReferedActType').change(function () {
                    var $that = $(this);
                    if ($that.val() !== undefined && $that.val() !== null && $that.val() !== '') {
                        $clearLink.css('visibility', 'visible');
                    }
                });
            }
            else if (settings.type === 'multilingual-dictionary') { // Multilingual dictionary
                $('#Cases_MultiDict_Text, #Law_MultiDict_Text').change(function () {
                    var $that = $(this);
                    if ($that.val() !== undefined && $that.val() !== null && $that.val() !== '') {
                        $clearLink.css('visibility', 'visible');
                    }
                    else {
                        $clearLink.css('visibility', 'hidden');
                    }
                });
            }
            else if (settings.type === 'finances-documents-type') { // Finances documents type
                $('#finances-document-type-tb').change(function () {
                    var $that = $(this);
                    if ($that.val() !== undefined && $that.val() !== null && $that.val() !== '') {
                        $clearLink.css('visibility', 'visible');
                    }
                });
            }

            //Clearing input
            $clearLink.click(function () {
                $input.val('');

                if (settings.type === 'classifier') {
                    clearTreeFilter(extractedName);
                } else if (settings.type === 'enactment') { // TODO: Consider getting ids from $input
                    $('#' + suffix + '_EnactmentText').val('');
                    $('#' + suffix + '_EnactmentDocLangId').val('');
                    $('#' + suffix + '_ProvisionText').val('');
                    $('#' + suffix + '_ProvisionId').val('');
                    $('#' + suffix + '_Provision_ClearLink').css('visibility', 'hidden');
                }
                else if (settings.type === 'enactment-index') { // TODO: Consider getting ids from $input
                    $('#Cases_EnactmentText_Index').val('');
                    $('#Cases_EnactmentDocLangId_Index').val('');
                    $('#Cases_ProvisionText_Index').val('');
                    $('#Cases_ProvisionId_Index').val('');
                    $('#Cases_Provision_ClearLink_Index').css('visibility', 'hidden');
                }
                else if (settings.type === 'provision') { // TODO: Consider getting ids from $input
                    $('#' + suffix + '_ProvisionText').val(Resources.storage['UI_JS_AllProvisions']);
                    $('#' + suffix + '_ProvisionId').val('');
                    $('#' + suffix + '_ProvisionParOriginal').val('');
                }
                else if (settings.type === 'provision-index') { //TODO: Consider getting ids from $input
                    $('#Cases_ProvisionText_Index').val(Resources.storage['UI_JS_AllProvisions']);
                    $('#Cases_ProvisionId_Index').val('');
                    $('#Cases_ProvisionParOriginal_Index').val('');
                }
                else if (settings.type === 'refered-act-echr') {
                    $('#Cases_ReferedActECHRDocLangId').val('');
                    $('#Cases_ReferedActTitle').val('');
                    $('#Cases_ECHRReferedActType').val(0);

                    if ($('#tbCases_HudocArticlesText').length) {
                        clearTreeFilter('Cases_HudocArticles');
                    }

                    if ($('#tbCases_RulesOfTheCourtText').length) {
                        clearTreeFilter('Cases_RulesOfTheCourt');
                    }

                    // showing correct privison div
                    $('#cases-echr-refered-act-provision').removeClass('display-none');
                    $('#cases-echr-court-rules').addClass('display-none');
                    $('#cases-echr-hudoc-articles').addClass('display-none');
                }
                else if (settings.type === 'multilingual-dictionary') {
                    multiDictClear();
                }
                else if (settings.type === 'finances-documents-type') {
                    $('#finances-document-type-container label input[type="checkbox"]').prop('checked', false);
                }

                $clearLink.css('visibility', 'hidden');
            });
        });
    };

}(jQuery));