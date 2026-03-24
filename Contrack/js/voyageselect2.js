(function ($) {
    $.fn.voyageSelect2 = function (options) {
        // Default options
        var settings = $.extend({
            url: '/Voyage/GetVoyageSearch',
            minLength: 1,
            createnew: true,
            allowClear: false,
            multiline: false,
            placeholder: 'Select',
        }, options);

        return this.each(function () {
            var $ddlvoyage = $(this);
            $ddlvoyage.select2({
                minimumInputLength: settings.minLength,
                allowClear: settings.allowClear,
                ajax: {
                    url: settings.url + '?addnew=' + (settings.createnew ? "1" : "2"),
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
                    return formatSelectionmultiline(data, settings);
                },
                escapeMarkup: function (m) { return m; }
            });

            // Load saved values if available
            var savedvoyage = {
                id: $ddlvoyage.attr("dataid"),
                text: $ddlvoyage.attr("dataname"),
                vesselName: $ddlvoyage.attr("datavesselName"),
            };

            if (savedvoyage.id) {
                var newOption = new Option(savedvoyage.text, savedvoyage.id, true, true);
                $ddlvoyage.append(newOption);//.trigger('change');

                $ddlvoyage.trigger({
                    type: 'select2:select',
                    params: { data: savedvoyage }
                });
            }
        });
    };
    function formatOption(option) {
        if (!option.id) return option.text;
        return `<div class="selected-customer multiline">
               <span class='flex items-center gap-3'>
                    <span class='flex flex-col content'>
                        <span class='maintext'>${option.text}</span>
                        <span class='subtextholder'>
                            <span class='subtext'>${option.vesselName}</span>
                        </span>
                    </span>
                </span>
            </div>`;
    }


    function formatSelectionmultiline(option, settings) {
        if (!option.id)
            return option.text;

        var vessel = option.vesselName ? ` - ${option.vesselName}` : '';

        return `<span class='flex items-center gap-3'>
                <span>${option.text}${vessel}</span>
            </span>`;
    }

})(jQuery);



