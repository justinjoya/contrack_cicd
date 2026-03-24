//Importance Sequance(Top to low) - Email,Required  (Top priority attribute will be shown)
//datarequired="yes" - Required
//datarequired-message
//datatype
var Valid = true;
(function ($) {
    $.fn.ResetForm = function () {
        $(this).find('input[type="text"],input[type="number"],input[type="password"],select,textarea').each(function () {
            $(this).val('');
        });
        $(this).find('input[type="checkbox"],input[type="radio"]').each(function () {
            $(this).attr('checked', false);
            $(this).prop('checked', false);
        });
    }
    $.fn.Validate = function () {
        Valid = true;
        var lastelement;
        $(this).find('input[type="hidden"][datavalidate="yes"],input[type="text"][datatype="email"],input[type="text"][datatype="number"]:visible,input[type="text"][datarequired="yes"]:visible,input[type="number"][datarequired="yes"]:visible,input[type="password"][datarequired="yes"]:visible,select[datarequired="yes"]:visible,select[datarequired="yes"][datatype="select2"],textarea[datarequired="yes"]:visible,input[type="file"][datarequired="yes"]:visible').each(function () {
            var inputvalue = $.trim($(this).val());
            var inpType = $(this).attr("type");
            var datarequired = $(this).attr("datarequired");
            var reqMessage = $(this).attr('datarequired-message');
            var appendparent = $(this).attr('data-append-parent');
            var datatype = $(this).attr('datatype');
            if (appendparent == "yes")
                $(this).parent().next('span.errordisp').remove();
            else
                $(this).next('span.errordisp').remove();

            if (datatype == "email") {
                if (!ValidateEmail(inputvalue) && (datarequired == "yes" || (inputvalue != ""))) {
                    Valid = false;
                    if (reqMessage == "" || reqMessage == undefined) {
                        if (appendparent == "yes")
                            $(this).parent().after('<span class="errordisp" style="color:red;display:block;">Invalid Email format</span>');
                        else
                            $(this).after('<span class="errordisp" style="color:red;display:block;">Invalid Email format</span>');
                    }
                    else {
                        if (appendparent == "yes")
                            $(this).parent().after('<span class="errordisp" style="color:red;display:block;">' + reqMessage + '</span>');
                        else
                            $(this).after('<span class="errordisp" style="color:red;display:block;">' + reqMessage + '</span>');
                    }
                    lastelement = $(this);
                }
            }
            else if (datatype == "emailphone") {
                if (!ValidateEmail(inputvalue) && !ValidatePhone(inputvalue) && (datarequired == "yes" || (inputvalue != ""))) {
                    Valid = false;
                    if (reqMessage == "" || reqMessage == undefined) {
                        if (appendparent == "yes")
                            $(this).parent().after('<span class="errordisp" style="color:red;display:block;">Invalid Email format</span>');
                        else
                            $(this).after('<span class="errordisp" style="color:red;display:block;">Invalid Email format</span>');
                    }
                    else {
                        if (appendparent == "yes")
                            $(this).parent().after('<span class="errordisp" style="color:red;display:block;">' + reqMessage + '</span>');
                        else
                            $(this).after('<span class="errordisp" style="color:red;display:block;">' + reqMessage + '</span>');
                    }
                    lastelement = $(this);
                }
            }

            else if (datatype == "number") {
                if (datarequired == "yes" || (inputvalue != "")) {
                    var maxlen = $(this).attr('maxlength');
                    var minlen = $(this).attr('dataminlength');
                    var inputlength = inputvalue.length;
                    if (minlen != "" && minlen != undefined) {
                        if (inputlength < minlen) {
                            Valid = false;
                            if (reqMessage == "" || reqMessage == undefined) {
                                if (appendparent == "yes")
                                    $(this).parent().after('<span class="errordisp" style="color:red;display:block;">Lesser value than required minimum length</span>');
                                else
                                    $(this).after('<span class="errordisp" style="color:red;display:block;">Lesser value than required minimum length</span>');
                            }
                            else {
                                if (appendparent == "yes")
                                    $(this).parent().after('<span class="errordisp" style="color:red;display:block;">' + reqMessage + '</span>');
                                else
                                    $(this).after('<span class="errordisp" style="color:red;display:block;">' + reqMessage + '</span>');
                            }
                        }

                    }
                    //else if (maxlen != "" && maxlen != undefined) {
                    //    if (inputlength != minlen) {
                    //        Valid = false;
                    //        if (reqMessage == "" || reqMessage == undefined) {
                    //            if (appendparent == "yes")
                    //                $(this).parent().after('<span class="errordisp" style="color:red;display:block;">Lesser value than required maximum length</span>');
                    //            else
                    //                $(this).after('<span class="errordisp" style="color:red;display:block;">Lesser value than required maximum length</span>');
                    //        }
                    //        else {
                    //            if (appendparent == "yes")
                    //                $(this).parent().after('<span class="errordisp" style="color:red;display:block;">' + reqMessage + '</span>');
                    //            else
                    //                $(this).after('<span class="errordisp" style="color:red;display:block;">' + reqMessage + '</span>');
                    //        }
                    //    }
                    //}
                    lastelement = $(this);
                }
            }
            else if (inputvalue == "") {
                Valid = false;

                if (reqMessage == "" || reqMessage == undefined) {
                    if (appendparent == "yes")
                        $(this).parent().after('<span class="errordisp" style="color:red;display:block;">This field is required</span>');
                    else
                        $(this).after('<span class="errordisp" style="color:red;display:block;">This field is required</span>');
                }
                else {
                    if (appendparent == "yes")
                        $(this).parent().after('<span class="errordisp" style="color:red;display:block;">' + reqMessage + '</span>');
                    else
                        $(this).after('<span class="errordisp" style="color:red;display:block;">' + reqMessage + '</span>');
                }
                lastelement = $(this);

            }

        });
        $(this).find('input[type="text"][datatype="confirmpass"]:visible,input[type="password"][datatype="confirmpass"]:visible').each(function () {
            var inputvalue = $.trim($(this).val());
            var reqMessage = $(this).attr('datarequired-message');
            var appendparent = $(this).attr('data-append-parent');
            var datamatchid = $(this).attr('datamatchid');
            var datamatchidvalue = $.trim($("#" + datamatchid).val());
            if (appendparent == "yes")
                $(this).parent().next('span.errordisp').remove();
            else
                $(this).next('span.errordisp').remove();
            if (datamatchidvalue != "") {
                if (inputvalue == "") {
                    Valid = false;
                    if (reqMessage == "" || reqMessage == undefined) {
                        if (appendparent == "yes")
                            $(this).parent().after('<span class="errordisp" style="color:red;display:block;">Please enter confirm password</span>');
                        else
                            $(this).after('<span class="errordisp" style="color:red;display:block;">Please enter confirm password</span>');
                    }
                    else {
                        if (appendparent == "yes")
                            $(this).parent().after('<span class="errordisp" style="color:red;display:block;">' + reqMessage + '</span>');
                        else
                            $(this).after('<span class="errordisp" style="color:red;display:block;">' + reqMessage + '</span>');
                    }
                    lastelement = $(this);
                }
                else if (datamatchidvalue != inputvalue) {
                    Valid = false;
                    if (reqMessage == "" || reqMessage == undefined) {
                        if (appendparent == "yes")
                            $(this).parent().after('<span class="errordisp" style="color:red;display:block;">Confirm password not matched</span>');
                        else
                            $(this).after('<span class="errordisp" style="color:red;display:block;">Confirm password not matched</span>');
                    }
                    else {
                        if (appendparent == "yes")
                            $(this).parent().after('<span class="errordisp" style="color:red;display:block;">' + reqMessage + '</span>');
                        else
                            $(this).after('<span class="errordisp" style="color:red;display:block;">' + reqMessage + '</span>');
                    }
                    lastelement = $(this);
                }
            }
        });

        $(this).find('input[type="checkbox"][datarequired="yes"]:visible,input[type="radio"][datarequired="yes"]:visible').each(function () {
            var CurrentName = $(this).attr("name");
            var reqMessage = $(this).attr('datarequired-message');
            var appendparent = $(this).attr('data-append-parent');
            if (appendparent == "yes")
                $("input[name='" + CurrentName + "']:last").parent().next('span.errordisp').remove();
            else
                $("input[name='" + CurrentName + "']:last").next('span.errordisp').remove();

            if (!$("input[name='" + CurrentName + "']").is(":checked")) {
                if (reqMessage == "" || reqMessage == undefined) {
                    Valid = false;
                    if (appendparent == "yes")
                        $("input[name='" + CurrentName + "']:last").parent().after('<span class="errordisp" style="color:red;display:block;">Please select any option</span>');
                    else
                        $("input[name='" + CurrentName + "']:last").after('<span class="errordisp" style="color:red;display:block;">Please select any option</span>');
                }
                else {
                    Valid = false;
                    if (appendparent == "yes")
                        $("input[name='" + CurrentName + "']:last").parent().after('<span class="errordisp" style="color:red;display:block;">' + reqMessage + '</span>');
                    else
                        $("input[name='" + CurrentName + "']:last").after('<span class="errordisp" style="color:red;display:block;">' + reqMessage + '</span>');
                }
                lastelement = $(this);
            }
        });
        try {
            if (!Valid)
                lastelement.focus();
        } catch (e) {

        }

        return Valid;
    }
}(jQuery));


function ValidateEmail(strValue) {
    var objRegExp = /^[a-z0-9]([a-z0-9_\-\.]*)@([a-z0-9_\-\.]*)(\.[a-z]{2,3}(\.[a-z]{2}){0,2})$/i;
    //check for valid email
    return objRegExp.test(strValue);
}

function ValidatePhone(strValue) {
    if (!isNaN(strValue) && strValue.length >= 10) {
        return true;
    }
    else {
        return false;
    }

    var objRegExp = /^[0-9]$/i;
    //check for valid email
    return objRegExp.test(strValue);
}


$("html").on("keypress", 'input[type="text"][datarequired="yes"],input[type="number"][datarequired="yes"],input[type="password"][datarequired="yes"],textarea[datarequired="yes"]', function () {
    $(this).next('span.errordisp').remove();
    $(this).parent().next('span.errordisp').remove();
});

$("html").on("change", 'select[datarequired="yes"],textarea[datarequired="yes"],input[type="file"][datarequired="yes"]', function () {
    $(this).next('span.errordisp').remove();
    $(this).parent().next('span.errordisp').remove();
});

$("html").on("click", 'input[type="checkbox"][datarequired="yes"],input[type="radio"][datarequired="yes"]', function () {
    var CurrentName = $(this).attr("name");
    $("input[name='" + CurrentName + "']:last").next('span.errordisp').remove();
    $("input[name='" + CurrentName + "']:last").parent().next('span.errordisp').remove();
});

$("html").on("keypress", 'input[type="number"],input[datatype="number"]', function (e) {
    if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
        return false;
    }
    if (e.which != 8 && e.which != 0) {
        if ($(this).attr("maxlength") != undefined) {
            var ctrlVal = $.trim($(this).val());
            if (ctrlVal.length > ($(this).attr("maxlength") - 1))
                return false;
        }
    }
});

$("html").on("keypress", 'input[datatype="textonly"]', function (evt) {
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if ((charCode > 64 && charCode < 91) || (charCode >= 96 && charCode < 123) || charCode == 46 || charCode == 8 || charCode == 9 || charCode == 32) {
        return true;
    }
    return false;
});

$("html").on("keyup keypress change", 'input[datatype="textonly"]', function () {
    var pattern = /^[a-zA-Z ]+$/;
    var txtval = $(this).val();
    if (!pattern.test(txtval)) {
        $(this).val($(this).val().replace(/[^a-zA-Z ]+/g, ''))
    }
});

$("html").on("keyup keypress change", 'input[datatype="textwithcomma"]', function () {
    var pattern = /^[a-zA-Z, ]+$/;
    var txtval = $(this).val();
    if (!pattern.test(txtval)) {
        $(this).val($(this).val().replace(/[^a-zA-Z, ]+/g, ''))
    }
});

$("html").on("keyup keypress change", 'input[datatype="textnumericcomma"]', function () {
    var pattern = /^[a-zA-Z0-9, ]+$/;
    var txtval = $(this).val();
    if (!pattern.test(txtval)) {
        $(this).val($(this).val().replace(/[^a-zA-Z0-9, ]+/g, ''))
    }
});

$("html").on("keyup keypress change", 'input[datatype="textnumeric"]', function () {
    var pattern = /^[a-zA-Z0-9 ]+$/;
    var txtval = $(this).val();
    if (!pattern.test(txtval)) {
        $(this).val($(this).val().replace(/[^a-zA-Z0-9 ]+/g, ''))
    }
});

$("html").on("keyup keypress change", 'input[datatype="date"]', function () {
    var pattern = /^[0-9\-\/: ]+$/;
    var txtval = $(this).val();
    if (!pattern.test(txtval)) {
        $(this).val($(this).val().replace(/[^0-9\-\/: ]+/g, ''))
    }
});

$("html").on("keyup keypress change", 'input[type="number"],input[datatype="number"]', function () {
    var pattern = /^[0-9]+$/;
    var txtval = $(this).val();
    if (!pattern.test(txtval)) {
        $(this).val($(this).val().replace(/[^0-9]+/g, ''))
    }
});

$("html").on("keyup keypress change", 'input[datatype="decimal"]', function () {
    var pattern = /^[0-9\.]+$/;
    var txtval = $(this).val();
    if (!pattern.test(txtval)) {
        $(this).val($(this).val().replace(/[^0-9\.]+/g, ''))
    }
});
$("html").on("keyup keypress change", 'input[datatype="phone"]', function () {
    var pattern = /^[0-9+]+$/; 
    var txtval = $(this).val();
    if (!pattern.test(txtval)) {
        $(this).val($(this).val().replace(/[^0-9+]+/g, ''))
    }
});

