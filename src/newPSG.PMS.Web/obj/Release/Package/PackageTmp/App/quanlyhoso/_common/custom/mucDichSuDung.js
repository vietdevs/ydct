(function () {
    appModule.directive('quanlyhoso.directives.custom.mucdichsudung', [
        function () {
            return {
                restrict: 'EA',
                replace: true,
                template: `<select style="width: 100%"
                        placeholder="Mục đích nhập khẩu"
                        kendo-drop-down-list
                        ng-model="selected"
                        k-options="options"
                        class="form-control"></select>`,
                scope: {
                    selected: '=?',
                    loaidonhang: '=',
                    datatext: '=',
                },
                link: function ($scope, element, attrs) {
                    var duocpham = [
                        {
                            id: 1,
                            name: 'Làm mẫu kiểm nghiệm, nghiên cứu thuốc'
                        },
                        {
                            id: 2,
                            name: 'Sản xuất thuốc xuất khẩu'
                        },
                        {
                            id: 3,
                            name: 'Sản xuất thuốc phục vụ yêu cầu quốc phòng, an ninh, phòng, chống dịch bệnh, khắc phục hậu quả thiên tai, thảm họa'
                        },
                        {
                            id: 4,
                            name: 'Làm mẫu kiểm nghiệm/nghiên cứu khoa học/thử nghiệm lâm sàng/đánh giá sinh khả dụng/tương đương sinh học'
                        }
                    ];
                    $scope.options = {
                        dataSource: new kendo.data.DataSource({
                            transport: {
                                read: function (options) {
                                    let _data = duocpham;
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
    appModule.directive('quanlyhoso.directives.custom.mucdichsudungxuatkhau', [
        function () {
            return {
                restrict: 'EA',
                replace: true,
                template: `<select style="width: 100%"
                        placeholder="Mục đích xuất khẩu"
                        kendo-drop-down-list
                        ng-model="selected"
                        k-options="options"
                        class="form-control"></select>`,
                scope: {
                    selected: '=?',
                    loaidonhang: '=',
                    datatext: '=',
                },
                link: function ($scope, element, attrs) {
                    var duocpham = [
                        {
                            id: 1,
                            name: 'Vì mục đích thương mại'
                        },
                        {
                            id: 2,
                            name: 'Tham gia trưng bày tại triển lãm, hội chợ'
                        },
                        {
                            id: 3,
                            name: 'Thử lâm sàng, thử tương đương sinh học, đánh giá sinh khả dụng, làm mẫu kiểm nghiệm, nghiên cứu khoa học, làm mẫu đăng ký'
                        }
                    ];
                    $scope.options = {
                        dataSource: new kendo.data.DataSource({
                            transport: {
                                read: function (options) {
                                    _data = duocpham;
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