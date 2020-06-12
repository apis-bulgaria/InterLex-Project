$().ready(function () {
    var docsToCompare = [];

    $('#mt-nav a').each(function myfunction() {
        $(this).click(function () {
            var target = $('#' + $(this).attr('data-target')),
                headerWidth = 0;

            $('#mt-nav span').removeClass('mt-nav-clicked');
            $(this).parent().addClass('mt-nav-clicked');

            $('.header-main')
                .children()
                .not('script')
                .not('#secAdvSearch')
                .each(function () {
                    headerWidth += $(this).height();
                    console.log(this);
                    console.log($(this).height());
                });

            $("html, body").animate({
                scrollTop: target.offset().top - (headerWidth + $('#doc-view-top-bar').height())
            }, 0);
            //container.scrollTop(target.offset().top - container.offset().top + container.scrollTop());
        });
    });

    $('.WordSection1').find('img')
        .each(function myfunction() {
            $(this).click(function myfunction() {
                window.open($(this).attr('src'), '_blank');
            });
        });

    $('#doc-view-top-holder').show();

    $("#doc-navigation-container").accordion({
        collapsible: true,
        heightStyle: "fill",
        icons: false,
        animate: false
    });

    $("h3").focus(function () {
        $(this).removeClass("ui-state-focus");
    });

    $('#btn-compare').click(function (e) {
        e.preventDefault();

        if (docsToCompare.length < 2) {
            notify(Resources.storage['UI_JS_NotEnoughDocumentsSelected'], Resources.storage['UI_JS_NotEnoughDocumentsSelectedFull'], 'warn');
        }
        else {
            if ((typeof docsToCompare[0] === "number") && Math.floor(docsToCompare[0]) === docsToCompare[0])
                window.open('/Compare/' + docsToCompare[0] + '/' + docsToCompare[1], '_blank');
            else
                window.open('/Compare/ByIdentifier/' + docsToCompare[0] + '/' + docsToCompare[1], '_blank');
        }
    });

    $('.chevron-doc-navigation').click(function (e) {
        e.preventDefault();

        $("#doc-view-top-holder").animate({ width: 'toggle' }, 350);

        setTimeout(function () {
            $('.WordSection1').animate({ 'width': '97%', 'margin-left': '2%', 'margin-right': '1%' }, 350);
            $('#doc-view-top-bar').animate({ 'width': '100%', 'margin-left': '0' }, 350);
            $('.chevron-doc-navigation-closed').show();

            //$('#doc-view-container').css('width', '100%');
            // $('#doc-view-container').css('margin-left', '0');
            // $('#doc-view-top-bar').css('width', '100%');
            // $('#doc-view-top-bar').css('margin-left', '0');
        }, 1);
    });

    $('.chevron-doc-navigation-closed').click(function (e) {
        e.preventDefault();

        $("#doc-view-top-holder").animate({ width: 'toggle' }, 350);
        setTimeout(function () {
            $('.WordSection1').animate({ 'width': '78%', 'margin-left': '21%', 'margin-right': '1%' }, 350);
            $('#doc-view-top-bar').animate({ 'width': '80%', 'margin-left': '20%' }, 350);
            $('#chevron-doc-navigation-closed').hide();

            //$('#doc-view-container').css('width', '100%');
            // $('#doc-view-container').css('margin-left', '0');
            // $('#doc-view-top-bar').css('width', '100%');
            // $('#doc-view-top-bar').css('margin-left', '0');
        }, 1);
    });

    $('.cb-version').change(function () {
        var that = $(this);
        var docLangId = that.data('doclangid');

        if (that.is(":checked")) {
            if (docsToCompare.length < 2) {
                docsToCompare.push(docLangId);
            }
            else {
                notify('Maximum elements to compare reached', 'Please select only two documents for comparision.', 'warn');
                that.attr('checked', false);
            }
        }
        else {
            var index = docsToCompare.indexOf(docLangId);

            if (index > -1) {
                docsToCompare.splice(index, 1);
            }
        }
    });
});

function responsiveMachinTranslationNavigationReact() {
    var deviceWidth = getDeviceWidth();
    if (deviceWidth < 800) {

    }
    else {

    }
}