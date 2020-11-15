(function () {
    appModule.directive('filter.common.donvitinh', ['$timeout', 'appSession',
        function ($timeout, appSession) {
            return {
                restrict: 'E',
                replace: true,
                template: `<input kendo-auto-complete
                                ng-model="selectedItems"
                                k-options="donViTinhOptions"
                                style="width: 100%;" />`,
                scope: {
                    selectedItems: '=?'
                },
                link: function ($scope, element, attrs) {
                    $scope.dataSource = new kendo.data.DataSource({
                        transport: {
                            read: function (options) {
                                appSession.donViTinh = app.sessionStorage.get("hcc_donViTinh");
                                if (appSession.donViTinh == null) {
                                    abp.services.app.danhMucCommon.getAllDonViTinh({ async: false }).done(function (result) {
                                        appSession.donViTinh = result;
                                        app.sessionStorage.set("hcc_donViTinh", result);
                                    });
                                }
                                options.success(appSession.donViTinh);
                            }
                        }
                    });

                    $scope.donViTinhOptions = {
                        dataSource: $scope.dataSource,
                        dataTextField: "ten",
                        optionLabel: 'Chọn ...',
                        filter: "contains",
                        dataBound: function () {
                        },
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
                        noDataTemplate: ''
                    };
                }
            };
        }
    ]);
})();