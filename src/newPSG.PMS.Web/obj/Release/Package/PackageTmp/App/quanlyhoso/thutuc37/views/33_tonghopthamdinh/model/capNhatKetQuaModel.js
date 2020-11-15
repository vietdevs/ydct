(function () {
    appModule.controller('app.quanlyhoso.thutuc37.views.tonghopthamdinh.model.capnhatketquamodel', [
        '$sce', '$rootScope', 'appSession', 'quanlyhoso.thutuc37.services.appChuKySo', '$uibModalInstance', 'dataItem', 'abp.services.app.xuLyHoSoChuyenVien37','FileUploader',
        function ($sce, $rootScope, appSession, appChuKySo, $uibModalInstance, dataItem, xuLyHoSoChuyenVien37, fileUploader) {

            var vm = this;
            vm.bienBanTongHopUrl = null;


            vm.uploader = function (tepItem) {
                var _maThuTuc = appChuKySo.MA_THU_TUC;
                var _maSoThue = vm.dataItem.maSoThue;
                var _strThuMucHoSo = vm.dataItem.strThuMucHoSo;
                var _folderName = "bienbantonghop";

                var uploader = new fileUploader({
                    url: abp.appPath + 'File/UploadTaiLieuHoSo?maThuTuc=' + _maThuTuc + '&maSoThue=' + _maSoThue + '&strThuMucHoSo=' + _strThuMucHoSo + '&folderName=' + _folderName,
                    headers: {
                        "X-XSRF-TOKEN": abp.security.antiForgery.getToken()
                    },
                    queueLimit: 1,
                    autoUpload: true,
                    removeAfterUpload: true,
                    filters: [{
                        name: 'excelFilter',
                        fn: function (item, options) {
                            //File type check
                            var _extension = "." + item.name.split('.').pop().toLowerCase();
                            if (['.pdf'].indexOf(_extension) === -1) {
                                abp.message.error(app.localize('Không đúng định dạng tập tin cho phép (.pdf)'));
                                $("#tailieu_" + tepItem.code).val('');
                                return false;
                            }
                            else {
                                //File size check
                                if (item.size > 15145728) //3MB
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
                        vm.bienBanTongHopUrl = ajaxResponse.result.filePath;
                        abp.notify.info(app.localize('SavedSuccessfully'));
                    } else {
                        abp.message.error(ajaxResponse.error.message);
                    }
                };
                return uploader;
            };

            vm.save = function () {
                let input = {
                    hoSoId: vm.dataItem.id,
                    bienBanTongHopUrl: vm.bienBanTongHopUrl
                }
                xuLyHoSoChuyenVien37.capNhatKetQuaChuyenVanThu(input).then(function (result) {
                    abp.notify.success(app.localize('Cập nhật kết quả thành công'));
                    $uibModalInstance.close();
                })
            }

            vm.cancel = function () {
                $uibModalInstance.dismiss();
            }

            var init = () => {
                if (dataItem.id > 0) {
                    vm.dataItem = dataItem
                }
            }
            init();
            vm.trustSrc = function (src) {
                if (vm.bienBanTongHopUrl == null) {
                    return;
                }
                src = "File/GoToViewTaiLieu?url=" + src;
                return $sce.trustAsResourceUrl(src);
            };
        }
    ])

})();