const SELECT2_FLAG_ASSET_PATH = '/assets/Flags/';
function formatResultWithFlag(data) {
    if (!data.id && !data.children) {
        return data.text;
    }
    if (data.children) {
        const groupParts = data.text.split('##');
        const flagFileName = groupParts[0] ? groupParts[0].trim() : '';
        const countryName = groupParts.length > 1 ? groupParts[1].trim() : data.text;

        let headerContent = `<div class="select2-group-header">`;

        if (flagFileName && flagFileName.toLowerCase() !== 'null' && flagFileName !== '') {
            const flagPath = SELECT2_FLAG_ASSET_PATH + flagFileName;
            headerContent += `<div class="flag-dropdown-icon">
                                  <img src="${flagPath}" alt="" />
                              </div>`;
        }
        headerContent += `<span>${countryName}</span>`;
        headerContent += `</div>`;

        return $(headerContent);
    }
    let portName = data.portName || data.text;
    let portCode = data.portCode;

    let content = `<div class="select2-child-item">
                     <div class="select2-child-item-content">
                       <span>${portName}</span>`;

    if (portCode) {
        content += `<span class="select2-port-code">${portCode}</span>`;
    }
    content += `</div></div>`;

    return $(content);
}

function formatPortSelection(data) {
    if (!data.id) {
        return data.text || 'Select Port';
    }
    let portName = data.portName;
    let portCode = data.portCode;
    let countryName = data.countryName;
    let flagFileName = data.flag;

    if (!portName) {
        const parts = data.text.split('|');
        if (parts.length >= 4) {
            portName = parts[0].trim();
            portCode = parts[1].trim();
            countryName = parts[2].trim();
            flagFileName = parts[3].trim();
        } else if (parts.length > 1) {
            portName = parts[0].trim();
            portCode = parts[1].trim();
        } else {
            portName = data.text.trim();
        }
    }
    const combinedName = countryName ? `${portName}, ${countryName}` : portName;
    let html = `<div class="select2-custom-selection selected">`;

    if (flagFileName && flagFileName.toLowerCase() !== 'null' && flagFileName.trim() !== '') {
        const flagPath = SELECT2_FLAG_ASSET_PATH + flagFileName.trim();
        html += `<div class="select2-icon-wrapper flag-icon-wrapper">
                    <img src="${flagPath}" alt="${countryName}" />
                 </div>`;
    } else {
        html += `<div class="select2-icon-wrapper selected-default-icon">
                    <i class="ki-outline ki-geolocation"></i> 
                 </div>`;
    }

    html += `<div class="select2-text-stack">
                <span class="select2-main-text">${combinedName}</span>`;

    if (portCode) {
        html += `<span class="select2-sub-text">${portCode}</span>`;
    }
    html += `</div></div>`;

    return $(html);
}

function matchCustomWithFlag(params, data) {
    if ($.trim(params.term) === '') return data;
    if (typeof data.text === 'undefined') return null;

    const searchTerm = params.term.toUpperCase();

    if (data.children) {
        const groupParts = data.text.toUpperCase().split('##');
        const countryName = groupParts.length > 1 ? groupParts[1].trim() : data.text.toUpperCase();

        if (countryName.indexOf(searchTerm) > -1) return data;

        var matchingChildren = [];
        $.each(data.children, function (idx, child) {
            var pName = (child.portName || '').toUpperCase();
            var pCode = (child.portCode || '').toUpperCase();

            if (pName.indexOf(searchTerm) > -1 || pCode.indexOf(searchTerm) > -1) {
                matchingChildren.push(child);
            }
        });

        if (matchingChildren.length > 0) {
            var modifiedData = $.extend({}, data, true);
            modifiedData.children = matchingChildren;
            return modifiedData;
        }
    }
    else {
        var pName = (data.portName || '').toUpperCase();
        var pCode = (data.portCode || '').toUpperCase();
        var cName = (data.countryName || '').toUpperCase();

        if (pName.indexOf(searchTerm) > -1 || pCode.indexOf(searchTerm) > -1 || cName.indexOf(searchTerm) > -1) return data;
    }
    return null;
}