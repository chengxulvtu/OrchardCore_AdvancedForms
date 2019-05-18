var Title, Id, Header, Footer, Description, Type, SubmissionId, Instructions, Owner, DraftSubmission;

function initValue(title, id, header, footer, description, type, submissionId, instructions, owner) {
    Title = title;
    Id = id;
    Header = header;
    Footer = footer;
    Description = description;
    Type = type;
    SubmissionId = submissionId;
    Instructions = instructions;
    Owner = owner;
}

function saveDraft() {
    saveForm(DraftSubmission.data, true);
}

function saveForm(submission, isDraft) {
    $.ajax({
        url: '/AdvancedForms/Entry',
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
            isDraft: isDraft
        },
        success: function (data) {
            window.location.replace("/submission-confirmation");
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

