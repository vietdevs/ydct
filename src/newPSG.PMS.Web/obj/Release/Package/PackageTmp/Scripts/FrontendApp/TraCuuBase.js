MyApp.controller('tracuubase', ['$scope',
    function ($scope) {
        var vm = this;
        vm.tenantName = null;
        vm.tenThuTuc = $("#tenThuTuc").text();

        $scope.$watch('vm.tenantName', function () {
            if (vm.tenantName && vm.tenThuTuc) {
                vm.dirTraCuu = `<div tracuu.` + vm.tenantName + `.` + vm.tenThuTuc + `></div>`;
            }
        });
    }
]);

