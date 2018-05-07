/*base Js: Start--------------------------------------------------------------------------------------------------------------------------------------------*/
base = {};
base.appPath = "";
base.loginID = "";

base.getUrl = function (controllerName, actionName, id) {
    return base.appPath + '/' + controllerName + '/' + actionName + (id == undefined || id == null ? '' : ('/' + id));
}
base.GetURLParameter = function GetURLParameter(sParam) {

    var sPageURL = window.location.search.substring(1);
    var sURLVariables = sPageURL.split('&');
    for (var i = 0; i < sURLVariables.length; i++) {
        var sParameterName = sURLVariables[i].split('=');
        if (sParameterName[0].toUpperCase() == sParam.toUpperCase()) {
            return sParameterName[1];
        }
    }
}

base.hasValue = function (val) {
    if (typeof (val) == 'undefined' || val == undefined || val == null || val == '')
        return false;
    else
        return true;
}
base.isNullOrEmpty = function (val) {
    var ret = false;
    if (base.hasValue(val)) {
        if (val == '' || val == null)
            ret = true;
        else
            ret = false;
    } else {
        ret = true;
    }
    return ret;
}

base.getTagName = function (id) { //frm1 city_id
    id = base.getCtrlId(id);
    if (base.isHtml(id)) {
        var prefix = id.indexOf('#') == -1 ? '#' : '';
        id = prefix + id;
        if (!base.isNullOrEmpty(id)) {
            return $(id).prop("tagName").toLowerCase();
        }
    }
    return null;
}
base.getCtrlId = function (idOrCtrl) { //frm city_id / #city_id
    var id = null;
    if (typeof (idOrCtrl) == 'string') { //frm city_id
        id = idOrCtrl;
    } else if (typeof (idOrCtrl) == 'object') { //this
        id = base.getFullCtrlId(idOrCtrl);
    }
    if (id.indexOf('#') == -1) { // no #, only frm city_id
        id = '#' + id.replace(' ', ' #')
    } else {
        //do nothing..., as id prefixed by # already
    }
    //if (base.isHtml(id)) {
    //}
    return id;
}
base.getCtrlNameOnly = function (ctrlId) {
    var myCtrl = ctrlId.split(' ').pop(-1).replace('#', ''); ////'#frm1 #ctrl_id' ==> ctrl_id
    return myCtrl;
}
base.getFullCtrlId = function (th) { //this
    var id = $(th).attr('id');
    var parentId = base.getParentId(th);
    var frmParentId = '';
    if (!base.isNullOrEmpty(parentId)) {
        frmParentId += ('#' + parentId + ' ');
    }
    //if (!base.isNullOrEmpty(formId)) {
    //    frmParentId += ('#' + formId + ' ');
    //}
    var fullId = (frmParentId + ((base.isNullOrEmpty(frmParentId) ? '#' : ' #') + id));
    return fullId;
}

base.getLength = function (str) {
    var len = -1;
    try {
        len = base.isNullOrEmpty(str) ? len : str.toString().length;
    }
    catch (ex) {
        console.log(ex.message);
    }
    return len;
}
base.replaceAll = function (str, replace, by) {
    return str.split(replace).join(by)
}



base.setCookie = function (cname, cvalue, exdays) {

    var d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toGMTString();
    document.cookie = cname + "=" + cvalue + "; " + expires + ';path=/';
}
base.getCookie = function (cname) {

    var name = cname + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}
base.deleteCookie = function (cname) {
    document.cookie = cname + "=;expires=Wed; 01 Jan 1970";
}



base.showMessage = function showMessage(MSG, TY, style, position, to) {
    $('.alertDivPosition').html('');
    // type =  info,warning, success, danger etc
    //position =left, top, bottom,top-left,top-right,bottom-left,bottom-right
    var message = MSG != '' ? MSG : decodeURI(base.GetURLParameter('result'));
    var type = TY != '' ? TY : base.GetURLParameter('MessageType');
    var Timeout = to != '' ? to : 10000;
    var style = style != '' ? style : 'Bar';
    var position = position != '' ? position : 'top';
 //   if (type == 'Error') { type = 'danger'; }
    if ( type != undefined) { type= type.toLowerCase(); }
     if (message != 'undefined' && type != 'undefined' && message != undefined && type != undefined) {
          
             swal({
                 title: type,
                 text: message,
                 type: type,
             })
 
        } else {
            return;
    }

    //e.preventDefault();


}

base.isValidEmailAddress = function isValidEmailAddress(emailAddress) {
    var pattern = new RegExp(/^[+a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/i);
    //alert( pattern.test(emailAddress) );
    return pattern.test(emailAddress);
};

base.isHtml = function (id) {
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
base.getInputType = function (ctrlId) {
    var type = null;
    ctrlId = base.getCtrlId(ctrlId);
    //var prefix = ctrlId.indexOf('#') == -1 ? '#' : '';
    //var ctrlId = prefix + ctrlId;
    var info = $(ctrlId).data();
    if (!base.isNullOrEmpty(info)) {
        if (base.isNullOrEmpty(info.inputType)) {
            if (base.getTagName(ctrlId) == 'input') {
                type = $(ctrlId).attr('type');
            }
        }
        else {
            type = info.inputType;
        }
    }
    return type;
}
base.isTrue = function (val) {
    var ret = false;
    if (base.hasValue(val)) {
        if (String(val).toUpperCase() == 'TRUE')
            return true;
    }
    return ret;
}
treeUtil = {};
treeUtil.processTree = function (s) {
    var ctrlId = base.getCtrlId(s.ctrlId);
    var data = s.data;
    var isPlugins = s.isPlugins;
    var info = $(ctrlId).data();
    var selected = eval(info.values);
    if (!base.isNullOrEmpty(selected)) {
    }
    var values = [];
    var nodes = [];
    var loop = 0;
    $.each(data, function () {
        var node = { "id": '' + this.id + '', "parent": '' + this.parent + '', "text": '' + this.text + '', "state": { "selected": this.selected } }
        if (this.selected) {
            values.push(this.id);
        }
        if (!base.isNullOrEmpty(selected)) {
            for (var i = 0; i < selected.length; i++) {
                if (this.id == selected[i]) {
                    node = { "id": '' + this.id + '', "parent": '' + this.parent + '', "text": '' + this.text + '', "state": { "selected": true } }
                }
            }
        }
        nodes.push(node);
    });
    $(ctrlId).attr('data-orig-values', values.toString());
    $(ctrlId).attr('data-values', values.toString());
    $(ctrlId).jstree({
        'core': {
            'data': nodes,
        },
        checkbox: {
            real_checkboxes: true,
            real_checkboxes_names: function (n) {
                return [("check_" + (n[0].id || Math.ceil(Math.random() * 10000))), n[0].id]
            }
        },
        plugins: isPlugins ? ["html_data", "types", "themes", "ui", "checkbox"] : ["html_data", "types", "themes", "ui"]

    });
    $(ctrlId).on('changed.jstree', function (e, data) {
        $(ctrlId).attr('data-values', treeUtil.getSelectedValues(ctrlId));
    });

    $(ctrlId).on('deselect_node.jstree Event', function (e, data) {
        //do nothing...
    });
    $(ctrlId).on('select_node.jstree Event', function (e, data) {
        //do nothing...
    });
}
treeUtil.getSelectedValues = function (ctrlId) {
    if (base.isNullOrEmpty(ctrlId)) {
        return null;
    } else {
        return $(ctrlId).jstree('get_selected');
    }
}
treeUtil.getValue = function (ctrlId) {
    return $('#' + ctrlId).attr('data-values');
    //return $('#' + ctrlId).data('values');
}

function setActiveMenu(settings) {

    if (base.hasValue(settings)) {
        var isEnabled = settings.isEnabled;
        if (isEnabled) {

            var scope = $('.menu-items');
            var path = window.location.pathname + window.location.search;
            if (path == '/') return;
            var find = $(scope).find('a[href*= "' + path + '"]').length > 0 ? true : false;
            if (!find) {
                //Lets try on controller basis
                //var info = path.split('/');
                //var controller = info[1];
                //path = '/' + controller + '/' //new path
                path = base.getCookie('selMenu');
            }

            $(scope).find('a[href*= "' + path + '"]').each(function () {
                //debugger;
                //alert(path);
                var li = $(this).parent('li');
                var span = $(li).find('span.icon-thumbnail');
                span.addClass('bg-success');
                var ul = $(li).parent('ul');
                if ($(ul).attr("class") == 'sub-menu')
                {
                    ul.css('display', 'block');

                    var liParent = $(ul).parent('li');
                    var spanParent = $(liParent).find('span.THP');
                    spanParent.addClass('bg-success');

                    //alert('in sub menu');

                }

                 
                ////$(this).css('border-bottom', '1px solid #282f33'); //#282f33
                ////$(this).css('border-top', '1px solid #282f33'); //#282f33
                //$(this).css('border-right', '3px solid #fcb040'); //#0e96ec
                //$(this).css('color', '#ffffff'); //#0e96ec

                //var level = 2
                //var li = $(this).parent('li');
                //for (var i = 1; i < level; i++) {
                //    var ul = $(li).parent('ul');
                //    var liTemp = ul.addClass('collapse in').parent('li').addClass('active');
                //    ////
                //    var a = $(liTemp).children('a');

                //    var span = $(a).find('span');
                //    span.css('vertical-align', 'middle');
                //    span.css('color', '#fcb040');
                //    span.css('padding-bottom', '3px');

                //    //span = $(a).find('span.nav-label');
                //    //span.css('border-bottom', '1px solid #0e96ec');

                //    ////
                //    var ulSiblings = $(ul).siblings('ul').addClass('collapse in');
                //    li = $(ul).parent('li');
                //}

                //var i = $(this).find('i');
                //i.addClass('fa fa-arrow-right'); //fa-arrow-right, bolt
                ////i.css('font-size', '15px');
                //i.css('vertical-align', 'text-bottom');
                //i.css('color', '#fcb040');
                //var span = $(this).find('span');
                //span.css('vertical-align', 'middle');
                //span.css('color', '#fcb040');
                //span.css('border-bottom', '1px solid #0e96ec');
                //span.css('padding-bottom', '3px');
                base.setCookie('selMenu', path, 1);
            });
        }
    }
}