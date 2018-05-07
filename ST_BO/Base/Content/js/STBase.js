//'**********************************************************************
//'*** REGION NAME:Base.Js
//'***  DESCRIPTION:FOR ALL COMMON FUNCTION FOR BASICS AND LOGICAL CLASSES

///CONST FOR STUTIL FUNCTION--

function haseValue(p) {
    if (p == '' || p == undefined || p == null || p == '0') {
        return false;
    }
    else {
        return true;
    }

}
function haseTrue(p) {
    if (p == '' || p == undefined || p == null || p == false) {
        return false;
    }
    else {
        return true;
    }

}
STFormUtil = {};
STMessage = {};
STMessage.CreateMessage = CreateMessage = "record Inserted Successfully.";
STMessage.UpdateMessage = "record updated Successfully.";
STMessage.DeleteMessage = "record deleted Successfully.";
STMessage.ConfirmDeleteMessage = "Are you sure want to delete?";
STMessage.success = "success";
STMessage.DeleteText = "You will not be able to recover this imaginary file!";

//For Clear Options------
//DESCRIPTION:FOR CLEAR OPTIONS
var StUtil = {};
StUtil.ClearOption = function (p) {
    if (p.ctrlId != '') {
        var ctrlId = p.ctrlId.split(',');
        var captions = p.caption.split(',');
        for (var i = 0; i < ctrlId.length; i++) {
            var caption = haseValue(captions[i]) ? captions[i] : 'Please Select'
            $(ctrlId[i]).children().remove();
            $(ctrlId[i]).append($("<option>").val("").text(caption));
        }
    }
}

//DESCRIPTION:FOR CASCADING OPTIONS
StUtil.Cascading = function (p) {
    debugger;
    var ctrlId = p.ctrlId;
    var formId = p.formId;
    var fullctrlId = haseValue(formId) ? ('#' + formId + ' #' + ctrlId) : '#' + ctrlId;
    var clearId = p.clearId;
    var controllerName = p.controllerName;
    var actionName = p.actionName;
    var parmVal = p.parmVal;
    var SelectedValue = p.SelectedValue;
    var ismultiSelect = haseValue(p.ismultiSelect) ? p.ismultiSelect : false;
    var ctrlcaption = haseValue(p.ctrlCaption) ? p.ctrlCaption : 'Please Select'
    if (parmVal != "") {
        $.post(base.getUrl(controllerName, actionName), { id: parmVal }, function (data) {
            !(ismultiSelect) ? StUtil.ClearOption({ ctrlId: fullctrlId, caption: ctrlcaption }) : $(fullctrlId).empty();
            $.each(data, function () {
                $(fullctrlId).append($("<option>").val(this.Value).text(this.Text));
            });
            if (haseValue(SelectedValue)) {
                $(fullctrlId).val(SelectedValue).trigger('change');
            }
        });


    }
    else {
        StUtil.ClearOption({ ctrlId: ctrlId, caption: ctrlcaption })
    }
    StUtil.ClearOption({ ctrlId: clearId, caption: p.clearCaption })
}

//DESCRIPTION:FOR ASSIGN VALUE
StUtil.AssignValue = function (p) {
    var ctrlId = p.ctrlId.split(',');
    var value = p.value.split(',');
    for (var i = 0; i < ctrlId.length; i++) {
        $('#' + ctrlId[i]).val(value[i])
    }
}
//DESCRIPTION:FOR DELETE MASTER COMMAN FUNCTION
//DESCRIPTION:FOR DELETE CONFIRM POPUP COMMAN FUNCTION
StUtil.confirm = function (p) {
    var title = p.title != '' ? p.title : STMessage.ConfirmDeleteMessage;
    var text = p.text != '' ? p.text : "";
    var showCancelButton = true;
    var confirmButtonColor = "#DD6B55";
    var confirmButtonText = "Yes, delete it!";
    var closeOnConfirm = false;
    swal({
        title: title,
        text: text,
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "Yes, delete it!",
        cancelButtonText: "No, cancel please!",
        closeOnConfirm: false,
        closeOnCancel: false,
        timer: 5000///1000 = 1 sec
    },
        function (isConfirm) {
            if (isConfirm) {
                var msg = StUtil.DeleteMaster({ CtrlId: p.DeleteFunction.CtrlId, Url: p.DeleteFunction.Url, IsLoad: p.DeleteFunction.IsLoad, PrimaryVal: p.DeleteFunction.PrimaryVal })
                if (msg == STMessage.success) {
                    swal("Deleted!", STMessage.DeleteMessage, "success");
                }
                else {
                    swal("Rrecord not deleted!", msg, "error");
                }
                return true;
            }
            else { swal("Cancelled", "Your imaginary file is safe :)", "error"); }
            if (p.DeleteFunction.IsLoad) {
                window.location.reload(true);
            }

        });

}
//DESCRIPTION:FOR GetFileSize FUNCTION
function GetFileSize(fileid) {
    try {
        var fileSize = 0;
        //for IE
        if ($.browser.msie) {
            //before making an object of ActiveXObject, 
            //please make sure ActiveX is enabled in your IE browser
            var objFSO = new ActiveXObject("Scripting.FileSystemObject"); var filePath = $("#" + fileid)[0].value;
            var objFile = objFSO.getFile(filePath);
            var fileSize = objFile.size; //size in kb
            fileSize = fileSize / 1048576; //size in mb 
        }
            //for FF, Safari, Opeara and Others
        else {
            fileSize = $("#" + fileid)[0].files[0].size //size in kb
            fileSize = fileSize / 1048576; //size in mb 
        }

        return fileSize;
    }
    catch (e) {
        alert("Error is :" + e);
    }
}

//DESCRIPTION:FOR GetFileSize FUNCTION
function getNameFromPath(strFilepath) {
    var objRE = new RegExp(/([^\/\\]+)$/);
    var strName = objRE.exec(strFilepath);

    if (strName == null) {
        return null;
    }
    else {
        return strName[0];
    }
}

//DESCRIPTION:FOR GetFileSize COMMAN FUNCTION
STFormUtil.Valid = function FormValiidation(btn) {


    var btnId = btn.id == undefined ? btn : btn.id;
    btnId = ('#' + btnId);
    if (typeof (tinyMCE) != 'undefined' && tinyMCE != undefined && tinyMCE != null) {
        tinyMCE.triggerSave();
    }
    var form = $(btnId).closest("form");
    var isValid = true;
    if (form != null) {
        form.removeData('validator');
        form.removeData('unobtrusiveValidation');
        $.validator.unobtrusive.parse(form);
        try {
            isValid = form.valid();
        }
        catch (ex) {
            console.log(ex.message);
        }
    }
    if (isValid) {
        return true;
    }
    else {
        return false;
    }
}

$(document).ready(function () {
    StUtil.addValidationRule()
    StUtil.FileValidation();//FOR FILE VALIDATION
    StUtil.SetExist();
    StUtil.focusInvalid();
});

StUtil.DynamicValMsg = function (element, ret) {
    var id = element.id;
    var formId = $('#' + element.id).closest("form").attr('id')
    var ctrlId = haseValue(formId) ? ('#' + formId + ' #' + id) : '#' + id;
    var ctrlTextInfo = id.split('_');
    var ctrlText = "";
    var msg = "";
    for (var i = 0; i < ctrlTextInfo.length; i++) {
        ctrlText = haseValue(ctrlText) ? ctrlText + " " + ctrlTextInfo[i] : ctrlTextInfo[i];
    }
    if (ret) {
        $(ctrlId).attr('title', 'This Field Is Required')
        //  msg =  'This Field Is Required';
    }
    else {
        $(ctrlId).attr('title', 'Please enter a valid ' + ctrlText)
    }
    return msg;

}
StUtil.FileValidation = function () {
    $('input[type="file"]').change(function () {
        //this.files[0].size gets the size of your file.
        var id = $(this).attr("id");
        var imgVal = $('#' + $(this).attr('id')).val();
        var imageExt = imgVal.substr((imgVal.lastIndexOf('.') + 1));
        var imgPreview = id + '_Preview';
        var imgSrc = '';
        ////$('#id').append("<img style='display:none' id='" + imgPreview + "' width='80px' height='80px' />")
        $('#' + id).parent().append("<img style='display:none'  id='" + imgPreview + "' width='80px' height='80px' />")
        if (base.hasValue(imgVal)) {
            switch (imageExt.toUpperCase()) {
                case 'CSV':
                    imgSrc = '/Base/Content/images/CSV.png';
                    break;
                case 'PDF':
                    imgSrc = '/Base/Content/images/pdf.png';
                    break;
                case 'DOC':
                    imgSrc = '/Base/Content/images/DOC.png';
                    break;
                default:
                    imgSrc = '';
            }
            $('#' + imgPreview).show()
        }
        else {
            $('#' + imgPreview).hide()
        }
        var reader = new FileReader();
        reader.onload = function () {
            var oustput = document.getElementById(imgPreview);
            oustput.src = haseValue(imgSrc) ? imgSrc : reader.result;
        };
        reader.readAsDataURL(event.target.files[0]);
        var fileInput = $('#' + id);
        StUtil.FileExtValid(id);
        var maxSize = fileInput.data('max-size');
        if (fileInput.get(0).files.length) {
            var fileSize = fileInput.get(0).files[0].size; // in bytes
            fileSize = parseFloat(fileSize) / 1000000.00;
            maxSize = parseFloat(maxSize);
            if (fileSize > maxSize) {
                base.showMessage('file size is more then ' + maxSize + ' MB, Please choose another file.', 'Error');
                $("#" + id).val('');
                $('#' + imgPreview).hide();
                return false;
            } else {
                $('#' + imgPreview).show();
            }
        } else {
            $('#' + imgPreview).hide();
            return false;
        }
    });
}
StUtil.FileExtValid = function (ctrlId) {
    var ret = false;
    var imgVal = $('#' + ctrlId).val();
    var fileInput = $('#' + ctrlId);
    var imgPreview = ctrlId + '_Preview';
    var imageExt = imgVal.substr((imgVal.lastIndexOf('.') + 1));
    var ctrlExt = $('#' + ctrlId).attr('data-file-ext');
    if (haseValue(ctrlExt)) {
        var exceExt = ctrlExt.split(',');
        for (var i = 0; i < exceExt.length; i++) {
            if (imageExt.toUpperCase().trim() == exceExt[i].toUpperCase().trim()) {
                ret = true;
            }
        }
        if (!ret) {
            $("#" + ctrlId).val('');
            $('#' + imgPreview).hide()
            base.showMessage(imageExt + ' File type not excepted plz try for  .' + ctrlExt, 'danger', 'Bar', 'top', 5000);
            $('#' + imgPreview).hide();
            return false;
        }
    }
    else {
        $('#' + imgPreview).show();

    }
}
//DESCRIPTION:FOR GetFileSize COMMAN FUNCTION
StUtil.validate = function validateform(btn) {
    var btnId = btn.id == undefined ? btn : btn.id;
    btnId = ('#' + btnId);
    if (typeof (tinyMCE) != 'undefined' && tinyMCE != undefined && tinyMCE != null) {
        tinyMCE.triggerSave();
    }
    var form = $(btnId).closest("form");
    var isValid = true;

    if (form != null) {

        form.removeData('validator');
        form.removeData('unobtrusiveValidation');
        $.validator.unobtrusive.parse(form);
        try {
            isValid = form.valid();
        }
        catch (ex) {
            console.log(ex.message);
        }
    }
    var isValidCustom = true;

    isValid = isValid && isValidCustom;
    //formUtil.autoValidation = true;
    // setInterval(function () { formUtil.resetValidation(); }, 500);
    if (isValid) {
        return true;
    }
    else {

        $('.field-validation-error').each(function () {
            if ($(this).prev().length > 0) {
                //MultiSelect
                var isMultiSelect = false;
                var prevId = '#' + $(this).prev().attr('id');
                if (hasValue($(prevId))) {
                    if (hasValue($(prevId).prop("tagName"))) {
                        if ($(prevId).prop("tagName").toUpperCase() == 'DL') {
                            $(prevId + ' a').addClass('val-error');
                            isMultiSelect = true;
                        }
                    }
                }
                //Other than MultiSelect
                if (!isMultiSelect) {
                    $(this).prev().addClass('val-error');
                }
            }
        });
        return false;
    }

}

StUtil.SetExist = function (p) {
    $('input,select').each(function () {
        var id = $(this).attr('id');
        var fromid = $(this).closest("form").attr("id");
        var isExist = $(this).attr('isExist');
        if (haseValue(isExist)) {
            if (isExist.toUpperCase().trim() == 'TRUE') {
                var cntrlFunction = id + '_isExist';
                if (typeof (window[cntrlFunction]) == "function") {
                    $('#' + id).blur(function () {
                        eval(cntrlFunction + "()");
                    });
                    $('#' + id).change(function () {
                        eval(cntrlFunction + "()");
                    });
                }
                else {
                    var cntrlFunction = fromid + "_" + id + '_isExist';
                    if (typeof (window[cntrlFunction]) == "function") {
                        $('#' + fromid + ' #' + id).blur(function () {
                            eval(cntrlFunction + "()");
                        });
                        $('#' + fromid + ' #' + id).change(function () {
                            eval(cntrlFunction + "()");
                        });

                    }
                }
            }
        }
    });
}
StUtil.IsExist = function (p) {
    //////////------------------------------
    var ctrlId = p.ctrlId;
    var ctrlVal = $('#' + p.ctrlId).val();
    var controllerName = p.controllerName;
    var actionName = p.actionName;
    var parmVal = p.parmVal;
    var formId = p.formId;
    var inputCtrlId = '#' + p.ctrlId;
    inputCtrlId = haseValue(formId) ? "#" + formId + " " + inputCtrlId : inputCtrlId;
    var ctrlText = $(inputCtrlId).parent().text().trim()
    ctrlText = $(inputCtrlId).is('input') ? (haseValue(ctrlText) ? ctrlText : 'This') : haseTrue($(inputCtrlId).parent().find('label').text()) ? $(inputCtrlId).parent().find('label').text() : 'This';
    if ($(inputCtrlId).is('select')) {
        ctrlVal = $(inputCtrlId + " option[value='" + ctrlVal + "']").text();
    }
    $.post(base.getUrl(controllerName, actionName), { id: parmVal }, function (data) {
        if (data == true) {
            base.showMessage(ctrlText + '  ' + ctrlVal + ' already exist', 'Error', 'Bar', 'top', 5000);
            $(inputCtrlId).focus();
            $(inputCtrlId).val('');
            return false;
        }
        else {
            return true;
        }
    });
}

StUtil.EditForm = function (p) {
    var parmVals = p.parmVals;
    var controllerName = p.controllerName;
    var actionName = p.actionName;
    $.ajaxSetup({ async: false });
    $.ajax({
        type: "POST",
        url: base.getUrl(controllerName, actionName),
        data: '{ id : ' + parmVals + '}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            var jsonObject = response;
            var prefix = '#';
            if (!base.isNullOrEmpty(p.formId)) {
                prefix += p.formId + ' #'
            }
            if (typeof (jsonObject) == 'object') {
                var properties = Object.keys(jsonObject);
                for (var i = 0; i < properties.length; i++) {
                    var property = properties[i];
                    var propId = prefix + property;
                    if (StUtil.isHtml(propId)) {
                        switch (base.getTagName(propId)) {
                            case 'textarea':
                                $(propId).text(jsonObject[property]);
                                $(propId).val(jsonObject[property]);
                                $(propId).code(jsonObject[property]);
                                break;
                            case 'input':
                                var type = base.getInputType(propId).toLowerCase();
                                if (base.isNullOrEmpty(type)) {
                                    return;
                                }
                                switch (type) {
                                    case 'checkbox':
                                        $(propId).prop('checked', base.isTrue(jsonObject[property]));


                                        if ($(propId).next().attr('class') == "switchery switchery-small") {

                                            if (base.isTrue(jsonObject[property])) {
                                                $(propId).next().attr('style', 'background-color: rgb(109, 92, 174); border-color: rgb(109, 92, 174); box-shadow: rgb(109, 92, 174) 0px 0px 0px 11px inset; transition: border 0.4s, box-shadow 0.4s, background-color 1.2s;')
                                                $(propId).next().find('small').attr('style', 'left: 13px; background-color: rgb(255, 255, 255); transition: left 0.2s;')
                                            }
                                            else {

                                                $(propId).next().attr('style', 'background-color: rgb(255, 255, 255); border-color: rgb(223, 223, 223); box-shadow: rgb(223, 223, 223) 0px 0px 0px 0px inset; transition: border 0.4s, box-shadow 0.4s;')
                                                $(propId).next().find('small').attr('style', 'left: 0px; background-color: rgb(255, 255, 255); transition: left 0.2s;')
                                            }
                                        }
                                        else {
                                            $(propId).val(base.isTrue(jsonObject[property]));
                                        }
                                        break;
                                    case 'datetime':
                                        var dt = base.dateFromJson(jsonObject[property]);
                                        $(propId).val(dt);
                                        break;
                                    case 'month':
                                        var ym = base.yearMonthFromJson(jsonObject[property]);
                                        $(propId).val(ym);
                                        break;
                                    case 'time':
                                        var tm = base.timeFromJson(jsonObject[property]);
                                        $(propId).val(tm);
                                        break;
                                    default:
                                        $(propId).val(jsonObject[property]);
                                }
                                break;
                            default:
                                $(propId).val(jsonObject[property]);
                        }
                    }
                    else {

                        if (haseValue(p.loadList)) {
                            var loadlistInfo = p.loadList.split(',');
                            for (var j = 0; j < loadlistInfo.length; j++) {
                                if (property.replace(loadlistInfo[j], '') == 'List') {


                                    var FullloadId = prefix + loadlistInfo[j];


                                    $(FullloadId).children().remove();
                                    $(FullloadId).append($("<option>").val("").text("Please Select"));
                                    $.each(jsonObject[property], function () {
                                        $(FullloadId).append($("<option>").val(this.Value).text(this.Text));
                                    });
                                }
                            }
                        }
                    }
                }
            }
            if (!base.isNullOrEmpty(p.formId)) {
                $('#' + p.formId).valid()
            }
        },
    });
}
StUtil.PostFrom = function (p) {
    var parmIds = p.parmIds;
    var ctrlId = p.ctrlId;
    var isShowMsg = p.isShowMsg == false ? false : true;
    var controllerName = p.controllerName;
    var actionName = p.actionName;
    var formId = p.formId;
    var dataObj = {};
    var splitChar = haseValue(p.splitChar) ? p.splitChar : ',';
    var infoIds = p.parmIds.split(splitChar);
    var infoVals = p.parmVals.split(splitChar);
    var cntrlFunction = ctrlId + 'AfterCall';
    for (var i = 0; i < infoIds.length; i++) {
        dataObj[infoIds[i]] = infoVals[i];
    }
    if ($('#' + formId).valid()) {
        $('#' + ctrlId).attr('disabled', 'disabled');
        $.ajaxSetup({ async: false });
        $.ajax({
            type: "POST",
            url: base.getUrl(controllerName, actionName),
            data: '{' + p.jsonObj + ': ' + JSON.stringify(dataObj) + '}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                $('#' + ctrlId).removeAttr('disabled');
                if (isShowMsg) {
                    base.showMessage(response.Message, 'Success', 'Bar', 'top', 10000);
                }
                if (typeof (window[cntrlFunction]) == "function") {
                    ///eval(cntrlFunction + "(" + response + ")");
                    return window[cntrlFunction](response);
                }
            },
            failure: function (response) {
                $('#' + ctrlId).removeAttr('disabled');
            },
            error: function (response) {
                $('#' + ctrlId).removeAttr('disabled');
            }

        });

    }
}

StUtil.isHtml = function (id) {
    ret = true;
    if (isNaN(id)) { //if not number then go...
        id = base.getCtrlId(id);
        //ret = arrNonHtml.indexOf(id) == -1;
        var r = new RegExp("^[a-z A-Z 0-9 #_]*$");
        ret = r.test(id);
        if (ret) {
            var prefix = id.indexOf('#') == -1 ? '#' : '';
            ret = $(prefix + id).length > 0;
        }
    } else {
        ret = false;
    }
    return ret;
}
StUtil.addValidationRule = function () {

    var id = '';
    $.validator.addMethod(
         "pattern",
         function (value, element, regexp) {
             if (regexp.constructor != RegExp) {
                 regexp = new RegExp(regexp);
             }
             else if (regexp.global) {
                 regexp.lastIndex = 0;
             }
             var ret = this.optional(element) || regexp.test(value)
             $.validator.messages.pattern = StUtil.DynamicValMsg(element, ret);
             id = element.id;

             return this.optional(element) || regexp.test(value);

         }, $.validator.messages.pattern);
}
StUtil.focusInvalid = function () {
    $('button').click(function () {
        var form = $(this).closest("form");
        if (haseValue(form.validate())) {
            form.validate().focusInvalid();

        }
    })
}
StUtil.datatableDateFormate = function (p) {
    var pattern = /Date\(([^)]+)\)/;
    var results = pattern.exec(p);
    var dt = new Date(parseFloat(results[1]));
    return zeroPad(dt.getDate(), 2) + "/" + zeroPad((dt.getMonth() + 1), 2) + "/" + dt.getFullYear();
}
function zeroPad(num, places) {
    var zero = places - num.toString().length + 1;
    return Array(+(zero > 0 && zero)).join("0") + num;
}
StUtil.ddlEnabledDisabled = function (settings) {
    var ctrlId = settings.ctrlId;
    ctrlId = '#' + ctrlId;
    var readOnly = settings.readOnly;
    $(ctrlId + " option").each(function () {
        if (!$(this).is(':selected')) {
            if (readOnly) {
                $(this).attr('disabled', true)
                $(ctrlId).css({ 'backgroundColor': 'lightgray' })
            }
            else {
                $(ctrlId).css({ 'backgroundColor': '' })
                $(this).attr('disabled', false)
            }
        }
    });
}
StUtil.GetMonthName = function (monthNumber) {
    var months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
    return months[monthNumber - 1];
}
StUtil.showDays = function (firstDate, secondDate) {
    var startDay = new Date(firstDate);
    var endDay = new Date(secondDate);
    var millisecondsPerDay = 1000 * 60 * 60 * 24;
    var millisBetween = startDay.getTime() - endDay.getTime();
    var days = millisBetween / millisecondsPerDay;
    return (Math.floor(days));
}

StUtil.formatDate = function (date) {
    var d = new Date(date),
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear();

    if (month.length < 2) month = '0' + month;
    if (day.length < 2) day = '0' + day;

    return [year, month, day].join('-');
}

StUtil.DynamicTimePicker24 = function (p) {
    var ctrlId = p.ctrlId;
    var interVal = haseValue(p.interVal) ? p.interVal : 0;
    var ctrlId = p.ctrlId;
    var caption = haseValue(p.caption) ? p.caption : 'Select Time';
    var selectedValue = haseValue(p.selectedValue) ? p.selectedValue : "";
    $('#' + ctrlId).empty()
    $('#' + ctrlId).append($("<option>").val('').text(caption));
    var mnt = 0
    for (var i = 0; i <= 24; i++) {
        mnt = 0;
        for (var j = 0; j <= 59; j++) {
            if ((i <= 23 && j > 0)) {
                var value = ("0" + i).slice(-2) + ':' + ("0" + mnt).slice(-2);
                $('#' + ctrlId).append($("<option>").val(value).text(value));
                j = j == 0 ? (j + interVal) : (j + interVal - 1);
                mnt = j;
            }
        }
    }
    selectedValue = haseValue(selectedValue) ? selectedValue.replace(/\s/g, '') : selectedValue;
    $('#' + ctrlId).val(selectedValue);

}
