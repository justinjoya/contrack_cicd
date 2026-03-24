(function ($) {
    $.fn.portSelect2 = function (options) {
        // Default options
        var settings = $.extend({
            url: '/Location/GetPorts',
            //showicon: false,
            icon: flagpath + 'pod.png',
            minLength: 1,
            allowClear: false,
            multiline: false,
            placeholder: 'Select',
            subtextplaceholder: 'N/A'
        }, options);

        return this.each(function () {
            var $ddlPort = $(this);
            $ddlPort.select2({
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
            var savedPort = {
                id: $ddlPort.attr("dataportid"),
                text: $ddlPort.attr("dataportname"),
                flag: $ddlPort.attr("dataflag"),
                portCode: $ddlPort.attr("dataportcode"),
                countryname: $ddlPort.attr("datacountryname"),
            };

            if (savedPort.id) {
                var text = savedPort.text + "||" + savedPort.flag + "||" + savedPort.portCode + "||" + savedPort.countryname;
                var newOption = new Option(text, savedPort.id, true, true);
                $ddlPort.append(newOption).trigger('change');

                $ddlPort.trigger({
                    type: 'select2:select',
                    params: { data: savedPort }
                });
            }
        });
    };


    function formatOption(option) {
        // Country group header
        if (option.children) {
            return `
            <div class="select-country">
                <img src="${option.flag}" class="flag" />
                <strong>${option.text}</strong>
            </div>
        `;
        }
        // Port item
        return `
        <div class="select-port" style="">
            <span class="portname">${option.text}</span>
            <span class="portcode" style="">${option.portCode}</span>
        </div>
    `;
    }


    function formatSelectionmultiline(option, settings) {
        if (!option.id) {
            option.text = settings.placeholder;
            option.flag = settings.icon;
            option.portCode = settings.subtextplaceholder;
            option.country = "";
        }
        else if (option.text.indexOf("||") >= 0) {
            var splitarray = option.text.split("||");
            if (splitarray.length >= 3) {
                option.text = splitarray[0];
                option.flag = splitarray[1];
                option.portCode = splitarray[2];
                option.country = splitarray[3];
            }
            else
                return option.text;
        }
        if (option.portCode == "")
            option.portCode = "N/A";

        /*if (option.flag) { }*/

        return `<div class="selected-port multiline">
                <img src="${option.flag}" class="flag" />
                <span class='flex flex-col content'>
                    <span>${option.text}${option.country ? `, ` + option.country : ``}</span>
                    <span class='subtext'>${option.portCode}</span>
                </span>
            </div>`;
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

