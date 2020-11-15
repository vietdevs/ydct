(function () {
    appModule.directive('quanlyhoso.thutuc99.directives.custom.mucdich', [
        function () {
            return {
                restrict: 'EA',
                replace: true,
                template: `<select style="width: 100%"
                        placeholder="Mục đích"
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
                            name: "Thử lâm sàng"
                        },
                        {
                            id: 2,
                            name: "Thử tương đương sinh học, đánh giá khả dụng"
                        },
                        {
                            id: 3,
                            name: "Làm mẫu kiểm nghiệm nghiên cứu khoa học"
                        },
                        {
                            id: 4,
                            name: "Làm mẫu đăng ký"
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

