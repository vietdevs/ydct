(function () {
    appModule.directive('dirExcel', function () {
        return {
            restrict: 'A',

            scope: {
                dirExcel: '@'
            },
            link: function (scope, iElement, iAttrs) {
                scope.$watch('dirExcel', function () {
                    var iFrame = iElement;
                    if (!(iFrame && iFrame.length > 0)) {
                        iFrame = $('<iframe></iframe>');
                        iElement.append(iFrame);
                    }
                    iFrame.attr("src", scope.dirExcel);
                });
            }
        };
    });

    appModule.controller('common.views.excel.ViewPDF_Template', [
        '$scope', '$uibModalInstance', 'controller', 'viewName', 'dataParams', 'allowExport', 'modalTitle',
        function ($scope, $uibModalInstance, controller, viewName, dataParams, allowExport, modalTitle) {
            var vm = this;
            vm.controller = controller;
            vm.viewName = viewName;
            vm.allowExport = allowExport;
            vm.modalTitle = modalTitle;

            $scope.frameUrl = '/' + vm.controller + "/" + vm.viewName + '?controller=' + controller + '&viewName=' + viewName + '&jsonParams=' + dataParams + '&allowExport=' + allowExport;
            console.log($scope.frameUrl);

            vm.close = function () {
                $uibModalInstance.dismiss();
            }

            vm.print = function () {
                if (printInfo.printType == "dangkybaocao") {
                    $window.open($scope.frameUrl);
                }
            }
        }

    ]);
})();