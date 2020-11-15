(function () {
    appModule.directive('filter.common.rolelevel', ['abp.services.app.danhMucCommon', '$timeout',
        function (commonService, $timeout) {
            return {
                restrict: 'EA',
                replace: true,
                template: `<select style="width: 100%"
                                kendo-drop-down-list
                                k-ng-model="idValue"
                                k-options="cbbOptions"
                                ng-disabled="isDisabled"
                                class="form-control"></select>`,
                scope: {
                    idValue: '=?',
                    //nameValue: '=?',
                    valueChange: '&',
                    thuTucEnum: '=?',
                    isDisabled: '=?'
                },
                link: function (scope, element, attrs) {
                    scope.cbbOptions = {
                        dataSource: new kendo.data.DataSource({
                            transport: {
                                read: function (options) {
                                    commonService.getListRole_Level().then(function (result) {
                                        options.success(result.data);
                                    }).finally(function () {
                                    });
                                }
                            },
                        }),
                        dataValueField: "id",
                        dataTextField: "name",
                        optionLabel: app.localize('Chọn ...'),
                        filter: "contains",
                        noDataTemplate: 'Không có dữ liệu',
                        filtering: function (ev) {
                            var filterValue = ev.filter != undefined ? ev.filter.value : "";
                            ev.preventDefault();
                            //get filter descriptor
                            this.dataSource.filter({
                                logic: "or",
                                filters: [
                                    {
                                        field: "name",
                                        operator: "startswith",
                                        value: filterValue
                                    }
                                ]
                            });
                            // handle the event
                        },
                        select: function (e) {
                            var dataItem = this.dataItem(e.item);
                            $timeout(function () {
                                scope.idValue = dataItem.id;
                                if (scope.valueChange())
                                    scope.valueChange()(dataItem);
                            });

                        }
                    };

                }
            };
        }
    ]);
})();
