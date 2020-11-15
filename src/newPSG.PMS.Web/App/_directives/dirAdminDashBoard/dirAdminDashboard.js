(function () {
    appModule.directive('app.directives.diradmindashboard', ['$compile', '$timeout', '$templateRequest', '$filter', 'appSession', '$linq',
        function ($compile, $timeout, $templateRequest, $filter, appSession, $linq) {
            var controller = ['$scope', '$timeout', '$window', '$filter', '$uibModal', '$state',
                function ($scope, $timeout, $window, $filter, $uibModal, $state) {
                    var vm = this;
                    var loadTotalLabel = () => {
                        let _req = {
                            formId: 92,
                            formCase: 1,
                            formCase2: 0,
                            page: 1,
                            pageSize: 10,
                            keyword: null,
                            ngayGuiTu: null,
                            ngayGuiToi: null,
                            loaiHoSoId: null,
                            tinhId: null,
                            doanhNghiepId: null,
                            phongBanId: null
                        };
                        _req.ngayNopTu = null;
                        _req.ngayNopToi = null;
                        _req.skipCount = 0;
                        _req.maxResultCount = 1;
                        abp.services.app.thuTucReport
                            .getTotalFormCase(_req)
                            .done(function (result) {
                                if (result) {
                                    $("#chartThongKeHoSo").kendoChart({
                                        title: {
                                            text: "THỐNG KÊ TÌNH HÌNH GIẢI QUYẾT HỒ SƠ",
                                            color: '#524F4F'
                                        },
                                        legend: {
                                            visible: false
                                        },
                                        seriesDefaults: {
                                            type: "column",
                                            stack: true
                                        },
                                        series: [{
                                            name: "Tổng số hồ sơ",
                                            data: [result.case1, result.case2, result.case3, result.case4],
                                            color: "#00BCD4"
                                        }],
                                        valueAxis: {
                                            max: result.case0 + 50,
                                            line: {
                                                visible: false
                                            },
                                            minorGridLines: {
                                                visible: true
                                            }
                                        },
                                        categoryAxis: {
                                            categories: ["Hồ sơ đã tiếp nhận", "Hồ sơ trả lại", "Hồ sơ chờ bổ sung", "Hồ sơ đã giải quyết"],
                                            majorGridLines: {
                                                visible: false
                                            }
                                        },
                                        tooltip: {
                                            visible: true,
                                            template: "#= series.name #: #= value #"
                                        }
                                    });
                                }
                            });
                    };
                    var init = () => {
                        loadTotalLabel();
                    };
                    init();
                }];
            return {
                restrict: 'EA',
                controller: controller,
                controllerAs: 'vm',
                bindToController: false,
                link: function (scope, elem, attr, ctrl) {
                    $templateRequest("/App/_directives/dirAdminDashBoard/dirAdminDashboard.html").then(function (html) {
                        var template = angular.element(html);
                        elem.append(template);
                        $compile(template)(scope);
                    });
                }
            };
        }
    ]);
})();