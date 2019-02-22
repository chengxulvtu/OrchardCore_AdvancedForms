function builderChange(builder) {
    builder.on('change', function () {
        updateContainer(builder);
    });
    this.updateContainer(builder);
    this.updatePanels(builder);
}

function updateContainer(builder) {
    if (builder.schema != null && builder.schema.components != null) {
        document.getElementById('Container').value = JSON.stringify(builder.schema);
    } else {
        document.getElementById('Container').value = null;
    }
    var formElement = document.getElementById('formio');
    formElement.innerHTML = '';
    var formJsonElement = document.getElementById('formJson');
    formElement.innerHTML = '';
    formJsonElement.innerHTML = '';
    if (builder.form.components) {
        Formio.createForm(formElement, builder.form).then(onForm);
        formJsonElement.appendChild(document.createTextNode(JSON.stringify(builder.schema, null, 4)));
    }
}

function builderAdminFieldsChange(builder) {
    builder.on('change', function () {
        if (builder.schema != null && builder.schema.components != null) {
            document.getElementById('AdminContainer').value = JSON.stringify(builder.schema);
        } else {
            document.getElementById('AdminContainer').value = null;
        }
    });
}

var onForm = function (form) {
    form.on('change', function () {
        var subJsonElement = document.getElementById('subJson');
        subJsonElement.innerHTML = '';
        subJsonElement.appendChild(document.createTextNode(JSON.stringify(form.submission, null, 4)));
    });
};

function updatePanels(builder) {
    var formElement = document.getElementById('formio');
    formElement.innerHTML = '';
    var formJsonElement = document.getElementById('formJson');
    formElement.innerHTML = '';
    formJsonElement.innerHTML = '';
    if (builder.form.components) {
        Formio.createForm(formElement, builder.form).then(onForm);
        formJsonElement.appendChild(document.createTextNode(JSON.stringify(builder.schema, null, 4)));
    }
}

function onFormSubmit(form) {
    var isValidate = true;
    if (form.Title.value == '') {
        isValidate = false;
        document.getElementById("TitleError").style.display = "list-item";
        document.getElementById("Title").classList.add("input-validation-error");
    } else {
        document.getElementById("TitleError").style.display = "none";
        document.getElementById("Title").classList.remove("input-validation-error");
    }

    if (form.Type == undefined || form.Type.value == '') {
        isValidate = false;
        document.getElementById("TagError").style.display = "list-item";
        document.getElementById("multiselect_Div").classList.add("editor-error");
    } else {
        document.getElementById("TagError").style.display = "none";
        document.getElementById("multiselect_Div").classList.remove("editor-error");
    }

    if (form.Description.value == '') {
        isValidate = false;
        document.getElementById("DescriptionError").style.display = "list-item";
        document.querySelector('.description .trumbowyg-editor').classList.add("editor-error");
    } else {
        document.getElementById("DescriptionError").style.display = "none";
        document.querySelector('.description .trumbowyg-editor').classList.remove("editor-error");
    }
    if (form.Container.value == '') {
        isValidate = false;
        document.getElementById("ContainerError").style.display = "list-item";
        document.querySelector('.formarea').classList.add("editor-error");
    } else {
        document.getElementById("ContainerError").style.display = "none";
        document.querySelector('.formarea').classList.remove("editor-error");
    }
    document.documentElement.scrollTop = 0;
    if (isValidate) {
        document.getElementById("notifyError").style.display = "none";
    } else {
        document.getElementById("notifyError").style.display = "block";
    }
    return isValidate;
}

$(function () {
    $('#Footer').trumbowyg().on('tbwchange', function () {
        $(document).trigger('contentpreview:render');
    });
    $('#Header').trumbowyg().on('tbwchange', function () {
        $(document).trigger('contentpreview:render');
    });
    $('#Description').trumbowyg().on('tbwchange', function () {
        $(document).trigger('contentpreview:render');
    });
    $('#Instructions').trumbowyg().on('tbwchange', function () {
        $(document).trigger('contentpreview:render');
    });
});

function activeButton(displayType) {
    $("#btnWizard").removeClass();
    $("#btnForm").removeClass();
    if (displayType == "wizard") {
        $("#btnWizard").addClass("btn btn-primary active");
        $("#btnForm").addClass("btn btn-default");
    } else {
        $("#btnWizard").addClass("btn btn-default");
        $("#btnForm").addClass("btn btn-primary active");
    }
}