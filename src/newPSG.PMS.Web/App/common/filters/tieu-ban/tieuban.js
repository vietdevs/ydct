(function () {
    appModule.directive('filter.common.tieuban', ['$timeout','baseService',
        function ($timeout, baseService) {
            return {
                restrict: 'EA',
                replace: true,
                template: `<select style="width: 100%"
                                kendo-drop-down-list
                                ng-model="value"
                                k-options="options"
                                ng-disabled="isDisabled"
                                class="form-control"></select>`,
                scope: {
                    value: '=?',
                    isDisabled: '=?'
                },
                link: function (scope, element, attrs) {
                    scope.options = {
                        dataSource: new kendo.data.DataSource({
                            transport: {
                                read: function (options) {
                                    let tieuBan = app.sessionStorage.get("hhc_tieu_ban_chuyen_gia");
                                    if (tieuBan == null) {
                                        abp.services.app.commonLookup.getListTieuBanChuyenGia().then(function (result) {
                                            options.success(result);
                                            app.sessionStorage.set("hhc_tieu_ban_chuyen_gia", result);
                                        })
                                    } else {
                                        options.success(tieuBan);
                                    }
                                }
                            }
                        }),
                        optionLabel: "Chọn tiểu ban...",
                        dataValueField: "id",
                        dataTextField: "name",
                        filter: "contains"
                    };
                }
            };
        }
    ]);
})();
