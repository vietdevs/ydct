(function () {
    appModule.directive('quanlyhoso.thutuc98.directives.custom.loaidonhang', [
        function () {
            return {
                restrict: 'EA',
                replace: true,
                template: `<select style="width: 100%"
                        placeholder="Loại đơn hàng"
                        kendo-drop-down-list
                        ng-model="selected"
                        k-options="options"
                        class="form-control"></select>`,
                scope: {
                    selected: '=?',
                    datatext: '=',
                },
                link: function ($scope, element, attrs) {
                    var data = [
                        {
                            id: 1,
                            name: "Thuốc phóng xạ"
                        },
                        {
                            id: 2,
                            name: "Thuốc độc"
                        },
                        {
                            id: 3,
                            name: "Thuốc trong danh mục thuốc, dược chất thuộc danh mục chất bị cấm sử dụng trong một số ngành, lĩnh vực"
                        }
                    ];
                    $scope.options = {
                        dataSource: new kendo.data.DataSource({
                            transport: {
                                read: function (options) {
                                    options.success(data);
                                }
                            },
                        }),
                        dataValueField: "id",
                        dataTextField: "name",
                        optionLabel: app.localize('Chọn ...'),
                        filter: "contains",
                        change: function (e) {
                            var dataItem = e.sender.dataItem();
                            $scope.datatext = (dataItem.name);
                        },
                    };
                    var init = () => {
                    }
                    init();
                }
            }
        }
    ]);
})();

