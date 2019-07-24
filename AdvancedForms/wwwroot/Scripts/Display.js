var Title, Id, Header, Footer, Description, Type, SubmissionId, Instructions, Owner, DraftSubmission, CaseID, HideFromListing, IsGlobalHeader, IsGlobalFooter, Group;

function initValue(title, id, header, footer, description, type, submissionId, instructions, owner, caseId, hideFromListing, isGlobalHeader, isGlobalFooter, group) {
    Title = title;
    Id = id;
    Header = header;
    Footer = footer;
    Description = description;
    Type = type;
    SubmissionId = submissionId;
    Instructions = instructions;
    Owner = owner;
    CaseID = caseId;
    HideFromListing = hideFromListing;
    IsGlobalHeader = isGlobalHeader;
    IsGlobalFooter = isGlobalFooter;
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
            container: document.getElementById('Container').value,
            header: Header,
            footer: Footer,
            description: Description,
            type: Type,
            submissionId: SubmissionId,
            instructions: Instructions,
            owner: Owner,
            isDraft: isDraft,
            hideFromListing: HideFromListing,
            isGlobalHeader: IsGlobalHeader,
            isGlobalFooter: IsGlobalFooter,
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
        if (keycode == '13') {
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
        if (currentTitle.toLowerCase().indexOf(searchTitle) == -1) {
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