(function () {
    appModule.directive('loaidonhangchung', ['$timeout', 'appSession',
        function ($timeout, appSession) {
            return {
                restrict: 'EA',
                replace: true,
                template: `
                    <select style="width: 100%"
                        kendo-drop-down-list
                        ng-model="selected"
                        k-options="options"
                        class="form-control"></select>`,
                scope: {
                    selected: '=?'
                },
                link: function ($scope, element, attrs) {
                    $scope.dataSource = new kendo.data.DataSource({
                        transport: {
                            read: function (options) {
                                abp.services.app.xuLyHoSoGridView
                                    .getListLoaiDonHangCommon({ async: false })
                                    .done(function (result) {
                                        options.success(result);
                                    });
                            }
                        }
                    });
                    $scope.options = {
                        dataSource: $scope.dataSource,
                        dataTextField: "name",
                        dataValueField: "ids",
                        noDataTemplate: ``,
                        filter: "contains"
                    };
                }
            };
        }
    ]);
})();