(function () {
    appModule.directive('app.directives.dircommondashboard', ['$compile', '$timeout', '$templateRequest', '$filter', 'appSession', '$linq',
        function ($compile, $timeout, $templateRequest, $filter, appSession, $linq) {
            var controller = ['$scope', '$timeout', '$window', '$filter', '$uibModal', '$state',
                function ($scope, $timeout, $window, $filter, $uibModal, $state) {
                    var vm = this;
                    var initVar = () => {
                    }
                    var mainFunc = () => {
                        vm.go_lanhdaocucduyet = () => {
                            $state.go('lanhdaocucduyet');
                        };
                    }
                    var loadTotalLabel = () => {
                        let _req = {
                            formId: 12,
                            formCase: 1, //0:TAT_CA, 1:CHUA_PHAN_CONG, 2:DA_PHAN_CONG, 3:DA_PHAN_CONG_TU_DONG
                            formCase2: 0,
                            page: 1,
                            pageSize: 10,
                            keyword: null,
                            ngayGuiTu: null,
                            ngayGuiToi: null,
                            loaiHoSoId: null,
                            tinhId: null,
                            //app-session
                            doanhNghiepId: null,
                            phongBanId: null
                        }
                        _req.ngayNopTu = null;
                        _req.ngayNopToi = null;
                        _req.skipCount = 0;
                        _req.maxResultCount = 1;
                        abp.services.app.thuTucReport.getTotalFormCase(_req).done(function (result) {
                            vm.totalFormCase = result;
                            $scope.$apply();
                        });
                    }
                    var init = () => {
                        initVar();
                        mainFunc();
                        loadTotalLabel();
                    }
                    init();
                }];
            return {
                restrict: 'EA',
                scope: {
                    thutucyeuthich: '=?'
                },
                controller: controller,
                controllerAs: 'vm',
                bindToController: false,
                link: function (scope, elem, attr, ctrl) {
                    $templateRequest("/App/_directives/dirCommonDashboard/dirCommonDashboard.html").then(function (html) {
                        var template = angular.element(html);
                        elem.append(template);
                        $compile(template)(scope);
                    });
                }
            };
        }
    ]);
})();