function renderCommentEditors() {
    $('#PublicComment').trumbowyg();
    $('#AdminComment').trumbowyg();
}

function clearEditors() {
    alert('ehllo');
    $('#PublicComment').trumbowyg('empty');
}

function submitAdminComment(id) {
    if ($("#AdminComment")[0].value == null) {
        return;
    }
    $.ajax({
        url: '/AdvancedForms/SaveAdminComment',
        method: 'POST',
        data: {
            __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
            id: id,
            comment: $("#AdminComment")[0].value
        },
        success: function (data) {
            clearEditors();
            GetAdminComments(id);
        },
        error: function (error) {
            var errorMsg = "Unable to Save. Try again later.";
            $('<div class="alert alert-danger" role="alert"></div>').text(errorMsg + error.responseText).appendTo($('#advancedForm-errors'));
        }
    });
}

function submitPublicComment(id) {
    if ($("#PublicComment")[0].value == null) {
        return;
    }
    $.ajax({
        url: '/AdvancedForms/SavePublicComment',
        method: 'POST',
        data: {
            __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
            id: id,
            comment: $("#PublicComment")[0].value
        },
        success: function (data) {
            clearEditors();
            GetPublicComments(id);
        },
        error: function (error) {
            var errorMsg = "Unable to Save. Try again later.";
            $('<div class="alert alert-danger" role="alert"></div>').text(errorMsg + error.responseText).appendTo($('#advancedForm-errors'));
        }
    });
}

function GetAdminComments(id) {
    $.ajax({
        url: '/AdvancedForms/GetAdminComments',
        method: 'GET',
        data: {
            id: id
        },
        success: function (data) {
            if (data != null) {
                var comments = "";
                $.each(data, function (index, value) {
                    comments += getPanel(value);
                });
                $("#AdminCommentsPanels").html(comments);
            }
        },
        error: function (error) {
            var errorMsg = "Unable to Save. Try again later.";
            $('<div class="alert alert-danger" role="alert"></div>').text(errorMsg + error.responseText).appendTo($('#advancedForm-errors'));
        }
    });
}

function GetPublicComments(id) {
    $.ajax({
        url: '/AdvancedForms/GetPublicComments',
        method: 'GET',
        data: {
            id: id
        },
        success: function (data) {
            if (data != null) {
                var comments = "";
                $.each(data, function (index, value) {
                    comments += getPanel(value);
                });
                $("#PublicCommentsPanels").html(comments);
            }
        },
        error: function (error) {
            var errorMsg = "Unable to Save. Try again later.";
            $('<div class="alert alert-danger" role="alert"></div>').text(errorMsg + error.responseText).appendTo($('#advancedForm-errors'));
        }
    });
}

function getPanel(value) {
    var panel = "";
    panel += '<div class="panel panel-default">';
    panel += '<div class="panel-heading"><b>' + value.Owner + '</b> ' + getDateString(value.CreatedUtc) + ' </div>';
    panel += '<div class="panel-body">' + value.HtmlBodyPart.Html + '</div>';
    panel += '</div>';
    return panel;
}

function getDateString(value) {
    date = new Date(value);
    return date.getMonth() + '/' + date.getDate() + '/' + date.getFullYear() + ' ' + date.toLocaleTimeString().replace(/:\d{2}\s/, ' ');
}

function onSubmissionFormSubmit(form) {
    var isValidate = true;
    if (form.Status == undefined || form.Status.value == '') {
        isValidate = false;
        document.getElementById("StatusError").style.display = "list-item";
        document.getElementById("multiselect_Div").classList.add("editor-error");
    } else {
        document.getElementById("StatusError").style.display = "none";
        document.getElementById("multiselect_Div").classList.remove("editor-error");
    }

    document.documentElement.scrollTop = 0;
    if (isValidate) {
        document.getElementById("notifyError").style.display = "none";
    } else {
        document.getElementById("notifyError").style.display = "block";
    }
    return isValidate;
}

function builderAdminFieldsChange(builder) {
    builder.on('change', function () {
        if (builder.schema != null && builder.schema.components != null) {
            document.getElementById('AdminContainer').value = JSON.stringify(builder.schema);
        } else {
            document.getElementById('AdminContainer').value = null;
        }

        if (builder.data != null) {
            debugger;
            document.getElementById('AdminSubmission').value = JSON.stringify(builder.data);
        } else {
            debugger;
            document.getElementById('AdminSubmission').value = null;
        }
    });
}