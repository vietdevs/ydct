(function () {
    appModule.directive('phamViDoanhNghiep', [
        function () {
            return {
                restrict: 'EA',
                replace: true,
                template: `<select style="width: 100%"
                        placeholder="Loại hình doanh nghiệp"
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
                            name: "DN/Cơ sở đăng ký trong nước"
                        },
                        {
                            id: 2,
                            name: "Doanh nghiệp nước ngoài"
                        }
                    ];

                    $scope.options = {
                        dataSource: new kendo.data.DataSource({
                            transport: {
                                read: function (options) {
                                    //_data = [];
                                    let _data = data;
                                    options.success(_data);
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