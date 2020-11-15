(function () {
    appModule.controller('common.views.excel.ViewPDF', [
        '$scope', '$uibModalInstance', 'controller', 'viewName', 'dataParams', 'allowExport', 'modalTitle',
        function ($scope, $uibModalInstance, controller, viewName, dataParams, allowExport, modalTitle) {
            var vm = this;
            vm.controller = controller;
            vm.viewName = viewName;
            vm.allowExport = allowExport;
            vm.modalTitle = modalTitle;

            //$scope.frameUrl = '/' + vm.controller + "/" + vm.viewName + '?controller=' + controller + '&viewName=' + viewName + '&jsonParams=' + dataParams + '&allowExport=' + allowExport;
            console.log($scope.frameUrl);
            var params = {
                control: vm.controller,
                actionName: vm.viewName,
                json: dataParams,
            }
            console.log(params, "params");
            $.ajax({
                type: 'POST',
                url: '/Home/ViewPDF',
                //headers: headers,
                data: {
                    control: vm.controller,
                    actionName: vm.viewName,
                    hoSoId: dataParams.HoSoId,
                },
                dataType: "html",
                success: function (data) {
                    $('#loadHtml').html(data);
                },
                error: function (xhr) {
                },
                complete: function () {
                    //UIBlockUI.stopLoadingPage();
                }
            });

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