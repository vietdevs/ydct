(function () {
    appModule.controller('danhmuc.views.loaihoso.createOrEditModal', [
        '$scope', '$uibModalInstance', 'abp.services.app.loaiHoSo', 'abp.services.app.thuTuc', 'loaihoso', 'FileUploader', 'baseService', 'appSession', 'abp.services.app.commonLookup',
        function ($scope, $uibModalInstance, loaiHoSoService, thuTucService, loaihoso, fileUploader, baseService, appSession, commonLookupService) {
            //variable
            var vm = this;
            vm.saving = false;
            vm.loaihoso = {
                isActive: true
            };
            vm.uploadedFileName = null;

            //controll
            vm.quiTrinhOptions = {
                dataSource: appSession.get_quytrinhthamdinh(),
                dataValueField: "id",
                dataTextField: "name",
                filter: "contains",
                optionLabel: "Chọn qui trình",
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

            vm.uploader = new fileUploader({
                url: abp.appPath + 'File/UploadIconLoaiHoSo',
                headers: {
                    "X-XSRF-TOKEN": abp.security.antiForgery.getToken()
                },
                queueLimit: 1,
                autoUpload: true,
                removeAfterUpload: true,
                filters: [{
                    name: 'iconFilter',
                    fn: function (item, options) {
                        //File type check
                        var type = '|' + item.type.slice(item.type.lastIndexOf('/') + 1) + '|';
                        if ('|jpg|jpeg|png|'.indexOf(type) === -1) {
                            abp.message.error(app.localize('ProfilePicture_Warn_FileType'));
                            return false;
                        }

                        //File size check
                        if (item.size > 1048576) //1MB
                        {
                            abp.message.error(app.localize('ProfilePicture_Warn_SizeLimit'));
                            return false;
                        }
                        return true;
                    }
                }]
            });

            vm.uploader.onAfterAddingFile = function (fileItem) {
                console.info('onAfterAddingFile', fileItem);
                vm.uploadedFileName = fileItem.file.name;
            };

            vm.uploader.onSuccessItem = function (fileItem, response, status, headers) {
                if (response.success) {
                    if (response.result.data) {
                        vm.loaihoso.dataImage = response.result.data;
                    }
                } else {
                    abp.message.error(response.error.message);
                }
            };

            //function
            vm.save = function () {
                vm.saving = true;
                baseService.ValidatorForm("#loaihosoCreateOrEditForm");
                var frmValidatorForm = angular.element(document.querySelector('#loaihosoCreateOrEditForm'));
                var formValidation = frmValidatorForm.data('formValidation').validate();
                if (formValidation.isValid()) {
                    loaiHoSoService.createOrUpdate(vm.loaihoso)
                        .then(function (result) {
                            if (result.data) {
                                vm.loaihoso.id = result.data;
                                vm.uploader.uploadAll();
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

            vm.cancel = function () {
                $uibModalInstance.dismiss();
            };

            var init = function () {
                if (loaihoso != null) {
                    vm.loaihoso = loaihoso;
                }
            }

            init();
        }
    ]);
})();