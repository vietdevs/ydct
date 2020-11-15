(function () {
    appModule.directive('filter.common.loaibienban', ['$timeout', 'baseService',
        function ($timeout, baseService) {
            return {
                restrict: 'EA',
                replace: true,
                template: `<select style="width: 100%"
                                id="DDL_LoaiBienBan_{{index}}"
                                kendo-drop-down-list
                                ng-model="value"
                                k-options="options"
                                ng-disabled="isDisabled"
                                class="form-control"></select>`,
                scope: {
                    value: '=?',
                    nameValue: '=?',
                    control: '=?',
                    isDisabled: '=?',
                    index: '=?'
                },
                link: function (scope, element, attrs) {
                    scope.options = {
                        dataSource: new kendo.data.DataSource({
                            transport: {
                                read: function (options) {
                                    scope.dataItem = app.sessionStorage.get("hhc_loaibienban");
                                    if (scope.dataItem == null) {
                                        abp.services.app.loaiBienBanThamDinh.getAllToDDL().then(function (result) {
                                            scope.dataItem = result;
                                            options.success(result);
                                            app.sessionStorage.set("hhc_loaibienban", result);
                                        })
                                    } else {
                                        options.success(scope.dataItem);
                                    }
                                }
                            }
                        }),
                        optionLabel: "Chọn tiểu ban...",
                        dataValueField: "id",
                        dataTextField: "name",
                        filter: "contains",
                        change: function (e) {
                            var dataItem = e.sender.dataItem();
                            scope.nameValue = dataItem.name;
                        },
                    };

                    scope.index = scope.index || 0;
                    scope.control = scope.control || {};
                    scope.control.setDataSource = (roleLevel, index) => {
                        let dataSource = [];
                        if (roleLevel && scope.dataItem) {
                            dataSource = scope.dataItem.filter(x => x.roleLevel == roleLevel);
                        }

                        index = index || 0;
                        let idDDL = '#DDL_LoaiBienBan_' + index;
                        let dropdownlist = $(idDDL).data("kendoDropDownList");
                        if (dropdownlist) {
                            dropdownlist.setDataSource(dataSource);
                        }
                    };
                }
            };
        }
    ]);
})();

