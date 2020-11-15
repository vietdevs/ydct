(function () {
    appModule.directive('filter.common.loaihoso', ['$timeout', 'appSession',
        function ($timeout, appSession) {
            return {
                restrict: 'EA',
                replace: true,
                template: ` <input
                       style="width: 100%"
                       kendo-drop-down-list
                       ng-model="selected"
                       k-options="loaiHoSoOptions"
                       class="form-control"/>
                        `,
                scope: {
                    selected: '=?',
                    thutucid: '=',
                    ishide: '=?',
                },
                link: function ($scope, element, attrs) {
                    $scope.loaiHoSoOptions = {
                        dataSource: {
                            transport: {
                                read: function (options) {
                                    abp.services.app.loaiHoSo.getListByThuTucId($scope.thutucid, { async: false })
                                        .done(function (result) {
                                            options.success(result);
                                            $scope.selected = result[0].id;
                                            if (result.length == 1) {
                                                $scope.ishide = true;
                                            }
                                        });
                                }
                            }
                        },
                        dataValueField: "id",
                        dataTextField: "name",
                        optionLabel: app.localize('Chọn ...'),
                        filter: "contains",
                    };
                    var init = function () {
                    }
                    init();
                }
            };
        }
    ]);
})();