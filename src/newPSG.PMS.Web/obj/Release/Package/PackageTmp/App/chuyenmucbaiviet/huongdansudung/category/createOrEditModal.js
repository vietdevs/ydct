(function () {
    appModule.controller('chuyenmucbaiviet.huongdansudung.category.createOrEditModal', [
        '$scope', '$uibModalInstance', 'abp.services.app.category', 'category', 'baseService',
        function ($scope, $uibModalInstance, categoryService, category, baseService) {
            //variable
            var vm = this;
            vm.saving = false;
            vm.category = {
                isActive: true,
                level: 0
            };
            $scope.ckDescription = {
                languague: 'vi',
                height: '320px'
            }
            vm.GetSeo = function () {
                vm.category.description = app.locdau(vm.category.title);
            }
            vm.ChooseImage = function () {
                var finder = new CKFinder();
                finder.selectActionFunction = function (fileUrl) {
                    $scope.$apply(function () {
                        vm.category.file = fileUrl;
                    })
                }
                finder.popup();
            }
            vm.categoryParentOptions = {
                dataSource: new kendo.data.DataSource({
                    transport: {
                        read: function (options) {
                            abp.services.app.category.allCategoryToDDL(vm.category.id).done(function (result) {
                                options.success(result);
                            });
                        }
                    }
                }),
                dataValueField: "id",
                dataTextField: "title",
                optionLabel: app.localize('Chọn ...'),
                filter: "contains",
                select: function (e) {
                    var dataItem = this.dataItem(e.item);
                    if (dataItem.level == undefined) {
                        vm.category.level = 0;
                    }
                    else {
                        vm.category.level = dataItem.level + 1;
                    }
                }
            };
            //function
            vm.save = function () {
                console.log(vm.category.roleLevel);
                baseService.ValidatorForm("#categoryCreateOrEditForm");
                var frmValidatorForm = angular.element(document.querySelector('#categoryCreateOrEditForm'));
                var formValidation = frmValidatorForm.data('formValidation').validate();
                if (formValidation.isValid()) {
                    vm.saving = true;
                    categoryService.createOrUpdate(vm.category).then(function (result) {
                        abp.notify.success(app.localize('SavedSuccessfully'));
                        $uibModalInstance.close();
                    }).finally(function () {
                        vm.saving = false;
                    });
                }
            };
            //setTimeout(
            //    function () {
            //        $('.modal-content').draggable({ handle: ".modal-header" });
            //    }, 0);
            vm.cancel = function () {
                $uibModalInstance.dismiss();
            };
            var init = function () {
                if (category != null) {
                    vm.category = category;
                }
            };
            init();
        }
    ]);
})();