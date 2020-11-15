(function () {
    appModule.directive('app.common.filter.danhsachthutuc', ['$timeout', 'appSession',
        function ($timeout, appSession) {
            return {
                restrict: 'EA',
                replace: true,
                //templateUrl: '/App/common/filters/tinh/filter-tinh.cshtml',
                template: `<select style="width: 100%"
                                kendo-drop-down-list
                                ng-model="selectedThuTuc"
                                k-options="options"
                                class="form-control"></select>`,
                scope: {
                    selectedThuTuc: '=?',
                    tracuu: '=',
                },
                link: function ($scope, element, attrs) {
                    let _thutuc = appSession.danhSachThuTuc;
                    if ($scope.tracuu) {
                        _thutuc = app.sessionStorage.get("prm_danhSachThuTuc_TraCuuUrl");
                    }

                    $scope.options = {
                        dataSource: _thutuc,
                        dataValueField: "thuTucIdEnum",
                        dataTextField: "maThuTuc",
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
                                        field: "maThuTuc",
                                        operator: "contains",
                                        value: filterValue
                                    },
                                ]
                            });
                            // handle the event
                        },
                        template: `<span class="badge margin-right-10 badge-success">#: data.maThuTuc #</span>
                                       <span class="bold">#: data.tenThuTuc #</span>
                                       <span class="badge margin-right-10 badge-danger">#: data.totalXuLy # Hồ sơ</span>
                                       `,
                    };
                }
            };
        }
    ]);
})();



(function () {
    appModule.directive('app.common.filter.thutucenum', ['$timeout', 'appSession',
        function ($timeout, appSession) {
            return {
                restrict: 'EA',
                replace: true,
                template: `<select style="width: 100%"
                                kendo-drop-down-list
                                ng-model="selected"
                                k-options="options"
                                ng-disabled="isDisabled"
                                class="form-control"></select>`,
                scope: {
                    selected: '=?',
                    valueChange: '&',
                    isDisabled: '=?'
                },
                link: function ($scope, element, attrs) {
                    $scope.options = {
                        dataSource: new kendo.data.DataSource({
                            transport: {
                                read: function (options) {
                                    let dataItem = app.sessionStorage.get("hhc_thutucenum");
                                    if (dataItem == null) {
                                        abp.services.app.commonLookup.getListThuTucEnum().then(function (result) {
                                            app.sessionStorage.set("hhc_thutucenum", result);
                                            options.success(result);
                                        });
                                    } else {
                                        options.success(dataItem);
                                    }
                                }
                            },
                        }),
                        dataValueField: "id",
                        dataTextField: "name",
                        optionLabel: app.localize('Chọn ...'),
                        filter: "contains",
                        select: function (e) {
                            var dataItem = this.dataItem(e.item);
                            if ($scope.valueChange())
                                $scope.valueChange()(dataItem);
                        }
                    };
                }
            };
        }
    ]);
})();