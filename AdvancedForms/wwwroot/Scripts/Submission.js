$(function () {
    renderCommentEditors();
});

function renderCommentEditors() {
    $('#AdminComment').trumbowyg();
    $('#PublicComment').trumbowyg();
}

function clearEditors() {
    $('#AdminComment').trumbowyg('empty');
    $('#PublicComment').trumbowyg('empty');
}

function submitAdminComment() {
    if ($("#AdminComment")[0].value == null) {
        return;
    }
    $.ajax({
        url: '/AdvancedForms/SaveAdminComment',
        method: 'POST',
        data: {
            __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
            id: '@Model.Id',
            comment: $("#AdminComment")[0].value
        },
        success: function (data) {
            clearEditors();
            GetAdminComments();
        },
        error: function (error) {
            var errorMsg = "Unable to Save. Try again later.";
            $('<div class="alert alert-danger" role="alert"></div>').text(errorMsg + error.responseText).appendTo($('#advancedForm-errors'));
        }
    });
}

function submitPublicComment() {
    if ($("#PublicComment")[0].value == null) {
        return;
    }
    $.ajax({
        url: '/AdvancedForms/SavePublicComment',
        method: 'POST',
        data: {
            __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
            id: '@Model.Id',
            comment: $("#PublicComment")[0].value
        },
        success: function (data) {
            clearEditors();
            GetPublicComments();
        },
        error: function (error) {
            var errorMsg = "Unable to Save. Try again later.";
            $('<div class="alert alert-danger" role="alert"></div>').text(errorMsg + error.responseText).appendTo($('#advancedForm-errors'));
        }
    });
}

function GetAdminComments() {
    $.ajax({
        url: '/AdvancedForms/GetAdminComments',
        method: 'GET',
        data: {
            id: '@Model.Id'
        },
        success: function (data) {
            if (data != null) {
                var comments = "";
                var date = new Date();
                $.each(data, function (index, value) {
                    date = new Date(value.CreatedUtc);
                    comments += '<div class="panel panel-default">';
                    comments += '<div class="panel-heading"><b>' + value.Owner + '</b> ' + date.getMonth() + '/' + date.getDate() + '/' + date.getFullYear() + ' ' + date.toLocaleString('en-US', { hour: 'numeric', minute: 'numeric', hour12: true }) + '</div>';
                    comments += '<div class="panel-body">' + value.AdminComment + '</div>';
                    comments += '</div>';
                    console.log(value);
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

function GetPublicComments() {
    $.ajax({
        url: '/AdvancedForms/GetPublicComments',
        method: 'GET',
        data: {
            id: '@Model.Id'
        },
        success: function (data) {
            if (data != null) {
                var comments = "";
                var date = new Date();
                $.each(data, function (index, value) {
                    date = new Date(value.CreatedUtc);
                    comments += '<div class="panel panel-default">';
                    comments += '<div class="panel-heading"><b>' + value.Owner + '</b> ' + date.getMonth() + '/' + date.getDate() + '/' + date.getFullYear() + ' ' + date.toLocaleString('en-US', { hour: 'numeric', minute: 'numeric', hour12: true }) + '</div>';
                    comments += '<div class="panel-body">' + value.AdminComment + '</div>';
                    comments += '</div>';
                    console.log(value);
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