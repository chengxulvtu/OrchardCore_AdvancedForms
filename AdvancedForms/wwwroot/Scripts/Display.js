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
    return date.getMonth() + '/' + date.getDate() + '/' + date.getFullYear() + ' ' + date.toLocaleTimeString().replace(/:\d{2}\s/, ' ');
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

