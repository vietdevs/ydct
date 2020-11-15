(function () {
    appModule.directive('common.filter.tinh', ['$timeout', 'appSession',
        function ($timeout, appSession) {
            return {
                restrict: 'EA',
                replace: true,
                template: `<select style="width: 100%"
                                kendo-drop-down-list
                                ng-model="selectedTinh"
                                k-options="tinhOptions"
                                class="form-control"></select>`,
                scope: {
                    selectedTinh: '=?'
                },
                link: function ($scope, element, attrs) {
                    $scope.tinhOptions = {
                        dataSource: appSession.get_tinh(),
                        dataValueField: "id",
                        dataTextField: "ten",
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
                                        field: "ten",
                                        operator: "contains",
                                        value: filterValue
                                    },
                                    {
                                        field: "tenKhongDau",
                                        operator: "contains",
                                        value: filterValue
                                    }
                                ]
                            });
                            // handle the event
                        },
                    };
                }
            };
        }
    ]);
})();