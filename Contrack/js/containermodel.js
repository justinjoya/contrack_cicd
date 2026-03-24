(function ($) {
    $.fn.containermodelSelect2 = function (options) {
        // Default options
        var settings = $.extend({
            url: '/ContainerModel/GetModels',
            minLength: 1,
            allowClear: false,
            multiline: false,
            placeholder: 'Select',
        }, options);

        return this.each(function () {
            var $ddlcontainermodel = $(this);
            $ddlcontainermodel.select2({
                minimumInputLength: settings.minLength,
                allowClear: settings.allowClear,
                ajax: {
                    url: settings.url,
                    dataType: 'json',
                    delay: 250,
                    data: function (params) {
                        return { q: params.term };
                    },
                    processResults: function (data) {
                        return { results: data.results || data };
                    },
                    cache: true
                },
                templateResult: formatOption,
                templateSelection: function (data) {
                    if (settings.multiline) {
                        return formatSelectionmultiline(data, settings);
                    } else {
                        return formatSelection(data, settings);
                    }
                },
                escapeMarkup: function (m) { return m; }
            });

            // Load saved values if available
            var savedmodel = {
                id: $ddlcontainermodel.attr("datamodeluuid"),
                text: $ddlcontainermodel.attr("datamodelname"),
                isocode: $ddlcontainermodel.attr("dataisocode"),
            };

            if (savedmodel.id) {
                var text = savedmodel.text + "||" + savedmodel.isocode;
                var newOption = new Option(text, savedmodel.id, true, true);
                $ddlcontainermodel.append(newOption).trigger('change');

                $ddlcontainermodel.trigger({
                    type: 'select2:select',
                    params: { data: savedmodel }
                });
            }
        });
    };
    function formatOption(option) {
        if (!option.id) return option.text;
        if (option.children) {
            return `<div class="select-containertype">
                <span>${option.text}</span>
            </div>`;
        }
        // Port item
        return `<div class="select-containermodel">
            <span class="modelname">${option.text}</span>
            <span class="isocode">ISO : ${option.isocode}</span>
        </div>`;
    }


    function formatSelectionmultiline(option, settings) {
        if (!option.id)
            return option.text;
        else if (option.text.indexOf("||") >= 0) {
            var splitarray = option.text.split("||");
            if (splitarray.length >= 2) {
                option.text = splitarray[0];
                option.isocode = splitarray[1];
            }
            else
                return option.text;
        }
        return `<span class='flex items-center gap-3'>
                    <span style='color:#5F5C55;'>${option.text} (${option.isocode})</span>
            </span>`;
    }
    function formatSelection(option, settings) {
        if (!option.id) return option.text;
        if (option.text.indexOf("||") >= 0) {
            var splitarray = option.text.split("||");
            if (splitarray.length >= 2) {
                option.text = splitarray[0];
                option.flag = splitarray[1];
                option.portCode = splitarray[2];
            }
            else
                return option.text;
        }
        return `<div class="selected-port">
                <img src="${option.flag}" class="flag" />
                <span>${option.text}` + (option.portCode != "" ? ` (${option.portCode})` : "") + ` </span>
            </div>`;
    }
})(jQuery);



