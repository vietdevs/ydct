(function () {
    appModule.controller('quanlyhoso.thutuc99.views.dangkyhoso.modal.chuyenKhoanModal', [
        '$uibModalInstance','$sce', 'dataItem', 'FileUploader', 'abp.services.app.xuLyHoSoDoanhNghiep99', 'baseService', 'appSession',
        function ($uibModalInstance, $sce, dataItem, fileUploader, xuLyHoSoDoanhNghiepService, baseService, appSession) {
            var vm = this;
            vm.dataCongBo = angular.copy(dataItem);
            vm.doanhnghiep = angular.copy(appSession.doanhNghiepInfo.doanhNghiep);
            vm.thanhtoan = {
                phiDaNop: 0,
                soTaiKhoanNop: null,
                soTaiKhoanHuong: null,
                maGiaoDich: null,
                maDonHang: null,
                ngayGiaoDich: null,
                duongDanHoaDonThanhToan: null
            };
            vm.lstautoCompleteMain = [];
            //Cấu hình controll
            {
                vm.itemValue = 1000;
                vm.itemValueOrg = angular.copy(vm.itemValue);
                vm.initListAutoComplete = function () {
                    vm.lstautoCompleteMain = [];
                    vm.itemValue = angular.copy(vm.itemValueOrg);
                    if (parseInt(vm.thanhtoan.phiDaNop) == 0) {
                        return;
                    } else {
                        var outInit = vm.itemValue * vm.thanhtoan.phiDaNop;
                        vm.lstautoCompleteMain.push(outInit);
                        for (var i = 0; i < 4; i++) {
                            outInit = outInit * 10;
                            vm.lstautoCompleteMain.push(outInit);
                        }
                    }
                };

                vm.uploaderHoaDon = function () {
                    var thutuc = 'THUTUC99';
                    var uploader = new fileUploader({
                        url: abp.appPath + 'File/UploadHoaDon?maSoThue=' + vm.dataCongBo.maSoThue + '&thuTuc=' + thutuc,
                        headers: {
                            "X-XSRF-TOKEN": abp.security.antiForgery.getToken()
                        },
                        queueLimit: 1,
                        autoUpload: true,
                        removeAfterUpload: true,
                        filters: [{
                            name: 'excelFilter',
                            fn: function (item, options) {
                                var _extension = "." + item.name.split('.').pop().toLowerCase();
                                if (['.pdf'].indexOf(_extension) === -1) {
                                    abp.message.error(app.localize('Không đúng định dạng tập tin cho phép (.pdf).'));
                                    $("#hoadon").val('');
                                    return false;
                                }
                                else {
                                    //File size check
                                    if (item.size > 15145728) //15MB
                                    {
                                        abp.message.error(app.localize('Dung lượng tập tin đính kèm vượt quá giới hạn cho phép (15MB)'));
                                        return false;
                                    }
                                    return true;
                                }
                            }
                        }]
                    });

                    uploader.onSuccessItem = function (item, ajaxResponse, status) {
                        if (ajaxResponse.success) {
                            vm.thanhtoan.duongDanHoaDonThanhToan = ajaxResponse.result.fileName;
                            vm.hoaDonUrl = "/File/GoToViewTaiLieu?url=" + vm.thanhtoan.duongDanHoaDonThanhToan + "#zoom=30";
                            abp.notify.info('Upload hóa đơn thành công');

                        } else {
                            abp.message.error(ajaxResponse.error.message);
                        }
                    };
                    return uploader;
                };
            }

            //Function
            {
                vm.save = function () {
                    if (baseService.isNullOrEmpty(vm.thanhtoan.duongDanHoaDonThanhToan)) {
                        abp.notify.error('Hãy upload hóa đơn thanh toán!');
                        return;
                    }

                    vm.saving = true;
                    baseService.ValidatorForm("#chuyenKhoanModal");
                    var frmValidatorForm = angular.element(document.querySelector('#chuyenKhoanModal'));
                    var formValidation = frmValidatorForm.data('formValidation').validate();
                    if (formValidation.isValid()) {
                        var input = {
                            HoSoId: vm.dataCongBo.id,
                            phiDaNop: vm.thanhtoan.phiDaNop,
                            soTaiKhoanNop: vm.thanhtoan.soTaiKhoanNop,
                            soTaiKhoanHuong: vm.thanhtoan.soTaiKhoanHuong,
                            maGiaoDich: vm.thanhtoan.maGiaoDich,
                            maDonHang: vm.thanhtoan.maDonHang,
                            ngayGiaoDich: vm.thanhtoan.ngayGiaoDich,
                            duongDanHoaDonThanhToan: vm.thanhtoan.duongDanHoaDonThanhToan,
                            ghiChu: vm.thanhtoan.ghiChu
                        };
                        xuLyHoSoDoanhNghiepService.thanhToanChuyenKhoan(input).then(function (result) {
                            if (result.data != 0) {
                                abp.notify.success(app.localize('SavedSuccessfully'));
                                $uibModalInstance.close();
                            }

                        }).finally(function () {
                            vm.saving = false;
                        });
                    }
                    else {
                        vm.saving = false;
                    }
                };
            }

            vm.trustSrc = function (src) {
                return $sce.trustAsResourceUrl(src);
            };

            vm.cancel = function () {
                $uibModalInstance.dismiss();
            };

            var init = function () {

            };
            init();

        }
    ]);
})();