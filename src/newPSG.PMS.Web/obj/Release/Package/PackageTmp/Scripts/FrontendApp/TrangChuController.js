MyApp.controller('ctrl.home', [
    '$scope', '$location', '$http', '$window',
    function ($scope, $location, $http, $window) {
        var vm = this;
        vm.commonFilter = '';
        vm.total = {
            dangKyCongBo: 0,
            tuCongBo: 0,
            doanhNghiep: 0,
            dangKyCongBoCuc: 0,
            dangKyCongBoCucDaTraGTN: 0
        };
        vm.top10 = {
            dangKyCongBo: [],
            tuCongBo: [],
            doanhNghiep: []
        }


        //vm.init = function () {
        //    $http.post('/api/services/app/thongKeChung/thongKeTrangChu').then(function (result) {
        //        var i = 1;
        //        var j = 1;
        //        var k = 1;
        //        if (result) {
        //            if (result.data.result.top10DangKyCongBo && result.data.result.top10dangkyhoso.length > 0) {
        //                result.data.result.top10dangkyhoso.forEach(function (item) {
        //                    item.STT = i;
        //                    i++;
        //                });
        //                vm.top10.dangKyCongBo = result.data.result.top10DangKyCongBo;
        //            }
        //            if (result.data.result.top10DoanhNghiep && result.data.result.top10DoanhNghiep.length > 0) {
        //                result.data.result.top10DoanhNghiep.forEach(function (item) {
        //                    item.STT = j;
        //                    j++;
        //                });
        //                vm.top10.doanhNghiep = result.data.result.top10DoanhNghiep;
        //            }
        //            if (result.data.result.top10TuCongBo && result.data.result.top10TuCongBo.length > 0) {
        //                result.data.result.top10TuCongBo.forEach(function (item) {
        //                    item.STT = k;
        //                    k++;
        //                });
        //                vm.top10.tuCongBo = result.data.result.top10TuCongBo;
        //            }

        //            if (result.data.result.totalDangKyCongBo) {
        //                vm.total.dangKyCongBo = result.data.result.totalDangKyCongBo;
        //                console.log(vm.total.dangKyCongBo, "vm.total.dangKyCongBo");
        //            }
        //            if (result.data.result.totalTuCongBo) {
        //                vm.total.tuCongBo = result.data.result.totalTuCongBo;
        //            }
        //            if (result.data.result.totalDoanhNghiep) {
        //                vm.total.doanhNghiep = result.data.result.totalDoanhNghiep;
        //            }

        //            if (result.data.result.totalDangKyCongBoCuc) {
        //                vm.total.dangKyCongBoCuc = result.data.result.totalDangKyCongBoCuc;
        //            }
        //            if (result.data.result.totalDangKyCongBoCuc) {
        //                vm.total.dangKyCongBoCucDaTraGTN = result.data.result.totalDangKyCongBoCucDaTraGTN;
        //            }
        //        }
               
        //    });
        //}
        vm.init();

        vm.banCongBo = function (item) {

            var printInfo = {
                HoSoId: item.id
            };
            window.open(
                abp.appPath + 'Report/gotoGiayTiepNhan?jsonParams=' + JSON.stringify(printInfo),
                '_blank'
            );
        }

        vm.banTuCongBo = function (item) {

            var printInfo = {
                HoSoId: item.id
            };
            window.open(
                abp.appPath + 'Report/gotoViewTuCongBoSanPham?jsonParams=' + JSON.stringify(printInfo),
                '_blank'
            );
        }
        vm.commonSearch = function () {
            vm.commonFilter = vm.commonFilter.trim();
            if (vm.commonFilter == '' || vm.commonFilter == null) {
                toastr["error"]("Vui lòng nhập từ khóa để tìm kiếm");
                return false;
            }
            else {
                sessionStorage.setItem("home_key_common_search", vm.commonFilter);
                $window.location.href = "/search";
            }
        }
        vm.changeSwitchFilter = function (d) {
            vm.switchFilter = {
                caseDoanhNghiep: true,
                caseSanPham: true,
            };
            if (d == 1) {
                vm.switchFilter = {
                    caseDoanhNghiep: true,
                    caseSanPham: false,
                };
            }
            if (d == 2) {
                vm.switchFilter = {
                    caseDoanhNghiep: false,
                    caseSanPham: true,
                };
            }

        }

        
    }
]);