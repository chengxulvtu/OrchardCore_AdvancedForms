﻿/*
** NOTE: This file is generated by Gulp and should not be edited directly!
** Any changes made directly to this file will be overwritten next time its asset group is processed by Gulp.
*/

function initializeContentPickerFieldEditorAFType(elementId, selectedItems, tenantPath, partName, fieldName, multiple) {
    var vueMultiselect = Vue.component('vue-multiselect', window.VueMultiselect.default);
    new Vue({
        el: '#' + elementId,
        components: { 'vue-multiselect': vueMultiselect },
        data: {
            value: null,
            arrayOfItems: selectedItems,
            options: [],
        },
        computed: {
            selectedIds: function () {
                return this.arrayOfItems.map(function (x) { return x.contentItemId }).join(',');
            },
            isDisabled: function () {
                return this.arrayOfItems.length > 0 && !multiple;
            }
        },
        watch: {
            selectedIds: function () {
                // We add a delay to allow for the <input> to get the actual value
                // before the form is submitted
                setTimeout(function () { $(document).trigger('contentpreview:render') }, 100);
            }
        },
        created: function () {
            var self = this;
            self.asyncFind();
        },
        methods: {
            asyncFind: function (query) {
                var self = this;
                self.isLoading = true;
                var searchUrl = tenantPath + '/GetAdvancedFormTypes';
                if (query) {
                    searchUrl += '&query=' + query;
                }
                fetch(searchUrl).then(function (res) {
                    res.json().then(function (json) {
                        self.options = json;
                        self.isLoading = false;
                    })
                });
            },
            onSelect: function (selectedOption, id) {
                var self = this;

                for (i = 0; i < self.arrayOfItems.length; i++) {
                    if (self.arrayOfItems[i].contentItemId === selectedOption.contentItemId) {
                        return;
                    }
                }
                self.arrayOfItems.push(selectedOption);
                console.log("selectedOption hideFromListing - ", selectedOption.hideFromListing);
                if (selectedOption.hideFromListing) {
                    $("#customControlAutosizing").prop("checked", true);
                    $("#customControlAutosizing").prop('disabled', true);
                } else {
                    $("#customControlAutosizing").prop("checked", false);
                    $("#customControlAutosizing").prop('disabled', false);
                }
            },
            remove: function (item) {
                this.arrayOfItems.splice(this.arrayOfItems.indexOf(item), 1);
                $("#customControlAutosizing").prop("checked", false);
                $("#customControlAutosizing").prop('disabled', false);
            }
        }
    });
}

function initializeContentPickerFieldEditorSecurityGroup(elementId, selectedItems, tenantPath, multiple) {
    var vueMultiselect = Vue.component('vue-multiselect', window.VueMultiselect.default);
    new Vue({
        el: '#' + elementId,
        components: { 'vue-multiselect': vueMultiselect },
        data: {
            value: null,
            arrayOfItems: selectedItems,
            options: [],
        },
        computed: {
            selectedIds: function () {
                return this.arrayOfItems.map(function (x) { return x.name }).join(',');
            },
            isDisabled: function () {
                return this.arrayOfItems.length > 0 && !multiple;
            }
        },
        watch: {
            selectedIds: function () {
                // We add a delay to allow for the <input> to get the actual value
                // before the form is submitted
                setTimeout(function () { $(document).trigger('contentpreview:render') }, 100);
            }
        },
        created: function () {
            var self = this;
            self.asyncFind();
        },
        methods: {
            asyncFind: function (query) {
                var self = this;
                self.isLoading = true;
                var searchUrl = tenantPath + '/GetUserRoles';
                if (query) {
                    searchUrl += '&query=' + query;
                }
                fetch(searchUrl).then(function (res) {
                    res.json().then(function (json) {
                        self.options = json;
                        self.isLoading = false;
                    })
                });
            },
            onSelect: function (selectedOption, id) {
                var self = this;
                for (i = 0; i < self.arrayOfItems.length; i++) {
                    if (self.arrayOfItems[i].name === selectedOption.name) {
                        return;
                    }
                }
                self.arrayOfItems.push(selectedOption);
            },
            remove: function (item) {
                this.arrayOfItems.splice(this.arrayOfItems.indexOf(item), 1);
            }
        }
    });
}