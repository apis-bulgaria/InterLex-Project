// Author: Stephen Korecky
// Website: http://stephenkorecky.com
// Plugin Website: http://github.com/skorecky/Add-Clear

;(function($, window, document, undefined) {

	// Create the defaults once
	var pluginName = "addClear",
		defaults = {
		    closeSymbol: '<span class="x-grey-on-white"></span>',
			color: "#CCC",
			top: 1,
			right: 0,
			returnFocus: true,
			showOnLoad: true,
			onClear: null,
			hideOnBlur: false,
			inputClassifierClass: 'clearable-classifier',
			inputEnactmentClass: 'clearable-enactment',
            inputProvisionClass: 'clearable-provision'
		};

	// The actual plugin constructor
	function Plugin(element, options) {
		this.element = element;

		this.options = $.extend({}, defaults, options);

		this._defaults = defaults;
		this._name = pluginName;

		this.init();
	}

	Plugin.prototype = {

		init: function() {
			var $this = $(this.element),
					me = this,
					options = this.options;

			$this.wrap("<span style='position:relative;' class='add-clear-span'></span>");
			$this.after($("<a href='#clear' style='display: none; margin-right: -5px;'>" + options.closeSymbol + "</a>"));
			$this.next().css({
				color: options.color,
				'text-decoration': 'none',
				display: 'none',
				'line-height': 1,
				overflow: 'hidden',
				position: 'absolute',
				right: options.right,
				top: options.top
			}, this);

			if ($this.val().length >= 1 && options.showOnLoad === true) {
				$this.siblings("a[href='#clear']").show();
			}

			$this.focus(function() {
				if ($(this).val().length >= 1) {
					$(this).siblings("a[href='#clear']").show();
				}
			});

			$this.blur(function() {
				var self = this;

				if (options.hideOnBlur) {
					setTimeout(function() {
						$(self).siblings("a[href='#clear']").hide();
					}, 50);
				}
			});

			$this.keyup(function() {
				if ($(this).val().length >= 1) {
					$(this).siblings("a[href='#clear']").show();
				} else {
					$(this).siblings("a[href='#clear']").hide();
				}
			});

			$("a[href='#clear']").click(function(e) {
			    $(this).siblings(me.element).val("");

			    if ($(this).siblings(me.element).hasClass(options.inputClassifierClass)) {
			        try {
			            var curId = $(this).siblings((me.element)).attr('id');
			            var match = curId.match("tb(.*)Text");
			            var extractedId = match[1];
			            clearTreeFilter(extractedId);
			        }
			        catch (e) {
			            console.log(e.message);
			        }
			    }

			    if ($(this).siblings(me.element).hasClass(options.inputEnactmentClass)) {
			        $('#Cases_EnactmentText').val('');
			        $('#Cases_EnactmentDocLangId').val('');
			        $('#Cases_ProvisionText').val('');
			        $('#Cases_ProvisionId').val('');
			    }

			    if ($(this).siblings(me.element).hasClass(options.inputProvisionClass)) {
			        if ($('#Cases_EnactmentDocLangId').val() != '') {
			            $('#Cases_ProvisionText').val('All provisions');
			            $('#Cases_ProvisionText').attr('title', 'All provisions');
			        }
			        else {
			            $('#Cases_ProvisionText').val('');
			            $('#Cases_ProvisionText').attr('');
			        }

			        $('#Cases_ProvisionId').val('');
			    }

				$(this).hide();
				if (options.returnFocus === true) {
					$(this).siblings(me.element).focus();
				}
				if (options.onClear) {
					options.onClear($(this).siblings("input"));
				}
				e.preventDefault();
			});
		}

	};

	$.fn[pluginName] = function(options) {
		return this.each(function() {
			if (!$.data(this, "plugin_" + pluginName)) {
				$.data(this, "plugin_" + pluginName,
					new Plugin(this, options));
			}
		});
	};

})(jQuery, window, document);
