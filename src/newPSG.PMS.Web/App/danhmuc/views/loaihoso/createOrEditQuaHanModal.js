(function () {
    appModule.controller('danhmuc.views.loaihoso.createOrEditQuaHanModal', [
        '$scope', '$uibModalInstance', 'abp.services.app.loaiHoSo', 'abp.services.app.thuTuc', 'loaihoso', 'FileUploader', 'baseService', 'appSession', 'abp.services.app.commonLookup',
        function ($scope, $uibModalInstance, loaiHoSoService, thuTucService, loaihoso, fileUploader, baseService, appSession, commonLookupService) {
            //variable
            var vm = this;
            vm.saving = false;
            vm.loaihoso = {
                isActive: true
            };
            vm.uploadedFileName = null;
            //app.DON_VI_XU_LY = {
                
            //    , KE_TOAN: 2
            //    , MOT_CUA_TIEP_NHAN: 3
            //    , PHONG_BAN_PHAN_CONG: 4
            //    , TRUONG_PHONG: 5
            //    , LANH_DAO_CUC: 6
            //    , LANH_DAO_BO: 7
            //    , VAN_THU: 8
            //    , THANH_TRA: 9
            //    , CHUYEN_VIEN_THAM_XET: 10
            //    , CHUYEN_VIEN_PHOI_HOP_THAM_XET: 11
            //    , CHUYEN_VIEN_THAM_XET_TONG_HOP: 12
            //    , PHO_PHONG: 13
            //    , CHUYEN_GIA: 14
            //    , TO_TRUONG_CHUYEN_GIA: 15
            //    , HOI_DONG_THAM_DINH: 16
            //    , TRUONG_DOAN_THANH_TRA: 17
            //    , MOT_CUA_PHAN_CONG: 31
            //};

            //văn thư là 1 cửa tiếp nhận trong thủ tục đang dùng
            
            var dataDonVi = [
                { name: "Doanh nghiệp", id: "1" },
                { name: "Kế toán", id: "2" },
                { name: "Một cửa tiếp nhận", id: "3" },
                { name: "Phòng ban phân công", id: "4" },
                { name: "Trưởng phòng", id: "5" },
                { name: "Lãnh đạo cục", id: "6" },
                { name: "Văn thư", id: "8" },
                { name: "Chuyên viên thẩm xét", id: "10" },
                { name: "Chuyên gia", id: "14" },
                { name: "Hội đồng thẩm định", id: "16" },
            ]
            var luongXuLy = [
                { name: "Thẩm định cho phép kiểm nghiệm", id: "1" },
                { name: "Thẩm định cấp số đăng ký", id: "2" },
            ]



            //controll
            vm.quiTrinhOptions = {
                dataSource: appSession.get_quytrinhthamdinh(),
                dataValueField: "id",
                dataTextField: "name",
                filter: "contains",
                optionLabel: "Chọn qui trình",
            }
            vm.donViXuLyOptions = {
                dataSource: dataDonVi,
                dataValueField: "id",
                dataTextField: "name",
                filter: "contains",
                optionLabel: "Chọn đơn vị",
            }
            vm.luongXuLyOptions = {
                dataSource: luongXuLy,
                dataValueField: "id",
                dataTextField: "name",
                filter: "contains",
                optionLabel: "Chọn luồng xử lý",
            }
            vm.thuTucOptions = {
                dataSource: {
                    transport: {
                        read: function (options) {
                            thuTucService.getAllToDDL().then(function (result) {
                                options.success(result.data);
                            });
                        }
                    }
                },
                dataValueField: "id",
                dataTextField: "name",
                filter: "contains",
                optionLabel: "Chọn thủ tục",
            }
            vm.jsonHanXuLy = [];

            vm.addNewCauHinhThamDinh = function () {
                vm.jsonHanXuLy.push
                    (
                        { donViGui: "", donViNhan: "", soNgayXuLy: "", isHoSoBS: false, luongXuLyOptions: "", moTa: "" },
                );
            };
            vm.deleteNewCauHinhThamDinh = function (item) {
                if (vm.jsonHanXuLy.length > 0) {
                    var index = vm.jsonHanXuLy.indexOf(item);
                    vm.jsonHanXuLy.splice(index, 1);
                }

            };

            //vm.uploader = new fileUploader({
            //    url: abp.appPath + 'File/UploadIconLoaiHoSo',
            //    headers: {
            //        "X-XSRF-TOKEN": abp.security.antiForgery.getToken()
            //    },
            //    queueLimit: 1,
            //    autoUpload: true,
            //    removeAfterUpload: true,
            //    filters: [{
            //        name: 'iconFilter',
            //        fn: function (item, options) {
            //            //File type check
            //            var type = '|' + item.type.slice(item.type.lastIndexOf('/') + 1) + '|';
            //            if ('|jpg|jpeg|png|'.indexOf(type) === -1) {
            //                abp.message.error(app.localize('ProfilePicture_Warn_FileType'));
            //                return false;
            //            }

            //            //File size check
            //            if (item.size > 1048576) //1MB
            //            {
            //                abp.message.error(app.localize('ProfilePicture_Warn_SizeLimit'));
            //                return false;
            //            }
            //            return true;
            //        }
            //    }]
            //});

            //vm.uploader.onAfterAddingFile = function (fileItem) {
            //    console.info('onAfterAddingFile', fileItem);
            //    vm.uploadedFileName = fileItem.file.name;
            //};

            //vm.uploader.onSuccessItem = function (fileItem, response, status, headers) {
            //    if (response.success) {
            //        if (response.result.data) {
            //            vm.loaihoso.dataImage = response.result.data;
            //        }
            //    } else {
            //        abp.message.error(response.error.message);
            //    }
            //};

            
            //function
            vm.save = function () {
                
                //baseService.ValidatorForm("#loaihosoCreateOrEditForm");
                //var frmValidatorForm = angular.element(document.querySelector('#loaihosoCreateOrEditForm'));
                //var formValidation = frmValidatorForm.data('formValidation').validate();
                var tong = 0;
                for (var i = 0; i < vm.jsonHanXuLy.length; i++) {
                    tong = tong + vm.jsonHanXuLy[i].soNgayXuLy;
                }
                if (tong > vm.loaihoso.soNgayXuLy) {
                    abp.notify.error("Số ngày xử lý đang lớn hơn ngày xử lý tối đa");
                    return;
                }
                vm.loaihoso.JsonHanXuLy = JSON.stringify(vm.jsonHanXuLy);
                debugger;
                vm.saving = true;
                //if (formValidation.isValid()) {
                loaiHoSoService.createOrUpdateHanXuLy(vm.loaihoso)
                        .then(function (result) {
                            if (result.data) {          
                                $uibModalInstance.close();
                                vm.cancel();
                                abp.notify.success(app.localize('SavedSuccessfully'));
                            }
                        }).finally(function () {
                            vm.saving = false;
                        });
                //}
                //else {
                //    vm.saving = false;
                //}
            };

            vm.cancel = function () {
                $uibModalInstance.dismiss();
            };

            var init = function () {
                if (loaihoso != null) {
                    vm.loaihoso = loaihoso;
                    if (vm.loaihoso.thuTucId > 0) {
                        //if (formValidation.isValid()) {
                        loaiHoSoService.getByThuTucId(vm.loaihoso.thuTucId)
                            .then(function (result) {
                                if (result.data) {
                                    for (var i = 0; i < result.data.listLoaiHoSoHanXuLy.length; i++) {
                                        vm.jsonHanXuLy = JSON.parse(result.data.listLoaiHoSoHanXuLy[0].jsonHanXuLy);
                                        vm.loaihoso.tenLoaiHoSo = result.data.listLoaiHoSoHanXuLy[0].thuTucId
                                    }
                                }
                            }).finally(function () {
                                vm.saving = false;
                            });
                    }
                }
            }

            init();
        }
    ]);
})();