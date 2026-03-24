(function ($) {
    $.fn.customerSelect2 = function (options) {
        // Default options
        var settings = $.extend({
            url: '/Client/GetCustomers',
            minLength: 1,
            allowClear: false,
            multiline: false,
            placeholder: 'Select',
        }, options);

        return this.each(function () {
            var $ddlcustomer = $(this);
            $ddlcustomer.select2({
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
            var savedcustomer = {
                id: $ddlcustomer.attr("dataid"),
                text: $ddlcustomer.attr("dataname"),
            };

            if (savedcustomer.id) {
                var newOption = new Option(savedcustomer.text, savedcustomer.id, true, true);
                $ddlcustomer.append(newOption);//.trigger('change');

                $ddlcustomer.trigger({
                    type: 'select2:select',
                    params: { data: savedcustomer }
                });
            }
        });
    };
    function formatOption(option) {
        if (!option.id) return option.text;
        return `<div class="selected-customer multiline">
               <span class='flex items-center gap-3'>
                    <span class="colorbox" style="color:${option.textcolor};background-color:${option.bgcolor}">${option.shortcode}</span>
                    <span class='flex flex-col content'>
                        <span class='maintext'>${option.text}</span>
                        <span class='subtextholder'>
                            <span class='subtext'>${option.address}</span>
                        </span>
                    </span>
                </span>
            </div>`;
    }


    function formatSelectionmultiline(option, settings) {
        if (!option.id)
            return option.text;
        //else if (option.text.indexOf("||") >= 0) {
        //    var splitarray = option.text.split("||");
        //    if (splitarray.length >= 3) {
        //        option.text = splitarray[0];
        //        option.textcolor = splitarray[1];
        //        option.bgcolor = splitarray[2];
        //        option.shortcode = splitarray[3];
        //    }
        //    else
        //        return option.text;
        //}
        return `<span class='flex items-center gap-3'>
                    <span style='color:#5F5C55;'>${option.text}</span>
            </span>`;
    }
    function formatSelection(option, settings) {
        return option.text;
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



