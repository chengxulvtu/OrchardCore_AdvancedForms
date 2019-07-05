var CurrentUser, ID;
var IsSubmission;

function setCurrentUser(id, user, isSubmission) {
    currentUser = user;
    IsSubmission = isSubmission;
    ID = id;
}

function renderCommentEditors() {
    $('#PublicComment').trumbowyg();
    $('#AdminComment').trumbowyg();
}

function clearEditors() {
    if ($("#PublicComment").parent().find(".trumbowyg-editor").length !== 0) {
        $("#PublicComment").parent().find(".trumbowyg-editor")[0].innerText = "";
        $('#PublicComment-ContentItmeID').parent().find(".publish-button")[0].textContent = "Save";
        $('#PublicComment-ContentItmeID').val("");
    }
    if ($("#AdminComment").parent().find(".trumbowyg-editor").length !== 0) {
        $("#AdminComment").parent().find(".trumbowyg-editor")[0].innerText = "";
        $('#AdminComment-ContentItmeID').parent().find(".publish-button")[0].textContent = "Save";
        $('#AdminComment-ContentItmeID').val("");
    }

    $("#publicAttachmentRemove").hide();
    $("#publicAttachment").text("");
    $("#publicAttachment").attr("href", "");

    $("#adminAttachmentRemove").hide();
    $("#adminAttachment").text("");
    $("#adminAttachment").attr("href", "");
}

function submitAdminComment(id) {
    $(".errorMessageAdmin").hide();
    if ($("#AdminComment").parent().find(".trumbowyg-editor").length === 0) {
        return;
    }
    var content = $("#AdminComment").parent().find(".trumbowyg-editor")[0].innerHTML;
    if (content === null || content === "") {
        $(".errorMessageAdmin").show();
        return;
    }
    var url = $("#adminAttachment").text();
    $.ajax({
        url: urlConfig.SaveUpdateAdminComment,
        method: 'POST',
        data: {
            __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
            id: id,
            contentItemId: $("#AdminComment-ContentItmeID").val(),
            comment: content,
            attachment: url
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
    $(".errorMessagePublic").hide();
    if ($("#PublicComment").parent().find(".trumbowyg-editor").length === 0) {
        return;
    }
    var content = $("#PublicComment").parent().find(".trumbowyg-editor")[0].innerHTML;
    if (content === null || content === "") {
        $(".errorMessagePublic").show();
        return;
    }
    var url = $("#publicAttachment").text();
    $.ajax({
        url: urlConfig.SaveUpdatePublicComment,
        method: 'POST',
        data: {
            __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
            id: id,
            contentItemId: $("#PublicComment-ContentItmeID").val(),
            comment: content,
            attachment: url
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

function RemoveAdminComment(contentItemId) {
    confirmDialog("Delete Comment", "Are you sure to want to delete the admin comment?", r => {
        if (r) {
            $.ajax({
                url: urlConfig.SaveUpdateAdminComment,
                method: 'POST',
                data: {
                    __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                    id: ID,
                    contentItemId: contentItemId,
                    comment: "Comment removed by moderator"
                },
                success: function (data) {
                    clearEditors();
                    GetAdminComments(ID);
                },
                error: function (error) {
                    var errorMsg = "Unable to Save. Try again later.";
                    $('<div class="alert alert-danger" role="alert"></div>').text(errorMsg + error.responseText).appendTo($('#advancedForm-errors'));
                }
            });
        }
    });
}

function MakePublicComment(contentItemId) {
    confirmDialog("Update Comment", "Are you sure to want to make the comment public?", r => {
        if (r) {
            $.ajax({
                url: urlConfig.MakePublicComment,
                method: 'POST',
                data: {
                    __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                    id: ID,
                    contentItemId: contentItemId
                },
                success: function (data) {
                    clearEditors();
                    GetAdminComments(ID);
                },
                error: function (error) {
                    var errorMsg = "Unable to Save. Try again later.";
                    $('<div class="alert alert-danger" role="alert"></div>').text(errorMsg + error.responseText).appendTo($('#advancedForm-errors'));
                }
            });
        }
    });
}

function RemovePublicComment(contentItemId) {
    confirmDialog("Delete Comment", "Are you sure to want to delete the public comment?", r => {
        if (r) {
            $.ajax({
                url: urlConfig.SaveUpdatePublicComment,
                method: 'POST',
                data: {
                    __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                    id: ID,
                    contentItemId: contentItemId,
                    comment: "Comment removed by moderator"
                },
                success: function (data) {
                    clearEditors();
                    GetPublicComments(ID);
                },
                error: function (error) {
                    var errorMsg = "Unable to Save. Try again later.";
                    $('<div class="alert alert-danger" role="alert"></div>').text(errorMsg + error.responseText).appendTo($('#advancedForm-errors'));
                }
            });
        }
    });
}

function GetAdminComments(id) {
    $.ajax({
        url: urlConfig.GetAdminComments,
        method: 'GET',
        data: {
            id: id
        },
        success: function (data) {
            if (data !== null) {
                var comments = "";
                $.each(data, function (index, value) {
                    comments += getPanel(value, false);
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
        url: urlConfig.GetPublicComments,
        method: 'GET',
        data: {
            id: id
        },
        success: function (data) {
            if (data !== null) {
                var comments = "";
                $.each(data, function (index, value) {
                    comments += getPanel(value, true);
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

function getPanel(value, isPublic) {
    var panel = "";
    var editorSelect = "AdminComment";
    panel += '<div class="panel panel-default">';
    var comment = '';
    var url = '';
    if (isPublic) {
        editorSelect = "PublicComment";
        comment = value.PublicComment.Comment.Html;
        if (value.PublicComment.Attachment !== undefined) {
            url = value.PublicComment.Attachment.Text;
        }
    }
    else {
        comment = value.AdminComment.Comment.Html;
        if (value.AdminComment.Attachment !== undefined) {
            url = value.AdminComment.Attachment.Text;
        }
    }
    panel += '<div class="panel-heading"><b>' + value.Owner + '</b> ' + getDateString(value.CreatedUtc);
    if (url !== null && url !== "") {
        panel += '<input type="hidden" class="downloadLink" value="' + url + '" />';
        panel += ' <a target="_blank" href="' + url + '">Download Attachment</a>';
    }

    if (IsSubmission) {
        if (isPublic) {
            panel += '<button class="pull-right btn btn-link" href="#" style="color:#007bff;margin-top:-6px;" onclick="RemovePublicComment(\'' + value.ContentItemId + '\')">Admin Remove</button>';
        } else {
            panel += '<button class="pull-right btn btn-link" href="#" style="color:#007bff;margin-top:-6px;" onclick="MakePublicComment(\'' + value.ContentItemId + '\')">Make Public</button>';
            panel += '<button class="pull-right btn btn-link" href="#" style="color:#007bff;margin-top:-6px;" onclick="RemoveAdminComment(\'' + value.ContentItemId + '\')">Admin Remove</button>';
        }
    }

    if (currentUser === value.Owner) {
        panel += '<button class="pull-right btn btn-link" href="#" style="color:#007bff;margin-top:-6px;" onclick="EditComment(\'' + value.ContentItemId + '\', this, \'' + editorSelect + '\')">Edit</button>';
    }
    panel += '</div>';
    panel += '<div class="panel-body">' + comment + '</div>';
    panel += '</div>';
    return panel;
}

function getDateString(value) {
    date = new Date(value);
    return (date.getMonth() + 1) + '/' + date.getDate() + '/' + date.getFullYear() + ' ' + date.toLocaleTimeString().replace(/:\d{2}\s/, ' ');
}

function onSubmissionFormSubmit(form) {
    var isValidate = true;
    if (form.Status === undefined || form.Status.value === '') {
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
        if (builder.schema !== null && builder.schema.components !== null) {
            document.getElementById('AdminContainer').value = JSON.stringify(builder.schema);
        } else {
            document.getElementById('AdminContainer').value = null;
        }
        if (builder.data !== null) {
            document.getElementById('AdminSubmission').value = JSON.stringify(builder.data);
        } else {
            document.getElementById('AdminSubmission').value = null;
        }
    });
}

$('#fileuploadPublic').fileupload({
    dataType: 'json',
    url: urlConfig.Upload,
    formData: function () {
        var antiForgeryToken = $("input[name=__RequestVerificationToken]").val();

        return [
            { name: 'path', value: "Form Comments" },
            { name: '__RequestVerificationToken', value: antiForgeryToken }
        ]
    },
    done: function (e, data) {
        $.each(data.result.files, function (index, file) {
            clearAttachment();
            if (file.error !== undefined) {
                alert(file.error);
            } else {
                var newFile = getNewFileName(file.name);
                var newPath = 'Form Comments/' + newFile;
                $.ajax({
                    url: urlConfig.MoveMedia + "?oldPath=" + encodeURIComponent(file.mediaPath) + "&newPath=" + encodeURIComponent(newPath),
                    method: 'POST',
                    data: {
                        __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val()
                    },
                    success: function (data) {
                        $("#publicAttachmentRemove").show();
                        $("#publicAttachment").text(urlConfig.Upload.replace("/OrchardCore.Media/Admin/Upload", "") + '/media/' + newPath);
                        $("#publicAttachment").attr("href", urlConfig.Upload.replace("/OrchardCore.Media/Admin/Upload", "") + '/media/' + newPath);
                    },
                    error: function (error) {
                        alert(error);
                    }
                });
            }
        });
    }
});

function getNewFileName(fileName) {
    var d = new Date();
    var n = d.getTime();
    var random = Math.floor((Math.random() * 1000) + 1);
    var fileExtension = fileName.replace(/^.*\./, '');
    return n + random + '.' + fileExtension;
}

$('#fileuploadAdmin').fileupload({
    dataType: 'json',
    url: urlConfig.Upload,
    formData: function () {
        var antiForgeryToken = $("input[name=__RequestVerificationToken]").val();

        return [
            { name: 'path', value: "Form Comments" },
            { name: '__RequestVerificationToken', value: antiForgeryToken }
        ]
    },
    done: function (e, data) {
        $.each(data.result.files, function (index, file) {
            clearAttachment();
            if (file.error !== undefined) {
                alert(file.error);
            } else {
                var newFile = getNewFileName(file.name);
                var newPath = 'Form Comments/' + newFile;
                $.ajax({
                    url: urlConfig.MoveMedia + "?oldPath=" + encodeURIComponent(file.mediaPath) + "&newPath=" + encodeURIComponent(newPath),
                    method: 'POST',
                    data: {
                        __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val()
                    },
                    success: function (data) {
                        $("#adminAttachmentRemove").show();
                        $("#adminAttachment").text(urlConfig.Upload.replace("/OrchardCore.Media/Admin/Upload", "") + '/media/' + newPath);
                        $("#adminAttachment").attr("href", urlConfig.Upload.replace("/OrchardCore.Media/Admin/Upload", "") + '/media/' + newPath);
                    },
                    error: function (error) {
                        alert(error);
                    }
                });
            }
        });
    }
});

$("#publicAttachmentRemove, #adminAttachmentRemove").click(function () {
    clearAttachment();
});

function clearAttachment() {
    $("#publicAttachmentRemove").hide();
    $("#publicAttachment").text("");
    $("#publicAttachment").attr("href", "");

    $("#adminAttachmentRemove").hide();
    $("#adminAttachment").text("");
    $("#adminAttachment").attr("href", "");
}

var $loading = $('.loading').hide();
$(document).ajaxStart(function () { $loading.show(); }).ajaxStop(function () { $loading.hide(); });