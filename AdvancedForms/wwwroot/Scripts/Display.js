var Title, Id, SubmissionId, Owner, DraftSubmission, CaseID, Group;

function initValue(title, id, submissionId, owner, caseId, group) {
    Title = title;
    Id = id;
    SubmissionId = submissionId;
    Owner = owner;
    CaseID = caseId;
    Group = group;
}

function saveDraft() {
    saveForm(DraftSubmission.data, true);
}

function saveForm(submission, isDraft) {
    $.ajax({
        url: urlConfig.Entry,
        method: 'POST',
        data: {
            __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
            submission: JSON.stringify(submission),
            Title: Title,
            id: Id,
            submissionId: SubmissionId,
            owner: Owner,
            isDraft: isDraft,
            group: Group
        },
        success: function (data) {
            if (CaseID !== undefined && CaseID !== null && CaseID !== '') {
                addAFCaseItem(data);
            } else {
                window.location.replace(urlConfig.Entry.replace("AdvancedForms/Entry", "") + "submission-confirmation");
            }
        },
        error: function (error) {
            var errorMsg = "Unable to Save. Try again later.";
            $('<div class="alert alert-danger" role="alert"></div>').text(errorMsg + error.responseText).appendTo($('#advancedForm-errors'));
        }
    });
}

var getDateString = function (value) {
    date = new Date(value);
    return (date.getMonth() +1) + '/' + date.getDate() + '/' + date.getFullYear() + ' ' + date.toLocaleTimeString().replace(/:\d{2}\s/, ' ');
};

function addAFCaseItem(content) {
    var itemText = content.DisplayText + ' Created By: ' + content.Owner + ' on ' + getDateString(content.CreatedUtc);
    $.ajax({
        url: urlConfig.AddCaseAttachItem,
        method: 'POST',
        data: {
            __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
            caseContentItemId: CaseID,
            attachContentItemId: content.ContentItemId,
            itemDisplayText: itemText
        },
        success: function (data) {
            window.location.replace(urlConfig.Entry.replace("AdvancedForms/Entry", "") + "submission-confirmation");
        },
        error: function (error) {
            var errorMsg = "Unable to Save. Try again later.";
            $('<div class="alert alert-danger" role="alert"></div>').text(errorMsg + error.responseText).appendTo($('#advancedForm-errors'));
        }
    });
}

function printButton() {
    window.open(window.location.href.toLowerCase().replace('/edit/', '/print/').replace('/view/', '/print/'), '_blank');
};

$(document).ready(function () {
    $(".collapse.in").each(function () {
        $(this).siblings(".panel-heading").find(".fa").addClass("fa-minus").removeClass("fa-plus");
    });
    $(".collapse").on('show.bs.collapse', function () {
        $(this).parent().find(".panel-heading .fa").removeClass("fa-plus").addClass("fa-minus");
    }).on('hide.bs.collapse', function () {
        $(this).parent().find(".panel-heading .fa").removeClass("fa-minus").addClass("fa-plus");
    });
    $('#inputTitle').keypress(function (event) {
        var keycode = (event.keyCode ? event.keyCode : event.which);
        if (keycode === '13') {
            searchForms();
        }
    });
    $("#btnSearch").click(function () {
        searchForms();
    });
});
function searchForms() {
    $(".forms_panel .collapse").collapse('show');
    var searchTitle = $("#inputTitle").val().toLowerCase();
    $(".forms_panel .panel a span").each(function () {
        var currentTitle = $(this).text();
        $(this).parent().parent().show();
        if (currentTitle.toLowerCase().indexOf(searchTitle) === -1) {
            $(this).parent().parent().hide();
        }
    });
    var noItem = true;
    $(".alert").hide();
    $(".forms_panel .panel").each(function () {
        var isAllHide = true;
        $(this).parent().show();
        var anchor = $(this).find("a span");
        anchor.each(function () {
            if (!$(this).is(":hidden")) {
                isAllHide = false;
                noItem = false;
            }
        });
        if (isAllHide) {
            $(this).parent().hide();
        }
    });
    if (noItem) {
        $(".alert").show();
    }
}
function collapseShow(id) {
    $("#" + id).collapse('toggle');
}