function SelectVendor(rfguid) {
    $('input[data-rfg="' + rfguid + '"]:not(:disabled)').prop("checked", true);
}

function OpenTaxesAndChargesModal(comparisonuuid, chargeuuid) {
    blockui();
    $.ajax({
        url: '/Purchase/GetTaxesAndChargesModal?comparisonuuid=' + comparisonuuid + '&chargeuuid=' + chargeuuid,
        type: 'GET',
        //dataType: "json",
        //contentType: "application/json",
        success: function (data) {
            unblockui();
            var findvalue = "[TAX]#";
            $("#modalsection").html(data);
            $("#modalsection").find(".select2").select2();
            $("#modalsection").find(".select3").select2({
                matcher: function (params, data) {
                    if (!data.text) return null;
                    if (!data.text.startsWith(findvalue) && data.id != "") {
                        return null;
                    }
                    return data;
                },
                templateSelection: function (data) {
                    if (data.text) {
                        return data.text.split('#')[1] || data.text;  // Show after "#"
                    }
                    return data.text;
                },
                templateResult: function (data) {
                    if (data.loading) return data.text;
                    return data.text.split('#')[1] || data.text;  // Show after "#" in dropdown
                }
            });

            $("#modalsection").find(".select4").select2({
                matcher: function (params, data) {
                    if (!data.text) return null;
                    if (!data.text.startsWith(findvalue) && data.id.length > 5) {
                        return null;
                    }
                    return data;
                },
                templateSelection: function (data) {
                    if (data.text) {
                        return data.text.split('#')[1] || data.text;  // Show after "#"
                    }
                    return data.text;
                },
                templateResult: function (data) {
                    if (data.loading) return data.text;
                    return data.text.split('#')[1] || data.text;  // Show after "#" in dropdown
                }
            });

            const ModalAddTaxesAndChargesModel = document.querySelector('#ModalAddTaxesAndChargesModel');
            const modal = KTModal.getInstance(ModalAddTaxesAndChargesModel);
            modal.show();
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Cannot Open Charges");
        }
    });
}

function ChangePercentage(current) {
    if ($(current).is(":checked")) {
        $("#div_percentagevalue").show();
        $("#div_percentageof").show();
        /*$("#div_rfqlist").hide();*/
        $("#th_percentage").html("Percentage");
    }
    else {
        $("#div_percentagevalue").hide();
        $("#div_percentageof").hide();
        /*$("#div_rfqlist").show();*/
        $("#th_percentage").html("Amount");
    }
}

function ChooseCharge(current) {
    var selectedText = $(current).find('option:selected').text();
    if ($(current).val() == "") {
        $("#div_chargename").show();
    }
    else {
        $("#div_chargename").hide();
    }

    if (selectedText.trim().endsWith("(%)")) {
        $("#chk_ispecentage").prop("checked", true);
    }
    else {
        $("#chk_ispecentage").prop("checked", false);
    }
    ChangePercentage($("#chk_ispecentage"));
}

function SaveTaxesAndCharges() {
    var validated = $("#formAddTaxesAndChargesModel").Validate();
    if (validated) {
        blockui();
        var formdata = $("#formAddTaxesAndChargesModel").serializeArray();
        $.ajax({
            url: '/Purchase/SaveTaxesAndCharges',
            data: formdata,
            type: 'POST',
            //dataType: "json",
            //contentType: "application/json",
            success: function (data) {
                unblockui();
                if (data.ResultId == 1) {
                    SuccessMessage("Charges Updated Successfully!");
                    setTimeout(
                        function () {
                            window.location = window.location.href;
                        }, 500);
                }
                else {
                    ErrorMessage(data.ResultMessage);
                }
            },
            error: function (data) {
                unblockui();
                ErrorMessage("Cannot Updated Charges");
            }
        });
    }
    else {
        ErrorMessage("Some of required items needs to be filled");
    }
}

$("html").on("keyup keypress change", '#txt_percentagevalue', function () {
    $("#div_rfqlist").find(".amount").val($(this).val());
});


function DeleteTaxesAndCharges(chargeuuid, comparisonuuid) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            blockui();
            $.ajax({
                url: '/Purchase/DeleteTaxesAndCharges',
                data: 'chargeuuid=' + chargeuuid + '&comparisonuuid=' + comparisonuuid,
                type: 'POST',
                //dataType: "json",
                //contentType: "application/json",
                success: function (data) {
                    unblockui();
                    if (data.ResultId == 1) {
                        SuccessMessage("Deleted Successfully!");
                        setTimeout(
                            function () {
                                window.location = window.location.href;
                            }, 500);
                    }
                    else {
                        ErrorMessage(data.ResultMessage);
                    }
                },
                error: function (data) {
                    unblockui();
                    ErrorMessage("Cannot Delete Charges");
                }
            });
        }
    });
}

function ShowHideTaxCharges(chargetype) {
    var findvalue = "[TAX]#";
    var findvalue2 = "[TAX]#";
    if (chargetype == 2) {
        findvalue = "[OTHERS]#";
        findvalue2 = "";
    }

    $('#txt_chargeuuid').select2({
        matcher: function (params, data) {
            if (!data.text) return null;

            // Hide if starts with 002 or 003
            if (!data.text.startsWith(findvalue) && data.id != "") {
                return null;
            }
            return data;
        },
        templateSelection: function (data) {
            if (data.text) {
                return data.text.split('#')[1] || data.text;  // Show after "#"
            }
            return data.text;
        },
        templateResult: function (data) {
            if (data.loading) return data.text;
            return data.text.split('#')[1] || data.text;  // Show after "#" in dropdown
        }
    });

    $('#txt_chargeuuid_target').select2({
        matcher: function (params, data) {
            if (!data.text) return null;

            // Hide if starts with 002 or 003
            if (!data.text.startsWith(findvalue2) && findvalue2 != "" && data.id.length > 5) {
                return null;
            }
            return data;
        },
        templateSelection: function (data) {
            if (data.text) {
                return data.text.split('#')[1] || data.text;  // Show after "#"
            }
            return data.text;
        },
        templateResult: function (data) {
            if (data.loading) return data.text;
            return data.text.split('#')[1] || data.text;  // Show after "#" in dropdown
        }
    });

    //$('#txt_chargeuuid option').each(function () {
    //    if ($(this).text().startsWith(findvalue)) {
    //        $(this).hide(); // Or use .hide() if you want to keep them in DOM
    //    }
    //});

    //$('#txt_chargeuuid').select2({
    //    templateSelection: function (data) {
    //        if (data.text) {
    //            return data.text.split('#')[1] || data.text;  // Show after "#"
    //        }
    //        return data.text;
    //    },
    //    templateResult: function (data) {
    //        if (data.loading) return data.text;
    //        return data.text.split('#')[1] || data.text;  // Show after "#" in dropdown
    //    }
    //});
}

function SaveQuoteComparision(comparisonuuid, sendforapproval) {
    $('#btnCompareSave').prop('disabled', true);
    blockui();
    let dataArray = [];
    $("#tablecomparision").find("tbody").find("tr").each(function () {
        var PIDetailID = $(this).find(".PIDetailID").val();
        var RFQDetailID = $(this).find(".RFQDetailID:checked").val();
        if (PIDetailID != undefined) {
            dataArray.push({ "PIDetailID": PIDetailID, "RFQDetailID": RFQDetailID });
        }

    });

    let formdata = JSON.stringify(dataArray);

    $.ajax({
        url: '/Purchase/CompareQuotes?comparisonuuid=' + comparisonuuid,
        data: formdata,
        type: 'POST',
        //dataType: "json",
        contentType: "application/json",
        success: function (data) {
            unblockui();
            if (data.ResultId == 1) {
                if (sendforapproval) {
                    OpenConfirmation(comparisonuuid);
                }
                else {
                    SuccessMessage("Saved Successfully!");
                    setTimeout(
                        function () {
                            window.location = window.location.href;
                        }, 500);
                }
            }
            else {
                ErrorMessage(data.ResultMessage);
            }
            $('#btnCompareSave').prop('disabled', false);
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Cannot Save Quote comparison");
            $('#btnCompareSave').prop('disabled', false);
        }
    });

}

function OpenRFQModal(rfqid) {
    blockui();
    $.ajax({
        url: '/Purchase/GetRFQModal?rfqid=' + rfqid,
        type: 'GET',
        //dataType: "json",
        //contentType: "application/json",
        success: function (data) {
            unblockui();
            $("#modalsection").html(data);
            const ModalRFQ = document.querySelector('#ModalRFQ');
            const modal = KTModal.getInstance(ModalRFQ);
            modal.show();
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Cannot Open  RFQ");
        }
    });
}

function OpenRFQReOrderModal(comparisonuuid) {
    blockui();
    $.ajax({
        url: '/Purchase/ReOrderRFQ?comparerfq=' + comparisonuuid,
        type: 'GET',
        //dataType: "json",
        //contentType: "application/json",
        success: function (data) {
            unblockui();
            $("#modalsection").html(data);

            new Sortable($("#modalsection").find('#sortable-list')[0], {
                animation: 150
            });
            const ModalReOrderRFQ = document.querySelector('#ModalReOrderRFQ');
            const modal = KTModal.getInstance(ModalReOrderRFQ);
            modal.show();
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Cannot Open Reorder RFQ");
        }
    });
}

function OpenRFQAddModal(RFQIDEnc, comparisonuuid) {
    blockui();
    $.ajax({
        url: '/Purchase/GetAddRFQ?piid=' + RFQIDEnc + '&comparerfq=' + comparisonuuid,
        type: 'GET',
        //dataType: "json",
        //contentType: "application/json",
        success: function (data) {
            unblockui();
            $("#modalsection").html(data);
            const ModalAddRFQToCompare = document.querySelector('#ModalAddRFQToCompare');
            const modal = KTModal.getInstance(ModalAddRFQToCompare);
            modal.show();
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Cannot Add RFQ");
        }
    });
}

function SaveRFQReOrder(comparisonuuid) {
    blockui();
    let dataArray = [];
    var index = 1;
    $("#sortable-list").find(".hdn_RFQUUID").each(function () {
        dataArray.push({ "RFQUUID": $(this).val(), "SortOrder": index++ });
    });

    let formdata = JSON.stringify(dataArray);

    $.ajax({
        url: '/Purchase/SaveReOrderRFQ?comparisonuuid=' + comparisonuuid,
        data: formdata,
        type: 'POST',
        //dataType: "json",
        contentType: "application/json",
        success: function (data) {
            unblockui();
            if (data.ResultId == 1) {
                SuccessMessage("Saved Successfully!");
                setTimeout(
                    function () {
                        window.location = window.location.href;
                    }, 500);
            }
            else {
                ErrorMessage(data.ResultMessage);
            }
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Cannot Order RFQs");
        }
    });

}


function AddRFQToComparison(comparisonuuid) {
    blockui();

    var rfqList = $("#formAddRFQ .RFQUUID:checked").map(function () {
        return $(this).val();
    }).get();

    if (rfqList.length > 0) {
        $.ajax({
            url: '/Purchase/AddRFQToComparison',
            data: {
                rfqlist: rfqList,
                comparisonuuid: comparisonuuid,
            },
            type: 'POST',
            //dataType: "json",
            //contentType: "application/json",
            success: function (data) {
                unblockui();
                if (data.ResultId == 1) {
                    //SuccessMessage("Quote Comparison created Successfully!");
                    setTimeout(
                        function () {
                            window.location = window.location.href;
                        }, 500);
                }
                else {
                    ErrorMessage(data.ResultMessage);
                }
            },
            error: function (data) {
                unblockui();
                ErrorMessage("Cannot Add RFQ");
            }
        });
    }
    else {
        unblockui();
        ErrorMessage("Please select atleast one RFQ to add");
    }
}

function RemoveRFQFromComparison(rfq_list, comparisonuuid) {
    Swal.fire({
        title: "Are you sure to delete?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            blockui();
            var rfqList = rfq_list.split(",");  // Replace with actual UUIDs
            $.ajax({
                url: '/Purchase/RemoveRFQFromComparison',
                data: {
                    rfqlist: rfqList,
                    comparisonuuid: comparisonuuid
                },
                type: 'POST',
                //dataType: "json",
                //contentType: "application/json",
                success: function (data) {
                    unblockui();
                    if (data.ResultId == 1) {
                        //SuccessMessage("Quote Comparison created Successfully!");
                        setTimeout(
                            function () {
                                window.location = window.location.href;
                            }, 500);
                    }
                    else {
                        ErrorMessage(data.ResultMessage);
                    }
                },
                error: function (data) {
                    unblockui();
                    ErrorMessage("Cannot Remove RFQ");
                }
            });
        }
    });
}


function OpenConfirmation(comparisonuuid) {
    blockui();
    $.ajax({
        url: '/Purchase/GetApproval?comparerfq=' + comparisonuuid,
        type: 'GET',
        //dataType: "json",
        //contentType: "application/json",
        success: function (data) {
            unblockui();
            $("#modalsection").html(data).find(".select2").select2();
            const ModalRFQSendApproval = document.querySelector('#ModalRFQSendApproval');
            const modal = KTModal.getInstance(ModalRFQSendApproval);
            modal.show();
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Cannot Open");
        }
    });
}


function SendForApproval(comparisonuuid) {
    if ($("#formSendApproval").Validate()) {
        blockui();
        var data = { "approver": $("#ddlApprovalUser").val(), "message_text": $("#txt_Comments").val(), "target_id": $("#hdn_comparisonid").val() };

        let formdata = JSON.stringify(data);

        $.ajax({
            url: '/Purchase/SendForApproval?comparisonuuid=' + comparisonuuid,
            data: formdata,
            type: 'POST',
            //dataType: "json",
            contentType: "application/json",
            success: function (data) {
                unblockui();
                if (data.ResultId == 1) {
                    SuccessMessage("Saved Successfully!");
                    setTimeout(
                        function () {
                            if (data.ResultMessage == "") {
                                window.location = window.location.href;
                            }
                            else {
                                window.location = data.ResultMessage;
                            }
                        }, 500);
                }
                else {
                    ErrorMessage(data.ResultMessage);
                }
            },
            error: function (data) {
                unblockui();
                ErrorMessage("Cannot Order RFQs");
            }
        });
    }
}

function OpenApproveReject(comparisonuuid, isapprove) {
    blockui();
    $.ajax({
        url: '/Purchase/GetApproveReject?comparerfq=' + comparisonuuid + '&accept=' + (isapprove ? 1 : 2),
        type: 'GET',
        //dataType: "json",
        //contentType: "application/json",
        success: function (data) {
            unblockui();
            $("#modalsection").html(data).find(".select2").select2();
            const ModalApprovaReject = document.querySelector('#ModalApprovaReject');
            const modal = KTModal.getInstance(ModalApprovaReject);
            modal.show();
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Cannot Open");
        }
    });
}
function ApproveQuote(comparisonuuid) {
    if ($("#formApprovalReject").Validate()) {
        var approver = $("#ddlApprovalUser").val();
        if (approver == undefined)
            approver = "";
        var data = {
            "approver": approver,
            "comparisonuuid": comparisonuuid,
            "isaccept": true,
            "comments": $("#ModalApprovaReject").find("#txt_Comments").val(),
            "comparisonid": $("#hdn_comparisonid").val()
        };
        ApproveRejectQuote(data);
    }
}

function RejectQuote(comparisonuuid) {
    if ($("#formApprovalReject").Validate()) {
        var data = {
            "comparisonuuid": comparisonuuid,
            "isaccept": false,
            "comments": $("#ModalApprovaReject").find("#txt_Comments").val(),
            "comparisonid": $("#hdn_comparisonid").val()
        };
        ApproveRejectQuote(data);
    }
}

function ApproveRejectQuote(data) {
    blockui();
    let formdata = JSON.stringify(data);
    $.ajax({
        url: '/Purchase/ApproveReject',
        data: formdata,
        type: 'POST',
        //dataType: "json",
        contentType: "application/json",
        success: function (data) {
            unblockui();
            if (data.ResultId == 1) {
                SuccessMessage("Saved Successfully!");
                setTimeout(
                    function () {
                        if (data.ResultMessage == "") {
                            window.location = window.location.href;
                        }
                        else {
                            window.location = data.ResultMessage;
                        }
                    }, 500);
            }
            else {
                ErrorMessage(data.ResultMessage);
            }
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Cannot Update Quote");
        }
    });
}

function DeleteQuoteComparison(compareuuid) {
    Swal.fire({
        title: "Are you sure to delete?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            blockui();
            $.ajax({
                url: '/Purchase/DeleteQuoteComparison?comparisonuuid=' + compareuuid,
                type: 'GET',
                //dataType: "json",
                //contentType: "application/json",
                success: function (data) {
                    unblockui();
                    if (data.ResultId == 1) {
                        SuccessMessage("Deleted Successfully!");
                        setTimeout(
                            function () {
                                window.location = window.location.href;
                            }, 500);
                    }
                    else {
                        ErrorMessage(data.ResultMessage);
                    }
                },
                error: function (data) {
                    unblockui();
                    ErrorMessage("Cannot Delete comparison");
                }
            });
        }
    });
}

function UpdateConversions() {
    blockui();
    var formdata = $("#formQuoteConversions").serializeArray();
    $.ajax({
        url: '/Purchase/UpdateConversions',
        data: formdata,
        type: 'POST',
        //dataType: "json",
        /*contentType: "application/json",*/
        success: function (data) {
            unblockui();
            SuccessMessage("Updated Successfully!");
            setTimeout(
                function () {
                    window.location = window.location.href;
                }, 500);
        },
        error: function (data) {
            unblockui();
            ErrorMessage("Cannot Update Conversions");
        }
    });
}

$(document).on("keyup keypress change", "#formQuoteConversions .convrate", function (e) {
    var convvalue = $(this).val();
    $(this).closest("tr").find(".converted").html(convvalue);
});
